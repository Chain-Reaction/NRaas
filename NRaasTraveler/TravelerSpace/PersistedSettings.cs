using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to pause the travel clock")]
        protected static bool kPauseTravel = false;

        [Tunable, TunableComment("Whether to handle custom destinations as vacation towns")]
        protected static bool kTreatAsVacation = true;

        [Tunable, TunableComment("Whether to prompt to save prior to travelling")]
        protected static bool kPromptToSave = true;

        [Tunable, TunableComment("The cost per day to visit a Traveler world")]
        protected static int kCostPerDay = 200;

        [Tunable, TunableComment("Whether to spawn the weather stone")]
        protected static bool kAllowSpawnWeatherStone = true;

        [Tunable, TunableComment("Whether to enable seasons while on vacation")]
        protected static bool kEnableSeasons = true;

        [Tunable, TunableComment("The length of a university term")]
        protected static int kUniversityTermLength = 7;

        [Tunable, TunableComment("Whether to perform progression travel actions when updating a vacation world")]
        protected static bool kPerformTravelActions = true;
     
        [Tunable, TunableComment("Whether to store vehicles parked on residential lots prior to returning home")]
        protected static bool kStoreVehicles = true;

        [Tunable, TunableComment("Which is allowed to travel (Pets, Friends, Pregnant, Toddlers, Children, Teens, Recovering)")]
        protected static CommonSpace.Helpers.TravelUtilEx.Type kTravelFilter = CommonSpace.Helpers.TravelUtilEx.Type.None;

        [Tunable, TunableComment("Whether to set inactive travelers as unselectable upon arrival on vacation")]
        protected static bool kSetAsUnselectable = false;

        public bool mPauseTravel = kPauseTravel;

        public bool mTreatAsVacation = kTreatAsVacation;

        public bool mPromptToSave = kPromptToSave;

        private bool mDebugging = Common.kDebugging;

        public int mCostPerDay = kCostPerDay;

        Dictionary<ulong, ulong> mTravelerHouseholds = null;

        Dictionary<ulong, string> mWorldForSims = new Dictionary<ulong, string>();

        public bool mAllowSpawnWeatherStone = kAllowSpawnWeatherStone;

        public bool mEnableSeasons = kEnableSeasons;

        public Dictionary<WorldName,bool> mAgelessForeign = new Dictionary<WorldName,bool>();

        public int mUniversityTermLength = kUniversityTermLength;

        public bool mPerformTravelActions = kPerformTravelActions;

        public bool mStoreVehicles = kStoreVehicles;

        public CommonSpace.Helpers.TravelUtilEx.Type mTravelFilter = CommonSpace.Helpers.TravelUtilEx.Type.None;

        public Dictionary<WorldName, bool> mHiddenWorlds = new Dictionary<WorldName, bool>();

        public bool mSetAsUnselectable = kSetAsUnselectable;

        public void StoreHouseholds(Dictionary<ulong, ulong> sims)
        {
            mTravelerHouseholds = new Dictionary<ulong, ulong>(sims);
        }

        public Dictionary<ulong, ulong> TravelerHouseholds
        {
            get { return mTravelerHouseholds; }
        }

        public void AddWorldForSim(ulong id)
        {
            mWorldForSims.Remove(id);
            mWorldForSims.Add(id, World.GetWorldFileName());
        }

        public void MergeFromCrossWorldData(Dictionary<ulong, string> lookup)
        {
            mWorldForSims = new Dictionary<ulong, string>(lookup);
        }

        public void MergeToCrossWorldData(Dictionary<ulong, string> lookup)
        {
            foreach (KeyValuePair<ulong, string> value in mWorldForSims)
            {
                lookup.Remove(value.Key);
                lookup.Add(value.Key, value.Value);
            }
        }

        public bool GetAgelessForeign(WorldName world)
        {
            return mAgelessForeign.ContainsKey(world);
        }

        public bool GetHiddenWorlds(WorldName world)
        {
            return mHiddenWorlds.ContainsKey(world);
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
