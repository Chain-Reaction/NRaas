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
    public static class WoohooerTuningCat
    {
        [Tunable, TunableComment("Range: 0-100. Minimum liking gate for performing autonomous actions")]
        public static int kLikingGatingForAutonomousWoohoo = 40;

        [Tunable, TunableComment("Range: 0-100. Chance of Sim getting Pregnant on RiskyWoohoo.")]
        public static int kRiskyBabyMadeChance = 10;

        [Tunable, TunableComment("Whether risky woohoo is autonomous")]
        public static bool kRiskyAutonomous = false;

        [Tunable, TunableComment("Whether regular woohoo is autonomous")]
        public static bool kWoohooAutonomous = false;

        [Tunable, TunableComment("Whether Try For Baby is autonomous")]
        public static bool kTryForBabyAutonomous = false;

        [Tunable, TunableComment("Whether a couple must be flagged as romantic before considering woohoo")]
        public static bool kMustBeRomanticForAutonomous = true;

        [Tunable, TunableComment("Whether to allow near relations to woohoo")]
        public static bool kAllowNearRelationWoohoo = false;

        [Tunable, TunableComment("Whether to allow near relations to autonomously woohoo")]
        public static bool kAllowNearRelationWoohooAutonomous = false;

        [Tunable, TunableComment("Whether to allow try for baby between male couples")]
        public static bool kAutonomousMaleMaleTryForBaby = false;

        [Tunable, TunableComment("Whether to allow try for baby between female couples")]
        public static bool kAutonomousFemaleFemaleTryForBaby = true;

        [Tunable, TunableComment("Whether to display try for baby for same-sex couples")]
        public static bool kAllowSameSexTryForBaby = true;

        [Tunable, TunableComment("Maximum number of sims per household for autonomous pregnancy")]
        public static int kMaximumHouseholdSizeForAutonomous = 6;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim considering a Woohoo interaction")]
        public static int kWoohooBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim considering a Risky Woohoo interaction")]
        public static int kRiskyBaseChanceScoring = 25;

        [Tunable, TunableComment("Range: -200 to 200. The base chance of a sim considering a Try For Baby interaction")]
        public static int kTryForBabyBaseChanceScoring = 10;

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

        [Tunable, TunableComment("Defines when the Woohoo Interactions are available for romantic sims.  Choices: Default, Partner, Spouse, AnyRomantic")]
        public static MyLoveBuffLevel kWoohooInteractionLevel = MyLoveBuffLevel.Default;

        [Tunable, TunableComment("(UNUSED) Defines which sim is the first choice for pregnancy during a Try For Baby.  Choices: Initiator, Target, Both")]
        public static PregnancyChoice kPregnancyChoice = PregnancyChoice.InitiatorThenTarget;

        [Tunable, TunableComment("Whether to use fertility treatment in the chance of pregnancy during Risky Woohoo")]
        public static bool kRiskyFertility = true;

        [Tunable, TunableComment("Whether to use fertility treatment in the chance of pregnancy during Try For Baby")]
        public static bool kTryForBabyFertility = true;

        [Tunable, TunableComment("Range: 0-100. Minimum liking gate for performing autonomous romance actions")]
        public static int kLikingGatingForAutonomousRomance = 0;

        [Tunable, TunableComment("Whether to test all the standard conditions that normally restrict Try For Baby")]
        public static bool kTestAllConditionsForUserDirected = false;

        [Tunable, TunableComment("Whether to set a new mate after woohoo")]
        public static bool kForcePetMate = true;

        [Tunable, TunableComment("Scoring factor applied for each woohoo performed during the day")]
        public static int kWoohooCountScoreFactor = -10;

        [Tunable, TunableComment("Whether to allow sims to engage in autonomous Risky while not on the active lot")]
        public static bool kAllowOffLotRiskyAutonomous = true;

        [Tunable, TunableComment("Whether to allow sims to engage in autonomous Try For Baby while not on the active lot")]
        public static bool kAllowOffLotTryForBabyAutonomous = true;

        [Tunable, TunableComment("The minimum number of minutes between autonomous woohoos")]
        public static int kWoohooCooldown = 60;

        [Tunable, TunableComment("The maximum number of sims allowed in a home before the 'Try For Baby' is disabled")]
        public static int kTryForBabyUserMaximum = 0;
    }
}
