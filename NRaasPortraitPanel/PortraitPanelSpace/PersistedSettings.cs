using NRaas.PortraitPanelSpace.Dialogs;
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

namespace NRaas.PortraitPanelSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Which set of sims to display on the panel (WholeTown, ActiveHousehold, SelectedSims, ActiveSimLot, ActiveFamilyLot, ViewedLot, ActiveHomeLot)")]
        public static SkewerEx.VisibilityType[] kSetType = new SkewerEx.VisibilityType[] { SkewerEx.VisibilityType.ActiveHousehold };
        [Tunable, TunableComment("Applied to the sims defined by SetType (OnlyIdle, OnlySelectable, ActiveSimLot, ActiveFamilyLot, ViewedLot, ActiveHomeLot)")]
        public static SkewerEx.VisibilityType[] kVisibilityType = new SkewerEx.VisibilityType[0];
        [Tunable, TunableComment("The method to sort the portraits (ByAge, ByMood, ByName, ByAutonomy, BySelectability, ByCustom, ActiveHousehold, BySpecies)")]
        public static SkewerEx.SortType kSortType = SkewerEx.SortType.ByAge;
        [Tunable, TunableComment("Whether to sort in reverse order")]
        public static bool kReverse = false;
        [Tunable, TunableComment("Whether the sim menu is displayed when you double click")]
        public static bool kMenuOnLeftClick = false;
        [Tunable, TunableComment("Whether the show the interactions for this mod in the sim menu")]
        public static bool kShowSimMenu = true;
        [Tunable, TunableComment("Whether to use Teleport interaction when Go Home is pressed (provided a Teleporter object is available)")]
        public static bool kGoHomeTeleport = false;
        [Tunable, TunableComment("Whether to always use Teleport interaction when Go Home is pressed")]
        public static bool kGoHomeTeleportForAll = false;
        [Tunable, TunableComment("Whether to allow the selection of inactive sims directly from the panel")]
        public static bool kDreamCatcher = false;
        [Tunable, TunableComment("Whether to zoom in from map view on a single right click")]
        public static bool kZoomInOnRightClick = false;
        [Tunable, TunableComment("The secondary sort method (ByAge, ByMood, ByName, ByAutonomy, BySelectability, ByCustom, ActiveHousehold, BySpecies)")]
        public static SkewerEx.SortType kSecondSortType = SkewerEx.SortType.ByName;
        [Tunable, TunableComment("Whether to perform the secondary sort in reverse order")]
        public static bool kSecondReverse = false;
        [Tunable, TunableComment("Whether to cycle portraits button when there are less than 24 sims listed")]
        public static bool kShowCycleButton = true;
        [Tunable, TunableComment("The tertiary sort method (ByAge, ByMood, ByName, ByAutonomy, BySelectability, ByCustom, ActiveHousehold, BySpecies)")]
        public static SkewerEx.SortType kThirdSortType = SkewerEx.SortType.ByName;
        [Tunable, TunableComment("Whether to perform the tertiary sort in reverse order")]
        public static bool kThirdReverse = false;
        [Tunable, TunableComment("The sims to display in column one (ActiveHousehold, ActiveHumans, ActiveAnimals, InactiveHumans, InactiveAnimals, SelectedSims)")]
        public static SkewerEx.VisibilityType[] kColumnFilter1 = new SkewerEx.VisibilityType[] { SkewerEx.VisibilityType.ActiveHumans };
        [Tunable, TunableComment("The sims to display in column two (ActiveHousehold, ActiveHumans, ActiveAnimals, InactiveHumans, InactiveAnimals, SelectedSims)")]
        public static SkewerEx.VisibilityType[] kColumnFilter2 = new SkewerEx.VisibilityType[] { SkewerEx.VisibilityType.ActiveAnimals };
        [Tunable, TunableComment("The sims to display in column three (ActiveHousehold, ActiveHumans, ActiveAnimals, InactiveHumans, InactiveAnimals, SelectedSims)")]
        public static SkewerEx.VisibilityType[] kColumnFilter3 = new SkewerEx.VisibilityType[0];

        [Tunable, TunableComment("Whether to revert to using a single list when there are too many sims listed")]
        public static bool kRevertToSingleListOnTooMany = true;
        [Tunable, TunableComment("Whether to revert to using a single list when one of the sims does not match any of the column filters")]
        public static bool kRevertToSingleListOnFilterFail = true;
        [Tunable, TunableComment("Whether to show the known info tooltip")]
        public static bool kShowKnownInfo = true;
        [Tunable, TunableComment("Whether to use Portrait seventeen which overlaps with the map tag filter in map view")]
        public static bool kUsePortraitSeventeen = false;

        public Dictionary<ulong, int> mSimsV2 = new Dictionary<ulong, int>();

        public Dictionary<ulong, int> mSimColumns = new Dictionary<ulong, int>();

        public SkewerEx.SortType mSortType = kSortType;
        public SkewerEx.SortType mSecondSortType = kSecondSortType;
        public SkewerEx.SortType mThirdSortType = kThirdSortType;

        public List<SkewerEx.VisibilityType> mColumnFilter1 = new List<SkewerEx.VisibilityType>(kColumnFilter1);
        public List<SkewerEx.VisibilityType> mColumnFilter2 = new List<SkewerEx.VisibilityType>(kColumnFilter2);
        public List<SkewerEx.VisibilityType> mColumnFilter3 = new List<SkewerEx.VisibilityType>(kColumnFilter3);

        public List<SkewerEx.VisibilityType> mSetTypeV3 = new List<SkewerEx.VisibilityType>(kSetType);

        public List<SkewerEx.VisibilityType> mVisibilityTypeV3 = new List<SkewerEx.VisibilityType>(kVisibilityType);

        [Persistable(false)]
        protected Dictionary<SkewerEx.VisibilityType, bool> mSetTypeLookup = null;

        [Persistable(false)]
        protected Dictionary<SkewerEx.VisibilityType, bool> mUseLookup = null;

        public bool mReverse = kReverse;
        public bool mSecondReverse = kSecondReverse;
        public bool mThirdReverse = kThirdReverse;

        public bool mMenuOnLeftClick = kMenuOnLeftClick;

        public bool mShowSimMenu = kShowSimMenu;
        public bool mShowKnownInfo = kShowKnownInfo;

        public bool mGoHomeTeleport = kGoHomeTeleport;
        public bool mGoHomeTeleportForAll = kGoHomeTeleportForAll;
        public bool mDreamCatcher = kDreamCatcher;
        public bool mZoomInOnRightClick = kZoomInOnRightClick;
        public bool mShowCycleButton = kShowCycleButton;
        public bool mUsePortraitSeventeen = kUsePortraitSeventeen;

        public bool mRevertToSingleListOnTooMany = kRevertToSingleListOnTooMany;
        public bool mRevertToSingleListOnFilterFail = kRevertToSingleListOnFilterFail;

        [Persistable(false)]
        Dictionary<ulong,SimDescription> mDescriptions = null;

        public PersistedSettings()
        {}

        public void ResetSetTypeLookup()
        {
            mSetTypeLookup = null;
        }

        public bool ContainsSetType(SkewerEx.VisibilityType value)
        {
            if (mSetTypeLookup == null)
            {
                mSetTypeLookup = new Dictionary<SkewerEx.VisibilityType, bool>();

                foreach (SkewerEx.VisibilityType type in mSetTypeV3)
                {
                    mSetTypeLookup[type] = true;
                }
            }

            return mSetTypeLookup.ContainsKey(value);
        }

        public void ResetUseLookup()
        {
            mUseLookup = null;
        }

        public bool HasSim(Sim sim)
        {
            return mSimsV2.ContainsKey(sim.SimDescription.SimDescriptionId);
        }

        public int GetSimColumn(SimDescription sim)
        {
            int value;
            if (mSimColumns.TryGetValue(sim.SimDescriptionId, out value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        public int GetSimSort(SimDescription sim)
        {
            int value;
            if (mSimsV2.TryGetValue(sim.SimDescriptionId, out value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        public void ResetSimSort()
        {
            foreach (ulong sim in new List<ulong>(mSimsV2.Keys))
            {
                mSimsV2[sim] = 0;
            }
        }

        public void SetSimColumn(SimDescription sim, int value)
        {
            if (!mSimsV2.ContainsKey(sim.SimDescriptionId)) return;

            mSimColumns[sim.SimDescriptionId] = value;
        }

        public void SetSimSort(SimDescription sim, int value)
        {
            if (!mSimsV2.ContainsKey(sim.SimDescriptionId)) return;

            mSimsV2[sim.SimDescriptionId] = value;
        }

        public void AddSim(Sim sim)
        {
            if (sim == null) return;

            if (HasSim(sim)) return;

            mSimsV2.Add(sim.SimDescription.SimDescriptionId, 0);

            mDescriptions = null;
        }

        public void RemoveSim(SimDescription sim)
        {
            if (sim == null) return;

            mSimsV2.Remove(sim.SimDescriptionId);

            mDescriptions = null;
        }

        public bool InUse(SkewerEx.VisibilityType type)
        {
            if (mUseLookup == null)
            {
                mUseLookup = new Dictionary<SkewerEx.VisibilityType, bool>();

                foreach (SkewerEx.VisibilityType value in mColumnFilter1)
                {
                    mUseLookup[value] = true;
                }

                foreach (SkewerEx.VisibilityType value in mColumnFilter2)
                {
                    mUseLookup[value] = true;
                }

                foreach (SkewerEx.VisibilityType value in mColumnFilter3)
                {
                    mUseLookup[value] = true;
                }

                foreach (SkewerEx.VisibilityType value in mSetTypeV3)
                {
                    mUseLookup[value] = true;
                }

                foreach (SkewerEx.VisibilityType value in mVisibilityTypeV3)
                {
                    mUseLookup[value] = true;
                }
            }

            return mUseLookup.ContainsKey(type);
        }

        public SimDescription GetSim(ulong id)
        {
            SimDescription sim;
            if ((mDescriptions != null) && (mDescriptions.TryGetValue(id, out sim)))
            {
                return sim;
            }
            else
            {
                MiniSimDescription miniSim = MiniSimDescription.Find(id);
                if ((miniSim != null) && (miniSim.Genealogy != null))
                {
                    return miniSim.Genealogy.mSim;
                }
                else
                {
                    return null;
                }
            }
        }
        public SimDescription GetSim(SimInfo info)
        {
            if (info.UninstantiatedSimDescriptionId == 0)
            {
                Sim sim = Sim.GetObject(info.mGuid) as Sim;
                if (sim == null) return null;

                return sim.SimDescription;
            }
            else
            {
                return GetSim(info.UninstantiatedSimDescriptionId);
            }
        }

        public void AddSelectedSimsFilter()
        {
            if (!ContainsSetType(SkewerEx.VisibilityType.SelectedSims))
            {
                mSetTypeV3.Add(SkewerEx.VisibilityType.SelectedSims);

                ResetSetTypeLookup();

                if ((!mColumnFilter1.Contains(SkewerEx.VisibilityType.SelectedSims)) &&
                    (!mColumnFilter2.Contains(SkewerEx.VisibilityType.SelectedSims)) &&
                    (!mColumnFilter3.Contains(SkewerEx.VisibilityType.SelectedSims)))
                {
                    mColumnFilter3.Add(SkewerEx.VisibilityType.SelectedSims);
                }
            }
        }

        public ICollection<SimDescription> SelectedSims
        {
            get
            {
                if (mDescriptions == null)
                {
                    mDescriptions = new Dictionary<ulong,SimDescription>();

                    foreach (SimDescription sim in Household.EverySimDescription())
                    {
                        if (!mSimsV2.ContainsKey(sim.SimDescriptionId)) continue;

                        mDescriptions[sim.SimDescriptionId] = sim;
                    }
                }

                return mDescriptions.Values;
            }
        }
    }
}
