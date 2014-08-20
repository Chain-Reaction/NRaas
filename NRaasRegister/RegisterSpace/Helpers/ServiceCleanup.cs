using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Rewards;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Dialogs;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class ServiceCleanup : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Task();
        }

        public class Task : Common.AlarmTask
        {
            public Task()
                : base(2, DaysOfTheWeek.All)
            { }

            protected static void CommonCorrections(SimDescription sim)
            {
                if (sim.AgingState == null)
                {
                    AgingManager.Singleton.AddSimDescription(sim);
                }
            }

            protected override void OnPerform()
            {
                if (Register.Settings.mDisableServiceCleanup) return;

                Perform();
            }

            public static void Perform()
            {
                Dictionary<ulong, bool> taxiDrivers = new Dictionary<ulong, bool>();

                if ((CarNpcManager.Singleton != null) && (CarNpcManager.Singleton.NpcDriversManager != null))
                {
                    for (int i=0; i<CarNpcManager.Singleton.NpcDriversManager.mDescPools.Length; i++)
                    {
                        Stack<SimDescription> stack = CarNpcManager.Singleton.NpcDriversManager.mDescPools[i];

                        if (stack == null) continue;

                        NpcDriversManager.NpcDrivers type = (NpcDriversManager.NpcDrivers)(i + 0x95d01441);

                        while (stack.Count > 10)
                        {
                            SimDescription sim = stack.Pop();

                            try
                            {
                                AttemptServiceDisposal(sim, false, "Too Many " + type);
                            }
                            catch (Exception e)
                            {
                                Common.Exception(sim, e);
                            }
                        }

                        foreach (SimDescription sim in stack)
                        {
                            if (taxiDrivers.ContainsKey(sim.SimDescriptionId)) continue;

                            taxiDrivers.Add(sim.SimDescriptionId, true);
                        }
                    }
                }

                Dictionary<ulong, bool> statueSims = new Dictionary<ulong, bool>();

                CauseEffectService instance = CauseEffectService.GetInstance();
                if (instance != null)
                {
                    statueSims[instance.GetTimeTravelerSimID()] = true;

                    List<ITimeStatueUiData> timeStatueData = instance.GetTimeAlmanacTimeStatueData();
                    if (timeStatueData != null)
                    {
                        foreach (ITimeStatueUiData data in timeStatueData)
                        {
                            TimeStatueRecordData record = data as TimeStatueRecordData;
                            if (record == null) continue;

                            statueSims[record.mRecordHolderId] = true;
                        }
                    }
                }

                foreach (SimDescription sim in new List<SimDescription> (Households.All(Household.NpcHousehold)))
                {
                    try
                    {
                        if (SimTypes.IsDead(sim)) continue;

                        if (taxiDrivers.ContainsKey(sim.SimDescriptionId)) continue;

                        CommonCorrections(sim);

                        if (sim.AssignedRole != null) continue;

                        if (SimTypes.InServicePool(sim, ServiceType.GrimReaper)) continue;

                        if (statueSims.ContainsKey(sim.SimDescriptionId)) continue;

                        uint spanLevel = 0;
                        OptionsModel.GetOptionSetting("AgingInterval", out spanLevel);

                        float averageElderAge = AgingManager.Singleton.GetAverageSimLifespanInDays((int)spanLevel);

                        ServiceType type = ServiceType.None;
                        if (sim.CreatedByService != null)
                        {
                            type = sim.CreatedByService.ServiceType;
                        }                        

                        if (AgingManager.Singleton.GetCurrentAgeInDays(sim) > averageElderAge)
                        {
                            AttemptServiceDisposal(sim, true, "Too Old " + type);
                        }
                        else if (SimTypes.IsOccult(sim, OccultTypes.ImaginaryFriend))
                        {
                            bool found = false;
                            foreach (ImaginaryDoll doll in Sims3.Gameplay.Queries.GetObjects<ImaginaryDoll>())
                            {
                                if (sim.SimDescriptionId == doll.mLiveStateSimDescId)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found) continue;

                            AttemptServiceDisposal(sim, false, "Imaginary");
                        }
                        else if (SimTypes.IsOccult(sim, OccultTypes.Genie))
                        {
                            bool found = false;
                            foreach (GenieLamp lamp in Sims3.Gameplay.Queries.GetObjects<GenieLamp>())
                            {
                                if (sim == lamp.mGenieDescription)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found) continue;

                            AttemptServiceDisposal(sim, false, "Genie");
                        }
                        else if (sim.IsBonehilda)
                        {
                            bool found = false;
                            foreach (BonehildaCoffin coffin in Sims3.Gameplay.Queries.GetObjects<BonehildaCoffin>())
                            {
                                if (sim == coffin.mBonehilda)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found) continue;

                            AttemptServiceDisposal(sim, false, "BoneHilda");
                        }
                        else if ((sim.AssignedRole == null) && (!SimTypes.InServicePool(sim)))
                        {
                            AttemptServiceDisposal(sim, false, "No Purpose");
                        }
                        else if ((type != ServiceType.None) && (sim.Age & ServiceNPCSpecifications.GetAppropriateAges(type.ToString())) == CASAgeGenderFlags.None)
                        {                            
                            AttemptServiceDisposal(sim, false, "Wrong Age " + type);
                        }
                        else if (!ServiceNPCSpecifications.ShouldUseServobot(type.ToString()) && sim.IsEP11Bot)
                        {                            
                            AttemptServiceDisposal(sim, false, "Not EP11 Bot " + type);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }

                foreach (SimDescription sim in new List<SimDescription> (Households.All(Household.PetHousehold)))
                {
                    try
                    {
                        if (SimTypes.IsDead(sim)) continue;

                        CommonCorrections(sim);

                        if ((sim.IsDeer) || (sim.IsRaccoon))
                        {
                            if (sim.AssignedRole == null)
                            {
                                AttemptServiceDisposal(sim, false, "Roleless");
                            }
                            else if (sim.Elder)
                            {
                                AttemptServiceDisposal(sim, true, "Elder");
                            }
                        }
                        else 
                        {
                            bool noPool;
                            PetPoolType pool = SimTypes.GetPetPool(sim, out noPool);

                            if ((!noPool) && (pool == PetPoolType.None))
                            {
                                AttemptServiceDisposal(sim, false, "Not Pooled");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
        }

        public static bool AttemptServiceDisposal(SimDescription sim, bool oldAge, string reason)
        {
            if (sim == null)
            {
                return false;
            }

            bool hasChildren = (Relationships.GetChildren(sim).Count > 0);

            if (!oldAge)
            {
                if (hasChildren)
                {
                    return false;
                }

                if ((sim.IsHuman) && (sim.TeenOrAbove))
                {
                    bool found = false;
                    foreach (SimDescription other in Households.Humans(sim.Household))
                    {
                        if (other == sim) continue;

                        if (other.TeenOrAbove)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return false;
                    }
                }

                foreach (Household house in Household.GetHouseholdsLivingInWorld())
                {
                    if (Notifications.HasSignificantRelationship(house, sim))
                    {
                        return false;
                    }
                }
            }

            if (hasChildren)
            {
                if (!Annihilation.Perform(sim, false))
                {
                    return false;
                }

                if (Common.kDebugging)
                {
                    Common.DebugNotify("Disposed: " + sim.FullName + " (" + sim.Species + ")" + Common.NewLine + reason);
                }
            }
            else
            {
                if (!Annihilation.Cleanse(sim))
                {
                    return false;
                }

                if (Common.kDebugging)
                {
                    Common.DebugNotify("Cleansed: " + sim.FullName + " (" + sim.Species + ")" + Common.NewLine + reason);
                }
            }

            return true;
        }
    }
}
