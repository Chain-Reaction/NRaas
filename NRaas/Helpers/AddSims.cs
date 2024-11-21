using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class AddSims
    {
        List<SimDescription> mOldMembers;
        List<SimDescription> mNewMembers = new List<SimDescription>();

        Dictionary<Household, bool> mOldHouses = new Dictionary<Household, bool>();

        public AddSims(Household house, IEnumerable<IMiniSimDescription> miniSims, bool overStuff, bool transferFunds, bool dreamCatcher)
        {
            mOldMembers = new List<SimDescription>(CommonSpace.Helpers.Households.All(house));

            foreach (IMiniSimDescription iMiniSim in new List<IMiniSimDescription>(miniSims))
            {
                if (overStuff)
                {
                    if (iMiniSim.IsHuman)
                    {
                        if (CommonSpace.Helpers.Households.NumHumansIncludingPregnancy(house) >= 8)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (CommonSpace.Helpers.Households.NumPetsIncludingPregnancy(house) >= 6)
                        {
                            continue;
                        }
                    }
                }

                MiniSimDescription miniSim = null;

                bool lastActiveSim = false;

                SimDescription sim = iMiniSim as SimDescription;
                if (sim == null)
                {
                    miniSim = iMiniSim as MiniSimDescription;
                    if (miniSim == null) continue;

                    sim = MiniSims.UnpackSimAndUpdateRel(miniSim);
                    if (sim == null) continue;
                }
                else if (sim.CreatedSim == Sim.ActiveActor)
                {
                    if (Households.NumSims(sim.Household) == 1)
                    {
                        lastActiveSim = true;
                    }
                    else
                    {
                        LotManager.SelectNextSim();
                    }
                }

                Household oldHouse = sim.Household;
                if (oldHouse != null)
                {
                    if (oldHouse == Household.ActiveHousehold)
                    {
                        Household.RoommateManager.RemoveRoommateInternal(sim);
                    }

                    if ((transferFunds) && (oldHouse.NumMembers == 1))
                    {
                        int funds = oldHouse.FamilyFunds;

                        if (oldHouse.LotHome != null)
                        {
                            funds += oldHouse.LotHome.Cost;
                        }

                        house.ModifyFamilyFunds(funds);

                        Households.TransferData(house, oldHouse);
                    }

                    house.AddWardrobeToWardrobe(oldHouse.Wardrobe);
                    house.AddServiceUniforms(oldHouse.mServiceUniforms);

                    oldHouse.Remove(sim, !oldHouse.IsSpecialHousehold);

                    if (PetAdoption.sNeighborAdoption != null)
                    {
                        PetAdoption.sNeighborAdoption.mPetsToAdopt.Remove(sim);
                    }

                    if (sim.CreatedSim != null)
                    {
                        oldHouse.GetCaregiverRoutingMonitor(sim.CreatedSim.LotCurrent, true);
                    }
                }

                if ((oldHouse != null) && (!mOldHouses.ContainsKey(oldHouse)))
                {
                    mOldHouses.Add(oldHouse, true);
                }

                mNewMembers.Add(sim);

                if (house.Name == null)
                {
                    house.Name = sim.LastName;
                }

                if ((sim.IsDead) && (!sim.IsPlayableGhost))
                {
                    Urnstone grave = Urnstones.CreateGrave(sim, false);
                    if (grave != null)
                    {
                        Urnstones.GhostToPlayableGhost(grave, house, house.LotHome.EntryPoint());
                    }
                }
                else
                {
                    // Must be performed or the Household:Add() will bounce
                    if (!sim.IsValidDescription)
                    {
                        //Common.Notify("Not valid description");
                        sim.Fixup();
                    }

                    MiniSims.ProtectedAddHousehold(house, sim);

                    if (sim.IsPet)
                    {
                        foreach (PetPoolType type in Enum.GetValues(typeof(PetPoolType)))
                        {
                            if (PetPoolManager.IsPetInPoolType(sim, type))
                            {
                                PetPoolManager.RemovePet(type, sim, true);
                            }
                        }
                    }

                    if ((CarNpcManager.Singleton != null) && (CarNpcManager.Singleton.NpcDriversManager != null))
                    {
                        if (CarNpcManager.Singleton.NpcDriversManager.mDescPools != null)
                        {
                            for (int i = 0; i < CarNpcManager.Singleton.NpcDriversManager.mDescPools.Length; i++)
                            {
                                Stack<SimDescription> stack = CarNpcManager.Singleton.NpcDriversManager.mDescPools[i];
                                if (stack == null) continue;

                                List<SimDescription> list = new List<SimDescription>();

                                foreach (SimDescription stackSim in stack)
                                {
                                    if (stackSim == sim)
                                    {
                                        while (stack.Count > 0)
                                        {
                                            list.Add(stack.Pop());
                                        }

                                        list.Remove(sim);

                                        for (int j = list.Count - 1; j >= 0; j--)
                                        {
                                            stack.Push(list[j]);
                                        }
                                    }
                                }
                            }
                        }

                        if ((sim.CreatedSim != null) && (CarNpcManager.Singleton.NpcDriversManager.mNpcDrivers != null))
                        {
                            CarNpcManager.Singleton.NpcDriversManager.mNpcDrivers.Remove(sim.CreatedSim);
                        }
                    }
                }

                if (house.LotHome != null)
                {
                    Instantiation.EnsureInstantiate(sim, house.LotHome);
                }

                if (miniSim != null)
                {
                    (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).OnSimCurrentWorldChanged(true, miniSim);
                }

                // Homeless Sims aren't being pushed to the aging manager but don't want to push Sims who are traveling on to it
                if((!sim.AgingEnabled && oldHouse.LotHome == null && !GameUtils.IsOnVacation()) || miniSim != null)
                {
                    //Common.Notify("Pushing aging");
                    AgingManager.Singleton.AddSimDescription(sim);

                    if (miniSim != null)
                    {
                        sim.AgingState.MergeTravelInformation(miniSim);
                    }

                    sim.SetFlags(SimDescription.FlagField.AgingEnabled, true);                    
                }

                try
                {
                    if (sim.Service != null)
                    {
                        sim.Service.EndService(sim);
                    }

                    if (oldHouse.LotHome == null)
                    {
                        // Some homeless Sims in Bridgeport have their homeworld set wrong it seems
                        //Common.Notify("Resetting HW");
                        sim.mHomeWorld = GameUtils.GetCurrentWorld();
                    }

                    if (sim.CreatedSim != null)
                    {
                        sim.Fixup();

                        sim.CreatedSim.BuffManager.RemoveElement(BuffNames.StrayPet);

                        DreamCatcher.AdjustSelectable(sim, sim.Household == Household.ActiveHousehold, dreamCatcher);

                        sim.CreatedSim.SetObjectToReset();

                        sim.CreatedSim.Motives.RecreateMotives(sim.CreatedSim);

                        Sim.MakeSimGoHome(sim.CreatedSim, false);

                        if (lastActiveSim)
                        {
                            PlumbBob.DoSelectActor(sim.CreatedSim, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }
        }

        public void SendEvents()
        {
            foreach (Household old in mOldHouses.Keys)
            {
                if (CommonSpace.Helpers.Households.NumSims(old) == 0) continue;

                EventTracker.SendEvent(new HouseholdUpdateEvent(EventTypeId.kHouseholdSplit, old));
            }

            foreach (SimDescription oldSim in mOldMembers)
            {
                foreach (SimDescription newSim in mNewMembers)
                {
                    if (oldSim == newSim) continue;

                    if (newSim.CreatedSim == null) continue;

                    if (oldSim.CreatedSim == null) continue;

                    EventTracker.SendEvent(EventTypeId.kMovedInTogether, newSim.CreatedSim, oldSim.CreatedSim);
                    EventTracker.SendEvent(EventTypeId.kMovedInTogether, oldSim.CreatedSim, newSim.CreatedSim);
                }
            }
        }
    }
}

