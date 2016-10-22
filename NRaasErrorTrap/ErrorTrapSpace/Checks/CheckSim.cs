using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckSim : Check<Sim>
    {
        //Dictionary<Sim, ResetSimTask.CreationProtection> mData = new Dictionary<Sim, ResetSimTask.CreationProtection>();

        Dictionary<Sim, bool> mInited = new Dictionary<Sim, bool>();

        //Sim.OnLotChanged mLotChangedEvents = null;

        protected override bool PrePerform(Sim sim, bool postLoad)
        {
            if (mInited.ContainsKey(sim)) return true;
            mInited.Add(sim, true);

            /*
            mLotChangedEvents = Sim.sOnLotChangedDelegates;
            Sim.sOnLotChangedDelegates = null;

            try
            {
                ErrorTrap.CheckTravelData();

                SimDescription simDesc = sim.SimDescription;

                try
                {
                    if (simDesc == null)
                    {
                        SimInitParameters objectInitParameters = Simulator.GetObjectInitParameters(sim.ObjectId) as SimInitParameters;
                        if ((objectInitParameters != null) && (objectInitParameters.mSimDescription != null))
                        {
                            Household.HouseholdSimsChangedCallback changedCallback = null;

                            simDesc = objectInitParameters.mSimDescription;

                            try
                            {
                                if (simDesc.Household != null)
                                {
                                    changedCallback = simDesc.Household.HouseholdSimsChanged;
                                    simDesc.Household.HouseholdSimsChanged = null;
                                }

                                sim.SetSimDescription(simDesc);

                                //DebugLogCorrection("Sim Description Added " + sim.FullName);
                            }
                            finally
                            {
                                if (simDesc.Household != null)
                                {
                                    simDesc.Household.HouseholdSimsChanged = changedCallback;
                                }
                            }
                        }
                    }

                    simDesc = sim.SimDescription;
                    if (simDesc != null)
                    {
                        if ((simDesc.Genealogy == null) || (simDesc.TraitManager == null) || (simDesc.SkillManager == null) || (simDesc.CelebrityManager == null) || (simDesc.OccultManager == null))
                        {
                            if (!postLoad)
                            {
                                DebugLogCorrection("Fixup Performed " + sim.FullName);

                                simDesc.Fixup();
                            }
                        }

                        Corrections.CleanupRelationship(simDesc, LogCorrection);

                        mData.Remove(sim);
                        mData.Add(sim, new ResetSimTask.CreationProtection(simDesc, null, true, !postLoad, false));
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
                
                if (simDesc == null) return false;

                if (sim.Autonomy == null)
                {
                    sim.mAutonomy = new Autonomy(sim.FullName, sim);

                    //DebugLogCorrection("Autonomy Applied " + sim.FullName);
                }

                if (simDesc.Household == null)
                {
                    Household house = Household.PetHousehold;
                    if ((simDesc.IsHuman) || (house == null))
                    {
                        house = Household.NpcHousehold;
                    }

                    // Several Sim::OnCreation() calls require a household, so set on right now
                    if (house != null)
                    {
                        try
                        {
                            if (house.AllSimDescriptions.Contains(simDesc))
                            {
                                simDesc.OnHouseholdChanged(house, true);
                            }
                            else
                            {
                                house.Add(simDesc);

                                DebugLogCorrection("Service Household Applied " + sim.FullName);
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }
                    }
                }

                if (postLoad)
                {
                    if ((sim.Household != null) && (!SimTypes.IsSpecial(sim.Household)))
                    {
                        new FixInvisibleTask(simDesc).AddToSimulator();
                    }
                }

                foreach (CommodityKind kind in CommodityLists.AllMotives)
                {
                    if (sim.GetMotiveTuning(kind) == null)
                    {
                        //LogCorrection("Motive Tuning Rebuilt " + sim.FullName);

                        sim.UpdateMotiveTunings();
                        break;
                    }
                }

                if (Sim.sOnLotChangedDelegates != null)
                {
                    List<Delegate> list = new List<Delegate>(Sim.sOnLotChangedDelegates.GetInvocationList());

                    foreach (Delegate del in list)
                    {
                        NpcParty.Happening npcParty = del.Target as NpcParty.Happening;
                        if (npcParty != null)
                        {
                            if ((npcParty.Parent == null) || (npcParty.Parent.Host == null))
                            {
                                Sim.sOnLotChangedDelegates -= npcParty.OnSimLotChanged;

                                LogCorrection("Dropped Unhosted NPCParty");
                            }
                        }
                    }
                }

                Household household = sim.Household;
                if (household != null)
                {
                    if ((household.SharedFamilyInventory != null) &&
                        (household.SharedFamilyInventory.Inventory == null))
                    {
                        household.mSharedFamilyInventory = SharedFamilyInventory.Create(household);

                        LogCorrection("Bogus SharedFamilyInventory replaced: " + sim.FullName);
                    }

                    if ((household.SharedFridgeInventory != null) &&
                        (household.SharedFridgeInventory.Inventory == null))
                    {
                        household.mSharedFridgeInventory = SharedFridgeInventory.Create(household);

                        LogCorrection("Bogus ShareFridgeInventory replaced: " + sim.FullName);
                    }
                }

                Check(sim, sim.OpportunityManager);

                Sim.sEnableFacialIdles = false;

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }*/
            return true;
        }

        protected override bool PostPerform(Sim sim, bool postLoad)
        {
            /*
            ResetSimTask.CreationProtection data;
            if (mData.TryGetValue(sim, out data))
            {
                mData.Remove(sim);

                if (data != null)
                {
                    data.Dispose(postLoad, false);
                }
            }

            ResetSimTask.UpdateInterface(sim);

            try
            {
                if ((sim.IdleManager != null) && (sim.IdleManager.FacialIdleStateMachineClient == null))
                {
                    bool flag2 = true;
                    if ((sim.SimDescription != null) && sim.SimDescription.IsPet)
                    {
                        flag2 = GameUtils.IsInstalled(ProductVersion.EP5);
                    }

                    if (flag2)
                    {
                        sim.IdleManager.FacialIdleStateMachineClient = OverlayData.GetGenericStateMachine(sim, false, true, false, true);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, null, "IdleManager", e);
            }

            Sim.sEnableFacialIdles = true;

            if (mLotChangedEvents != null)
            {
                Sim.sOnLotChangedDelegates += mLotChangedEvents;
                mLotChangedEvents = null;
            }
            */
            return true;
        }
        /*
        protected void Check(Sim sim, OpportunityManager manager)
        {
            if (manager == null) return;

            List<Opportunity> remove = new List<Opportunity>();

            foreach (Opportunity opp in manager.List)
            {
                if (opp.mSharedData == null)
                {
                    remove.Add(opp);

                    LogCorrection("Opportunity Dropped: " + sim.FullName);
                }
            }

            foreach (Opportunity opp in remove)
            {
                try
                {
                    manager.CancelOpportunity(opp);
                    continue;
                }
                catch
                { }

                foreach (KeyValuePair<ulong,Opportunity> element in manager.mValues)
                {
                    if (object.ReferenceEquals(opp, element.Value))
                    {
                        manager.mValues.Remove(element.Key);
                        break;
                    }
                }
            }

            // We cannot check for destroyed objects at this point, however we still need to check for NULL items
            Inventories.CheckInventory(LogCorrection, DebugLogCorrection, "CheckSim", sim.Inventory, false);
        }*/
    }
}