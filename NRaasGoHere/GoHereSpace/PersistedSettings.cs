using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to disable auto-grouping when using Go Here With")]
        protected static bool kDisallowAutoGroup = false;

        [Tunable, TunableComment("Whether to disallow the use Go Home entirely for active sims")]
        protected static bool kDisallowActiveGoHome = false;

        [Tunable, TunableComment("Whether to allow vampires to teleport when moving between lots")]
        protected static bool kVampireTeleport = false;

        [Tunable, TunableComment("Whether Teleport is available for all sims, not just the undead")]
        protected static bool kTeleportForAll = false;

        [Tunable, TunableComment("Whether to allow Go Here interactions to queue")]
        protected static bool kAllowGoHereStack = true;

        [Tunable, TunableComment("Whether to allow Visit Community Lot pushes at night")]
        protected static bool kAllowNightVisitLot = true;

        [Tunable, TunableComment("Whether to allow vampires to be pushed during the day")]
        protected static bool kAllowVampireDayPush = true;

        [Tunable, TunableComment("The relationship level required to disable the rude guest check")]
        protected static int kRudeGuestLikingGate = 40;

        [Tunable, TunableComment("Whether to hire babysitters to watch over inactive children")]
        protected static bool kInactiveChildrenAsActive = false;

        [Tunable, TunableComment("Whether to allow the system to teleport inactive children to the active household")]
        protected static bool kAllowActiveDayCare = true;

        [Tunable, TunableComment("Whether to try using day care career sims instead of babysitters")]
        protected static bool kUseDayCareSims = true;

        [Tunable, TunableComment("Whether to allow babies/toddlers to be home alone")]
        protected static bool kAllowChildHomeAlone = false;

        [Tunable, TunableComment("Whether to disable certain 'Go Home' for inactives, whenever they are on a lot containing an active")]
        protected static bool kDisallowInactiveLeaveActiveLot = true;       

        [Tunable, TunableComment("Whether to allow cars during routing operations")]
        protected static bool kDisallowCarRouting = false;

        [Tunable, TunableComment("Whether to allow boats during routing operations")]
        protected static bool kDisallowBoatRouting = false;

        [Persistable(false)]
        public bool mIgnoreLogs = true;

        [Persistable(false)]
        public bool mDetailedRouting = false;

        public bool mDisallowAutoGroup = kDisallowAutoGroup;

        public bool mDisallowActiveGoHome = kDisallowActiveGoHome;

        public bool mTeleportForAll = kTeleportForAll;

        public bool mVampireTeleport = kVampireTeleport;

        public bool mAllowGoHereStack = kAllowGoHereStack;

        public bool mAllowNightVisitLot = kAllowNightVisitLot;

        public bool mAllowVampireDayPush = kAllowVampireDayPush;

        public int mRudeGuestLikingGate = kRudeGuestLikingGate;

        protected bool mDebugging = false;

        public bool mAllowActiveDayCare = kAllowActiveDayCare;

        public bool mUseDayCareSims = kUseDayCareSims;

        public bool mInactiveChildrenAsActive = kInactiveChildrenAsActive;

        public bool mAllowChildHomeAlone = kAllowChildHomeAlone;

        public bool mDisallowInactiveLeaveActiveLot = kDisallowInactiveLeaveActiveLot;

        public Dictionary<ulong, CaregiverMonitorHelper.Caregivers> mCaregivers = new Dictionary<ulong, CaregiverMonitorHelper.Caregivers>();

        public bool mAllowCarRouting = !kDisallowCarRouting;

        public bool mAllowBoatRouting = !kDisallowBoatRouting;

        public bool AllowPush(Sim sim, Lot lot)
        {
            if (!GoHere.ExternalAllowPush(sim.SimDescription, lot)) return false;

            if (SimClock.IsNightTime())
            {
                if (!mAllowNightVisitLot) return false;
            }
            else
            {
                if (SimTypes.IsOccult(sim.SimDescription, Sims3.UI.Hud.OccultTypes.Vampire))
                {
                    if (!mAllowVampireDayPush) return false;
                }
            }

            return true;
        }

        public bool DisallowAutoGroup(Sim sim)
        {
            if (!SimTypes.IsSelectable(sim)) return false;

            return mDisallowAutoGroup;
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
    }
}
