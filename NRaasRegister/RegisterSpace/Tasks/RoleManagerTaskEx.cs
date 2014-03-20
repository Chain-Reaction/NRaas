using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.RegisterSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Tasks
{
    public class RoleManagerTaskEx : RoleManagerTask, Common.IWorldQuit
    {
        static ObjectGuid sTask;

        static Common.MethodStore sActiveFamilyCheck = new Common.MethodStore("NRaasTraveler", "NRaas.TravelerSpace.Helpers.GameStatesEx", "WasActiveFamilyMember", new Type[] { typeof(ulong) });
        static Common.MethodStore sHomeworldCheck = new Common.MethodStore("NRaasTraveler", "NRaas.TravelerSpace.Helpers.GameStatesEx", "IsHomeworldResident", new Type[] { typeof(ulong) });

        static bool sInitial = true;

        public static bool IsLoading
        {
            get 
            {
                List<Role> roles;
                if (!RoleManager.sRoleManager.mRoles.TryGetValue(Role.RoleType.LocationMerchant, out roles)) return false;

                return (roles == null);
            }
        }

        public void OnWorldQuit()
        {
            Simulator.DestroyObject(sTask);
            sTask = ObjectGuid.InvalidObjectGuid;
        }

        public static void SafeRemoveSimFromRole(string prefix, Role role)
        {
            if (role == null) return;

            try
            {
                string msg = null;

                if (role.mSim != null)
                {
                    if (role.mSim.AssignedRole != role)
                    {
                        msg += Common.NewLine + "(A) " + role.mSim.FullName;

                        role.mSim.AssignedRole = null;
                    }
                    else
                    {
                        msg += Common.NewLine + "(B) " + role.mSim.FullName;
                    }
                }

                if (role.mRoleGivingObject != null)
                {
                    if (role.mRoleGivingObject.CurrentRole != role)
                    {
                        msg += Common.NewLine + "(A) " + role.mRoleGivingObject.GetType().ToString().Replace("Sims3.Gameplay.Objects.", "");
                        msg += Common.NewLine + "(A) " + role.mRoleGivingObject.ObjectId;

                        role.mRoleGivingObject = null;
                    }
                    else
                    {
                        msg += Common.NewLine + "(B) " + role.mRoleGivingObject.GetType().ToString().Replace("Sims3.Gameplay.Objects.", "");
                        msg += Common.NewLine + "(B) " + role.mRoleGivingObject.ObjectId;
                    }
                }

                try
                {
                    role.EndRole();
                }
                catch 
                { }

                role.RemoveAlarms();

                if ((role.mSim != null) && (role.mSim.AssignedRole != null))
                {
                    role.mSim.AssignedRole = null;
                    role.mSim.RemoveOutfits(OutfitCategories.Career, true);

                    msg += Common.NewLine + "(C) " + role.mSim;
                }

                role.mOutfit = null;

                if ((role.mRoleGivingObject != null) && (role.mRoleGivingObject.CurrentRole != null))
                {
                    role.mRoleGivingObject.CurrentRole = null;

                    msg += Common.NewLine + "(C) " + role.mRoleGivingObject.GetType().ToString().Replace("Sims3.Gameplay.Objects.", "");
                    msg += Common.NewLine + "(C) " + role.mRoleGivingObject.ObjectId;
                }

                if (RoleManager.sRoleManager != null)
                {
                    RoleManager.sRoleManager.RemoveRole(role);
                }

                if (msg != null)
                {
                    Logger.AddTrace("Role Dropped (" + prefix + "):" + msg);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(role.mSim, e);
            }
        }

        private int FillRolesEx(RoleToFill roleInfo, List<SimDescription> residents, List<SimDescription> townies)
        {
            if (!Register.Settings.mAllowPaparazzi)
            {
                if (roleInfo.RoleType == Role.RoleType.Paparazzi) return 0;
            }

            RoleData.GetDataForCurrentWorld(roleInfo.RoleType, true);
            int num = 0x0;
            while (num < roleInfo.NumToFill)
            {
                Role toAdd = FillInWorldRoleEx(roleInfo.RoleType, null, residents, townies);
                if (toAdd == null)
                {
                    break;
                }

                RoleManager.sRoleManager.AddRole(toAdd);

                Register.ShowNotice(toAdd);

                Logger.AddTrace("Role Assigned:" + RoleToString(toAdd));

                num++;
            }
            return (roleInfo.NumToFill - num);
        }

        private Role FillInWorldRoleEx(Role.RoleType role, IRoleGiver roleGiver, List<SimDescription> residents, List<SimDescription> townies)
        {
            RoleData dataForCurrentWorld = RoleData.GetDataForCurrentWorld(role, true);

            if (dataForCurrentWorld == null)
            {
                // Safely catch the script error if the data for the role is missing
                try
                {
                    throw new NullReferenceException();
                }
                catch (Exception e)
                {
                    Common.Exception("Missing Role: " + role.ToString(), e);
                    return null;
                }
            }

            if (!dataForCurrentWorld.UseServobot || !GameUtils.IsFutureWorld())
            {
                List<SimDescription> possibles = (dataForCurrentWorld.FillRoleFrom == Role.RoleFillFrom.Residents) ? residents : townies;

                int chance = RandomUtil.GetInt(possibles.Count - 0x1);
                for (int i = 0x0; i < possibles.Count; i++)
                {
                    int num4 = (i + chance) % possibles.Count;

                    SimDescription sim = possibles[num4];

                    if (SimTypes.IsSelectable(sim)) continue;

                    if (!RoleEx.IsSimValidForAnyRole(sim, role)) continue;

                    if (RoleEx.IsSimGoodForRole(sim, dataForCurrentWorld, roleGiver))
                    {
                        possibles.Remove(sim);

                        try
                        {
                            if (sim.CreatedSim == null)
                            {
                                Lot lot = null;
                                if (roleGiver != null)
                                {
                                    lot = roleGiver.LotCurrent;
                                }

                                if (FixInvisibleTask.InstantiateOffLot(sim, lot, null) == null) continue;
                            }
                            else
                            {
                                sim.CreatedSim.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                            };

                            // Remove all career outfits prior to creating the role, if the role requires such an outfit it can create it again
                            sim.RemoveOutfits(OutfitCategories.Career, true);

                            if (roleGiver != null)
                            {
                                roleGiver.CurrentRole = null;
                            }

                            using (MiniSims.HomeworldReversion reversion = new MiniSims.HomeworldReversion(sim))
                            {
                                return Role.CreateRole(dataForCurrentWorld, sim, roleGiver);
                            }
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }
                    }
                }
            }
            else
            {
                SimDescription simDescription = null;
                foreach (SimDescription choice in Household.ServobotHousehold.SimDescriptions)
                {
                    if (choice == null) continue;
                    
                    if (choice.HasActiveRole) continue;

                    if (choice.Service != null) continue;

                    choice.AssignedRole = null;

                    if (choice.CreatedSim == null)
                    {
                        Lot lot = null;
                        if (roleGiver != null)
                        {
                            lot = roleGiver.LotCurrent;
                        }

                        if (FixInvisibleTask.InstantiateOffLot(choice, lot, null) == null) continue;
                    }

                    simDescription = choice;
                    break;
                }

                if (simDescription == null)
                {
                    CASAgeGenderFlags gender = RandomUtil.CoinFlip() ? CASAgeGenderFlags.Female : CASAgeGenderFlags.Male;
                    simDescription = OccultRobot.MakeRobot(RoleManager.GetRobotRoleUniformKey(role, gender));
                    if (simDescription != null)
                    {
                        Household.ServobotHousehold.Add(simDescription);

                        Lot lot = null;
                        if (roleGiver != null)
                        {
                            lot = roleGiver.LotCurrent;
                        }

                        if (FixInvisibleTask.InstantiateOffLot(simDescription, lot, null) == null)
                        {
                            return null;
                        }

                        Service.AddRobotTraitChips(simDescription);
                    }
                }

                return Role.CreateRole(dataForCurrentWorld, simDescription, roleGiver);
            }

            return null;
        }

        private List<IRoleGiver> FillRoleGiverObjectsEx(List<SimDescription> residents, List<SimDescription> townies)
        {
            Dictionary<ulong, List<Role.RoleType>> lotIdToPerLotRoles = new Dictionary<ulong, List<Role.RoleType>>();

            List<IRoleGiver> results = new List<IRoleGiver>(Sims3.Gameplay.Queries.GetObjects<IRoleGiver>());
            for (int i = results.Count - 1; i >= 0; i--)
            {                
                IRoleGiver giver = results[i];                

                if (Register.Settings.mDisabledAssignment.ContainsKey(giver.ObjectId))
                {                    
                    results.RemoveAt(i);
                    continue;
                }

                Lot lotCurrent = giver.LotCurrent;                

                if ((lotCurrent == null) || (!lotCurrent.IsCommunityLot))
                {                    
                    results.RemoveAt(i);
                    continue;
                }
                else if (!giver.InWorld)
                {                    
                    results.RemoveAt(i);
                    continue;
                }                

                Role role = giver.CurrentRole;
                if ((role != null) && (!RoleEx.IsSimGood(role)))
                {                    
                    SafeRemoveSimFromRole("E", role);

                    role = null;
                }

                if (role != null)
                {
                    IRoleGiverOneNpcPerLot objAsOnePerLot = giver as IRoleGiverOneNpcPerLot;
                    if ((objAsOnePerLot != null) && objAsOnePerLot.DoesRoleLotTypeMatch(objAsOnePerLot.LotCurrent))
                    {                       
                        RecordOnePerLotRole(objAsOnePerLot, objAsOnePerLot.LotCurrent.LotId, lotIdToPerLotRoles);
                    }                    

                    results.RemoveAt(i);
                }
                else if (giver.RoleType == Role.RoleType.None)
                {                    
                    results.RemoveAt(i);
                }
            }

            for (int i = results.Count - 1; i >= 0; i--)
            {
                IRoleGiver giver = results[i];                

                if (!IsValidTimeForRole(giver)) continue;                

                Lot lotCurrent = giver.LotCurrent;

                if ((lotCurrent.CommercialLotSubType == CommercialLotSubType.kEP11_BaseCampFuture) && (giver is IFutureFoodSynthesizer))
                {                    
                    results.RemoveAt(i);
                    continue;
                }
                if ((lotCurrent.CommercialLotSubType == CommercialLotSubType.kEP10_Resort) && (giver is IResortStaffedObject))
                {                    
                    results.RemoveAt(i);
                    continue;
                }

                IRoleGiverOneNpcPerLot lot3 = giver as IRoleGiverOneNpcPerLot;
                if (lot3 != null)
                {
                    if (lot3.DoesRoleLotTypeMatch(giver.LotCurrent))
                    {
                        List<Role.RoleType> list3;
                        if (lotIdToPerLotRoles.TryGetValue(giver.LotCurrent.LotId, out list3) && list3.Contains(lot3.RoleType))
                        {                            
                            results.RemoveAt(i);
                            continue;
                        }
                    }
                    else
                    {                        
                        results.RemoveAt(i);
                        continue;
                    }
                }

                Role role = FillInWorldRoleEx(giver.RoleType, giver, residents, townies);
                if (role != null)
                {                    
                    results.RemoveAt(i);

                    RoleManager.sRoleManager.AddRole(role);
                    giver.CurrentRole = role;

                    Logger.AddTrace("Role Assigned:" + RoleToString(role));

                    if (lot3 != null)
                    {
                        RecordOnePerLotRole(lot3, giver.LotCurrent.LotId, lotIdToPerLotRoles);
                    }

                    Register.ShowNotice(role);

                    SpeedTrap.Sleep();
                }
            }            

            return results;
        }

        public static bool AllowForeignSim(MiniSimDescription sim)
        {
            if (!sActiveFamilyCheck.Valid)
            {
                return RoleManagerEx.sVacationWorlds.Contains(sim.HomeWorld);
            }
            else
            {
                if (sActiveFamilyCheck.Invoke<bool>(new object[] { sim.SimDescriptionId }))
                {
                    return false;
                }
            }

            if (!Register.Settings.mAllowHomeworldTourists)
            {
                if (sHomeworldCheck.Invoke<bool>(new object[] { sim.SimDescriptionId })) return false;
            }

            if (Register.Settings.mDisabledTouristWorlds.ContainsKey(sim.HomeWorld))
            {
                return false;
            }

            return true;
        }

        private Role FillForeignRoleEx(Role.RoleType role, RoleData data, List<MiniSimDescription> foreignMSDs)
        {
            SimDescription simDescription = null;
            MiniSimDescription simDesc = null;
            if ((foreignMSDs != null) && (foreignMSDs.Count > 0x0))
            {
                int @int = RandomUtil.GetInt(foreignMSDs.Count - 0x1);
                int num2 = 0x0;
                while (num2 < foreignMSDs.Count)
                {
                    int num3 = (num2 + @int) % foreignMSDs.Count;
                    MiniSimDescription s = foreignMSDs[num3];
                    if (!Role.IsSimValidForAnyRole(s) || s.Instantiated )
                    {
                        foreignMSDs.Remove(s);
                    }
                    else if (!AllowForeignSim(s))
                    {
                        foreignMSDs.Remove(s);
                    }
                    else
                    {
                        if (RoleEx.IsSimGoodForRole(s, data, null))
                        {
                            foreignMSDs.Remove(s);

                            if (s.CASGenealogy == null)
                            {
                                s.mGenealogy = new Genealogy(s.FullName);
                                s.mGenealogy.mMiniSim = s;
                            }

                            simDescription = MiniSims.UnpackSimAndUpdateRel(s);

                            //bool flag = ((data.Type == Role.RoleType.Proprietor) && (simDescription != null)) && simDescription.OccultManager.HasAnyOccultType();
                            if (simDescription != null)
                            {
                                try
                                {
                                    if (simDescription != null)
                                    {
                                        simDesc = s;
                                        switch(role)
                                        {
                                            case Role.RoleType.Explorer:
                                                OutfitCategories[] categories = new OutfitCategories[] { OutfitCategories.Naked, OutfitCategories.Sleepwear };
                                                simDescription.BuildOutfitsOfType(categories, "explorer");
                                                break;
                                            case Role.RoleType.FutureHobo:
                                                OutfitCategories[] categoriesArray2 = new OutfitCategories[] { OutfitCategories.Naked, OutfitCategories.Sleepwear };
                                                simDescription.BuildOutfitsOfType(categoriesArray2, "hobo");
                                                break;
                                        }

                                        break;
                                    }
                                }
                                catch (ResetException)
                                {
                                    throw;
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(simDescription, null, "BuildOutfitsOfType", e);
                                }
                            }

                            SpeedTrap.Sleep();
                            continue;
                        }
                        num2++;
                    }
                }
            }

            // Check to Instance and CASModel as they are used during Instantiate()
            if ((simDescription == null) && (Sims3.UI.Responder.Instance != null) && (Sims3.UI.Responder.Instance.CASModel != null))
            {
                try
                {
                    SimUtils.SimCreationSpec spec = new SimUtils.SimCreationSpec();
                    spec.Gender = RandomUtil.CoinFlip() ? CASAgeGenderFlags.Female : CASAgeGenderFlags.Male;
                    spec.Age = CASAgeGenderFlags.Adult;
                    WorldName randomVacationWorld = GetRandomVacationWorld(role);
                    spec.GivenName = SimUtils.GetRandomGivenName(spec.IsMale, randomVacationWorld);
                    spec.Normalize();
                    uint outfitCategoriesToBuild = 0x2;
                    
                    switch (role)
                    {
                        case Role.RoleType.Explorer:
                        case Role.RoleType.FutureHobo:
                            outfitCategoriesToBuild |= 0x9;
                            break;
                    }
                    simDescription = spec.Instantiate(randomVacationWorld, outfitCategoriesToBuild);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception("FillForeignRole (A)", e);
                }
            }

            SpeedTrap.Sleep();

            if (simDescription != null)
            {
                try
                {
                    SimOutfit outfit = simDescription.GetOutfit(OutfitCategories.Everyday, 0x0);
                    if ((outfit != null) && outfit.IsValid)
                    {
                        Household.CreateTouristHousehold();

                        if (Household.TouristHousehold != null)
                        {
                            Household.TouristHousehold.AddTemporary(simDescription);
                            if (simDesc != null)
                            {
                                simDesc.Instantiated = true;
                                HudModel model = Sims3.UI.Responder.Instance.HudModel as HudModel;
                                if (model != null)
                                {
                                    model.OnSimCurrentWorldChanged(true, simDesc);
                                }

                                if (simDescription.AgingState != null)
                                {
                                    simDescription.AgingState.MergeTravelInformation(simDesc);
                                }
                            }


                            bool agingEnabled = simDescription.AgingEnabled;

                            try
                            {
                                simDescription.AgingEnabled = false;

                                return Role.CreateRole(data, simDescription, null);
                            }
                            catch (Exception e)
                            {
                                Common.Exception(simDescription, e);
                            }
                            finally
                            {
                                simDescription.AgingEnabled = agingEnabled;
                            }
                        }
                    }
                    simDescription.Dispose(false, true);
                }
                // Custom code
                catch (ResetException)
                {
                    throw;
                }
                // Custom code
                catch (Exception e)
                {
                    Common.Exception(simDescription, null, "FillForeignRole (B)", e);
                }
            }
            return null;
        }

        private void AddMoreInWorldSimsEx()
        {
            if (GameStates.IsTravelling) return;

            WorldName currentWorld = GameUtils.GetCurrentWorld();
            if (currentWorld == WorldName.Undefined) return;

            if (Household.GetPersistGroup() == null) return;

            if (Sims3.UI.Responder.Instance == null) return;

            if (Sims3.UI.Responder.Instance.CASModel == null) return;

            foreach (KeyValuePair<Role.RoleType, int> pair in mSimsToAdd)
            {
                if (!Register.Settings.mAllowPaparazzi)
                {
                    if (pair.Key == Role.RoleType.Paparazzi) continue;
                }

                RoleData dataForCurrentWorld = RoleData.GetDataForCurrentWorld(pair.Key, true);
                Role.RoleType role = pair.Key;
                int num = pair.Value;
                if (role != Role.RoleType.None)
                {
                    while (num > 0x0)
                    {
                        Lot lot = null;
                        if (dataForCurrentWorld.FillRoleFrom == Role.RoleFillFrom.Residents)
                        {
                            LotSelectionHelper helper = new LotSelectionHelper(0x1);
                            lot = LotManager.SelectRandomLotForNPCMoveIn(new LotManager.LotPredicate(helper.IsLotValidHomeForRoleSims));
                        }

                        if ((dataForCurrentWorld.FillRoleFrom == Role.RoleFillFrom.Residents) && (lot == null))
                        {
                            break;
                        }

                        if ((dataForCurrentWorld.FillRoleFrom == Role.RoleFillFrom.Townies) || ((dataForCurrentWorld.FillRoleFrom == Role.RoleFillFrom.Residents) && (lot != null)))
                        {
                            SimUtils.HouseholdCreationSpec spec = SimUtils.HouseholdCreationSpec.MakeFamilyForRoles(0x1, dataForCurrentWorld);
                            Household household = spec.Instantiate(currentWorld);

                            if ((dataForCurrentWorld.FillRoleFrom == Role.RoleFillFrom.Residents) && (lot != null))
                            {
                                lot.MoveIn(household);
                            }

                            Common.StringBuilder msg = new Common.StringBuilder("Role Immigration");

                            foreach (SimDescription description in household.AllSimDescriptions)
                            {
                                description.SendSimHome();

                                msg += Common.NewLine + description.FullName;
                            }

                            Common.DebugNotify(msg);
                        }

                        num--;
                        SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(5f, TimeUnit.Minutes));
                    }
                }
            }

            mSimsToAdd.Clear();
        }

        public static bool IsValidTimeForRole(IRoleGiver giver)
        {
            if (giver.LotCurrent == null) return false;            

            IRoleGiverExtended extended = giver as IRoleGiverExtended;
            if (extended != null)
            {                
                float startTime;
                float endTime;
                extended.GetRoleTimes(out startTime, out endTime);                
                return SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, startTime, endTime);
            }
            else
            {
                try
                {
                    RoleData roleData = RoleData.GetDataForCurrentWorld(giver.RoleType, true);

                    // Allow the exception to catch broken tuning errors
                    //if (roleData == null) return false;                    

                    if (!roleData.IsValidTimeForRole())
                    {                        
                        return false;
                    }                    

                    // Apply the same open/close hours to allow roles on the lot
                    Bartending.BarData data;
                    if (Bartending.TryGetBarData(giver.LotCurrent.GetMetaAutonomyType, out data) && data.HasHours())
                    {
                        // fix for when EA hibernate alarm goes off before the role is set to end... because...EA.
                        float newClose = 0f;
                        if (giver.CurrentRole != null && giver.CurrentRole.SimInRole == null)
                        {
                            newClose = data.HourClose - 1f;
                        }
                        else
                        {
                            newClose = data.HourClose;
                        }                        
                        return SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, data.HourOpen, newClose);
                    }
                    else
                    {                        
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(giver, "Type: " + giver.RoleType, "GetDataForCurrentWorld", e);
                    return false;
                }
            }
        }
        public static bool IsValidTimeForRole(Role role)
        {
            if (role.RoleGivingObject != null)
            {
                return IsValidTimeForRole(role.RoleGivingObject);
            }
            else
            {
                try
                {
                    RoleData roleData = RoleData.GetDataForCurrentWorld(role.Type, true);

                    // Allow the exception to catch broken tuning errors
                    //if (roleData == null) return false;

                    return roleData.IsValidTimeForRole();
                }
                catch (Exception e)
                {
                    Common.Exception(role.mSim, "Type: " + role.Type, "GetDataForCurrentWorld", e);
                    return false;
                }
            }
        }

        public override void Simulate()
        {
            NRaas.SpeedTrap.Begin();

            try
            {
                while (true)
                {
                    try
                    {
                        SpeedTrap.Sleep();
                    }
                    catch
                    {
                        return;
                    }

                    try
                    {
                        if (GameStates.IsTravelling) continue;

                        if (RoleManager.sRoleManager == null)
                        {
                            continue;
                        }

                        // Game loading, come back later
                        if ((Household.GetPersistGroup() == null) || (LoadSaveManager.GetGlobalObjectGroup() == null))
                        {
                            continue;
                        }

                        try
                        {
                            RoleManager.sRoleManager.VerifyRoleGivingObjectRoles();
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            if (sInitial)
                            {
                                Common.Exception("VerifyRoleGivingObjectRoles", e);
                            }
                            else
                            {
                                Common.DebugException("VerifyRoleGivingObjectRoles", e);
                            }

                            sInitial = false;
                            continue;
                        }

                        List<RoleToFill> foreignRoles = null;
                        List<RoleToFill> customRoles = null;

                        for (int i = 0; i < 2; i++)
                        {
                            List<RoleToFill> localRoles;
                            List<SimDescription> residents = Household.AllSimsLivingInWorld();
                            List<SimDescription> townies = new List<SimDescription>();

                            foreach (SimDescription townie in Household.EverySimDescription())
                            {
                                Career career = townie.Occupation as Career;
                                if (career != null)
                                {
                                    if ((career.Level1 != null) && (career.Level1.DayLength != 0)) continue;
                                }
                                else if (townie.Occupation != null)
                                {
                                    continue;
                                }

                                // Due to the chance they could be outside all day
                                if (OccultTypeHelper.HasType(townie, OccultTypes.Vampire)) continue;

                                // Due to the chance they are outside of water all day
                                if (OccultTypeHelper.HasType(townie, OccultTypes.Mermaid)) continue;

                                // Specialty sim
                                if (OccultTypeHelper.HasType(townie, OccultTypes.TimeTraveler)) continue;

                                if (!Register.Settings.mAllowResidentAssignment)
                                {
                                    if (townie.LotHome != null) continue;

                                    if (OccultTypeHelper.HasType(townie, OccultTypes.ImaginaryFriend)) continue;

                                    if (OccultTypeHelper.HasType(townie, OccultTypes.Genie)) continue;

                                    if (OccultTypeHelper.HasType(townie, OccultTypes.Mummy)) continue;
                                }
                                else
                                {
                                    if (SimTypes.IsService(townie.Household)) continue;
                                }

                                if (townie.IsZombie) continue;

                                if (townie.Household == null) continue;

                                if (townie.Household == Household.ActiveHousehold) continue;

                                if (!RoleEx.IsSimValidForAnyRole(townie, Role.RoleType.None)) continue;

                                if ((SimTypes.IsSkinJob(townie)) && (!townie.IsEP11Bot)) continue;

                                Role role = townie.AssignedRole;
                                if (role != null)
                                {
                                    if (ValidateRole(role) == ValidationResult.None) continue;

                                    SafeRemoveSimFromRole("C", role);
                                }

                                if (townie.IsHuman)
                                {
                                    SkillManager manager = townie.SkillManager;
                                    if (manager == null) continue;

                                    if (manager.AddElement(SkillNames.Bartending) == null) continue;

                                    if (manager.AddElement(SkillNames.Styling) == null) continue;

                                    if (manager.AddElement(SkillNames.Tattooing) == null) continue;
                                }

                                townies.Add(townie);
                            }

                            foreach (IRoleGiver giver in FillRoleGiverObjectsEx(residents, townies))
                            {
                                if (IsValidTimeForRole(giver))
                                {
                                    int num3;
                                    if (mSimsToAdd.TryGetValue(giver.RoleType, out num3))
                                    {
                                        mSimsToAdd[giver.RoleType] = num3 + 1;
                                    }
                                    else
                                    {
                                        mSimsToAdd.Add(giver.RoleType, 0x1);
                                    }
                                }
                            }

                            if (RoleManagerEx.UpdateAndGetRolesThatNeedPeople(RoleManager.sRoleManager, out localRoles, out foreignRoles, out customRoles))
                            {
                                foreach (RoleToFill fill in localRoles)
                                {
                                    int num4 = FillRolesEx(fill, residents, townies);
                                    if (num4 > 0x0)
                                    {
                                        if (mSimsToAdd.ContainsKey(fill.RoleType))
                                        {
                                            mSimsToAdd[fill.RoleType] += num4;
                                        }
                                        else
                                        {
                                            mSimsToAdd.Add(fill.RoleType, num4);
                                        }
                                    }
                                }
                            }

                            if (mSimsToAdd.Count == 0) break;

                            if (!Register.Settings.mAllowImmigration)
                            {
                                if (Register.Settings.mShowImmigrationNotice)
                                {
                                    string unFilledRoles = Common.Localize("UnfilledRoles:Title");

                                    Dictionary<Role.RoleType, int> totals = new Dictionary<Role.RoleType, int>();

                                    foreach (IRoleGiver giver in Sims3.Gameplay.Queries.GetObjects<IRoleGiver>())
                                    {
                                        int value = 0;
                                        if (totals.TryGetValue(giver.RoleType, out value))
                                        {
                                            totals[giver.RoleType] = value + 1;
                                        }
                                        else
                                        {
                                            totals[giver.RoleType] = 1;
                                        }
                                    }

                                    foreach (RoleToFill fill in localRoles)
                                    {
                                        RoleData data = RoleData.GetDataForCurrentWorld(fill.RoleType, false);
                                        if (data == null) continue;

                                        totals[fill.RoleType] = data.Number;
                                    }

                                    bool found = false;
                                    foreach (KeyValuePair<Role.RoleType, int> unfilled in mSimsToAdd)
                                    {
                                        if (unfilled.Value > 0)
                                        {
                                            int total = 0;
                                            totals.TryGetValue(unfilled.Key, out total);

                                            unFilledRoles += Common.NewLine + Common.Localize("UnfilledRoles:Element", false, new object[] { Register.GetRoleName(unfilled.Key), total - unfilled.Value, unfilled.Value });
                                            found = true;
                                        }
                                    }

                                    if (found)
                                    {
                                        Common.Notify(unFilledRoles);
                                    }
                                }

                                mSimsToAdd.Clear();
                                break;
                            }
                            else if (i == 0)
                            {
                                AddMoreInWorldSimsEx();

                                SpeedTrap.Sleep();
                            }
                        }

                        if (RoleManager.sRoleManager == null) continue;

                        if ((customRoles.Count > 0) && !TravelUtil.PlayerMadeTravelRequest)
                        {
                            foreach (RoleToFill fill2 in customRoles)
                            {
                                try
                                {
                                    RoleData dataForCurrentWorld = RoleData.GetDataForCurrentWorld(fill2.RoleType, true);

                                    int num5 = 0;
                                    while ((num5 < fill2.NumToFill) && !TravelUtil.PlayerMadeTravelRequest)
                                    {
                                        Role toAdd = FillCustomCreatedSimRole(fill2.RoleType, dataForCurrentWorld);
                                        if (toAdd != null)
                                        {
                                            RoleManager.sRoleManager.AddRole(toAdd);

                                            Register.ShowNotice(toAdd);

                                            SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(5f, TimeUnit.Minutes));
                                        }

                                        num5++;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception("RoleType: " + fill2.RoleType, e);
                                }
                            }
                        }

                        if ((Register.Settings.mAllowTourists) && (foreignRoles.Count > 0x0) && !TravelUtil.PlayerMadeTravelRequest)
                        {
                            List<MiniSimDescription> vacationWorldSimDescriptions = MiniSimDescriptionEx.GetVacationWorldSimDescriptions();
                            do
                            {
                                RoleToFill fill2 = foreignRoles[0x0];
                                RoleData data = RoleData.GetDataForCurrentWorld(fill2.RoleType, true);
                                bool flag = true;
                                if (data.IsValidTimeForRole())
                                {
                                    Role toAdd = FillForeignRoleEx(fill2.RoleType, data, vacationWorldSimDescriptions);
                                    if (toAdd != null)
                                    {
                                        RoleManager.sRoleManager.AddRole(toAdd);

                                        Register.ShowNotice(toAdd);

                                        fill2.NumToFill--;
                                        if (fill2.NumToFill > 0x0)
                                        {
                                            foreignRoles[0x0] = fill2;
                                            flag = false;
                                        }
                                    }
                                }

                                if (flag)
                                {
                                    foreignRoles.RemoveAt(0x0);
                                }

                                SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(5f, TimeUnit.Minutes));
                            }
                            while ((foreignRoles.Count > 0x0) && !TravelUtil.PlayerMadeTravelRequest);
                        }

                        SimulateRoles();
                    }
                    catch (ResetException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Simulate", e);
                    }
                }
            }
            finally
            {
                NRaas.SpeedTrap.End();
            }
        }

        protected static string RoleToString(Role role)
        {
            string msg = Common.NewLine + "  Type: " + role.Type;

            if (role.mSim == null)
            {
                msg += Common.NewLine + "  Sim: empty";
            }
            else
            {
                msg += Common.NewLine + "  Sim: " + role.mSim;
            }

            if (role.mRoleGivingObject == null)
            {
                msg += Common.NewLine + "  Object: empty";
            }
            else
            {
                msg += Common.NewLine + "  Object: " + role.mRoleGivingObject.GetType().ToString().Replace("Sims3.Gameplay.Objects.", "");
                msg += Common.NewLine + "  Guid: " + role.mRoleGivingObject.ObjectId;
            }

            return msg;
        }

        public enum ValidationResult : uint
        {
            None = 0x00,
            NoSim = 0x01,
            Sim = 0x02,
            Object = 0x04,
        }

        protected static ValidationResult ValidateRole(Role role)
        {
            ValidationResult result = ValidationResult.None;

            if (role == null) return result;

            if (role is Raccoon)
            {
                if (Register.Settings.mMaximumRaccoon < 0) return ValidationResult.Object;
            }
            else if (role is Deer)
            {
                if (Register.Settings.mMaximumDeer < 0) return ValidationResult.Object;
            }

            string msg = Common.NewLine + "Role:" + RoleToString(role);

            if (role.mSim == null)
            {
                result |= ValidationResult.NoSim;
            }
            else if (role.mSim.AssignedRole != null)
            {
                msg += Common.NewLine + "Sim Role:" + RoleToString(role.mSim.AssignedRole);

                if (!object.ReferenceEquals(role.mSim.AssignedRole, role))
                {
                    if (role.mSim.AssignedRole.mSim == role.mSim)
                    {
                        role.mSim.AssignedRole.mSim = null;
                        role.mSim.AssignedRole.mRoleGivingObject = null;

                        role.mSim.AssignedRole = role;

                        Logger.AddTrace("Reattached (A): " + RoleToString(role));
                    }
                    else
                    {
                        result |= ValidationResult.Sim;
                    }
                }
            }
            else
            {
                role.mSim.AssignedRole = role;                

                Logger.AddTrace("Reattached (B): " + RoleToString(role));
            }

            if (role.RoleGivingObject != null)
            {
                if (role.RoleGivingObject.LotCurrent == null)
                {
                    result |= ValidationResult.Object;
                }
                else if (!role.RoleGivingObject.InWorld)
                {
                    result |= ValidationResult.Object;
                }
                else if (role.RoleGivingObject.CurrentRole != null)
                {
                    msg += Common.NewLine + "Object Role:" + RoleToString(role.mRoleGivingObject.CurrentRole);

                    if (!object.ReferenceEquals(role.mRoleGivingObject.CurrentRole, role))
                    {
                        if (role.mRoleGivingObject.CurrentRole.mSim == role.mSim)
                        {
                            role.mRoleGivingObject.CurrentRole.mSim = null;
                            role.mRoleGivingObject.CurrentRole.mRoleGivingObject = null;

                            role.mRoleGivingObject.CurrentRole = role;

                            Logger.AddTrace("Reattached (C): " + RoleToString(role));
                        }
                        else
                        {
                            result |= ValidationResult.Object;
                        }
                    }
                }
            }

            if (result != ValidationResult.None)
            {
                Logger.AddTrace("Invalid Role:" + Common.NewLine + result + msg);
            }

            return result;
        }

        public static void SimulateRoles()
        {            
            SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(30f, TimeUnit.Minutes));

            bool paySims = (Register.Settings.mPayPerHour > 0);

            if ((RoleManager.sRoleManager == null) || (RoleManager.sRoleManager.mRoles == null)) return;

            List<Role> remove = new List<Role>();

            foreach (List<Role> roles in RoleManager.sRoleManager.mRoles.Values)
            {                
                if (roles == null) continue;

                foreach (Role role in roles)
                {                    
                    if (role == null) continue;                    

                    try
                    {
                        if (ValidateRole(role) != ValidationResult.None)
                        {                            
                            remove.Add(role);
                            continue;
                        }

                        if (IsValidTimeForRole(role))
                        {                            
                            role.mIsActive = true;

                            if (role.SimInRole != null)
                            {                                
                                bool shouldAge = false;

                                if (role.SimInRole.SimDescription.AgingState != null)
                                {
                                    shouldAge = AgingManager.Singleton.SimIsOldEnoughToTransition(role.SimInRole.SimDescription.AgingState);
                                }

                                if ((role.SimInRole.InteractionQueue.Count > 8) || (shouldAge))
                                {
                                    role.SimInRole.InteractionQueue.CancelAllInteractions();

                                    if (shouldAge)
                                    {
                                        AgeTransitionTask.Perform(role.SimInRole);

                                        Common.DebugNotify("Force Ageup: " + role.SimInRole.FullName);
                                    }
                                }
                            }

                            if (role.Type == Role.RoleType.Tourist)
                            {
                                RoleTouristEx.SimulateRole(role, 30f);
                            }
                            else
                            {
                                role.SimulateRole(30f);
                            }

                            if ((role.mSim == null) || (role.RoleGivingObject == null)) continue;

                            if (role.SimInRole == null)
                            {                                
                                bool success = false;
                                try
                                {
                                    FixInvisibleTask.InstantiateOffLot(role.mSim, role.RoleGivingObject.LotCurrent, null);
                                    success = true;
                                }
                                catch
                                { }

                                if ((!success) || (role.SimInRole == null))
                                {
                                    remove.Add(role);
                                    continue;
                                }
                            }

                            if ((role is RoleTourist) || (role is RoleExplorer)) continue;

                            InteractionInstance interaction = role.SimInRole.CurrentInteraction;

                            bool repush = (interaction == null);

                            if (!repush)
                            {
                                switch (role.Type)
                                {
                                    case Role.RoleType.Bartender:
                                        if (!(interaction.Target is IBarProfessional))
                                        {
                                            repush = true;
                                        }
                                        break;
                                    case Role.RoleType.Bouncer:
                                        if (!(interaction.Target is IVelvetRopes))
                                        {
                                            repush = true;
                                        }
                                        break;
                                    case Role.RoleType.Pianist:
                                        if (!(interaction.Target is IPiano))
                                        {
                                            repush = true;
                                        }
                                        break;
                                }
                            }

                            if (repush)
                            {                                
                                bool activeFound = false;

                                if (interaction != null)
                                {
                                    Sim target = interaction.Target as Sim;
                                    if (target != null)
                                    {
                                        activeFound = target.IsSelectable;
                                    }
                                    else
                                    {
                                        GameObject obj = interaction.Target as GameObject;
                                        if (obj != null)
                                        {
                                            foreach (Sim actor in obj.ActorsUsingMe)
                                            {
                                                if (actor.IsSelectable)
                                                {
                                                    activeFound = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!activeFound)
                                {                                    
                                    if (role.mSim.CreatedSim.LotCurrent != role.mRoleGivingObject.LotCurrent)
                                    {
                                        role.mSim.CreatedSim.InteractionQueue.Add(Sim.GoToLotThatSatisfiesMyRole.Singleton.CreateInstance(role.mSim.CreatedSim, role.mSim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), true, true));
                                    }
                                    role.mRoleGivingObject.AddRoleGivingInteraction(role.mSim.CreatedSim);
                                    role.mRoleGivingObject.PushRoleStartingInteraction(role.SimInRole);

                                    //Logger.AddTrace(role.mSim.FullName + " " + role.Type + " Repushed", role.mSim.CreatedSim.ObjectId);
                                }
                            }

                            if ((paySims) && (role.mSim.Household != null))
                            {
                                if (IsValidTimeForRole(role))
                                {
                                    int pay = Register.Settings.GetPayPerHour(role) / 2;
                                    if (role.mRoleGivingObject != null)
                                    {
                                        if (role.mRoleGivingObject.LotCurrent != role.SimInRole.LotCurrent)
                                        {
                                            pay = 0;
                                        }
                                    }

                                    if (pay > 0)
                                    {
                                        role.mSim.ModifyFunds(pay);

                                        if (role.Type == Role.RoleType.Bartender)
                                        {
                                            Skill skill = role.mSim.SkillManager.GetElement(SkillNames.Bartending);
                                            if (skill != null)
                                            {
                                                skill.UpdateXpForEarningMoney(pay);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (role.SimInRole != null)
                            {
                                role.EndRole();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(role.mSim, e);
                    }
                }
            }

            foreach (Role role in remove)
            {
                SafeRemoveSimFromRole("D", role);
            }
            
            RoleManager.sForceSimulate = false;
        }

        public class Logger : Common.TraceLogger<Logger>
        {
            static Logger sLogger = new Logger();

            public static void AddTrace(string msg)
            {
                sLogger.PrivateAddTrace(msg);

                Common.DebugNotify(msg);
            }
            public static void AddTrace(string msg, ObjectGuid id)
            {
                sLogger.PrivateAddTrace(msg);

                Common.DebugNotify(msg, id);
            }
            public static void AddError(string msg)
            {
                sLogger.PrivateAddError(msg);
            }

            protected override string Name
            {
                get { return "Role Log"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }
        }

        public class StartupTask : Common.AlarmTask
        {
            Dictionary<Role.RoleType, List<Role>> mRoles = null;

            public StartupTask()
                : base(1f, TimeUnit.Seconds)
            {
                if (RoleManager.sRoleManager != null)
                {
                    mRoles = RoleManager.sRoleManager.mRoles;

                    // Intentionally bounce the EA Role Manager in the most benign way possible
                    RoleManager.sRoleManager.mRoles = new Dictionary<Role.RoleType, List<Role>>();
                    RoleManager.sRoleManager.mRoles.Add(Role.RoleType.LocationMerchant, null);
                }
            }

            protected override void OnPerform()
            {
                if (!Sims3.SimIFace.Environment.HasEditInGameModeSwitch)
                {
                    Simulator.DestroyObject(RoleManager.sRoleManager.mRoleTask);
                    RoleManager.sRoleManager.mRoleTask = ObjectGuid.InvalidObjectGuid;

                    sTask = Simulator.AddObject(new RoleManagerTaskEx());
                }

                if (mRoles != null)
                {
                    RoleManager.sRoleManager.mRoles = mRoles;
                }
            }
        }
    }
}
