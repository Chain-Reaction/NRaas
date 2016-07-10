using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.RegisterSpace.Tasks;
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
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class RoleManagerEx
    {
        public static readonly List<WorldName> sVacationWorlds = new List<WorldName>(new WorldName[] { WorldName.France, WorldName.Egypt, WorldName.China });

        // From RoleData
        public static List<RoleData> GetRolesForWorld(bool includeDefaults)
        {
            List<RoleData> list = new List<RoleData>();
            foreach (Role.RoleType type in Role.RoleTypes)
            {
                RoleData dataForCurrentWorld = RoleData.GetDataForCurrentWorld(type, includeDefaults);
                if (dataForCurrentWorld == null) continue;

                if (GameUtils.IsInstalled(dataForCurrentWorld.ValidProductVersion))
                {
                    list.Add(dataForCurrentWorld);
                }
            }

            return list;
        }

        public static bool UpdateAndGetRolesThatNeedPeople(RoleManager ths, out List<RoleToFill> rolesThatNeedInWorldSim, out List<RoleToFill> rolesThatNeedOtherWorldSim, out List<RoleToFill> rolesThatNeedCustomCreatedSim)
        {
            rolesThatNeedInWorldSim = new List<RoleToFill>();
            rolesThatNeedOtherWorldSim = new List<RoleToFill>();
            rolesThatNeedCustomCreatedSim = new List<RoleToFill>();

            if (ths == null) return false;

            Common.StringBuilder msg = new Common.StringBuilder("Role Check:");

            foreach (RoleData data in GetRolesForWorld(true))
            {
                switch(data.Type)
                {
                    case Role.RoleType.Paparazzi:
                        if (!Register.Settings.mAllowPaparazzi || Register.Settings.mMaximumPaparazzi <= 0) continue;
                        break;
                    case Role.RoleType.Explorer:
                        if (!sVacationWorlds.Contains(GameUtils.GetCurrentWorld())) continue;
                        break;
                    case Role.RoleType.Tourist:
                        if (sVacationWorlds.Contains(GameUtils.GetCurrentWorld())) continue;
                        break;
                }

                List<Role> list2;
                bool globalProperTimeForRole = data.IsValidTimeForRole();
                int num = 0x0;

                msg += Common.NewLine + data.Type + " " + data.World + " " + data.Number + " " + globalProperTimeForRole + " " + data.FillRoleFrom;

                if (ths.mRoles.TryGetValue(data.Type, out list2))
                {
                    int num2 = 0x0;
                    while (num2 < list2.Count)
                    {
                        bool found = true;

                        bool localProperTimeForObject = globalProperTimeForRole;
                        Role role = list2[num2];
                        IRoleGiverExtended roleGivingObject = role.RoleGivingObject as IRoleGiverExtended;
                        if (roleGivingObject != null)
                        {
                            if ((roleGivingObject.LotCurrent == null) || (roleGivingObject.HasBeenDestroyed))
                            {
                                RoleManagerTaskEx.SafeRemoveSimFromRole("A", role);

                                found = false;
                            }
                            else
                            {
                                float startHour;
                                float endHour;
                                roleGivingObject.GetRoleTimes(out startHour, out endHour);
                                if (!SimClock.IsTimeBetweenTimes(startHour, endHour))
                                {                                    
                                    localProperTimeForObject = false;
                                }
                            }
                        }

                        if (found)
                        {
                            if (!RoleEx.IsSimGood(role))
                            {
                                RoleManagerTaskEx.SafeRemoveSimFromRole("A", role);

                                found = false;
                            }
                            else if (localProperTimeForObject && (!role.IsTryingToStartRole) && (role.SimInRole == null))
                            {
                                bool success = false;

                                if (Household.ActiveHousehold != null)
                                {
                                    try
                                    {
                                        Sim sim = Instantiation.PerformOffLot(role.mSim, Household.ActiveHousehold.LotHome, null);
                                        if (sim != null)
                                        {
                                            success = true;

                                            Common.DebugNotify("Added To Game\n" + role.mSim.FullName, sim);
                                        }
                                    }
                                    catch (ResetException)
                                    {
                                        throw;
                                    }
                                    catch (Exception e)
                                    {
                                        Common.DebugException(role.mSim, e);
                                    }
                                }

                                if ((!success) || (role.SimInRole == null))
                                {
                                    RoleManagerTaskEx.SafeRemoveSimFromRole("B", role);

                                    found = false;
                                }
                            }
                            else if ((role.RoleGivingObject != null) && (!role.RoleGivingObject.InWorld))
                            {
                                RoleManagerTaskEx.SafeRemoveSimFromRole("C", role);

                                found = false;
                            }
                        }

                        if (found)
                        {
                            if (role.RoleGivingObject == null)
                            {
                                num++;
                            }
                            num2++;
                        }
                    }
                }

                if ((num < data.Number) && globalProperTimeForRole)
                {                    
                    RoleToFill item = new RoleToFill(data.Type, data.Number - num);
                    if ((data.FillRoleFrom == Role.RoleFillFrom.Residents) || (data.FillRoleFrom == Role.RoleFillFrom.Townies))
                    {
                        rolesThatNeedInWorldSim.Add(item);
                    }
                    else if (data.FillRoleFrom == Role.RoleFillFrom.PeopleWhoDontLiveInThisWorld)
                    {
                        rolesThatNeedOtherWorldSim.Add(item);
                    }
                    else if (data.FillRoleFrom == Role.RoleFillFrom.CustomCreatedSim)
                    {
                        bool allow = true;
                        switch (data.Type)
                        {
                            case Role.RoleType.Deer:
                                if (Register.Settings.GetMaximumPoolSize(PetPoolType.NPCDeer) <= 0)
                                {
                                    allow = false;
                                }
                                break;
                            case Role.RoleType.Raccoon:
                                if (Register.Settings.GetMaximumPoolSize(PetPoolType.NPCRaccoon) <= 0)
                                {
                                    allow = false;
                                }
                                break;
                        }

                        if (allow)
                        {
                            rolesThatNeedCustomCreatedSim.Add(item);
                        }
                    }
                }
            }

            Common.DebugNotify(msg);

            return true;
        }
    }
}