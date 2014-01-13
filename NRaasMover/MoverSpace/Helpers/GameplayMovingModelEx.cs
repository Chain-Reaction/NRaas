using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Helpers
{
    public class GameplayMovingModelEx : GameplayMovingModel
    {
        static readonly OutfitCategories[] sCategories = new OutfitCategories[] { OutfitCategories.Naked, OutfitCategories.Formalwear, OutfitCategories.Sleepwear, OutfitCategories.Swimwear, OutfitCategories.Athletic };

        public GameplayMovingModelEx(Sim sim)
            : base(sim)
        {
            AlterList(mSourceSimList);
            AlterList(mTargetSimList);
        }
        public GameplayMovingModelEx(Sim sim1, Sim sim2)
            : base(sim1, sim2)
        {
            AlterList(mSourceSimList);
            AlterList(mTargetSimList);
        }
        public GameplayMovingModelEx(Sim simMoving, Household newHousehold)
            : base(simMoving)
        {
            try
            {
                mSimMoving = simMoving;
                mPreselectedHousehold = newHousehold;
                if (newHousehold.IsTravelHousehold)
                {
                    // Custom
                    BuildTravelHouseholdEx();
                }

                mPreviousHouseholdSims = new List<Sim>(newHousehold.AllActors);
                mTargetLot = mPreselectedHousehold.LotHome;
                if (mSimMoving.Household.IsTouristHousehold)
                {
                    // Custom
                    BuildForeignHouseholdEx();
                }
                else
                {
                    mSourceHousehold = mSimMoving.Household;
                }

                bool hasSourceHousehold = (mSourceHousehold != null);

                if ((!hasSourceHousehold) || ((mSourceHousehold.IsServiceNpcHousehold) && mbSourceForeignHousehold) || mSourceHousehold.IsAlienHousehold || mSourceHousehold.IsMermaidHousehold)
                {
                    mSourceHouseholdName = mSimMoving.SimDescription.LastName;
                }
                else
                {
                    mSourceHouseholdName = mSourceHousehold.Name;
                }

                mTargetHouseholdName = mPreselectedHousehold.Name;
                mSourceHouseholdDescription = mSourceHousehold.BioText;
                mTargetHouseholdDescription = mPreselectedHousehold.BioText;
                mSourceHouseholdFunds = ((mSourceHousehold.LotHome != null) || mbSourceForeignHousehold) ? mSourceHousehold.FamilyFunds : 0x0;
                mTargetHouseholdFunds = mPreselectedHousehold.FamilyFunds;
                if (mSourceHousehold == Household.ActiveHousehold)
                {
                    int num = mPreselectedHousehold.Sims[0x0].IsRich ? kMaxMoneyTransferredRich : kMaxMoneyTransferredNormal;
                    mTargetHouseholdFunds = Math.Min(mTargetHouseholdFunds, num);
                }

                mSourceSimList = new Dictionary<ISimDescription, bool>();
                mTargetSimList = new Dictionary<ISimDescription, bool>();

                if ((hasSourceHousehold) && ((mSourceHousehold.IsServiceNpcHousehold) || (mSourceHousehold.IsAlienHousehold) || (mSourceHousehold.IsMermaidHousehold)))
                {
                    hasSourceHousehold = false;
                }

                if (hasSourceHousehold)
                {
                    foreach (SimDescription description in mSourceHousehold.AllSimDescriptions)
                    {
                        if (description != mSimMoving.SimDescription)
                        {
                            mSourceSimList.Add(description, true);
                        }
                    }
                }

                mTargetSimList.Add(mSimMoving.SimDescription, false);
                foreach (SimDescription description2 in mPreselectedHousehold.AllSimDescriptions)
                {
                    mTargetSimList.Add(description2, false);
                }

                mSellHome = false;
                mPackFurniture = false;
                mMoveOut = false;
                mAutoMove = false;
                mSourceHouseholdActive = false;
            }
            catch (Exception e)
            {
                Common.Exception(simMoving, e);
            }

            // Custom
            AlterList(mSourceSimList);
            AlterList(mTargetSimList);
        }

        private void BuildForeignHouseholdEx()
        {
            mSimsToReturnToForeignHousehold = new List<SimDescription>();
            mSourceHousehold = Household.Create();
            mSourceHousehold.Name = mSimMoving.SimDescription.LastName;
            Household.TouristHousehold.RemoveTemporary(mSimMoving.SimDescription);
            mSimsToReturnToForeignHousehold.Add(mSimMoving.SimDescription);
            mSourceHousehold.AddTemporary(mSimMoving.SimDescription);
            MiniSimDescription description = MiniSimDescription.Find(mSimMoving.SimDescription.SimDescriptionId);
          
            if (((description != null) && (description.HouseholdMembers != null)) && !description.IsServicePerson)
            {
                ulong simDescriptionId = mSimMoving.SimDescription.SimDescriptionId;
                foreach (ulong num2 in description.HouseholdMembers)
                {
                    if (num2 != simDescriptionId)
                    {
                        MiniSimDescription msd = MiniSimDescription.Find(num2);
                        if (msd != null)
                        {
                            try
                            {
                                SimDescription simDescription = null;
                                bool flag = false;
                                if (msd.Instantiated)
                                {
                                    simDescription = Household.TouristHousehold.FindMember(num2);
                                    if (simDescription != null)
                                    {
                                        Household.TouristHousehold.RemoveTemporary(simDescription);
                                        mSimsToReturnToForeignHousehold.Add(simDescription);
                                    }
                                }
                                else
                                {
                                    // Custom
                                    simDescription = MiniSims.UnpackSimAndUpdateRel(msd);
                                    flag = true;
                                }

                                if (simDescription != null)
                                {
                                    mSourceHousehold.AddTemporary(simDescription);
                                    msd.Instantiated = true;
                                    if (flag)
                                    {
                                        if (simDescription.AgingState != null)
                                        {
                                            simDescription.AgingState.MergeTravelInformation(msd);
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(msd.FullName, e);
                            }
                        }
                    }
                }
            }
            mbSourceForeignHousehold = true;
        }

        private void BuildTravelHouseholdEx()
        {
            mTemporaryTravelMembers = new List<SimDescription>();
            MiniSimDescription description = MiniSimDescription.Find(mPreselectedHousehold.AllSimDescriptions[0x0].SimDescriptionId);
            if ((description != null) && (description.HouseholdMembers != null))
            {
                foreach (ulong num2 in description.HouseholdMembers)
                {
                    if (mPreselectedHousehold.CurrentMembers.GetSimDescriptionFromId(num2) == null)
                    {
                        MiniSimDescription miniSim = MiniSimDescription.Find(num2);
                        if (miniSim != null)
                        {
                            try
                            {
                                SimDescription item = null;
                                if (!miniSim.Instantiated)
                                {
                                    // Custom
                                    item = MiniSims.UnpackSimAndUpdateRel(miniSim);
                                    if (item != null)
                                    {
                                        mTemporaryTravelMembers.Add(item);
                                        mPreselectedHousehold.AddSilent(item);
                                        miniSim.Instantiated = true;
                                        item.OnHouseholdChanged(mPreselectedHousehold, false);
                                        item.AgingState.MergeTravelInformation(miniSim);
                                        (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).OnSimCurrentWorldChanged(true, miniSim);
                                    }
                                }
                                else
                                {
                                    item = GameStates.GetEarlyDepatureSim(num2);
                                    if (item != null)
                                    {
                                        mTemporaryTravelMembers.Add(item);
                                        mPreselectedHousehold.AddTemporary(item);
                                        miniSim.Instantiated = true;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(miniSim.FirstName + " " + miniSim.LastName, e);
                            }
                        }
                    }
                }
            }
        }

        private static void AlterList(Dictionary<ISimDescription, bool> list)
        {
            List<ISimDescription> sims = new List<ISimDescription>(list.Keys);
            list.Clear();

            foreach (ISimDescription sim in sims)
            {
                list.Add(sim, true);
            }
        }

        private void TransferSims(Household newHouse, Household oldHouse, bool willBeActive, bool mergeInventories, Dictionary<ISimDescription,bool> sims)
        {
            Common.StringBuilder msg = new Common.StringBuilder("TransferSims" + Common.NewLine);

            try
            {
                newHouse.AddGreetedLotToHousehold(newHouse.LotHome, ObjectGuid.InvalidObjectGuid);

                msg += "A";

                List<Sim> simsMovingToHousehold = new List<Sim>();
                WorldName currentWorld = GameUtils.GetCurrentWorld();

                foreach (ISimDescription iSim in sims.Keys)
                {
                    SimDescription simDesc = iSim as SimDescription;

                    msg += Common.NewLine + iSim.FullName + Common.NewLine;

                    try
                    {
                        if ((!willBeActive) && (simDesc.IsEnrolledInBoardingSchool()))
                        {
                            msg += "B";

                            Sim createdSim = simDesc.CreatedSim;
                            if (createdSim == null)
                            {
                                BoardingSchool.Remove(null, simDesc, BoardingSchool.RemovalReasons.Removed, false);
                            }
                            else
                            {
                                InteractionInstance currentInteraction = createdSim.CurrentInteraction;
                                if ((currentInteraction != null) && ((currentInteraction is BoardingSchoolPickUpSituation.GetInCar) || (currentInteraction is PoliceSituation.Wait)))
                                {
                                    createdSim.SetObjectToReset();
                                    SpeedTrap.Sleep();
                                    createdSim.AttemptToPutInSafeLocation(true);
                                }

                                if (simDesc.BoardingSchool != null)
                                {
                                    simDesc.BoardingSchool.OnRemovedFromSchool();
                                }

                                simDesc.AssignSchool();
                            }
                        }

                        if (!newHouse.Contains(simDesc))
                        {
                            msg += "C";

                            if (simDesc.CreatedSim == null)
                            {
                                msg += Common.NewLine + "C1 " + simDesc.FullName + Common.NewLine;

                                TryInstantiateSim(simDesc);
                            }

                            if (simDesc.CreatedSim != null)
                            {
                                if (simDesc.Household != newHouse)
                                {
                                    EventTracker.SendEvent(EventTypeId.kMovedHousehold, simDesc.CreatedSim);
                                }

                                simsMovingToHousehold.Add(simDesc.CreatedSim);
                                Household.AddServobotToHouseholdFixup(simDesc.CreatedSim);
                                EventTracker.SendEvent(EventTypeId.kMovedHouses, simDesc.CreatedSim);
                                MidlifeCrisisManager.OnMoved(simDesc);

                                MiniSimDescription miniSim = MiniSimDescription.Find(simDesc.SimDescriptionId);
                                if (miniSim != null)
                                {
                                    miniSim.Instantiated = true;
                                }

                                if (mSimsToReturnToForeignHousehold != null)
                                {
                                    mSimsToReturnToForeignHousehold.Remove(simDesc);
                                }

                                if (simDesc.HomeWorld != currentWorld)
                                {
                                    simDesc.BuildOutfitsOfType(sCategories, "movingin");
                                }
                            }
                        }

                        if ((newHouse.LotHome == null) &&
                            (simDesc != null) &&
                            (simDesc.Household != null) &&
                            (simDesc.CreatedSim != null) &&
                            (simDesc.CreatedSim.Inventory != null) &&
                            (simDesc.CreatedSim.Inventory.NumItemsStored > 0x0))
                        {
                            msg += "D";

                            Sim toGiveTo = null;
                            foreach (Sim sim4 in simDesc.Household.Sims)
                            {
                                if (sim4 == null) continue;

                                if ((((sim4.SimDescription != simDesc) && sims.ContainsKey(sim4.SimDescription)) &&
                                    ((sim4.SimDescription.DeathStyle == SimDescription.DeathType.None) || (sim4.SimDescription.IsPlayableGhost))) &&
                                    ((toGiveTo == null) || (sim4.SimDescription.Age > toGiveTo.SimDescription.Age)))
                                {
                                    toGiveTo = sim4;
                                }
                            }

                            if (toGiveTo != null)
                            {
                                simDesc.CreatedSim.MoveInventoryItemsToSim(toGiveTo);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(simDesc, null, msg, e);
                    }
                }

                msg += "E";

                if (willBeActive)
                {
                    if ((oldHouse.SharedFamilyInventory != null) && (oldHouse.SharedFamilyInventory.Inventory != null))
                    {
                        if ((newHouse.SharedFamilyInventory != null) && (newHouse.SharedFamilyInventory.Inventory != null))
                        {
                            oldHouse.SharedFamilyInventory.Inventory.MoveObjectsTo(newHouse.SharedFamilyInventory.Inventory);
                        }
                    }

                    msg += "F";

                    if ((GameUtils.IsAnyTravelBasedWorld ()) && (newHouse == GameStates.TravelHousehold))
                    {
                        foreach (ISimDescription simDesc in sims.Keys)
                        {
                            if (mTemporaryTravelMembers != null)
                            {
                                bool found = false;
                                foreach (SimDescription foreign in mTemporaryTravelMembers)
                                {
                                    if (foreign == simDesc)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (found) continue;
                            }

                            ulong simDescriptionId = simDesc.SimDescriptionId;
                            if ((GameStates.TravelerIds != null) && !GameStates.TravelerIds.Contains(simDescriptionId))
                            {
                                GameStates.TravelerIds.Add(simDescriptionId);
                            }
                        }
                    }
                }

                msg += "G";

                UpdateAttributesForNewSimsEx(willBeActive, simsMovingToHousehold);

                if (oldHouse.LotHome != null)
                {
                    foreach (UniqueObject uniqueObject in oldHouse.LotHome.GetObjects<UniqueObject>())
                    {
                        GameEntryMovingModel.SplitMergeHouseholds -= uniqueObject.GameEntryMovingModel_SplitMergeHouseholds;
                    }
                }

                GameEntryMovingModel.TriggerSplitMergeHouseholdsCallback(oldHouse, newHouse, simsMovingToHousehold, mergeInventories);
                HouseholdEx.AddSims(newHouse, simsMovingToHousehold, oldHouse, false, willBeActive, Mover.Settings.mDreamCatcher);

                CheckForBabiesAndToddlers(newHouse);

                UpdateHouseholdMemberInMiniSimDescriptionEx(newHouse);
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
        }

        private static void UpdateHouseholdMemberInMiniSimDescriptionEx(Household household)
        {
            foreach (SimDescription description in household.AllSimDescriptions)
            {
                MiniSimDescription description2 = MiniSimDescription.Find(description.SimDescriptionId);
                if ((description2 == null) && (description.HomeWorld != GameUtils.GetCurrentWorld()))
                {
                    MiniSimDescription.AddMiniSim(description);

                    description2 = MiniSimDescription.Find(description.SimDescriptionId);

                    // Custom
                    if (description2 != null)
                    {
                        description2.Instantiated = true;
                    }
                }

                if (description2 != null)
                {
                    description2.UpdateHouseholdMembers(household.AllSimDescriptions);
                }
            }
        }

        private static void UpdateAttributesForNewSimsEx(bool willBeActive, List<Sim> movingSims)
        {
            bool hasVirtualResidentialSlots = false;
            bool isPenthouse = false;

            if (movingSims.Count > 0x0)
            {
                Lot lotHome = movingSims[0x0].LotHome;
                if (lotHome != null)
                {
                    hasVirtualResidentialSlots = lotHome.HasVirtualResidentialSlots;
                    if (hasVirtualResidentialSlots)
                    {
                        BuildableShell[] buildableShells = lotHome.BuildableShells;
                        if (buildableShells.Length > 0x0)
                        {
                            isPenthouse = buildableShells[0x0].Tuning.kIsPenthouse;
                        }

                        if (willBeActive)
                        {
                            Tutorialette.TriggerLesson(Lessons.ApartmentLiving, movingSims[0x0]);
                        }
                    }
                }
            }

            foreach (Sim sim in movingSims)
            {
                if (hasVirtualResidentialSlots)
                {
                    EventTracker.SendEvent(EventTypeId.kMovedIntoApartment, sim);
                    if (isPenthouse)
                    {
                        EventTracker.SendEvent(EventTypeId.kMovedIntoPenthouse, sim);
                    }
                }

                sim.ResetMapTagManager();

                if (willBeActive)
                {
                    School school = sim.School;
                    if (school != null)
                    {
                        school.RescheduleCarpool();
                    }

                    Occupation occupation = sim.Occupation;
                    if (occupation != null)
                    {
                        occupation.RescheduleCarpool();
                    }
                }
            }
        }

        private void PostTransfer(Household newHouse, Household oldHouse, Lot oldLot, List<SimDescription> previousSims, List<Sim> movingSims, List<Sim> newSims)
        {
            foreach (Sim sim in Households.AllSims(newHouse))
            {
                if ((sim == mMarryingSimSelectable) || !previousSims.Contains(sim.SimDescription))
                {
                    foreach (SimDescription previousSimDesc in previousSims)
                    {
                        Sim previousSim = previousSimDesc.CreatedSim;
                        if (previousSim == null) continue;

                        if (sim != previousSim)
                        {
                            EventTracker.SendEvent(EventTypeId.kMovedInTogether, sim, previousSim);
                            EventTracker.SendEvent(EventTypeId.kMovedInTogether, previousSim, sim);
                        }
                    }

                    if ((sim.LotCurrent != newHouse.LotHome) && ShouldMove(sim, oldLot))
                    {
                        if ((!(sim.CurrentInteraction is ICountsAsWorking)) && (sim.GetSituationOfType<HostedSituation>() == null))
                        {
                            movingSims.Add(sim);
                        }
                    }

                    if (sim != mMarryingSimSelectable)
                    {
                        newSims.Add(sim);
                    }
                }
                else if ((oldHouse.AllActors.Count == 0x0) && (sim.LotCurrent == oldLot))
                {
                    if ((!(sim.CurrentInteraction is ICountsAsWorking)) && (sim.GetSituationOfType<HostedSituation>() == null))
                    {
                        Sim.MakeSimGoHome(sim, false, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                    }
                }
            }

            if (newHouse.NumMembers == 0)
            {
                EventTracker.SendEvent(new HouseholdUpdateEvent(EventTypeId.kHouseholdMerged, newHouse));
            }
            else
            {
                bool split = false;
                foreach (ISimDescription iSim in previousSims)
                {
                    SimDescription simDesc = iSim as SimDescription;
                    if (simDesc == null) continue;

                    if (!newHouse.Contains(simDesc))
                    {
                        split = true;
                        break;
                    }
                }

                if (split)
                {
                    EventTracker.SendEvent(new HouseholdUpdateEvent(EventTypeId.kHouseholdSplit, newHouse));
                }
            }
        }

        private static void PackFurnitureEx(Lot oldLot, Household household, ref Dictionary<ObjectGuid, ObjectGuid> originalsToClones)
        {
            if (household.SharedFamilyInventory != null)
            {
                Dictionary<ObjectGuid, bool> stored = new Dictionary<ObjectGuid, bool>();

                originalsToClones = new Dictionary<ObjectGuid, ObjectGuid>();
                List<IGameObject> objList = new List<IGameObject>();
                List<IGameObject> list2 = new List<IGameObject>();
                foreach (GameObject obj2 in oldLot.GetObjects<GameObject>())
                {
                    // Custom
                    if (stored.ContainsKey(obj2.ObjectId)) continue;
                    stored.Add(obj2.ObjectId, true);

                    if (Lot.CanObjBePackedOrSold(obj2, false))
                    {
                        objList.Add(obj2);
                        if (obj2.Inventory != null)
                        {
                            foreach (GameObject obj3 in Inventories.QuickFind<GameObject>(obj2.Inventory))
                            {
                                if (Lot.CanObjBePackedOrSold(obj3, true))
                                {
                                    // Custom
                                    if (!stored.ContainsKey(obj3.ObjectId))
                                    {
                                        stored.Add(obj3.ObjectId, true);
                                        objList.Add(obj3);
                                    }
                                }
                                else
                                {
                                    list2.Add(obj3);
                                }
                            }
                        }
                    }
                    else
                    {
                        list2.Add(obj2);
                    }
                }

                for (int i = list2.Count - 0x1; i >= 0x0; i--)
                {
                    ISubObject item = list2[i] as ISubObject;
                    if ((item != null) && objList.Contains(item.SubObjectOwner))
                    {
                        list2.RemoveAt(i);

                        // Custom
                        if (!stored.ContainsKey(item.ObjectId))
                        {
                            stored.Add(item.ObjectId, true);
                            objList.Add(item);
                        }
                    }
                }

                foreach (GameObject obj5 in objList)
                {
                    if ((obj5 is GameObject.IDoNotDuplicateOnClone) && (obj5.ActorsUsingMe.Count > 0x0))
                    {
                        obj5.SetObjectToReset();
                        SpeedTrap.Sleep();
                    }
                }

                List<IGameObject> list3 = GameObject.CloneObjects(objList, originalsToClones);
                if (list3 != null)
                {
                    foreach (IGameObject obj6 in list3)
                    {
                        if ((obj6.ItemComp == null) || ((obj6.ItemComp != null) && (obj6.ItemComp.InventoryParent == null)))
                        {
                            household.SharedFamilyInventory.Inventory.TryToAdd(obj6);
                        }
                    }
                }
            }
        }

        public override void Apply()
        {
            string msg = "Apply" + Common.NewLine;

            try
            {
                try
                {
                    sbMovingCurrentlyInProgress = true;
                    ProgressDialog.Show(Common.LocalizeEAString("Ui/Caption/Global:Processing"), false);

                    Household sourceHousehold = mSourceHousehold;

                    Lot sourceLot = sourceHousehold.LotHome;

                    mTempHousehold.ClearSilent();
                    mTempHousehold.Destroy();
                    mTempHousehold = null;

                    msg += "A";

                    ParentsLeavingTownSituation situation = ParentsLeavingTownSituation.FindParentsGoneSituationForHousehold(sourceHousehold);
                    if ((situation != null) && (ParentsLeavingTownSituation.Adults != null))
                    {
                        bool freeVacation = false;
                        foreach (ISimDescription targetSim in mTargetSimList.Keys)
                        {
                            if (ParentsLeavingTownSituation.Adults.Contains(targetSim.SimDescriptionId))
                            {
                                freeVacation = true;
                                break;
                            }
                        }

                        if (!freeVacation)
                        {
                            bool flag3 = false;
                            foreach (ISimDescription iSourceSim in mSourceSimList.Keys)
                            {
                                if (iSourceSim.TeenOrAbove && !ParentsLeavingTownSituation.Adults.Contains(iSourceSim.SimDescriptionId))
                                {
                                    SimDescription sourceSim = iSourceSim as SimDescription;
                                    if ((sourceSim != null) && (sourceSim.CreatedSim != null))
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }
                            }
                            freeVacation = !flag3;
                        }

                        if (freeVacation)
                        {
                            situation.BringParentsBack();
                        }
                    }

                    msg += "B";

                    Household targetHousehold = mPreselectedHousehold;

                    // Must be ahead of check mSourceSimList
                    Lot targetLot = null;
                    if (targetHousehold != null)
                    {
                        targetLot = targetHousehold.LotHome;
                    }

                    if (targetHousehold == null)
                    {
                        if (mSourceSimList.Count == 0)
                        {
                            targetHousehold = sourceHousehold;
                        }
                    }

                    if (targetLot == null)
                    {
                        targetLot = mTargetLot;
                    }

                    if ((mMoveOut) && (targetLot == null))
                    {
                        msg += "B1";

                        List<Lot> allChoices = new List<Lot>();
                        List<Lot> affordable = new List<Lot>();
                        foreach (Lot lot in LotManager.AllLots)
                        {
                            if (!string.IsNullOrEmpty(Households.IsValidResidentialLot(lot))) continue;

                            Lot.LotMetrics metrics = new Lot.LotMetrics();
                            lot.GetLotMetrics(ref metrics);

                            if (metrics.FridgeCount == 0) continue;

                            allChoices.Add(lot);

                            if (Mover.GetLotCost(lot) < mNewTargetFunds)
                            {
                                affordable.Add(lot);
                            }
                        }

                        if (affordable.Count > 0)
                        {
                            affordable.Sort(new Comparison<Lot>(Households.SortByCost));

                            targetLot = affordable[0];
                        }
                        else if (allChoices.Count > 0)
                        {
                            if ((!Mover.Settings.mFreeRealEstate) && (!AcceptCancelDialog.Show(Common.Localize("NoAffordable:Prompt"))))
                            {
                                return;
                            }

                            allChoices.Sort(new Comparison<Lot>(Households.SortByCost));

                            targetLot = allChoices[0];
                        }

                        if (targetLot == null)
                        {
                            if (!AcceptCancelDialog.Show(Common.Localize("Homeless:Prompt")))
                            {
                                return;
                            }
                        }
                        else
                        {
                            mNewTargetFunds -= Mover.GetLotCost(targetLot);

                            if (mNewTargetFunds < 0)
                            {
                                mNewTargetFunds = 0;
                            }
                        }
                    }

                    msg += "C";

                    if (targetHousehold == null)
                    {
                        targetHousehold = Household.Create();
                    }

                    Household activeHouse = targetHousehold;
                    Household saleHouse = sourceHousehold;
                    if (mSourceHouseholdActive)
                    {
                        msg += "C1";

                        activeHouse = sourceHousehold;
                        saleHouse = targetHousehold;
                    }

                    Lot saleLot = saleHouse.LotHome;

                    Dictionary<ObjectGuid, ObjectGuid> originalsToClones = null;

                    if (mSellHome)
                    {
                        msg += "C2";

                        if ((mPackFurniture) && (saleHouse != Household.NpcHousehold))
                        {
                            PackFurnitureEx(saleHouse.LotHome, activeHouse, ref originalsToClones);
                        }

                        saleHouse.MoveOut();
                    }

                    if (targetHousehold != null)
                    {
                        msg += "C3";

                        if ((targetLot != null) && (targetHousehold.LotHome != targetLot))
                        {
                            targetHousehold.MoveOut();

                            // Perform this preior to MoveIn() to inform StoryProgresion that it does not need to adjust lot funds
                            Mover.PresetStoryProgressionLotHome(targetLot, targetHousehold);
                            targetLot.MoveIn(targetHousehold);

                            targetHousehold.AddGreetedLotToHousehold(targetLot, ObjectGuid.InvalidObjectGuid);
                        }

                        if (mTargetSimList.Count > 0)
                        {
                            if (targetHousehold != sourceHousehold)
                            {
                                targetHousehold.BioText = mTargetHouseholdDescription;
                                targetHousehold.Name = mTargetHouseholdName;
                            }

                            targetHousehold.SetFamilyFunds(mNewTargetFunds);
                        }
                    }

                    msg += "D";

                    if (sourceHousehold != null)
                    {
                        msg += "D1";

                        if ((sourceHousehold != Household.NpcHousehold) && (mSourceSimList.Count > 0))
                        {
                            msg += "D2";

                            sourceHousehold.SetFamilyFunds(mNewSourceFunds);
                            sourceHousehold.BioText = mSourceHouseholdDescription;
                            sourceHousehold.Name = mSourceHouseholdName;
                        }
                    }

                    if (mSellHome)
                    {
                        msg += "E";

                        List<SimDescription> otherSims = new List<SimDescription>();

                        List<Sim> movingSims = new List<Sim>();
                        foreach (SimDescription simDesc in Households.All(saleHouse))
                        {
                            Sim sim = simDesc.CreatedSim;
                            if (sim != null)
                            {
                                movingSims.Add(sim);

                                EventTracker.SendEvent(EventTypeId.kMovedHouses, sim);
                            }
                            else
                            {
                                otherSims.Add(simDesc);
                            }

                            MidlifeCrisisManager.OnMoved(simDesc);
                        }

                        if (saleHouse != activeHouse)
                        {
                            Households.TransferData(activeHouse, saleHouse);
                        }

                        foreach (SimDescription simDesc in otherSims)
                        {
                            if (simDesc.Household != null)
                            {
                                simDesc.Household.Remove(simDesc, false);
                            }

                            activeHouse.Add(simDesc);
                        }

                        HouseholdEx.AddSims(activeHouse, movingSims, saleHouse, (saleHouse != activeHouse), true, Mover.Settings.mDreamCatcher);

                        UpdateAttributesForNewSimsEx(true, movingSims);

                        Sim tokenSim = null;

                        for (int i = movingSims.Count - 1; i >= 0; i--)
                        {
                            Sim sim = movingSims[i];

                            if ((sim.CurrentInteraction is ICountsAsWorking) || (sim.GetSituationOfType<HostedSituation>() != null))
                            {
                                if (tokenSim == null)
                                {
                                    tokenSim = movingSims[i];
                                }

                                movingSims.RemoveAt(i);
                            }
                        }

                        if ((movingSims.Count == 0) && (tokenSim != null))
                        {
                            movingSims.Add(tokenSim);
                        }

                        if (movingSims.Count > 0)
                        {
                            MovingSituation.MoveAllActiveSimsToActiveLot(saleLot, activeHouse.LotHome, movingSims, mPackFurniture, originalsToClones);
                        }
                    }
                    else
                    {
                        msg += "F";

                        List<SimDescription> previousTarget = new List<SimDescription>(Households.All(targetHousehold));
                        List<SimDescription> previousSource = new List<SimDescription>(Households.All(sourceHousehold));

                        if (mSourceHouseholdActive)
                        {
                            msg += "F1";

                            if (mTargetSimList.Count == 0)
                            {
                                Households.TransferData(sourceHousehold, targetHousehold);
                            }
                            else if ((targetHousehold.RealEstateManager.AllProperties.Count > 0) &&
                                (Mover.Settings.mPromptTransferRealEstate) &&
                                (AcceptCancelDialog.Show(Common.Localize("TransferRealEstate:Prompt"))))
                            {
                                Households.TransferRealEstate(sourceHousehold, targetHousehold);
                            }

                            TransferSims(targetHousehold, sourceHousehold, false, (mSourceSimList.Count == 0), mTargetSimList);
                            TransferSims(sourceHousehold, targetHousehold, true, (mTargetSimList.Count == 0), mSourceSimList);
                        }
                        else
                        {
                            msg += "F2";

                            if (mSourceSimList.Count == 0)
                            {
                                Households.TransferData(targetHousehold, sourceHousehold);
                            }
                            else if ((sourceHousehold.RealEstateManager.AllProperties.Count > 0) &&
                                (Mover.Settings.mPromptTransferRealEstate) &&
                                (AcceptCancelDialog.Show(Common.Localize("TransferRealEstate:Prompt"))))
                            {
                                Households.TransferRealEstate(targetHousehold, sourceHousehold);
                            }

                            TransferSims(sourceHousehold, targetHousehold, false, (mTargetSimList.Count == 0), mSourceSimList);
                            TransferSims(targetHousehold, sourceHousehold, true, (mSourceSimList.Count == 0), mTargetSimList);
                        }

                        msg += "G";

                        if (targetHousehold.NumMembers == 0)
                        {
                            msg += "G1";

                            targetHousehold.HandleLastSimsDeath();
                        }

                        ThumbnailManager.GenerateHouseholdThumbnail(targetHousehold.HouseholdId, targetHousehold.HouseholdId, ThumbnailSizeMask.Large | ThumbnailSizeMask.Medium);

                        if (sourceHousehold.NumMembers == 0)
                        {
                            msg += "G2";

                            sourceHousehold.HandleLastSimsDeath();
                        }

                        ThumbnailManager.GenerateHouseholdThumbnail(sourceHousehold.HouseholdId, sourceHousehold.HouseholdId, ThumbnailSizeMask.Large | ThumbnailSizeMask.Medium);

                        if (GameStates.IsOnVacation)
                        {
                            using (DreamCatcher.HouseholdStore store = new DreamCatcher.HouseholdStore(activeHouse, true))
                            {
                                PlumbBob.ForceSelectActor(activeHouse.GetRandomNonRoommateSim());
                            }
                        }
                        else
                        {
                            DreamCatcher.Select(activeHouse.GetRandomNonRoommateSim(), true, Mover.Settings.mDreamCatcher, true);
                        }

                        msg += "H";

                        if ((saleHouse != null) && (saleHouse != activeHouse))
                        {
                            using (DreamCatcher.HouseholdStore store = new DreamCatcher.HouseholdStore((Household)null, Mover.Settings.mDreamCatcher))
                            {
                                foreach (Sim sim in Households.AllSims(saleHouse))
                                {
                                    msg += "H2";

                                    sim.OnBecameUnselectable();
                                }
                            }
                        }

                        foreach (Sim sim in Households.AllSims(activeHouse))
                        {
                            msg += "I2";

                            sim.ResetMapTagManager();
                        }

                        msg += "K";

                        List<Sim> sourceNewSims = new List<Sim>();
                        List<Sim> sourceMovingSims = new List<Sim>();
                        PostTransfer(sourceHousehold, targetHousehold, targetLot, previousSource, sourceMovingSims, sourceNewSims);

                        List<Sim> targetNewSims = new List<Sim>();
                        List<Sim> targetMovingSims = new List<Sim>();
                        PostTransfer(targetHousehold, sourceHousehold, sourceLot, previousTarget, targetMovingSims, targetNewSims);

                        msg += Common.NewLine + " Target Moving";
                        foreach (Sim sim in targetMovingSims)
                        {
                            msg += Common.NewLine + " " + sim.FullName;
                        }

                        msg += Common.NewLine + " Source Moving";
                        foreach (Sim sim in sourceMovingSims)
                        {
                            msg += Common.NewLine + " " + sim.FullName;
                        }

                        msg += "L";

                        if (targetHousehold == Household.ActiveHousehold)
                        {
                            msg += "L1";

                            if (targetMovingSims.Count > 0x0)
                            {
                                MovingSituation.MoveNPCsToActiveHousehold(targetHousehold.LotHome, targetMovingSims, targetNewSims);
                            }
                            else if (sourceMovingSims.Count > 0)
                            {
                                MovingSituation.MoveActiveSimsToNPCLot(sourceHousehold.LotHome, sourceMovingSims, sourceNewSims, false);
                            }
                        }
                        else
                        {
                            msg += "L2";

                            if (targetMovingSims.Count > 0x0)
                            {
                                MovingSituation.MoveActiveSimsToNPCLot(targetHousehold.LotHome, targetMovingSims, targetNewSims, false);
                            }
                            else if (sourceMovingSims.Count > 0)
                            {
                                MovingSituation.MoveNPCsToActiveHousehold(sourceHousehold.LotHome, sourceMovingSims, sourceNewSims);
                            }
                        }

                        msg += "M";

                        sbMovingCurrentlyInProgress = false;
                        TombRoomManager.OnChangeHousehold(Household.ActiveHousehold);
                    }

                    msg += "N";

                    CleanUpTempTravelHoushold();
                    CleanUpSourceTempHousehold(true);

                    mSourceSimList.Clear();
                    mTargetSimList.Clear();

                    mSourceHousehold = null;
                    mPreselectedHousehold = null;
                    if (mPreviousHouseholdSims != null)
                    {
                        mPreviousHouseholdSims.Clear();
                        mPreviousHouseholdSims = null;
                    }

                    GameEntryMovingModel.TriggerSplitMergeHouseholdsCompleteCallback();
                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }
                finally
                {
                    Common.DebugNotify(msg);

                    sbMovingCurrentlyInProgress = false;
                    ProgressDialog.Close();
                }
            }
            catch (ExecutionEngineException)
            { }
        }

        public override bool AlwaysShowActiveButton()
        {
            if (mbSourceForeignHousehold)
            {
                return false;
            }
            else if ((mSourceHousehold == Household.ActiveHousehold) && (!mSourceHousehold.LotHome.IsApartmentLot))
            {
                return true;
            }
            else if (GameUtils.IsOnVacation())
            {
                return false;
            }

            return true;
            //return (mbForMarriage && (mSourceHousehold.LotHome != null));
        }

        public override int GetLotWorth(bool isSource)
        {
            if (Mover.Settings.mFreeRealEstate) return 0;

            if (isSource && (mPreselectedHousehold != null))
            {
                return -1;
            }

            if (isSource)
            {
                if (mSourceHousehold.LotHome != null)
                {
                    return Mover.GetLotCost(mSourceHousehold.LotHome);
                }
            }
            else
            {
                if (mTargetLot != null)
                {
                    return Mover.GetLotCost(mTargetLot);
                }
            }

            return -1;
        }

        public override MoveValidity IsLotValid(bool isSource, ref string reason)
        {
            if (isSource)
            {
                if (mNewSourceFunds < 0)
                {
                    reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:CantAfford");
                    return MoveValidity.None;
                }
            }
            else
            {
                if (mNewTargetFunds < 0)
                {
                    reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:CantAfford");
                    return MoveValidity.None;
                }
            }

            if (mbForMarriage)
            {
                bool flag = mSourceSimList.ContainsKey(mMarryingSimNPC.SimDescription);
                bool flag2 = mSourceSimList.ContainsKey(mMarryingSimSelectable.SimDescription);
                if ((!flag || !flag2) && (flag || flag2))
                {
                    reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:NewlywedsMustStayTogether");
                    //return MoveValidity.None;
                }
            }

            if (!isSource || !SimTypes.IsSpecial(mSourceHousehold))
            {
                if (BaseIsLotValid(this, mSourceSimList, mTargetSimList, mSourceHousehold, isSource, ref reason) == MoveValidity.None)
                {
                    return MoveValidity.None;
                }

                if (!isSource)
                {
                    bool testMetrics = !mbForMarriage;
                    if ((mPreselectedHousehold != null) && (mPreselectedHousehold.LotHome != null))
                    {
                        Lot.LotMetrics metrics = new Lot.LotMetrics();
                        mPreselectedHousehold.LotHome.GetLotMetrics(ref metrics);

                        if (metrics.BedCount < mPreselectedHousehold.NumMembers)
                        {
                            testMetrics = false;
                        }
                    }

                    int numSims = 0;
                    if ((Mover.Settings.mHomeInspection) && (testMetrics) && (mSourceHousehold.LotHome != null))
                    {
                        numSims = GameEntryMovingModel.CountSimsIncludingPregnancy(mTargetSimList, true);
                    }

                    if (mSourceHouseholdActive)
                    {
                        if (!IsLotValid(mTargetLot, mTargetHouseholdName, numSims, mSourceHouseholdActive, ref reason))
                        {
                            if (string.IsNullOrEmpty(mTargetHouseholdName))
                            {
                                return MoveValidity.None;
                            }

                            reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:CantSupportPopulationOrMove");
                            if (mSourceHousehold != Household.ActiveHousehold)
                            {
                                return MoveValidity.None;
                            }

                            return MoveValidity.PlayerOnly;
                        }

                        if (mTargetLot != null)
                        {
                            return MoveValidity.All;
                        }

                        reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:NeedTargetLot");
                        if (mSourceHousehold != Household.ActiveHousehold)
                        {
                            return MoveValidity.None;
                        }

                        return MoveValidity.PlayerOnly;
                    }
                    else
                    {
                        if (mTargetLot == null)
                        {
                            reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:NeedTargetLot");
                            return MoveValidity.None;
                        }

                        if (mNewTargetFunds < 0x0)
                        {
                            reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:CantAfford");
                            return MoveValidity.None;
                        }

                        if (!IsLotValid(mTargetLot, mTargetHouseholdName, numSims, mSourceHouseholdActive, ref reason))
                        {
                            return MoveValidity.None;
                        }
                    }
                }
                else if (mSourceSimList.Count > 0x0)
                {
                    if (mSourceHouseholdActive)
                    {
                        if (mNewSourceFunds < 0x0)
                        {
                            reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:CantAfford");
                            return MoveValidity.None;
                        }
                    }
                    else
                    {
                        bool testMetrics = !mbForMarriage;
                        if ((mSourceHousehold != null) && (mSourceHousehold.LotHome != null))
                        {
                            Lot.LotMetrics metrics = new Lot.LotMetrics();
                            mSourceHousehold.LotHome.GetLotMetrics(ref metrics);

                            if (metrics.BedCount < mSourceHousehold.NumMembers)
                            {
                                testMetrics = false;
                            }
                        }

                        int numSims = 0;
                        if ((Mover.Settings.mHomeInspection) && (testMetrics))
                        {
                            numSims = GameEntryMovingModel.CountSimsIncludingPregnancy(mSourceSimList, true);
                        }

                        if ((mSourceHousehold == Household.ActiveHousehold) &&
                            (!IsLotValid(mSourceHousehold.LotHome, mSourceHouseholdName, numSims, !mSourceHouseholdActive, ref reason)))
                        {
                            reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:CantSupportPopulationOrMove");
                            if (mSourceHousehold != Household.ActiveHousehold)
                            {
                                return MoveValidity.None;
                            }
                            return MoveValidity.PlayerOnly;
                        }
                    }
                }
            }
            return MoveValidity.All;
        }

        public override int GetHouseholdBuyingPowerFunds(bool isSource)
        {
            if (Mover.Settings.mFreeRealEstate)
            {
                return int.MaxValue;
            }
            else
            {
                return GetHouseholdFunds(isSource);
            }
        }

        public override int GetHouseholdFunds(bool isSource)
        {
            int funds = GetHouseholdFunds(this, isSource);

            if (isSource)
            {
                mNewSourceFunds = funds;
            }
            else
            {
                mNewTargetFunds = funds;
            }

            return funds;
        }

        private static int GetHouseholdFunds(GameEntryMovingModel ths, Dictionary<ISimDescription, bool> sourceSimList, Household sourceHousehold, Lot sourceLot, int sourceHouseholdFunds, Dictionary<ISimDescription, bool> targetSimList, Household targetHousehold, Lot targetLot, int targetHouseholdFunds)
        {
            Common.StringBuilder msg = new Common.StringBuilder("HouseholdFunds");

            try
            {
                if (Households.NumHumans(sourceSimList.Keys) == 0x0)
                {
                    msg += Common.NewLine + "No Source";
                    return 0x0;
                }

                int originalTargetSims = 0x0;
                if (targetHousehold != null)
                {
                    originalTargetSims = Households.NumHumans(targetHousehold);
                }

                int numTargetSimsStaying = Households.NumHumans(targetSimList.Keys);

                msg += Common.NewLine + "Orig Targets: " + originalTargetSims;
                msg += Common.NewLine + "Stay Targets: " + numTargetSimsStaying;

                if (numTargetSimsStaying == 0x0) // Must be actual sims, not just humans
                {
                    // No sims in target household, so all of them must be in the source

                    int value = sourceHouseholdFunds + targetHouseholdFunds;

                    if ((targetSimList.Count > 0x0) || (targetLot == null) || (targetHousehold == null))
                    {
                        msg += Common.NewLine + "A: " + value;
                    }
                    else if (ths.mPackFurniture)
                    {
                        // Taking Furniture

                        if (!Mover.Settings.mFreeRealEstate)
                        {
                            value += Lots.GetUnfurnishedCost(targetLot);
                        }

                        msg += Common.NewLine + "B: " + value;
                    }
                    else
                    {
                        // Selling Furniture
                        value += Mover.GetLotCost(targetLot);

                        msg += Common.NewLine + "C: " + value;
                    }

                    return value;
                }
                else if (numTargetSimsStaying == originalTargetSims)
                {
                    msg += Common.NewLine + "D: " + sourceHouseholdFunds;

                    // No change in the number of target sims that have moved
                    return sourceHouseholdFunds;
                }

                float movePerPersonPercentage = (Mover.Settings.mMovePerPersonPercentage / 100f);

                int maxMoneyTransferred = Mover.Settings.mMaxMoneyTransferred;

                msg += Common.NewLine + "Move Percent: " + movePerPersonPercentage;
                msg += Common.NewLine + "Max Transfer: " + maxMoneyTransferred;

                int num5 = 0x0;
                if (numTargetSimsStaying > originalTargetSims)
                {
                    msg += Common.NewLine + "Staying > Original";

                    // Sims added to target household, and removed from source

                    if ((sourceHousehold != null) && (Households.NumHumans(sourceHousehold) > 0))
                    {
                        movePerPersonPercentage = Math.Min(movePerPersonPercentage, 1f / Households.NumHumans(sourceHousehold));

                        msg += Common.NewLine + "Adj Percent: " + movePerPersonPercentage;

                        int funds = sourceHousehold.FamilyFunds;
                        if (sourceHousehold.LotHome != null)
                        {
                            funds += Mover.GetLotCost(sourceHousehold.LotHome);
                        }

                        if (funds > SimDescription.kSimoleonThresholdForBeingRich)
                        {
                            maxMoneyTransferred = Mover.Settings.mMaxMoneyTransferredRich;

                            msg += Common.NewLine + "Adj Transfer: " + maxMoneyTransferred;
                        }
                    }

                    num5 = (int)(((numTargetSimsStaying - originalTargetSims) * movePerPersonPercentage) * sourceHouseholdFunds);

                    msg += Common.NewLine + "A: " + num5;

                    if (maxMoneyTransferred > 0)
                    {
                        num5 = Math.Min(num5, maxMoneyTransferred);

                        msg += Common.NewLine + "B: " + num5;
                    }

                    num5 = Math.Max(num5, Math.Min(Mover.Settings.mMinMoneyTransferred, sourceHouseholdFunds));

                    msg += Common.NewLine + "Min Transfer: " + Mover.Settings.mMinMoneyTransferred;
                    msg += Common.NewLine + "Source Funds: " + sourceHouseholdFunds;
                    msg += Common.NewLine + "C: " + num5;

                    if (num5 > sourceHouseholdFunds)
                    {
                        num5 = sourceHouseholdFunds;

                        msg += Common.NewLine + "D: " + num5;
                    }

                    msg += Common.NewLine + "E: " + (sourceHouseholdFunds - num5).ToString();

                    return (sourceHouseholdFunds - num5);
                }
                else
                {
                    // Sims removed from target household, and added to source

                    if ((targetHousehold != null) && (Households.NumHumans(targetHousehold) > 0))
                    {
                        movePerPersonPercentage = Math.Min(movePerPersonPercentage, 1f / Households.NumHumans(targetHousehold));

                        msg += Common.NewLine + "Adj Percent: " + movePerPersonPercentage;

                        int funds = targetHousehold.FamilyFunds;
                        if (targetHousehold.LotHome != null)
                        {
                            funds += Mover.GetLotCost(targetHousehold.LotHome);
                        }

                        if (funds > SimDescription.kSimoleonThresholdForBeingRich)
                        {
                            maxMoneyTransferred = Mover.Settings.mMaxMoneyTransferredRich;

                            msg += Common.NewLine + "Adj Transfer: " + maxMoneyTransferred;
                        }
                    }

                    num5 = (int)(((originalTargetSims - numTargetSimsStaying) * movePerPersonPercentage) * targetHouseholdFunds);

                    msg += Common.NewLine + "A: " + num5;

                    if (maxMoneyTransferred > 0)
                    {
                        num5 = Math.Min(num5, maxMoneyTransferred);

                        msg += Common.NewLine + "B: " + num5;
                    }

                    num5 = Math.Max(num5, Math.Min(Mover.Settings.mMinMoneyTransferred, targetHouseholdFunds));

                    msg += Common.NewLine + "Min Transfer: " + Mover.Settings.mMinMoneyTransferred;
                    msg += Common.NewLine + "Target Funds: " + targetHouseholdFunds;
                    msg += Common.NewLine + "C: " + num5;

                    if (num5 > targetHouseholdFunds)
                    {
                        num5 = targetHouseholdFunds;

                        msg += Common.NewLine + "D: " + num5;
                    }

                    msg += Common.NewLine + "E: " + (sourceHouseholdFunds + num5).ToString();

                    return (sourceHouseholdFunds + num5);
                }
            }
            finally
            {
                Common.DebugNotify(msg);
            }
        }

        private static int PrivateGetHouseholdFunds(GameEntryMovingModel ths, bool isSource)
        {
            Household targetHousehold = null;
            if (ths.mTargetLot != null)
            {
                targetHousehold = ths.mTargetLot.Household;
            }

            if (isSource)
            {
                return GetHouseholdFunds(
                    ths,
                    ths.mSourceSimList,
                    ths.mSourceHousehold,
                    ths.mSourceHousehold.LotHome,
                    ths.mSourceHousehold.FamilyFunds,
                    ths.mTargetSimList,
                    targetHousehold,
                    ths.mTargetLot,
                    ths.mTargetHouseholdFunds
                );
            }
            else
            {
                return GetHouseholdFunds(
                    ths,
                    ths.mTargetSimList,
                    targetHousehold,
                    ths.mTargetLot,
                    ths.mTargetHouseholdFunds,
                    ths.mSourceSimList,
                    ths.mSourceHousehold,
                    ths.mSourceHousehold.LotHome,
                    ths.mSourceHousehold.FamilyFunds
                );
            }
        }

        public static int GetHouseholdFunds(GameEntryMovingModel ths, bool isSource)
        {
            int funds = PrivateGetHouseholdFunds(ths, isSource);

            if ((ths.mTargetLot != null) && (ths.mTargetLot.Household == null))
            {
                float totalFunds = PrivateGetHouseholdFunds(ths, true) + PrivateGetHouseholdFunds(ths, false);

                float factor = isSource ? 1f : 0f;
                if (totalFunds != 0)
                {
                    factor = (funds / totalFunds);
                }

                if (factor > 1f)
                {
                    factor = 1f;
                }

                funds -= (int)(Mover.GetLotCost(ths.mTargetLot) * factor);
            }

            return funds;
        }

        public static MoveValidity BaseIsLotValid(IMovingModel model, Dictionary<ISimDescription, bool> sourceSimList, Dictionary<ISimDescription, bool> targetSimList, Household household, bool isSource, ref string reason)
        {
            Dictionary<ISimDescription, bool> simList;

            if (isSource)
            {
                simList = sourceSimList;
            }
            else
            {
                simList = targetSimList;
            }

            if (!Mover.Settings.mAllowGreaterThanEight)
            {
                if (CountSimsIncludingPregnancy(simList, true) > 0x8)
                {
                    if (simList.Count > 0x8)
                    {
                        reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:TooManySims");
                    }
                    else
                    {
                        reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:TooManySims_Pregnant");
                    }
                    return MoveValidity.None;
                }
            }

            // Keep as GameStates
            if (((GameStates.IsOnVacation) && isSource) && ((simList.Count < 0x1) && household.IsActive))
            {
                reason = Common.LocalizeEAString("Ui/Caption/MovingDialog/EP01:CantSplitEveryone");
                return MoveValidity.None;
            }

            if (!Mover.Settings.mAllowNoAdult)
            {
                bool flag = false;
                bool flag2 = false;

                foreach (KeyValuePair<ISimDescription, bool> pair in simList)
                {
                    CASAgeGenderFlags age = pair.Key.Age;
                    if (age <= CASAgeGenderFlags.Teen)
                    {
                        flag = true;
                    }

                    if (age >= CASAgeGenderFlags.YoungAdult)
                    {
                        flag2 = true;
                    }
                }
                if (flag && !flag2)
                {
                    reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:AdultRequired");
                    return MoveValidity.None;
                }
            }

            if ((simList.Count >= 0x1) && string.IsNullOrEmpty(model.GetHouseholdName(isSource)))
            {
                reason = Common.LocalizeEAString("Ui/Caption/MovingDialog:MustHaveHouseholdName");
                return MoveValidity.None;
            }
            return MoveValidity.All;
        }

        public class ProtectFunds : IDisposable
        {
            int mMaxMoneyTransferredRich;
            int mMaxMoneyTransferredNormal;

            public ProtectFunds(Household house)
            {
                mMaxMoneyTransferredRich = GameplayMovingModel.kMaxMoneyTransferredRich;
                mMaxMoneyTransferredNormal = GameplayMovingModel.kMaxMoneyTransferredNormal;

                if (house != null)
                {
                    GameplayMovingModel.kMaxMoneyTransferredRich = house.FamilyFunds;
                    GameplayMovingModel.kMaxMoneyTransferredNormal = house.FamilyFunds;
                }
            }

            public void Dispose()
            {
                GameplayMovingModel.kMaxMoneyTransferredRich = mMaxMoneyTransferredRich;
                GameplayMovingModel.kMaxMoneyTransferredNormal = mMaxMoneyTransferredNormal;
            }
        }
    }
}
