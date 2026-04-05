using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Services;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System.Collections.Generic;

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

        [Tunable, TunableComment("Whether to show the active household lot or the last active lot when loading a save")]
        public static LoadingScreenControllerEx.LoadingImageType kLoadscreenImageType = LoadingScreenControllerEx.LoadingImageType.Standard;

        [Tunable, TunableComment("Whether to disable the generate of decendants and stop the llama messages")]
        public static bool kDisableDescedants = false;

        [Tunable, TunableComment("Chance of descendant being a hybrid")]
        public static int kChanceOfOccultHybrid = 0;

        [Tunable, TunableComment("Chance of decendant occult mutating")]
        public static int kChanceOfOccultMutation = 0;

        [Tunable, TunableComment("Maximum occults a descendant can have")]
        public static int kMaxOccult = 2;

        [Tunable, TunableComment("Disable alteration of descendants")]
        public static bool kDisableDescendantModification = false;

        //public static string agingDebug = "";

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

        public LoadingScreenControllerEx.LoadingImageType mLoadScreenImageType = kLoadscreenImageType;

        public string mLastFocusedLot = "";

        public string mLastActiveLot = "";

        public bool mDisableDescendants = kDisableDescedants;

        public int mChanceOfOccultMutation = kChanceOfOccultMutation;

        public int mChanceOfOccultHybrid = kChanceOfOccultHybrid;

        public int mMaxOccult = kMaxOccult;

        public bool mDisableDescendantModification = kDisableDescendantModification;

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
            //Common.WriteLog("MergeFromCrossWorldData: " + lookup.Count);
            mWorldForSims = new Dictionary<ulong, string>(lookup);            
        }

        public void MergeToCrossWorldData(Dictionary<ulong, string> lookup)
        {
            //Common.WriteLog("MergeToCrossWorldData: " + lookup.Count);
            foreach (KeyValuePair<ulong, string> value in mWorldForSims)
            {
                lookup.Remove(value.Key);
                lookup.Add(value.Key, value.Value);
            }
        }

        public static int mNotice = 0;

        public WorldName GetHomeWorld(IMiniSimDescription desc)
        {
            //agingDebug += "GetHomeWorld";
            //agingDebug += Common.NewLine;

            if (desc == null) return WorldName.Undefined;

            //agingDebug += "Passed null check";
            //agingDebug += Common.NewLine;

            if (desc.HomeWorld != WorldName.Undefined && desc.HomeWorld != WorldName.UserCreated)
            {
                //agingDebug += "Returning EA Homeworld " + desc.HomeWorld;
                //agingDebug += Common.NewLine;
                return desc.HomeWorld;
            }

            //agingDebug += "Checking mWorldsForSims...";
            //agingDebug += Common.NewLine;

            if (!mWorldForSims.ContainsKey(desc.SimDescriptionId)) return WorldName.Undefined;

            //agingDebug += "Passed";
            //agingDebug += Common.NewLine;

            string world = mWorldForSims[desc.SimDescriptionId];

            //agingDebug += "Got " + world;
            //agingDebug += Common.NewLine;

            string name = world.Replace(".world", "");

            WorldName worldName = WorldName.Undefined;

            try
            {
                worldName = unchecked((WorldName)ResourceUtils.HashString32(name.Replace(" ", "")));
                //agingDebug += "Unchecked";
                //agingDebug += Common.NewLine;
            }
            catch
            {
                //agingDebug += "Failed";
                //agingDebug += Common.NewLine;
                return WorldName.Undefined;
            }                

            return worldName;
        }


        public bool GetAgelessForeign(MiniSimDescription desc)
        {
            //agingDebug += "GetAgelessForeign called for " + desc.FullName;
            //agingDebug += Common.NewLine;
            if (desc.IsDead && !desc.IsPlayableGhost)
            {
                //agingDebug += "Dead";
                //agingDebug += Common.NewLine;
                return true;
            }

            if (desc.IsEP11Bot)
            {
                //agingDebug += "Robot";
                //agingDebug += Common.NewLine;
                return true;
            }

            if (desc.IsDeer || desc.IsRaccoon)
            {
                //agingDebug += "NPC Animal";
                //agingDebug += Common.NewLine;
                return true;
            }

            if (desc.mTraits != null && desc.HasTrait((ulong)TraitNames.SuperVampire))
            {
                //agingDebug += "Vampire";
                //agingDebug += Common.NewLine;
                return true;
            }

            if (desc.mTraits != null && desc.HasTrait((ulong)TraitNames.ForeverYoung))
            {
                //agingDebug += "ForeverYoung";
                //agingDebug += Common.NewLine;
                return true;
            }

            SimDescription sDesc = SimDescription.Find(desc.mSimDescriptionId);
            if (sDesc != null)
            {
                if(sDesc.IsTimeTraveler)
                {
                    //agingDebug += "TimeTraveler";
                    //agingDebug += Common.NewLine;
                    return true;
                }

                if (sDesc.IsBonehilda)
                {
                    //agingDebug += "Bonehilda";
                    //agingDebug += Common.NewLine;
                    return true;
                }

                if (GrimReaper.sGrimReaper != null)
                {
                    if (GrimReaper.sGrimReaper.mPool != null)
                    {
                        if(GrimReaper.sGrimReaper.mPool.Contains(sDesc))
                        {
                            //agingDebug += "Grim";
                            //agingDebug += Common.NewLine;
                            return true;
                        }
                    }
                }

                if (sDesc.IsImaginaryFriend)
                {
                    if(sDesc.OccultManager != null)
                    {
                        OccultImaginaryFriend friend = sDesc.OccultManager.GetOccultType(OccultTypes.ImaginaryFriend) as OccultImaginaryFriend;
                        if (friend != null)
                        {
                            if (!friend.IsReal)
                            {
                                //agingDebug += "Fake imaginary friend";
                                //agingDebug += Common.NewLine;
                                return true;
                            }
                        }
                    }
                }

                if (sDesc.IsGenie)
                {
                    if (sDesc.OccultManager != null)
                    {
                        OccultGenie genie = sDesc.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                        if (genie != null)
                        {
                            if (genie.IsTiedToLamp)
                            {
                                //agingDebug += "Lamp genie";
                                //agingDebug += Common.NewLine;
                                return true;
                            }
                        }
                    }
                }
            }

            WorldName name = GetHomeWorld(desc as IMiniSimDescription);
            //agingDebug += "Got back " + name;
            //agingDebug += Common.NewLine;
            if (name != WorldName.Undefined)
            {
                //agingDebug += "Returning " + mAgelessForeign.ContainsKey(name);
                //agingDebug += Common.NewLine;
                return mAgelessForeign.ContainsKey(name);
            }

            //agingDebug += "Returning default false (true)";
            //agingDebug += Common.NewLine;

            return false;
        }

        public bool GetHiddenWorlds(WorldName world)
        {
            return mHiddenWorlds.ContainsKey(world);
        }

        public void HandleDescendants(bool fromWorldLoadFinished)
        {
            bool disabled = (Traveler.Settings.mDisableDescendants || (Traveler.Settings.mDisableDescendantModification && FutureDescendantServiceEx.ActiveHouseholdHasDescendants()));

            if (!disabled && !fromWorldLoadFinished && !GameUtils.IsFutureWorld())
            {
                // originally had this GameUtils.IsFutureWorld and a call to RegenerateDescendants here but it
                // creates problems when all the progninators don't travel to the future. It's a solvable issue
                // but not right now
                FutureDescendantServiceEx.AddListeners();
            }
            else if (disabled && !fromWorldLoadFinished)
            {
                if (Traveler.Settings.mDisableDescendants)
                {
                    FutureDescendantServiceEx.WipeDescendants();
                }
                FutureDescendantServiceEx.ClearListeners();
            }
            else if (disabled && fromWorldLoadFinished)
            {
                FutureDescendantServiceEx.ClearListeners();
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
    }
}
