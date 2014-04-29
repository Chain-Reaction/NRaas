using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
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
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupFutureDescendantService : DelayedLoadupOption
    {
        public static bool wasRescheduled = false;

        public override void OnDelayedWorldLoadFinished()
        {
            RunStatic();
        }

        public static void RunStatic()
        {      
            Overwatch.Log("CleanupFutureDescendantService");

            FutureDescendantService instance = FutureDescendantService.GetInstance();
            if (instance != null)
            {              
                List<ulong> removeFromMap = new List<ulong>();
                List<ulong> simsToPossiblyVanish = new List<ulong>();
                List<ulong> simsToKeep = new List<ulong>();
                Dictionary<ulong, FutureDescendantService.FutureDescendantHouseholdInfo> houses = new Dictionary<ulong, FutureDescendantService.FutureDescendantHouseholdInfo>();

                foreach (ulong num in FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Keys)
                {
                    bool flag = false;
                    IMiniSimDescription ancestorSim = SimDescription.GetIMiniSimDescription(num);
                    if (ancestorSim == null)
                    {
                        flag = true;
                        removeFromMap.Add(num);
                    }                    

                    foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.DescendantHouseholdsMap[num])
                    {
                        Household descendantHousehold = info.DescendantHousehold;

                        if (descendantHousehold != null)
                        {
                            if (!houses.ContainsKey(info.mHouseholdId))
                            {
                                houses.Add(info.mHouseholdId, info);
                            }

                            foreach (SimDescription desc in descendantHousehold.SimDescriptions)
                            {
                                if (flag)
                                {
                                    if (!simsToKeep.Contains(desc.SimDescriptionId))
                                    {
                                        simsToPossiblyVanish.Add(desc.SimDescriptionId);
                                    }
                                }
                                else
                                {
                                    if (simsToPossiblyVanish.Contains(desc.SimDescriptionId))
                                    {
                                        simsToPossiblyVanish.Remove(desc.SimDescriptionId);
                                    }
                                    simsToKeep.Add(desc.SimDescriptionId);
                                }
                            }
                        }
                    }
                }

                foreach (ulong num in removeFromMap)
                {
                    FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Remove(num);

                    Overwatch.Log(" Missing Removed: " + num);
                }

                if (!GameUtils.IsFutureWorld())
                {
                    // because sims can possibly be deleted beyond here and I don't believe you can delete sims who are minisims while in the homeworld?
                    return;
                }
                else
                {
                    // this causes a script error if it deletes Sims while cleanup of relationships is running so let that run and come back in a bit...
                    if (!wasRescheduled)
                    {
                        new Common.AlarmTask(10, TimeUnit.Minutes, RunStatic);                        
                        wasRescheduled = true;
                        return;
                    }
                }                

                Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);
                foreach (SimDescription sim in sims.Values)
                {
                    if (sim.TraitManager == null) continue;

                    foreach (KeyValuePair<ulong, Trait> trait in sim.TraitManager.mValues)
                    {
                        if ((trait.Value.Guid == TraitNames.DescendantHiddenTrait) && (!simsToKeep.Contains(sim.SimDescriptionId)) && (sim.Household != null && !sim.Household.IsActive))
                        {
                            simsToPossiblyVanish.Add(sim.SimDescriptionId);
                            break;
                        }
                    }                    
                }
                
                foreach (ulong desc in simsToPossiblyVanish)
                {
                    IMiniSimDescription mini = SimDescription.GetIMiniSimDescription(desc);
                    if (mini != null)
                    {                        
                        Annihilation.Cleanse(mini);
                    }

                    if (houses.ContainsKey(mini.LotHomeId))
                    {
                        houses[mini.LotHomeId].mHouseholdMembers.Remove(desc);
                    }

                    Overwatch.Log(" Annihilated: " + mini.FullName);
                }

                FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo = new List<FutureDescendantService.FutureDescendantHouseholdInfo>();
                foreach (KeyValuePair<ulong, FutureDescendantService.FutureDescendantHouseholdInfo> houseInfo in houses)
                {
                    Household house = houseInfo.Value.DescendantHousehold;
                    if ((house != null) && (houseInfo.Value.mHouseholdMembers.Count == 0))
                    {
                        Annihilation.Cleanse(house);
                        Overwatch.Log(" Annihilated household: " + house.Name);
                    }
                    else
                    {
                        FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo.Add(houseInfo.Value);
                    }

                }

                FutureDescendantService.sPersistableData.InvalidDescendantHouseholdsInfo.Clear();
                houses = null;
                simsToPossiblyVanish = null;            
                simsToKeep = null;                
            }            
        }
    }
}
