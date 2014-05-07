using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable,TunableComment("Whether to enable Testing Cheats on load-up")]
        public static bool kTestingCheatsEnabled = false;
        [Tunable, TunableComment("Whether certain actions affect the active lot")]
        public static bool kAffectActiveLot = false;
        [Tunable, TunableComment("Whether to reset all errant gnomes on a nightly basis")]        
        public static bool kResetAllGnomes = true;
        [Tunable, TunableComment("Whether to delete all magic gnomes in the world on a nightly basis")]
        public static bool kDeleteAllMagicGnomes = false;
        [Tunable, TunableComment("Whether to turn off all stereos nightly")]
        public static bool kTurnOffStereos = true;
        [Tunable, TunableComment("Whether to turn off all televisions nightly")]
        public static bool kTurnOffTelevisions = true;
        [Tunable, TunableComment("Whether to periodically recover any missing residents")]
        public static bool kRecoverMissingSims = true;
        [Tunable, TunableComment("Whether to periodically return all toddlers to the their home lot")]
        public static bool kRecoverStrandedToddlers = true;
        [Tunable, TunableComment("Whether to periodically eliminate all the homeless in town")]
        public static bool kKillAllHomeless = false;
        [Tunable, TunableComment("Whether to remove the genetic hair-style from all sims")]
        public static bool kPurgeGeneticHair = false;
        [Tunable, TunableComment("Whether to perform a genealogy cleanup and repair on load-up")]
        public static bool kCleanupGenealogy = true;
        [Tunable, TunableComment("Whether to cancel invalid player-held concerts nightly")]
        public static bool kCleanupConcert = true;
        [Tunable, TunableComment("Whether to sell off items that say they are in an inventory, but actually are not")]
        public static bool kCleanupInventory = true;
        [Tunable, TunableComment("Whether to remove orphaned vehicles on a nightly basis")]
        public static bool kCleanupVehicles = true;
        [Tunable, TunableComment("Whether to periodically reset any community laundry machines")]
        public static bool kCleanupLaundromat = true;
        [Tunable, TunableComment("Whether to remove duplicated singed outfits off sims")]
        public static bool kCleanupSinged = true;
        [Tunable, TunableComment("Whether to reset obviously bogus skill modifiers back to base-line")]
        public static bool kCleanupSkillModifiers = true;
        [Tunable, TunableComment("Whether to periodically reset any elevators in town")]
        public static bool kCleanupElevators = true;
        [Tunable, TunableComment("Whether to periodically correct aging in town")]
        public static bool kCleanupAging = true;
        [Tunable, TunableComment("Whether to periodically remove all unnecessary urnstones from the mausoleum")]
        public static bool kCleanseTheDead = false;
        [Tunable, TunableComment("Whether to display the interactions in the Sim menu")]
        public static bool kShowSimMenu = true;
        [Tunable, TunableComment("Whether to display the nightly alarm notifications")]
        public static bool kShowNotices = true;
        [Tunable, TunableComment("Whether to log the results of any actions performed")]
        public static bool kLogging = false;
        [Tunable, TunableComment("Comma delimited list of console commands")]
        public static string[] kStoredCommands = new string[] { "buydebug", "moveobjects on", "moveobjects off" };
        [Tunable, TunableComment("Comma delimited list of commands to run nightly")]
        public static string[] kAlarmCommands = new string[0];
        [Tunable, TunableComment("Comma delimited list of commands to run on load-up")]
        public static string[] kAutoCommands = new string[0];
        [Tunable, TunableComment("Hour at which to run the nightly alarm")]
        public static float kAlarmHour = 3f;

        [Tunable, TunableComment("Whether to check for stuck sims each night")]
        public static bool kStuckCheck = true;
        [Tunable, TunableComment("Whether to automatically reset sims found to be stuck")]
        public static bool kStuckCheckReset = true;
        [Tunable, TunableComment("Whether to disable the neighborhood pet adoption system")]
        public static bool kStopPetAdoption = false;
        [Tunable, TunableComment("Whether to toggle digital picture frames off and on again each night")]
        public static bool kRestartDigitalPhotos = true;
        [Tunable, TunableComment("Whether to check conditions that require a reset")]
        public static bool kResetCheck = true;
        [Tunable, TunableComment("Whether to notify on the first reset check")]
        public static bool kReportFirstResetCheck = true;
        [Tunable, TunableComment("Whether to check whenever a sim is instantiated and see whether they should in-game or not")]
        public static bool kInstantationCheck = true;
        [Tunable, TunableComment("Whether to clean up broken situations on a nightly basis")]
        public static bool kCleanupSituations = false;
        [Tunable, TunableComment("Whether to clean up broken relationships on a nightly basis")]
        public static bool kCleanupRelationships = true;

        [Tunable, TunableComment("Minimum family tree depth to initiate an automatic compression of a dead sim")]
        public static int kCompressFamilyLevel = 0;
        [Tunable, TunableComment("Minimum number of route failures in a row to force a reset")]
        public static int kMinimumRouteFail = 10;
        [Tunable, TunableComment("The number of sim-minutes from the start of the route failure to when the sim will be reset")]
        public static int kRouteFailTestMinutes = 30;

        public bool mTestingCheatsEnabled = kTestingCheatsEnabled;
        public bool mAffectActiveLot = kAffectActiveLot;        
        public bool mResetAllGnomes = kResetAllGnomes;
        public bool mDeleteAllMagicGnomes = kDeleteAllMagicGnomes;
        public bool mTurnOffStereos = kTurnOffStereos;
        public bool mTurnOffTelevisions = kTurnOffTelevisions;
        public bool mRecoverMissingSims = kRecoverMissingSims;
        public bool mRecoverStrandedToddlers = kRecoverStrandedToddlers;
        public bool mKillAllHomeless = kKillAllHomeless;
        public bool mPurgeGeneticHair = kPurgeGeneticHair;
        public bool mCleanupGenealogy = kCleanupGenealogy;
        public bool mCleanupConcert = kCleanupConcert;
        public bool mCleanupInventory = kCleanupInventory;
        public bool mCleanupVehicles = kCleanupVehicles;
        public bool mCleanupLaundromat = kCleanupLaundromat;
        public bool mCleanupSinged = kCleanupSinged;
        public bool mCleanupSkillModifiers = kCleanupSkillModifiers;
        public bool mCleanupElevators = kCleanupElevators;
        public bool mCleanupAging = kCleanupAging;
        public bool mCleanseTheDead = kCleanseTheDead;
        public bool mShowSimMenu = kShowSimMenu;
        public bool mShowNotices = kShowNotices;
        private bool mLogging = kLogging;
        public float mAlarmHour = kAlarmHour;

        public bool mStuckCheckV2 = kStuckCheck;
        public bool mStuckCheckReset = kStuckCheckReset;
        public bool mStopPetAdoption = kStopPetAdoption;
        public bool mRestartDigitalPhotos = kRestartDigitalPhotos;
        public bool mResetCheck = kResetCheck;
        public bool mReportFirstResetCheck = kReportFirstResetCheck;
        public bool mInstantationCheck = kInstantationCheck;
        public bool mCleanupSituations = kCleanupSituations;
        public bool mCleanupRelationships = kCleanupRelationships;

        public int mCompressFamilyLevel = kCompressFamilyLevel;
        public int mMinimumRouteFail = kMinimumRouteFail;
        public int mRouteFailTestMinutesV2 = kRouteFailTestMinutes;

        public bool mCleanupVehiclesHourly = false;

        public List<string> mAlarmCommands = new List<string>(kAlarmCommands);
        public List<string> mAutoCommands = new List<string>(kAutoCommands);
        public List<string> mStoredCommands = new List<string>(kStoredCommands);

        public int mCurrentPhaseIndex = -1;

        public bool Logging
        {
            get
            {
                return mLogging;
            }
            set
            {
                mLogging = value;

                Common.kDebugging = value;
            }
        }
    }
}
