using NRaas.WoohooerSpace;
using NRaas.WoohooerSpace.Options.Romance;
using NRaas.WoohooerSpace.Options.TryForBaby;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public static class WoohooerTuning
    {
        [Tunable, TunableComment("Range: 0-100. Minimum liking gate for performing autonomous actions")]
        public static int kLikingGatingForAutonomousWoohoo = 40;

        [Tunable, TunableComment("Range: 0-100. Chance of Sim getting Pregnant on RiskyWoohoo.")]
        public static int kRiskyBabyMadeChance = 10;

        [Tunable, TunableComment("Whether risky woohoo is autonomous")]
        public static bool kRiskyAutonomous = false;

        [Tunable, TunableComment("Whether risky woohoo is autonomous for teens")]
        public static bool kTeenRiskyAutonomous = false;

        [Tunable, TunableComment("Whether regular woohoo is autonomous")]
        public static bool kWoohooAutonomous = false;

        [Tunable, TunableComment("Whether woohooty texting is autonomous")]
        public static bool kWoohootyTextAutonomous = false;

        [Tunable, TunableComment("Whether Try For Baby is autonomous")]
        public static bool kTryForBabyAutonomous = false;

        [Tunable, TunableComment("Whether Try For Baby is autonomous for teens")]
        public static bool kTeenTryForBabyAutonomous = false;

        [Tunable, TunableComment("Whether a couple must be flagged as romantic before considering woohoo")]
        public static bool kMustBeRomanticForAutonomous = true;

        [Tunable, TunableComment("Whether teens are allowed to woohoo")]
        public static bool kAllowTeenWoohoo = false;

        [Tunable, TunableComment("Whether to allow teens and adults to woohoo together")]
        public static bool kAllowTeenAdultWoohoo = false;

        [Tunable, TunableComment("Whether to allow near relations to woohoo")]
        public static bool kAllowNearRelationWoohoo = false;

        [Tunable, TunableComment("Whether to allow near relations to autonomously woohoo")]
        public static bool kAllowNearRelationWoohooAutonomous = false;

        [Tunable, TunableComment("Whether to allow try for baby between male couples")]
        public static bool kAutonomousMaleMaleTryForBaby = false;

        [Tunable, TunableComment("Whether to allow try for baby between female couples")]
        public static bool kAutonomousFemaleFemaleTryForBaby = true;

        [Tunable, TunableComment("Whether to hide woohoo entirely")]
        public static bool kHideWoohoo = false;

        [Tunable, TunableComment("Whether to display try for baby for same-sex couples")]
        public static bool kAllowSameSexTryForBaby = true;

        [Tunable, TunableComment("Maximum number of sims per household for autonomous pregnancy")]
        public static int kMaximumHouseholdSizeForAutonomous = 8;

        [Tunable, TunableComment("Whether to enforce privacy during Woohoo")]
        public static bool kEnforcePrivacy = false;

        [Tunable, TunableComment("Whether to display debugging messages")]
        public static bool kDebugging = false;

        [Tunable, TunableComment("Whether to use trait scoring in general")]
        public static bool kUseTraitScoring = true;

        [Tunable, TunableComment("Whether to use trait scoring during manual interactions")]
        public static bool kTraitScoringForUserDirected = false;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim considering a Woohoo interaction")]
        public static int kWoohooBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim considering a Risky Woohoo interaction")]
        public static int kRiskyBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim considering a Try For Baby interaction")]
        public static int kTryForBabyBaseChanceScoring = 10;

        [Tunable, TunableComment("Whether to use a sims mood during scoring")]
        public static bool kUseMoodInScoring = true;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim reacting to a woohoo jealousy incident")]
        public static int kReactToJealousyBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim noticing another lover is in the room")]
        public static int kSneakinessBaseChanceScoring = 25;

        [Tunable, TunableComment("Whether to use trait scoring to determine woohoo privacy")]
        public static bool kTraitScoredPrivacy = true;

        [Tunable, TunableComment("Whether to allow teen-adult romantic socials")]
        public static bool kAllowTeenAdultRomance = true;

        [Tunable, TunableComment("Whether to allow near-relation romantic socials")]
        public static bool kAllowNearRelationRomance = false;

        [Tunable, TunableComment("Whether to allow autonomous near-relation romantic socials")]
        public static bool kAllowNearRelationRomanceAutonomous = false;

        [Tunable, TunableComment("How many woohoo events are wiped from a sim's count each night")]
        public static int kWoohooRechargeRate = 5;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim being interested in a romantic interaction")]
        public static int kRomanceBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim being attracted to another")]
        public static int kAttractionBaseChanceScoring = 0;

        [Tunable, TunableComment("Range: 0-100. Chance of Sim getting Pregnant on Try For Baby.")]
        public static int kTryForBabyMadeChance = 75;

        [Tunable, TunableComment("When enabled, sims who are romantic with both woohooers do not get jealous.")]
        public static bool kPolyamorousWoohooJealousy = false;

        [Tunable, TunableComment("Defines when to provide the 'My Love' buff to romantic sims.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kMyLoveBuffLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("Whether to display the Woohooer interaction in the Sim Menu.")]
        public static bool kSimMenuVisibility = true;

        [Tunable, TunableComment("Whether to apply the Stride Of Pride moodlet upon woohoo.")]
        public static bool kAllowStrideOfPride = true;

        [Tunable, TunableComment("Whether to allow teens to skinny dip")]
        public static bool kAllowTeenSkinnyDip = false;

        [Tunable, TunableComment("Whether to enforce age privacy restrictions for skinny dips")]
        public static bool kEnforceSkinnyDipPrivacy = true;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim enforcing privacy during woohoo")]
        public static int kPrivacyBaseChanceScoring = 0;

        [Tunable, TunableComment("Whether to apply the romantic liking gate to user-directed interactions")]
        public static bool kLikingGateForUserDirected = false;

        [Tunable, TunableComment("Whether to apply woohoo buffs after a successful romp")]
        public static bool kApplyBuffs = true;

        [Tunable, TunableComment("Whether to perform the Make Out interaction prior to all woohoos")]
        public static bool kAllowForeplay = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Hot Tub")]
        public static bool kAutonomousHotTub = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Elevator")]
        public static bool kAutonomousElevator = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Sarcophagus")]
        public static bool kAutonomousSarcophagus = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Time Machine")]
        public static bool kAutonomousTimeMachine = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Actor Trailer")]
        public static bool kAutonomousActorTrailer = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Bed")]
        public static bool kAutonomousBed = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Tent")]
        public static bool kAutonomousTent = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Treehouse")]
        public static bool kAutonomousTreehouse = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Shower")]
        public static bool kAutonomousShower = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Rabbitholes")]
        public static bool kAutonomousRabbithole = true;

        [Tunable, TunableComment("Whether to disallow homeless sims from autonomously performing romantic interactions")]
        public static bool kDisallowHomeless = false;

        [Tunable, TunableComment("Defines when the Woohoo Interactions are available for romantic sims.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kWoohooInteractionLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("Defines which sim is the first choice for pregnancy during a Try For Baby.  Choices: Initiator, Target, InitiatorThenTarget, TargetThenInitiator, Random")]
        public static PregnancyChoice kTryForBabyPregnancyChoice = PregnancyChoice.InitiatorThenTarget;

        [Tunable, TunableComment("How much a trip to the matchmaker costs")]
        public static int kMatchmakerCost = 500;

        [Tunable, TunableComment("A comma separated list of service types that are not allowed to be interacted with autonomously")]
        public static ServiceType[] kDisallowAutonomousServiceTypes = new ServiceType[0];

        [Tunable, TunableComment("A comma separated list of role types that are not allowed to be interacted with autonomously")]
        public static Role.RoleType[] kDisallowAutonomousRoleTypes = new Role.RoleType[0];

        [Tunable, TunableComment("Whether to replace regular Woohoo with Risky")]
        public static bool kReplaceWithRisky = false;

        [Tunable, TunableComment("Whether to test gender preference during user-directed interactions")]
        public static bool kGenderPreferenceForUserDirected = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Hot Tub")]
        public static bool kNakedOutfitHotTub = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Elevator")]
        public static bool kNakedOutfitElevator = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Sarcophagus")]
        public static bool kNakedOutfitSarcophagus = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Time Machine")]
        public static bool kNakedOutfitTimeMachine = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Actor Trailer")]
        public static bool kNakedOutfitActorTrailer = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in Bed")]
        public static bool kNakedOutfitBed = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Hay Stacks")]
        public static bool kAutonomousHayStack = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Box Stalls")]
        public static bool kAutonomousBoxStall = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Pet Houses")]
        public static bool kAutonomousPetHouse = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos on the computer")]
        public static bool kAutonomousComputer = true;

        [Tunable, TunableComment("Whether to use fertility treatment in the chance of pregnancy during Risky Woohoo")]
        public static bool kRiskyFertility = true;

        [Tunable, TunableComment("Whether to use fertility treatment in the chance of pregnancy during Try For Baby")]
        public static bool kTryForBabyFertility = true;

        [Tunable, TunableComment("Range: 0-100. Minimum liking gate for performing autonomous romance actions")]
        public static int kLikingGatingForAutonomousRomance = 0;

        [Tunable, TunableComment("Whether to test all the standard conditions that normally restrict Try For Baby")]
        public static bool kTestAllConditionsForUserDirected = false;

        [Tunable, TunableComment("Scoring factor applied for each woohoo performed during the day")]
        public static int kWoohooCountScoreFactor = -20;

        [Tunable, TunableComment("Whether to display the chance for Risky Woohoo on the menu interaction")]
        public static bool kShowRiskyChance = true;

        [Tunable, TunableComment("Defines which sim is the first choice for pregnancy during a Risky Woohoo.  Choices: Initiator, Target, InitiatorThenTarget, TargetThenInitiator, Random")]
        public static PregnancyChoice kRiskyPregnancyChoice = PregnancyChoice.InitiatorThenTarget;

        [Tunable, TunableComment("Whether to allow autonomous romantic interactions for actives on community lots")]
        public static bool kAllowAutonomousRomanceCommLot = true;

        [Tunable, TunableComment("Whether to switch into Everyday after Naked Woohoo")]
        public static bool kSwitchToEverydayAfterNakedWoohoo = false;

        [Tunable, TunableComment("Whether to allow sims to engage in autonomous Risky while not on the active lot")]
        public static bool kAllowOffLotRiskyAutonomous = true;

        [Tunable, TunableComment("Whether to allow sims to engage in autonomous Try For Baby while not on the active lot")]
        public static bool kAllowOffLotTryForBabyAutonomous = true;

        [Tunable, TunableComment("Defines when to provide the 'Go Steady' and 'Propose Marriage' interactions for sims.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kPartneringInteractionLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("Whether to restrict teen marriage to only those sims who already have offspring")]
        public static bool kRestrictTeenMarriage = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the photobooth")]
        public static bool kAutonomousPhotoBooth = true;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Photobooth")]
        public static bool kNakedOutfitPhotoBooth = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Box of Mystery")]
        public static bool kAutonomousBoxOfMystery = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Sauna")]
        public static bool kAutonomousSauna = true;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Sauna")]
        public static bool kNakedOutfitSaunaWoohoo = false;

        [Tunable, TunableComment("Whether to allow naked sitting in the Sauna")]
        public static bool kNakedOutfitSaunaGeneral = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Gypsy Caravan")]
        public static bool kAutonomousGypsyCaravan = true;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Gypsy Caravan")]
        public static bool kNakedOutfitGypsyCaravan = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Wardrobe")]
        public static bool kAutonomousWardrobe = true;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Wardrobe")]
        public static bool kNakedOutfitWardrobe = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Fairy House")]
        public static bool kAutonomousFairyHouse = true;

        [Tunable, TunableComment("The chance of a human having quads")]
        public static float kChanceOfQuads = 0.0001f;

        [Tunable, TunableComment("Range: 0-100. Chance of a Teen sim getting Pregnant on RiskyWoohoo.")]
        public static int kRiskyTeenBabyMadeChance = 10;

        [Tunable, TunableComment("Range: 0-100. Chance of a Teen sim getting Pregnant on Try For Baby.")]
        public static int kTryForBabyTeenBabyMadeChance = 75;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a teen considering a Woohoo interaction")]
        public static int kWoohooTeenBaseChanceScoring = 50;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a teen considering a Risky Woohoo interaction")]
        public static int kRiskyTeenBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a teen considering a Try For Baby interaction")]
        public static int kTryForBabyTeenBaseChanceScoring = 0;

        [Tunable, TunableComment("The minimum number of minutes between autonomous woohoos")]
        public static int kWoohooCooldown = 60;

        [Tunable, TunableComment("Whether to restrict the matchmaker list to only residents")]
        public static bool kOnlyResidentMatchmaker = false;

        [Tunable, TunableComment("The level of jealousy incurred for getting caught romancing (None, Low, Medium, High)")]
        public static JealousyLevel kRomanceJealousyLevel = JealousyLevel.Medium;

        [Tunable, TunableComment("The level of jealousy incurred for getting caught woohooing (None, Low, Medium, High)")]
        public static JealousyLevel kWoohooJealousyLevel = JealousyLevel.High;

        [Tunable, TunableComment("The level at which a single sim is allowed to autonomously romance another sim.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kRomanceInteractionLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("The level at which a steady sim is allowed to autonomously romance another sim.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kSteadyInteractionLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("The level at which a married sim is allowed to autonomously romance another sim.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kMarriedInteractionLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("Whether to change into sleepwear when engaging in non-naked bed woohoo")]
        public static bool kChangeForBedWoohoo = true;

        [Tunable, TunableComment("Whether to use EA Standard rule-set for determining the result of a woohoo interaction")]
        public static bool kEAStandardWoohoo = false;

        [Tunable, TunableComment("Whether to display certain interactions under 'Romance' instead of 'Friendly'")]
        public static bool kInteractionsUnderRomance = false;

        [Tunable, TunableComment("Whether to change outfits when the sim walks through a door after naked woohoo")]
        public static bool kChangeRoomClothings = true;

        [Tunable, TunableComment("The maximum number of sims allowed in a home before the 'Try For Baby' is disabled")]
        public static int kTryForBabyUserMaximum = 0;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Leaf Pile")]
        public static bool kAutonomousLeafPile = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Igloo")]
        public static bool kAutonomousIgloo = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Outdoor Shower")]
        public static bool kAutonomousOutdoorShower = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Hot Air Balloons")]
        public static bool kAutonomousHotAirBalloon = true;

        [Tunable, TunableComment("Whether to access settings in [StoryProgression] when testing interaction availability")]
        public static bool kLinkToStoryProgression = false;

        [Tunable, TunableComment("Whether to use settings in [StoryProgression] for user-directed interactions")]
        public static bool kStoryProgressionForUserDirected = false;

        [Tunable, TunableComment("Whether to unlock certain non-tunable interactions for teenagers")]
        public static bool kUnlockTeenActions = false;

        [Tunable, TunableComment("Whether to use normal pregnancy for plantsims")]
        public static bool kAllowPlantSimPregnancy = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Ancient Portal")]
        public static bool kAutonomousAncientPortal = true;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Ancient Portal")]
        public static bool kNakedOutfitAncientPortal = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Leaf Pile")]
        public static bool kNakedOutfitLeafPile = false;

        [Tunable, TunableComment("Whether to allow naked woohoos in the Hay Stack")]
        public static bool kNakedOutfitHayStack = false;

        [Tunable, TunableComment("Whether to set the romance flag when you kiss a sim")]
        public static bool kRemoveRomanceOnKiss = false;

        [Tunable, TunableComment("Whether to allow zombie woohoo")]
        public static bool kAllowZombie = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the All In One Bathroom")]
        public static bool kAutonomousAllInOneBathroom = true;

        [Tunable, TunableComment("Whether to use the naked outfit for romantic massages")]
        public static bool kNakedOutfitMassage = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Resort Tower")]
        public static bool kAutonomousResort = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Deep Sea Cave")]
        public static bool kAutonomousUnderwaterCave = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Bot Making Station")]
        public static bool kAutonomousBotMaker = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos using Jet Packs")]
        public static bool kAutonomousJetPack = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Hover Train")]
        public static bool kAutonomousHoverTrain = true;

        [Tunable, TunableComment("Whether to use the naked outfit for Bot Maker")]
        public static bool kNakedOutfitBotMaker = false;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in the Time Portal")]
        public static bool kAutonomousTimePortal = true;

        [Tunable, TunableComment("Whether to use the naked outfit for Time Portal")]
        public static bool kNakedOutfitTimePortal = false;

		[Tunable, TunableComment("Whether to allow autonomous woohoos in the Eiffel Tower")]
		public static bool kAutonomousEiffelTower = true;

        [Tunable, TunableComment("Whether to allow autonomous woohoos in Toilet Stalls")]
        public static bool kAutonomousToiletStall = true;

        [Tunable, TunableComment("Chances per attraction level that autonmous flirting will happen")]
        public static float[] kAttractionLevelForAutonmousFlirting = new float[] { 0f, 0f, 0f, 0.15f, 0.3f, 0.6f };

        [Tunable, TunableComment("Chances per attraction level that autonmous woohoo will happen")]
        public static float[] kAttractionLevelForAutonmousWoohoo = new float[] { 0f, 0f, 0f, 0.10f, 0.25f, 0.50f };

        [Tunable, TunableComment("Chances per confidence level that autonmous flirting will happen")]
        public static float[] kConfidenceLevelForAutonmousFlirting = new float[] { 0.05f, 0.1f, 0.30f, 0.45f, 0.65f};

        [Tunable, TunableComment("Chances per confidence level that autonmous woohoo will happen")]
        public static float[] kConfidenceLevelForAutonmousWoohoo = new float[] { 0.25f, 0.1f, 0.2f, 0.4f, 0.6f };
    }
}
