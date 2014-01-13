using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.CustomResets;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class Reset : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "ResetLot";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override Lot GetLot(SimDescription sim)
        {
            if (sim.CreatedSim != null)
            {
                return sim.CreatedSim.LotCurrent;
            }
            else
            {
                return base.GetLot(sim);
            }
        }

        public static void ResetLot(Lot lot, bool resetAll)
        {
            try
            {
                if (resetAll)
                {
                    if (MasterController.Settings.IsExcludedLot(lot)) return;
                }

                //Services.ClearServicesForLot(lot);

                /* This is dangerous (in the past it has deleted the Ocean object and corrupted saves)
                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    if (obj.InInventory) continue;

                    if (obj.LotCurrent == null)
                    {
                        obj.Destroy();
                    }
                }
                */
                foreach (Situation situation in new List<Situation> (Situation.sAllSituations))
                {
                    if (situation.Lot == lot)
                    {
                        // Ignore the Butler situation, as exiting it will end the butler service
                        if (situation is ButlerSituation) continue;

                        try
                        {
                            situation.Exit();
                        }
                        catch(Exception e)
                        {
                            Common.DebugException(lot.Name + Common.NewLine + situation.GetType().ToString(), e);
                        }
                    }
                }
                
                if (lot.mSavedData != null)
                {
                    if (lot.mSavedData.mBroadcastersWithSims != null)
                    {
                        List<ReactionBroadcaster> broadcasters = new List<ReactionBroadcaster>(lot.mSavedData.mReactions);
                        foreach (ReactionBroadcaster broadcaster in broadcasters)
                        {
                            try
                            {
                                broadcaster.Dispose();
                            }
                            catch (Exception e)
                            {
                                Common.DebugException(lot, e);
                            }
                        }
                    }
                }

                if (lot.FireManager != null)
                {
                    lot.FireManager.RemoveAllFires();
                }

                if (Firefighter.sFirefighterDictionary != null)
                {
                    Firefighter.sFirefighterDictionary.Remove(lot.LotId);
                }

                ResetHousehold(lot.Household, resetAll);
            }
            catch (Exception exception)
            {
                Common.Exception(lot, exception);
            }
        }

        public static void ResetHousehold(Household house, bool resetAll)
        {
            if (house == null) return;

            if (resetAll)
            {
                if (MasterController.Settings.IsExcludedLot(house.LotHome)) return;
            }

            /*
            Butler instance = Butler.Instance;
            if ((instance != null) && (instance.IsServiceRequested(lot) || instance.IsAnySimAssignedToLot(lot)))
            {
                List<Sim> assigned = instance.GetSimsAssignedToLot(lot);
                foreach (Sim sim in assigned)
                {
                    IGameObject reservedTile = null;
                    if (sim.FindRoutablePointInsideNearFrontDoor(lot, out reservedTile))
                    {
                        sim.ResetBindPoseWithRotation();
                        sim.SetPosition(reservedTile.Position);
                        sim.SetForward(reservedTile.ForwardVector);
                        sim.RemoveFromWorld();
                        sim.AddToWorld();
                        sim.SetHiddenFlags(HiddenFlags.Nothing);
                        sim.SetOpacity(1f, 0f);
                    }
                }
            }
            */
            house.UniqueObjectsObtained = UniqueObjectKey.None;

            foreach (SimDescription sim in new List<SimDescription>(CommonSpace.Helpers.Households.All(house)))
            {
                try
                {
                    if (sim == null) continue;

                    Sim createdSim = sim.CreatedSim;
                    if (createdSim != null)
                    {
                        createdSim = ResetSim(createdSim, resetAll);
                    }

                    if (createdSim == null)
                    {
                        if (sim.LotHome != null)
                        {
                            new InstantiateTask(sim);
                        }
                    }
                    else if (createdSim.Inventory != null)
                    {
                        foreach (UniqueObject uniqueObj in Inventories.QuickFind<UniqueObject>(createdSim.Inventory))
                        {
                            house.UniqueObjectsObtained |= uniqueObj.SpawnedObjectKey;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            if ((house.SharedFamilyInventory != null) && (house.SharedFamilyInventory.Inventory != null))
            {
                foreach (UniqueObject uniqueObj in Inventories.QuickFind<UniqueObject>(house.SharedFamilyInventory.Inventory))
                {
                    house.UniqueObjectsObtained |= uniqueObj.SpawnedObjectKey;
                }
            }
        }

        public static bool ResetObject(IGameObject obj, bool resetAll)
        {
            try
            {
                if (obj is Sim)
                {
                    ResetSim(obj as Sim, resetAll);
                }
                else if (obj is Lot)
                {
                    ResetLot(obj as Lot, resetAll);
                }
                else
                {
                    if (resetAll)
                    {
                        if (MasterController.Settings.IsExcludedLot(obj.LotCurrent)) return false;
                    }

                    GameObject gameObj = obj as GameObject;
                    if (gameObj != null)
                    {
                        ObjectComponents.Cleanup(gameObj, null);

                        foreach (Sim sim in new List<Sim>(gameObj.mReferenceList))
                        {
                            try
                            {
                                if ((sim != null) && !sim.HasBeenDestroyed)
                                {
                                    sim.InteractionQueue.PurgeInteractions(gameObj);
                                }
                            }
                            catch (Exception e)
                            {
                                Common.DebugException(sim, e);
                            }
                        }
                    }

                    bool success = false;

                    /*
                    if (obj.InInventory)
                    {
                        obj.RemoveFromWorld();
                        obj.AddToWorld();
                        obj.SetHiddenFlags(HiddenFlags.Nothing);
                        obj.SetOpacity(1f, 0f);
                    }
                    */
                    foreach (ICustomReset reset in Common.DerivativeSearch.Find<ICustomReset>())
                    {
                        if (reset.Perform(obj))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (!success)
                    {
                        obj.SetObjectToReset();
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                Common.Exception(obj, exception);
                return false;
            }
        }

        public static Sim ResetSim(Sim sim, bool resetAll)
        {
            if (resetAll)
            {
                if (MasterController.Settings.IsExcludedLot(sim.LotCurrent)) return sim;

                if (MasterController.Settings.IsExcludedLot(sim.LotHome)) return sim;
            }

            return ResetSimTask.Perform(sim, true);
        }

        protected override OptionResult Run(Lot lot, Household house)
        {           
            if ((lot == null) && (house == null))
            {
                Common.Notify(Common.Localize("ResetLot:NoLot"));
                return OptionResult.Failure;
            }
            else if (!ApplyAll)
            {
                string name = null;

                if (lot != null)
                {
                    name = lot.Name;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = lot.Address;
                    }
                }
                else
                {
                    name = house.Name;
                }

                if (!AcceptCancelDialog.Show(Common.Localize ("ResetLot:Prompt", false, new object[] { name } )))
                {
                    return OptionResult.Failure;
                }
            }

            if (lot != null)
            {
                List<IGameObject> objects = new List<IGameObject>(lot.GetObjects<IGameObject>());
                foreach (IGameObject obj in objects)
                {
                    ResetObject(obj, false);

                    if (lot.Household != null)
                    {
                        UniqueObject uniqueObj = obj as UniqueObject;
                        if (uniqueObj != null)
                        {
                            lot.Household.UniqueObjectsObtained |= uniqueObj.SpawnedObjectKey;
                        }
                    }
                }

                ResetLot(lot, false);
            }
            else
            {
                ResetHousehold(house, false);
            }

            Common.Notify(Common.Localize ("ResetLot:Success"));
            return OptionResult.SuccessClose; ;
        }

        protected class InstantiateTask : Common.AlarmTask
        {
            SimDescription mSim;

            public InstantiateTask(SimDescription sim)
                : base(1, TimeUnit.Seconds)
            {
                mSim = sim;
            }

            protected override void OnPerform()
            {
                Instantiation.Perform(mSim, ResetSimTask.Perform);
            }
        }
    }
}
