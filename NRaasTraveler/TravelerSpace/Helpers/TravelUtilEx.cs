using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Dialogs;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class TravelUtilEx
    {
        public enum PlaceResults
        {
            Failure,
            BaseCamp,
            Residential,
        }

        public static bool FinalBoardingCall(Household travelingHousehold, List<Sim> allTravelers, WorldName worldName, bool isWorldMove, ref string reason)
        {
            foreach (Sim sim in allTravelers)
            {
                reason = CommonSpace.Helpers.TravelUtilEx.CheckForReasonsToFailTravel(sim.SimDescription, Traveler.Settings.mTravelFilter, worldName, isWorldMove, false);
                if (!string.IsNullOrEmpty(reason))
                {
                    return false;
                }

                /*
                if (sim.Household != travelingHousehold)
                {
                    reason = "Not in Household: " + sim.FullName;
                    return false;
                }
                */
            }

            reason = null;
            return true;
        }

        private static bool CanMoveWorldsInternal(Sim actor, bool testFamilyPregnancy, WorldName worldName, bool ignoreCertainTransformBuffs, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            if ((actor == null) || GameUtils.IsOnVacation() || actor.BuffManager.HasElement(BuffNames.HeartOfGold) || (Household.RoommateManager.IsNPCRoommate(actor)))
            {
                return false;
            }

            CommonSpace.Helpers.TravelUtilEx.Type filter = Traveler.Settings.mTravelFilter;
            if (testFamilyPregnancy)
            {
                filter &= ~CommonSpace.Helpers.TravelUtilEx.Type.Pregnant;
            }

            string reason = CommonSpace.Helpers.TravelUtilEx.CheckForReasonsToFailTravel(actor.SimDescription, filter, worldName, false, testMoveRequested);
            if (!string.IsNullOrEmpty(reason))
            {
                callback = delegate { return reason; };
                return false;
            }

            return true;
        }

        public static bool CanMoveFutureWorlds(Sim actor, bool testFamilyPregnancy, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            return CanMoveWorldsInternal(actor, testFamilyPregnancy, WorldName.FutureWorld, true, testMoveRequested, ref callback);
        }

        public static bool CanMoveWorlds(Sim actor, bool testFamilyPregnancy, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            return CanMoveWorldsInternal(actor, testFamilyPregnancy, WorldName.Undefined, false, testMoveRequested, ref callback);
        }

        public static bool CanSimTriggerTravelToFutureWorld(Sim actor, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP11))
            {
                return false;
            }

            if (!CanMoveFutureWorlds(actor, false, testMoveRequested, ref callback))
            {
                return false;
            }
            return true;
        }

        public static bool CanSimTriggerTravelToUniversityWorld(Sim actor, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            return CommonSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToUniversityWorld(actor, Traveler.Settings.mTravelFilter, testMoveRequested, ref callback);
        }

        public static bool CanSimTriggerTravelToHomeWorld(Sim actor, ref GreyedOutTooltipCallback callback)
        {
            if ((actor == null) || (!GameUtils.IsOnVacation ()))
            {
                callback = Common.DebugTooltip("Not Vacation World Type");
                return false;
            }

            if ((actor.BuffManager != null) && actor.BuffManager.HasTransformBuff())
            {
                callback = new GreyedOutTooltipCallback(new TravelUtil.GreyedOutTooltipHelper(actor, "NoTravelWhileTransformed", true).TextTextAndAway);
                return false;
            }

            if (GameStates.TravelHousehold != Household.ActiveHousehold)
            {
                callback = delegate
                {
                    return Common.Localize("TravelHome:NotActive");
                };
                return false;
            }

            string failReason = null;
            if (!InWorldSubState.IsEditTownValid(actor.LotHome, ref failReason))
            {
                callback = new GreyedOutTooltipCallback(new TravelUtil.GreyedOutTooltipHelper(actor, "EditTownInvalid", false).TextTextAndAway);
                return false;
            }

            return true;
        }

        public static bool CanSimTriggerTravelToVacationWorld(Sim actor, bool testMoveRequested, ref GreyedOutTooltipCallback callback)
        {
            if (actor == null)
            {
                callback = Common.DebugTooltip("No Actor");
                return false;
            }

            if (!Traveler.HasBeenSaved ())
            {
                callback = delegate { return Common.Localize("Save:Prompt"); };
                return false;
            }

            /*
            if (!GameUtils.IsInstalled(ProductVersion.EP1))
            {
                callback = Common.DebugTooltip("Pack Not Installed");
                return false;
            }
             */

            string reason = CommonSpace.Helpers.TravelUtilEx.CheckForReasonsToFailTravel(actor.SimDescription, Traveler.Settings.mTravelFilter, WorldName.Undefined, false, testMoveRequested);
            if (!string.IsNullOrEmpty(reason))
            {
                callback = delegate { return reason; };
                return false;
            }

            return true;
        }

        public static Lot PromptForLot()
        {
            Lot baseCampLot = LotManager.GetBaseCampLot();
            if (!UIUtils.IsOkayToStartModalDialog(false, true))
            {
                return null;
            }

            List<IMapTagPickerInfo> mapTagPickerInfos = new List<IMapTagPickerInfo>();
            mapTagPickerInfos.Add(new MapTagPickerLotInfo(baseCampLot, MapTagType.BaseCamp));
            Dictionary<ulong, bool> dictionary = new Dictionary<ulong, bool>();

            foreach (Lot lot in LotManager.AllLots)
            {
                if (!lot.IsResidentialLot) continue;

                if (lot.IsWorldLot) continue;

                if (lot.Household != null) continue;

                if (lot.ResidentialLotSubType == ResidentialLotSubType.kEP1_PlayerOwnable)
                {
                    continue;
                }

                Lot.LotMetrics metrics = new Lot.LotMetrics();
                lot.GetLotMetrics(ref metrics);
                if (metrics.FridgeCount == 0)
                {
                    continue;
                }

                if ((lot != null) && !dictionary.ContainsKey(lot.LotId))
                {
                    dictionary[lot.LotId] = true;
                    mapTagPickerInfos.Add(new MapTagPickerLotInfo(lot, MapTagType.AvailableHousehold));
                }
            }

            GameUtils.EnableSceneDraw(true);
            LoadingScreenController.Unload();
            while (LoadingScreenController.Instance != null)
            {
                SpeedTrap.Sleep(0);
            }

            IMapTagPickerInfo info = MapTagPickerDialog.Show(mapTagPickerInfos, TravelUtil.LocalizeString("ChooseHomeLot", new object[0]), TravelUtil.LocalizeString("Accept", new object[0]), false);
            if (info == null)
            {
                return null;
            }

            return LotManager.GetLot(info.LotId);
        }

        public static Lot FindLot()
        {
            for (int i = 0; i < 2; i++)
            {
                Lot choice = null;

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (!lot.IsResidentialLot) continue;

                    if (lot.IsWorldLot) continue;

                    if (lot.Household != null) continue;

                    if (i == 0)
                    {
                        if (lot.ResidentialLotSubType == ResidentialLotSubType.kEP1_PlayerOwnable)
                        {
                            continue;
                        }

                        Lot.LotMetrics metrics = new Lot.LotMetrics();
                        lot.GetLotMetrics(ref metrics);
                        if (metrics.FridgeCount == 0)
                        {
                            continue;
                        }
                    }

                    if ((choice == null) || (choice.Cost > lot.Cost))
                    {
                        choice = lot;
                    }
                }

                if (choice != null) return choice;
            }

            return null;
        }

        public static PlaceResults LocateHomeAndPlaceSimsAtVacationWorld(Household household, ref Sim simToSelect)
        {
            PlaceResults results = PlaceResults.Failure;

            Common.StringBuilder msg = new Common.StringBuilder("LocateHomeAndPlaceSimsAtVacationWorld" + Common.NewLine);

            try
            {
                if (household != null)
                {
                    msg += "A";

                    if (GameStates.DestinationTravelWorld == WorldName.University)
                    {
                        Dictionary<SimDescription,AcademicDegreeManager> managers = new Dictionary<SimDescription,AcademicDegreeManager>();

                        try
                        {
                            foreach (SimDescription sim in Households.All(household))
                            {
                                if (sim.CareerManager == null) continue;

                                managers[sim] = sim.CareerManager.DegreeManager;

                                if ((sim.ChildOrBelow) || (sim.IsPet))
                                {
                                    sim.CareerManager.mDegreeManager = null;
                                }
                                else if ((sim.CareerManager.DegreeManager != null) && (sim.CareerManager.DegreeManager.EnrollmentCouseLoad == 0))
                                {
                                    sim.CareerManager.mDegreeManager = null;
                                }
                            }

                            TravelUtil.MoveIntoUniversityHousehold(household);

                            foreach (SimDescription sim in Households.All(household))
                            {
                                CustomAcademicDegrees.AdjustCustomAcademics(sim);
                            }
                        }
                        finally
                        {
                            foreach (SimDescription sim in Households.All(household))
                            {
                                if (sim.CareerManager == null) continue;

                                AcademicDegreeManager manager;
                                if (!managers.TryGetValue(sim, out manager)) continue;

                                sim.CareerManager.mDegreeManager = manager;
                            }
                        }

                        results = PlaceResults.BaseCamp;
                    }
                    else
                    {
                        TravelUtil.ProcessDeedsAndMoveInHousehold(household);

                        if (household.LotHome == null)
                        {
                            msg += "B";

                            bool manual = false;

                            Lot choice = PromptForLot();
                            if (choice == null)
                            {
                                choice = FindLot();
                            }
                            else
                            {
                                manual = true;
                            }

                            if (choice != null)
                            {
                                msg += "C";

                                msg += Common.NewLine + choice.Name + Common.NewLine;

                                choice.MoveIn(household);

                                Mailbox mailboxOnLot = Mailbox.GetMailboxOnLot(choice);
                                if (mailboxOnLot != null)
                                {
                                    mailboxOnLot.ListenToReturnFromWorld();
                                }

                                if (household.LotHome != null)
                                {
                                    msg += "D";

                                    if (manual)
                                    {
                                        results = PlaceResults.BaseCamp;
                                    }
                                    else
                                    {
                                        results = PlaceResults.Residential;
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg += "E";

                            results = PlaceResults.BaseCamp;
                        }
                    }

                    if (household.LotHome != null)
                    {
                        msg += "F";

                        foreach (Service service in Services.AllServices)
                        {
                            if (service == null) continue;

                            if (service.DefaultIsRequested()) continue;

                            service.MakeServiceRequest(household.LotHome, false, ObjectGuid.InvalidObjectGuid);
                        }

                        TravelUtil.TriggerTutorial(household);
                        TravelUtil.PlaceSimsOnSafeSpots(household, ref simToSelect);
                    }
                }
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
            finally
            {
                Traveler.InsanityWriteLog(msg);
            }

            return results;
        }

        private static int[] GetPartnershipBonuses(SimDescription simDesc)
        {
            int[] numArray = new int[TravelUtil.kVacationWorldNames.Length]; // Changed
            if ((simDesc != null) && (simDesc.CreatedSim != null))
            {
                foreach (ICertificateOfPartnership partnership in Inventories.QuickDuoFind<ICertificateOfPartnership,GameObject>(simDesc.CreatedSim.Inventory))
                {
                    int index = Array.IndexOf<WorldName>(TravelUtil.kVacationWorldNames, partnership.CertificatesWorld);
                    numArray[index] = partnership.BonusDaysOfTravel;
                }
            }
            return numArray;
        }

        public static bool ShowTravelToVacationWorldDialog(Sim travelInitiator, out TripPlannerDialog.Result tripPlannerResult)
        {
            tripPlannerResult = null;
            if (((travelInitiator == null) || (travelInitiator.Household == null)) || !travelInitiator.IsSelectable)
            {
                Common.DebugNotify("Not IsSelectable");
                return false;
            }
            else if (!UIUtils.IsOkayToStartModalDialog())
            {
                Common.DebugNotify("Not IsOkayToStartModalDialog");
                return false;
            }
            else if (!TravelUtil.CommonTravelTests(travelInitiator.LotHome))
            {
                Common.DebugNotify("Not CommonTravelTests");
                return false;
            }

            bool onVacation = (GameStates.sTravelData != null);

            string worldFile = World.GetWorldFileName();

            List<TripPlannerDialog.IDestinationInfo> destinations = new List<TripPlannerDialog.IDestinationInfo>();
            for (int i = 0x0; i < TravelUtil.kVacationWorldNames.Length; i++)
            {
                if (worldFile == WorldData.GetWorldFile(TravelUtil.kVacationWorldNames[i])) continue;

                if (Traveler.Settings.GetHiddenWorlds(TravelUtil.kVacationWorldNames[i])) continue;

                if (GameStates.sTravelData != null)
                {
                    if (worldFile == GameStates.sTravelData.mHomeWorld) continue;
                }

                DestinationInfoEx item = new DestinationInfoEx(TravelUtil.DestinationInfoImage[i], TravelUtil.DestinationInfoName[i], TravelUtil.DestinationInfoDescription[i], TravelUtil.DestinationInfoConfirmImage[i], TravelUtil.DestinationInfoComfirmDescription[i], TravelUtil.DestinationInfoIndex[i], TravelUtil.kVacationWorldNames[i]);
                destinations.Add(item);
            }

            TravelUtil.SimTravelInfo defaultTraveler = null;
            List<TripPlannerDialog.ISimTravelInfo> simTravelInfoList = new List<TripPlannerDialog.ISimTravelInfo>();

            foreach (KeyValuePair<SimDescription,string> key in CommonSpace.Helpers.TravelUtilEx.GetTravelChoices(travelInitiator, Traveler.Settings.mTravelFilter, false))
            {
                SimDescription description = key.Key;

                WorldData.OnLoadFixup(description.VisaManager);

                foreach (WorldName worldName in TravelUtil.kVacationWorldNames)
                {
                    switch (worldName)
                    {
                        case WorldName.China:
                        case WorldName.Egypt:
                        case WorldName.France:
                            continue;
                    }

                    description.VisaManager.SetVisaLevel(worldName, 3);
                }

                List<int> visas = TravelUtil.GetVisas(description);

                int costForSimWithAge = TravelUtil.GetCostForSimWithAge(description.Age);
                int durationMod = 0x0;
                if (description.TraitManager.HasElement(TraitNames.PreparedTraveler))
                {
                    durationMod = TraitTuning.PreparedTravelerDurationIncrease;
                }

                if (description.TraitManager.HasElement(TraitNames.Jetsetter))
                {
                    costForSimWithAge = (int)(costForSimWithAge * TraitTuning.JetsetterCostMultiplier);
                }

                if ((description.CareerManager != null) && (description.CareerManager.DegreeManager != null))
                {
                    // Used in LocateHomeAndPlaceSimsAtVacationWorld to differentiate between regular travel and university
                    description.CareerManager.DegreeManager.mEnrollmentData.mCourseLoad = 0;
                }

                int[] partnershipBonuses = GetPartnershipBonuses(description);

                if (description == travelInitiator.SimDescription)
                {
                    defaultTraveler = new TravelUtil.SimTravelInfo(description, costForSimWithAge, durationMod, null, visas, partnershipBonuses);
                }
                else
                {
                    simTravelInfoList.Add(new TravelUtil.SimTravelInfo(description, costForSimWithAge, durationMod, key.Value, visas, partnershipBonuses));
                }
            }

            if (TravelUtil.PlayerMadeTravelRequest)
            {
                Common.DebugNotify("PlayerMadeTravelRequest Fail");
                return false;
            }

            List<int> durations = new List<int>(TravelUtil.kTravelDurations);
            tripPlannerResult = TripPlannerDialogEx.Show(destinations, durations, simTravelInfoList, defaultTraveler);

            if (tripPlannerResult != null)
            {
                DestinationInfoEx destination = tripPlannerResult.Destination as DestinationInfoEx;
                if (destination != null)
                {
                    tripPlannerResult.mDestination = new TravelUtil.DestinationInfo(
                        destination.Image,
                        destination.Name,
                        destination.Description,
                        destination.ConfirmImage,
                        destination.ConfirmDescription,
                        destination.Index,
                        destination.WorldName
                    );
                }
            }

            TravelUtil.PlayerMadeTravelRequest = (tripPlannerResult != null);
            return TravelUtil.PlayerMadeTravelRequest;
        }

        public class DestinationInfoEx : TripPlannerDialog.IDestinationInfo
        {
            private string mConfirmDescription;
            private string mConfirmImage;
            private string mDescription;
            private string mImage;
            private int mIndex;
            private string mName;
            private WorldName mWorldName;

            public DestinationInfoEx(string image, string name, string description, string confirmImage, string confirmDescription, int index, WorldName worldName)
            {
                mImage = image;
                mName = name;
                mDescription = description;
                mConfirmImage = confirmImage;
                mConfirmDescription = confirmDescription;
                mIndex = index;
                mWorldName = worldName;
            }

            public int BaseCost(int simDays)
            {
                switch (mWorldName)
                {
                    case WorldName.China:
                    case WorldName.France:
                    case WorldName.Egypt:
                        return TravelUtil.GetTripCost(simDays, mWorldName);
                }

                return (simDays * Traveler.Settings.mCostPerDay);
            }

            public int MaxBaseDuration(int visaLevel)
            {
                return VisaManager.GetMaxTripDuration(visaLevel);
            }

            public string ConfirmDescription
            {
                get
                {
                    return mConfirmDescription;
                }
            }

            public string ConfirmImage
            {
                get
                {
                    return mConfirmImage;
                }
            }

            public string Description
            {
                get
                {
                    return mDescription;
                }
            }

            public string Image
            {
                get
                {
                    return mImage;
                }
            }

            public int Index
            {
                get
                {
                    return mIndex;
                }
            }

            public string Name
            {
                get
                {
                    return mName;
                }
            }

            internal WorldName WorldName
            {
                get
                {
                    return mWorldName;
                }
            }
        }
    }
}
