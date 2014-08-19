using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Options.Romance;
using NRaas.WoohooerSpace.Options.TryForBaby;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        public Dictionary<ulong, bool> mOutfitChangers = new Dictionary<ulong, bool>();

        public Dictionary<ulong, int> mWoohooCount = new Dictionary<ulong, int>();

        public Dictionary<ulong, DateAndTime> mLastWoohoo = new Dictionary<ulong, DateAndTime>();

        public bool[] mMustBeRomanticForAutonomousV2 = new bool[] { WoohooerTuning.kMustBeRomanticForAutonomous, WoohooerTuningHorse.kMustBeRomanticForAutonomous, WoohooerTuningDog.kMustBeRomanticForAutonomous, WoohooerTuningCat.kMustBeRomanticForAutonomous };
        public bool mEnforcePrivacy = WoohooerTuning.kEnforcePrivacy;
        private bool mDebugging = WoohooerTuning.kDebugging;
        public bool mSimMenuVisibility = WoohooerTuning.kSimMenuVisibility;
        public int mPrivacyBaseChanceScoring = WoohooerTuning.kPrivacyBaseChanceScoring;
        public int[] mWoohooCountScoreFactor = new int[] { WoohooerTuning.kWoohooCountScoreFactor, WoohooerTuningHorse.kWoohooCountScoreFactor, WoohooerTuningDog.kWoohooCountScoreFactor, WoohooerTuningCat.kWoohooCountScoreFactor };
        public bool mInteractionsUnderRomance = WoohooerTuning.kInteractionsUnderRomance;
        public bool mChangeRoomClothings = WoohooerTuning.kChangeRoomClothings;

        public bool mUseTraitScoring = WoohooerTuning.kUseTraitScoring;
        public bool mTraitScoringForUserDirected = WoohooerTuning.kTraitScoringForUserDirected;
        public bool mUseMoodInScoring = WoohooerTuning.kUseMoodInScoring;
        public int mReactToJealousyBaseChanceScoring = WoohooerTuning.kReactToJealousyBaseChanceScoring;
        public int mSneakinessBaseChanceScoring = WoohooerTuning.kSneakinessBaseChanceScoring;
        public bool mTraitScoredPrivacy = WoohooerTuning.kTraitScoredPrivacy;
        public MyLoveBuffLevel mMyLoveBuffLevel = WoohooerTuning.kMyLoveBuffLevel;
        public MyLoveBuffLevel[] mWoohooInteractionLevelV2 = new MyLoveBuffLevel[] { WoohooerTuning.kWoohooInteractionLevel, WoohooerTuningHorse.kWoohooInteractionLevel, WoohooerTuningDog.kWoohooInteractionLevel, WoohooerTuningCat.kWoohooInteractionLevel };
        public MyLoveBuffLevel mPartneringInteractionLevel = WoohooerTuning.kPartneringInteractionLevel;
        public bool mRestrictTeenMarriage = WoohooerTuning.kRestrictTeenMarriage;
        public bool mOnlyResidentMatchmaker = WoohooerTuning.kOnlyResidentMatchmaker;

        public int[] mLikingGatingForAutonomousRomance = new int[] { WoohooerTuning.kLikingGatingForAutonomousRomance, WoohooerTuningHorse.kLikingGatingForAutonomousRomance, WoohooerTuningDog.kLikingGatingForAutonomousRomance, WoohooerTuningCat.kLikingGatingForAutonomousRomance };
        public int[] mAttractionBaseChanceScoringV3 = new int[] { WoohooerTuning.kAttractionBaseChanceScoring, WoohooerTuningHorse.kAttractionBaseChanceScoring, WoohooerTuningDog.kAttractionBaseChanceScoring, WoohooerTuningCat.kAttractionBaseChanceScoring };
        public int[] mRomanceBaseChanceScoringV2 = new int[] { WoohooerTuning.kRomanceBaseChanceScoring, WoohooerTuningHorse.kRomanceBaseChanceScoring, WoohooerTuningDog.kRomanceBaseChanceScoring, WoohooerTuningCat.kRomanceBaseChanceScoring };
        public bool mAllowTeenAdultRomance = WoohooerTuning.kAllowTeenAdultRomance;
        public bool[] mAllowNearRelationRomanceV2 = new bool[] { WoohooerTuning.kAllowNearRelationRomance, WoohooerTuningHorse.kAllowNearRelationRomance, WoohooerTuningDog.kAllowNearRelationRomance, WoohooerTuningCat.kAllowNearRelationRomance };
        public bool[] mAllowNearRelationRomanceAutonomousV2 = new bool[] { WoohooerTuning.kAllowNearRelationRomanceAutonomous, WoohooerTuningHorse.kAllowNearRelationRomanceAutonomous, WoohooerTuningDog.kAllowNearRelationRomanceAutonomous, WoohooerTuningCat.kAllowNearRelationRomanceAutonomous };
        public bool mLikingGateForUserDirected = WoohooerTuning.kLikingGateForUserDirected;
        public bool mGenderPreferenceForUserDirectedV2 = WoohooerTuning.kGenderPreferenceForUserDirected;
        public bool mDisallowHomeless = WoohooerTuning.kDisallowHomeless;
        public bool mAllowAutonomousRomanceCommLot = WoohooerTuning.kAllowAutonomousRomanceCommLot;
        public JealousyLevel mRomanceJealousyLevel = WoohooerTuning.kRomanceJealousyLevel;
        public MyLoveBuffLevel mRomanceInteractionLevel = WoohooerTuning.kRomanceInteractionLevel;
        public MyLoveBuffLevel mSteadyInteractionLevel = WoohooerTuning.kSteadyInteractionLevel;
        public MyLoveBuffLevel mMarriedInteractionLevel = WoohooerTuning.kMarriedInteractionLevel;

        public List<ServiceType> mDisallowAutonomousServiceTypes = new List<ServiceType>(WoohooerTuning.kDisallowAutonomousServiceTypes);

        public List<Role.RoleType> mDisallowAutonomousRoleTypes = new List<Role.RoleType>(WoohooerTuning.kDisallowAutonomousRoleTypes);

        public int[] mLikingGatingForAutonomousWoohoo = new int[] { WoohooerTuning.kLikingGatingForAutonomousWoohoo, WoohooerTuningHorse.kLikingGatingForAutonomousWoohoo, WoohooerTuningDog.kLikingGatingForAutonomousWoohoo, WoohooerTuningCat.kLikingGatingForAutonomousWoohoo };
        public int[] mWoohooBaseChanceScoringV2 = new int[] { WoohooerTuning.kWoohooBaseChanceScoring, WoohooerTuningHorse.kWoohooBaseChanceScoring, WoohooerTuningDog.kWoohooBaseChanceScoring, WoohooerTuningCat.kWoohooBaseChanceScoring };
        public int mWoohooTeenBaseChanceScoring = WoohooerTuning.kWoohooTeenBaseChanceScoring;
        public bool mHideWoohoo = WoohooerTuning.kHideWoohoo;
        public bool[] mWoohooAutonomousV2 = new bool[] { WoohooerTuning.kWoohooAutonomous, WoohooerTuningHorse.kWoohooAutonomous, WoohooerTuningDog.kWoohooAutonomous, WoohooerTuningCat.kWoohooAutonomous };
        public bool[] mWoohootyTextAutonomous = new bool[] { WoohooerTuning.kWoohootyTextAutonomous };
        public bool mAllowTeenWoohoo = WoohooerTuning.kAllowTeenWoohoo;
        public bool mAllowTeenAdultWoohoo = WoohooerTuning.kAllowTeenAdultWoohoo;
        public bool[] mAllowNearRelationWoohooV2 = new bool[] { WoohooerTuning.kAllowNearRelationWoohoo, WoohooerTuningHorse.kAllowNearRelationWoohoo, WoohooerTuningDog.kAllowNearRelationWoohoo, WoohooerTuningCat.kAllowNearRelationWoohoo };
        private bool[] mAllowNearRelationWoohooAutonomousV2 = new bool[] { WoohooerTuning.kAllowNearRelationWoohooAutonomous, WoohooerTuningHorse.kAllowNearRelationWoohooAutonomous, WoohooerTuningDog.kAllowNearRelationWoohooAutonomous, WoohooerTuningCat.kAllowNearRelationWoohooAutonomous };
        public int mWoohooRechargeRate = WoohooerTuning.kWoohooRechargeRate;
        public bool mPolyamorousWoohooJealousy = WoohooerTuning.kPolyamorousWoohooJealousy;
        public bool mAllowStrideOfPride = WoohooerTuning.kAllowStrideOfPride;
        public bool mApplyBuffs = WoohooerTuning.kApplyBuffs;
        public int mMatchmakerCost = WoohooerTuning.kMatchmakerCost;
        public bool mAllowForeplay = WoohooerTuning.kAllowForeplay;
        public bool[] mForcePetMate = new bool[] { false, WoohooerTuningHorse.kForcePetMate, WoohooerTuningDog.kForcePetMate, WoohooerTuningCat.kForcePetMate };
        public bool mSwitchToEverydayAfterNakedWoohoo = WoohooerTuning.kSwitchToEverydayAfterNakedWoohoo;
        public int[] mWoohooCooldown = new int[] { WoohooerTuning.kWoohooCooldown, WoohooerTuningHorse.kWoohooCooldown, WoohooerTuningDog.kWoohooCooldown, WoohooerTuningCat.kWoohooCooldown };
        public JealousyLevel mWoohooJealousyLevel = WoohooerTuning.kWoohooJealousyLevel;

        public int[] mRiskyBaseChanceScoringV2 = new int[] { WoohooerTuning.kRiskyBaseChanceScoring, WoohooerTuningHorse.kRiskyBaseChanceScoring, WoohooerTuningDog.kRiskyBaseChanceScoring, WoohooerTuningCat.kRiskyBaseChanceScoring };
        public int mRiskyTeenBaseChanceScoring = WoohooerTuning.kRiskyTeenBaseChanceScoring;
        public int[] mRiskyBabyMadeChanceV2 = new int[] { WoohooerTuning.kRiskyBabyMadeChance, WoohooerTuningHorse.kRiskyBabyMadeChance, WoohooerTuningDog.kRiskyBabyMadeChance, WoohooerTuningCat.kRiskyBabyMadeChance };
        public int mRiskyTeenBabyMadeChance = WoohooerTuning.kRiskyTeenBabyMadeChance;
        public bool[] mRiskyAutonomousV2 = new bool[] { WoohooerTuning.kRiskyAutonomous, WoohooerTuningHorse.kRiskyAutonomous, WoohooerTuningDog.kRiskyAutonomous, WoohooerTuningCat.kRiskyAutonomous };
        public bool mTeenRiskyAutonomous = WoohooerTuning.kTeenRiskyAutonomous;
        public bool mReplaceWithRisky = WoohooerTuning.kReplaceWithRisky;
        public bool[] mRiskyFertility = new bool[] { WoohooerTuning.kRiskyFertility, WoohooerTuningHorse.kRiskyFertility, WoohooerTuningDog.kRiskyFertility, WoohooerTuningCat.kRiskyFertility };
        public bool mShowRiskyChance = WoohooerTuning.kShowRiskyChance;
        public PregnancyChoice mRiskyPregnancyChoice = WoohooerTuning.kRiskyPregnancyChoice;
        public bool[] mAllowOffLotRiskyAutonomous = new bool[] { WoohooerTuning.kAllowOffLotRiskyAutonomous, WoohooerTuningHorse.kAllowOffLotRiskyAutonomous, WoohooerTuningDog.kAllowOffLotRiskyAutonomous, WoohooerTuningCat.kAllowOffLotRiskyAutonomous };

        public int[] mMaximumHouseholdSizeForAutonomousV2 = new int[] { WoohooerTuning.kMaximumHouseholdSizeForAutonomous, WoohooerTuningHorse.kMaximumHouseholdSizeForAutonomous, WoohooerTuningDog.kMaximumHouseholdSizeForAutonomous, WoohooerTuningCat.kMaximumHouseholdSizeForAutonomous };
        public int[] mTryForBabyUserMaximum = new int[] { WoohooerTuning.kTryForBabyUserMaximum, WoohooerTuningHorse.kTryForBabyUserMaximum, WoohooerTuningDog.kTryForBabyUserMaximum, WoohooerTuningCat.kTryForBabyUserMaximum };
        public int[] mTryForBabyBaseChanceScoringV2 = new int[] { WoohooerTuning.kTryForBabyBaseChanceScoring, WoohooerTuningHorse.kTryForBabyBaseChanceScoring, WoohooerTuningDog.kTryForBabyBaseChanceScoring, WoohooerTuningCat.kTryForBabyBaseChanceScoring };
        public int mTryForBabyTeenBaseChanceScoring = WoohooerTuning.kTryForBabyTeenBaseChanceScoring;
        public bool[] mAutonomousMaleMaleTryForBabyV2 = new bool[] { WoohooerTuning.kAutonomousMaleMaleTryForBaby, WoohooerTuningHorse.kAutonomousMaleMaleTryForBaby, WoohooerTuningDog.kAutonomousMaleMaleTryForBaby, WoohooerTuningCat.kAutonomousMaleMaleTryForBaby };
        public bool[] mAutonomousFemaleFemaleTryForBabyV2 = new bool[] { WoohooerTuning.kAutonomousFemaleFemaleTryForBaby, WoohooerTuningHorse.kAutonomousFemaleFemaleTryForBaby, WoohooerTuningDog.kAutonomousFemaleFemaleTryForBaby, WoohooerTuningCat.kAutonomousFemaleFemaleTryForBaby };
        public int[] mTryForBabyMadeChanceV2 = new int[] { WoohooerTuning.kTryForBabyMadeChance, WoohooerTuningHorse.kTryForBabyMadeChance, WoohooerTuningDog.kTryForBabyMadeChance, WoohooerTuningCat.kTryForBabyMadeChance };
        public int mTryForBabyTeenBabyMadeChance = WoohooerTuning.kTryForBabyTeenBabyMadeChance;
        public bool[] mTryForBabyAutonomousV2 = new bool[] { WoohooerTuning.kTryForBabyAutonomous, WoohooerTuningHorse.kTryForBabyAutonomous, WoohooerTuningDog.kTryForBabyAutonomous, WoohooerTuningCat.kTryForBabyAutonomous };
        public bool mTeenTryForBabyAutonomous = WoohooerTuning.kTeenTryForBabyAutonomous;
        public bool[] mAllowSameSexTryForBabyV2 = new bool[] { WoohooerTuning.kAllowSameSexTryForBaby, WoohooerTuningHorse.kAllowSameSexTryForBaby, WoohooerTuningDog.kAllowSameSexTryForBaby, WoohooerTuningCat.kAllowSameSexTryForBaby };
        public bool[] mTryForBabyFertility = new bool[] { WoohooerTuning.kTryForBabyFertility, WoohooerTuningHorse.kTryForBabyFertility, WoohooerTuningDog.kTryForBabyFertility, WoohooerTuningCat.kTryForBabyFertility };
        public bool[] mTestAllConditionsForUserDirected = new bool[] { WoohooerTuning.kTestAllConditionsForUserDirected, WoohooerTuningHorse.kTestAllConditionsForUserDirected, WoohooerTuningDog.kTestAllConditionsForUserDirected, WoohooerTuningCat.kTestAllConditionsForUserDirected };
        public PregnancyChoice mTryForBabyPregnancyChoice = WoohooerTuning.kTryForBabyPregnancyChoice;
        public bool[] mAllowOffLotTryForBabyAutonomous = new bool[] { WoohooerTuning.kAllowOffLotTryForBabyAutonomous, WoohooerTuningHorse.kAllowOffLotTryForBabyAutonomous, WoohooerTuningDog.kAllowOffLotTryForBabyAutonomous, WoohooerTuningCat.kAllowOffLotTryForBabyAutonomous };
        public float mChanceOfQuads = WoohooerTuning.kChanceOfQuads;
        public bool mEAStandardWoohoo = WoohooerTuning.kEAStandardWoohoo;

        public bool mAllowTeenSkinnyDip = WoohooerTuning.kAllowTeenSkinnyDip;
        public bool mEnforceSkinnyDipPrivacy = WoohooerTuning.kEnforceSkinnyDipPrivacy;

        public bool mAutonomousHotTub = WoohooerTuning.kAutonomousHotTub;
        public bool mAutonomousElevator = WoohooerTuning.kAutonomousElevator;
        public bool mAutonomousSarcophagus = WoohooerTuning.kAutonomousSarcophagus;
        public bool mAutonomousTimeMachine = WoohooerTuning.kAutonomousTimeMachine;
        public bool mAutonomousActorTrailer = WoohooerTuning.kAutonomousActorTrailer;
        public bool mAutonomousBed = WoohooerTuning.kAutonomousBed;
        public bool mAutonomousTreehouse = WoohooerTuning.kAutonomousTreehouse;
        public bool mAutonomousTent = WoohooerTuning.kAutonomousTent;
        public bool mAutonomousShower = WoohooerTuning.kAutonomousShower;
        public bool mAutonomousRabbithole = WoohooerTuning.kAutonomousRabbithole;
        public bool mAutonomousHayStack = WoohooerTuning.kAutonomousHayStack;
        //public bool mAutonomousBoxStallHuman = WoohooerTuning.kAutonomousBoxStall;
        public bool mAutonomousBoxStallHorse = WoohooerTuning.kAutonomousBoxStall;
        public bool mAutonomousPetHouse = WoohooerTuning.kAutonomousPetHouse;
        public bool mAutonomousComputer = WoohooerTuning.kAutonomousComputer;
        public bool mAutonomousPhotoBooth = WoohooerTuning.kAutonomousPhotoBooth;
        public bool mAutonomousBoxOfMystery = WoohooerTuning.kAutonomousBoxOfMystery;
        public bool mAutonomousSauna = WoohooerTuning.kAutonomousSauna;
        public bool mAutonomousGypsyCaravan = WoohooerTuning.kAutonomousGypsyCaravan;
        public bool mAutonomousWardrobe = WoohooerTuning.kAutonomousWardrobe;
        public bool mAutonomousFairyHouse = WoohooerTuning.kAutonomousFairyHouse;
        public bool mAutonomousLeafPile = WoohooerTuning.kAutonomousLeafPile;
        public bool mAutonomousIgloo = WoohooerTuning.kAutonomousIgloo;
        public bool mAutonomousOutdoorShower = WoohooerTuning.kAutonomousOutdoorShower;
        public bool mAutonomousHotAirBalloon = WoohooerTuning.kAutonomousHotAirBalloon;
        public bool mAutonomousAncientPortal = WoohooerTuning.kAutonomousAncientPortal;
        public bool mAutonomousAllInOneBathroom = WoohooerTuning.kAutonomousAllInOneBathroom;
        public bool mAutonomousResort = WoohooerTuning.kAutonomousResort;
        public bool mAutonomousUnderwaterCave = WoohooerTuning.kAutonomousUnderwaterCave;
        public bool mAutonomousJetPack = WoohooerTuning.kAutonomousJetPack;
        public bool mAutonomousHoverTrain = WoohooerTuning.kAutonomousHoverTrain;
        public bool mAutonomousBotMaker = WoohooerTuning.kAutonomousBotMaker;
        public bool mAutonomousTimePortal = WoohooerTuning.kAutonomousTimePortal;
		public bool mAutonomousEiffelTower = WoohooerTuning.kAutonomousEiffelTower;
		public bool mAutonomousToiletStall = WoohooerTuning.kAutonomousToiletStall;

        public bool mNakedOutfitHotTub = WoohooerTuning.kNakedOutfitHotTub;
        public bool mNakedOutfitElevator = WoohooerTuning.kNakedOutfitElevator;
        public bool mNakedOutfitSarcophagus = WoohooerTuning.kNakedOutfitSarcophagus;
        public bool mNakedOutfitTimeMachine = WoohooerTuning.kNakedOutfitTimeMachine;
        public bool mNakedOutfitActorTrailer = WoohooerTuning.kNakedOutfitActorTrailer;
        public bool mNakedOutfitBed = WoohooerTuning.kNakedOutfitBed;
        public bool mNakedOutfitPhotoBooth = WoohooerTuning.kNakedOutfitPhotoBooth;
        public bool mNakedOutfitSaunaGeneral = WoohooerTuning.kNakedOutfitSaunaGeneral;
        public bool mNakedOutfitSaunaWoohoo = WoohooerTuning.kNakedOutfitSaunaWoohoo;
        public bool mNakedOutfitGypsyCaravan = WoohooerTuning.kNakedOutfitGypsyCaravan;
        public bool mNakedOutfitWardrobe = WoohooerTuning.kNakedOutfitWardrobe;
        public bool mNakedOutfitAncientPortal = WoohooerTuning.kNakedOutfitAncientPortal;
        public bool mNakedOutfitLeafPile = WoohooerTuning.kNakedOutfitLeafPile;
        public bool mNakedOutfitHayStack = WoohooerTuning.kNakedOutfitHayStack;
        public bool mNakedOutfitMassage = WoohooerTuning.kNakedOutfitMassage;
        public bool mNakedOutfitBotMaker = WoohooerTuning.kNakedOutfitBotMaker;
        public bool mNakedOutfitTimePortal = WoohooerTuning.kNakedOutfitTimePortal;

        public bool mChangeForBedWoohoo = WoohooerTuning.kChangeForBedWoohoo;
        public bool mUnlockTeenActions = WoohooerTuning.kUnlockTeenActions;
        public bool mAllowPlantSimPregnancy = WoohooerTuning.kAllowPlantSimPregnancy;

        public bool mLinkToStoryProgression = WoohooerTuning.kLinkToStoryProgression;
        public bool mStoryProgressionForUserDirected = WoohooerTuning.kStoryProgressionForUserDirected;
        public bool mRemoveRomanceOnKiss = WoohooerTuning.kRemoveRomanceOnKiss;
        public bool mAllowZombie = WoohooerTuning.kAllowZombie;

        public bool mVerboseDebugging = false;

        public float GetRiskyBabyMadeChance(Sim actor)
        {
            if (actor.SimDescription.Teen)
            {
                return mRiskyTeenBabyMadeChance;
            }
            else
            {
                return mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(actor)];
            }
        }

        public bool TestStoryProgression(bool autonomous)
        {
            if (!mLinkToStoryProgression) return false;

            if (!autonomous)
            {
                if (!mStoryProgressionForUserDirected) return false;
            }

            return true;
        }

        public void AddChange(Sim obj)
        {
            if (!Woohooer.Settings.mChangeRoomClothings) return;

            if (mOutfitChangers.ContainsKey(obj.SimDescription.SimDescriptionId)) return;

            mOutfitChangers.Add(obj.SimDescription.SimDescriptionId, true);
        }

        public bool NeedsChange(Sim obj)
        {
            return mOutfitChangers.ContainsKey(obj.SimDescription.SimDescriptionId);
        }

        public void RemoveChange(Sim obj)
        {
            mOutfitChangers.Remove(obj.SimDescription.SimDescriptionId);
        }

        public int GetCount(SimDescription sim)
        {
            ulong id = sim.SimDescriptionId;
            if (!mWoohooCount.ContainsKey(id)) return 0;

            return mWoohooCount[id];
        }

        public void AddCount(Sim sim)
        {
            ulong id = sim.SimDescription.SimDescriptionId;
            if (!mWoohooCount.ContainsKey(id))
            {
                mWoohooCount.Add(id, 1);
            }
            else
            {
                mWoohooCount[id]++;
            }
        }

        public bool AllowNearRelationWoohooAutonomousV2(CASAgeGenderFlags species)
        {
            int speciesIndex = GetSpeciesIndex(species);

            if (!mAllowNearRelationRomanceAutonomousV2[speciesIndex]) return false;

            return mAllowNearRelationWoohooAutonomousV2[speciesIndex];
        }

        public void SetAllowNearRelationWoohooAutonomousV2(CASAgeGenderFlags species, bool value)
        {
            mAllowNearRelationWoohooAutonomousV2[GetSpeciesIndex(species)] = value;
        }

        public static int GetSpeciesIndex(Sim sim)            
        {
            return GetSpeciesIndex(sim.SimDescription.Species);
        }
        public static int GetSpeciesIndex(SimDescription sim)
        {
            return GetSpeciesIndex(sim.Species);
        }
        public static int GetSpeciesIndex(CASAgeGenderFlags species)
        {
            switch (species)
            {
                case CASAgeGenderFlags.Human:
                    return 0;
                case CASAgeGenderFlags.Horse:
                    return 1;
                case CASAgeGenderFlags.Dog:
                case CASAgeGenderFlags.LittleDog:
                    return 2;
                case CASAgeGenderFlags.Cat:
                    return 3;
                default:
                    return 0;
            }           
        }

        public string GetRiskyChanceText(Sim sim)
        {
            if (!mShowRiskyChance) return null;

            return Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(sim)] });
        }

        public bool TraitScoringForUserDirected
        {
            get
            {
                if (!UsingTraitScoring) return false;

                return mTraitScoringForUserDirected;
            }
        }

        public bool ReplaceWithRisky
        {
            get
            {
                if (mRiskyBabyMadeChanceV2[GetSpeciesIndex(CASAgeGenderFlags.Human)] <= 0) return false;

                return mReplaceWithRisky;
            }
            set
            {
                mReplaceWithRisky = value;
            }
        }

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }

        public bool UsingTraitScoring
        {
            get
            {
                if (ScoringLookup.GetScoring("InterestInCommon", false) == null) return false;

                return mUseTraitScoring;
            }
        }

        public bool AllowNearRelation(CASAgeGenderFlags species, bool woohoo, bool autonomous)
        {
            if (woohoo)
            {
                if (!mAllowNearRelationWoohooV2[GetSpeciesIndex(species)]) return false;

                if (autonomous)
                {
                    if (!mAllowNearRelationWoohooAutonomousV2[GetSpeciesIndex(species)]) return false;
                }
            }

            if (!mAllowNearRelationRomanceV2[GetSpeciesIndex(species)]) return false;

            if (autonomous)
            {
                if (!mAllowNearRelationRomanceAutonomousV2[GetSpeciesIndex(species)]) return false;
            }

            return true;
        }

        public bool AllowTeen(bool woohoo)
        {
            if (woohoo)
            {
                return mAllowTeenWoohoo;
            }
            else
            {
                return true;
            }
        }

        public bool AllowTeenAdult(bool woohoo)
        {
            if (woohoo)
            {
                if (!mAllowTeenAdultRomance) return false;

                return mAllowTeenAdultWoohoo;
            }
            else
            {
                return mAllowTeenAdultRomance;
            }
        }
    }
}
