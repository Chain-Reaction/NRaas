using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to all more the eight sims in a family when using Add Sim or Pollinate")]
        public static bool kAllowOverStuffed = true;
        [Tunable, TunableComment("Whether to retain Dreams and Opportunities when switching between families")]
        public static bool kDreamCatcher = false;
        [Tunable, TunableComment("A multiple used to increase the min/max range of facial sliders")]
        public static int kSliderMultiple = 1;
        [Tunable, TunableComment("Whether to display the menu on sims")]
        public static bool kMenuVisibleSim = true;
        [Tunable, TunableComment("Whether to display the menu on lots")]
        public static bool kMenuVisibleLot = true;
        [Tunable, TunableComment("Whether to display the menu on the computer")]
        public static bool kMenuVisibleComputer = true;
        [Tunable, TunableComment("How many levels to display on the family tree")]
        public static int kFamilyTreeLevels = 4;
        [Tunable, TunableComment("Whether to run Tattoo CAS using the naked outfit")]
        public static bool kNakedTattoo = false;
        [Tunable, TunableComment("Whether to reset the active lot during 'Reset Everything'")]
        public static bool kResetEverythingOnActive = true;
        [Tunable, TunableComment("Comma separated list of hot key actions (Interaction IDs are the same as those used in STBL translation)")]
        public static string[] kHotkeys = new string[0];
        [Tunable, TunableComment("Comma separated list of custom selected buffs")]
        public static string[] kCustomBuffs = new string[0];
        [Tunable, TunableComment("Comma separated list of skills and minimum levels, where the first element is the name of the stamp.  eg: GMStamp,Guitar:10,MartialArts:5")]
        public static string[] kSkillStamp = new string[0];
        [Tunable, TunableComment("Whether to display the Set Value interaction in the object menu")]
        public static bool kMenuVisibleSetValue = true;
        [Tunable, TunableComment("Whether to display the Open and Close interactions in the object menu")]
        public static bool kMenuVisibleOpenSesame = true;
        [Tunable, TunableComment("Whether to display the Sort Inventory interaction in the object menu")]
        public static bool kMenuVisibleSortInventory = true;
        [Tunable, TunableComment("Whether to display the Add Sim and Evict interactions in the object menu")]
        public static bool kMenuVisibleAddSim = true;
        [Tunable, TunableComment("Whether to handle the CAS Slider panels within this mod, and ignore custom Core changes")]
        public static bool kOverrideCoreSliders = false;
        [Tunable, TunableComment("Number of intervals to display on Sims By Money and the money filters")]
        public static int kByMoneyIntervals = 10;
        [Tunable, TunableComment("Whether to display the Ownership interaction in the object menu")]
        public static bool kMenuVisibleOwnership = true;        
        [Tunable, TunableComment("Whether to retain separate hair coloring for each sim outfit")]
        public static bool kHairColorByOutfit = false;
        [Tunable, TunableComment("Whether to unlock all hairs for every gender")]
        public static bool kUniGenderHair = false;
        [Tunable, TunableComment("Whether to unlock the beard panel for all sims")]
        public static bool kBeardsForAll = false;
        [Tunable, TunableComment("Whether to display all makeup parts in CAS")]
        public static bool kDisableMakeupFilter = true;
        [Tunable, TunableComment("The amount to offset each body slider to produce lower than normally possible values")]
        public static int kBodySliderOffset = 0;

        [Tunable, TunableComment("The default species to display whenever the Species is not explicitly being used (Human, LittleDog, Dog, Cat, Horse)")]
        public static CASAgeGenderFlags[] kDefaultSpecies = new CASAgeGenderFlags[] { CASAgeGenderFlags.Human };
        [Tunable, TunableComment("Whether to display the Transfer To interactions in the object menu")]
        public static bool kMenuVisibleTransferTo = true;
        [Tunable, TunableComment("Whether to display the Transfer From interactions in the object menu")]
        public static bool kMenuVisibleTransferFrom = true;
        [Tunable, TunableComment("Whether to check for the existence of any double bed during home inspection")]
        public static bool kCheckForNoDoubleBed = true;
        [Tunable, TunableComment("Whether to display the numeric values for each slider, while in CAS")]
        public static bool kShowCASSliderValues = true;
        [Tunable, TunableComment("A multiple used to increase the min/max range of body sliders")]
        public static int kBodySliderMultiple = 1;
        [Tunable, TunableComment("What order to display Clothing in CAS (EAStandard, CustomContentTop, CustomContentBottom)")]
        public static CASBase.SortOrder kClothingSortOrder = CASBase.SortOrder.EAStandard;
        [Tunable, TunableComment("Whether to allow for multiple accessories per body position")]
        public static bool kAllowMultipleAccessories = false;
        [Tunable, TunableComment("Whether to allow for multiple makeup parts per body position")]
        public static bool kAllowMultipleMakeup = false;
        [Tunable, TunableComment("Whether to remove the limit on the number of active topics choices when displaying the sim menu")]
        public static bool kRemoveActiveTopicLimit = true;
        [Tunable, TunableComment("Whether to allow different eye color to be used for each outfit")]
        public static bool kEyeColorByCategory = false;

        [Tunable, TunableComment("Whether to display the Radius Purge interactions in the object menu")]
        public static bool kMenuVisibleRadiusPurge = true;
        [Tunable, TunableComment("Whether to display the filter during party selections")]
        public static bool kUsePartyFilter = true;
        [Tunable, TunableComment("Whether to display the advanced slider panel by default")]
        public static bool kDefaultAdvancedPanel = true;
        [Tunable, TunableComment("Whether to allow the use of Add Sim on community lots")]
        public static bool kCommunityAddSim = false;
        [Tunable, TunableComment("Whether to display the Radius Add To Inventory interactions in the object menu")]
        public static bool kMenuVisibleRadiusAddToInventory = true;
        [Tunable, TunableComment("Whether to allow selection of male adult clothing for teenagers")]
        public static bool kMaleAdultClothesForTeen = false;
        [Tunable, TunableComment("Whether to allow selection of female adult clothing for teenagers")]
        public static bool kFemaleAdultClothesForTeen = false;
        [Tunable, TunableComment("Whether to display Clothing CAS using the compact method (faster, but missing all the presets)")]
        public static bool kCompactClothingCAS = false;
        [Tunable, TunableComment("Whether to display Hat CAS using the compact method (faster, but missing all the presets)")]
        public static bool kCompactHatCAS = false;
        [Tunable, TunableComment("Whether to display Accessory CAS using the compact method (faster, but missing all the presets)")]
        public static bool kCompactAccessoryCAS = false;
        [Tunable, TunableComment("Whether to unlock the body hair panel for teens")]
        public static bool kBodyHairForTeens = false;
        [Tunable, TunableComment("Whether to unlock the body hair panel for females")]
        public static bool kBodyHairForFemales = false;
        [Tunable, TunableComment("Whether to unlock all accessories for every gender")]
        public static bool kUniGenderAccessories = false;
        [Tunable, TunableComment("Whether to unlock all toddler/child clothing for males")]
        public static bool kUniGenderChildMale = false;
        [Tunable, TunableComment("Whether to unlock all toddler/child clothing for females")]
        public static bool kUniGenderChildFemale = false;
        [Tunable, TunableComment("Whether to allow selection of female adult clothing for elders")]
        public static bool kAdultClothesForElders = false;
        [Tunable, TunableComment("Whether to allow selection of adult accessories for teens")]
        public static bool kAdultAccessoriesForTeens = false;
        [Tunable, TunableComment("Whether to disable the clothing filter and allow all outfits in every category (Everyday, Career, Outerwear, Formalwear, Swimwear, Sleepwear, Athletic)")]
        public static OutfitCategories[] kDisableClothingFilterV2 = new OutfitCategories[] { OutfitCategories.Career };
        [Tunable, TunableComment("Whether to filter party guests by the type of party chosen")]
        public static bool kPartyAgeFilter = false;
        [Tunable, TunableComment("Which expansion packs to hide in CAS by default (EP1, SP1, EP2, SP2, etc)")]
        public static ProductVersion[] kHideByProduct = new ProductVersion[0];        

        [Tunable, TunableComment("Whether to display normally hidden CAS parts")]
        public static bool kShowHiddenParts = false;
        [Tunable, TunableComment("Whether to unlock all adults clothing for all genders")]
        public static bool kUniGenderAdult = false;

        [Tunable, TunableComment("The number of sliders to allow in CAS")]
        public static int kMaxCASSliders = 100;

        public List<SelectionCriteria.SavedFilter> mFilters = new List<SelectionCriteria.SavedFilter>();
        public List<OutfitCategories> mDisableClothingFilterV2 = new List<OutfitCategories>(kDisableClothingFilterV2);
        public bool mAllowOverStuffed = kAllowOverStuffed;
        public bool mDreamCatcher = kDreamCatcher;
        public int mSliderMultiple = kSliderMultiple;
        public int mBodySliderMultiple = kBodySliderMultiple;
        public bool mMenuVisibleSim = kMenuVisibleSim;
        public bool mMenuVisibleLot = kMenuVisibleLot;
        public bool mMenuVisibleComputer = kMenuVisibleComputer;
        public int mFamilyTreeLevels = kFamilyTreeLevels;
        public bool mNakedTattoo = kNakedTattoo;
        public bool mResetEverythingOnActive = kResetEverythingOnActive;
        public bool mMenuVisibleSetValue = kMenuVisibleSetValue;
        public bool mMenuVisibleOpenSesame = kMenuVisibleOpenSesame;
        public bool mMenuVisibleSortInventory = kMenuVisibleSortInventory;
        public bool mMenuVisibleAddSim = kMenuVisibleAddSim;
        public bool mOverrideCoreSliders = kOverrideCoreSliders;
        public int mByMoneyIntervals = kByMoneyIntervals;
        public bool mMenuVisibleOwnership = kMenuVisibleOwnership;       
        public bool mHairColorByOutfit = kHairColorByOutfit;
        public bool mUniGenderHair = kUniGenderHair;
        public bool mBeardsForAll = kBeardsForAll;
        public bool mDisableMakeupFilter = kDisableMakeupFilter;
        public int mBodySliderOffset = kBodySliderOffset;

        public List<CASAgeGenderFlags> mDefaultSpecies = new List<CASAgeGenderFlags>(kDefaultSpecies);
        public bool mMenuVisibleTransferTo = kMenuVisibleTransferTo;
        public bool mMenuVisibleTransferFrom = kMenuVisibleTransferFrom;
        public bool mCheckForNoDoubleBed = kCheckForNoDoubleBed;
        public bool mShowCASSliderValues = kShowCASSliderValues;
        public bool mMaleAdultClothesForTeen = kMaleAdultClothesForTeen;
        public bool mFemaleAdultClothesForTeen = kFemaleAdultClothesForTeen;
        public CASBase.SortOrder mClothingSortOrder = kClothingSortOrder;
        public bool mAllowMultipleAccessories = kAllowMultipleAccessories;
        public bool mAllowMultipleMakeup = kAllowMultipleMakeup;
        public bool mRemoveActiveTopicLimit = kRemoveActiveTopicLimit;
        public bool mEyeColorByCategory = kEyeColorByCategory;

        public bool mMenuVisibleRadiusPurge = kMenuVisibleRadiusPurge;
        public bool mUsePartyFilter = kUsePartyFilter;
        public bool mDefaultAdvancedPanel = kDefaultAdvancedPanel;
        public bool mCommunityAddSim = kCommunityAddSim;
        public bool mMenuVisibleRadiusAddToInventory = kMenuVisibleRadiusAddToInventory;

        public bool mCompactClothingCAS = kCompactClothingCAS;
        public bool mCompactHatCAS = kCompactHatCAS;
        public bool mCompactAccessoryCAS = kCompactAccessoryCAS;
        public bool mBodyHairForTeens = kBodyHairForTeens;
        public bool mBodyHairForFemales = kBodyHairForFemales;
        public bool mUniGenderAccessories = kUniGenderAccessories;
        public bool mUniGenderChildMale = kUniGenderChildMale;
        public bool mUniGenderChildFemale = kUniGenderChildFemale;
        public bool mAdultClothesForElders = kAdultClothesForElders;
        public bool mAdultAccessoriesForTeens = kAdultAccessoriesForTeens;
        public bool mPartyAgeFilter = kPartyAgeFilter;
        public List<ProductVersion> mHideByProduct = new List<ProductVersion>(kHideByProduct);        
        public bool mShowHiddenParts = kShowHiddenParts;
        public bool mUniGenderAdult = kUniGenderAdult;

        public int mMaxCASSliders = kMaxCASSliders;

        public List<string> mHotkeys = new List<string>(kHotkeys);
        public List<string> mCustomBuffs = new List<string>(kCustomBuffs);
        private List<string> mSkillStamp = new List<string>(kSkillStamp);

        private Dictionary<ResourceKey, List<InvalidPart>> mBlacklistParts = new Dictionary<ResourceKey, List<InvalidPart>>();

        private Dictionary<ulong, bool> mExcludedResetLots = new Dictionary<ulong, bool>();

        public SelectionCriteria.SavedFilter mLastTagFilter = null;

        public SelectionCriteria.SavedFilter mMostRecentFilter = null;

        [Persistable(false)]
        private List<SkillStamp> mSkillStamps = null;

        public List<SkillStamp> SkillStamps
        {
            get
            {
                if (mSkillStamps == null)
                {
                    mSkillStamps = SkillStamp.Create(mSkillStamp);
                }

                return mSkillStamps;
            }
        }

        public int BlacklistPartsCount
        {
            get
            {
                return mBlacklistParts.Count;
            }
        }

        public IEnumerable<ResourceKey> BlacklistKeys
        {
            get
            {
                return mBlacklistParts.Keys;
            }
        }

        public void RemoveBlacklistKey(ResourceKey key)
        {
            mBlacklistParts.Remove(key);
        }

        public void ClearBlacklist()
        {
            mBlacklistParts.Clear();

            ApplyBlacklistParts();
        }

        public void ImportBlacklist(Persistence.Lookup settings)
        {
            mBlacklistParts.Clear();

            int count = settings.GetInt("Count", 0);

            for (int i = 0; i < count; i++)
            {
                using (Persistence.Lookup.Pusher pusher2 = new Persistence.Lookup.Pusher(settings, i.ToString()))
                {
                    string key = settings.GetString("Key");
                    if (key == null) continue;

                    string[] values = key.Split(':');
                    if (values.Length != 3) continue;

                    uint type;
                    if (!uint.TryParse(values[0], out type)) continue;

                    uint group;
                    if (!uint.TryParse(values[1], out group)) continue;

                    ulong instance;
                    if (!ulong.TryParse(values[2], out instance)) continue;

                    InvalidPart part = new InvalidPart();
                    if (part.Import(settings))
                    {
                        AddBlacklistPart(new ResourceKey(instance, type, group), part);
                    }
                }
            }
        }

        public void ExportBlacklist(Persistence.Lookup settings)
        {
            settings.Add("Count", mBlacklistParts.Count.ToString());

            int count = 0;
            foreach (KeyValuePair<ResourceKey, List<InvalidPart>> value in mBlacklistParts)
            {
                foreach (InvalidPart part in value.Value)
                {
                    using (Persistence.Lookup.Pusher pusher2 = new Persistence.Lookup.Pusher(settings, count.ToString()))
                    {
                        count++;

                        settings.Add("Key", value.Key.TypeId.ToString() + ":" + value.Key.GroupId.ToString() + ":" + value.Key.InstanceId.ToString());

                        part.Export(settings);
                    }
                }
            }
        }

        public string GetBlacklistParts()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Common.NewLine + "<base>");

            builder.Append(Common.NewLine + "  <InvalidParts>");
            builder.Append(Common.NewLine + "    <!-- This is the default set, do not alter or remove it -->");
            builder.Append(Common.NewLine + "    <BodyType></BodyType>");
            builder.Append(Common.NewLine + "    <Instance></Instance>");
            builder.Append(Common.NewLine + "    <Group></Group>");
            builder.Append(Common.NewLine + "    <Age>AgeMask</Age>");
            builder.Append(Common.NewLine + "    <Gender>GenderMask</Gender>");
            builder.Append(Common.NewLine + "    <Species>SpeciesMask</Species>");
            builder.Append(Common.NewLine + "    <Categories>All</Categories>");
            builder.Append(Common.NewLine + "    <Extended></Extended>");
            builder.Append(Common.NewLine + "    <WorldTypes></WorldTypes>");
            builder.Append(Common.NewLine + "  </InvalidParts>");

            foreach (KeyValuePair<ResourceKey, List<InvalidPart>> value in mBlacklistParts)
            {
                foreach(InvalidPart part in value.Value)
                {
                    builder.Append(Common.NewLine + part.ToXML(value.Key, "  "));
                }
            }

            builder.Append(Common.NewLine + "</base>");

            return builder.ToString();
        }

        public void AddBlacklistPart(ResourceKey key, InvalidPart invalid)
        {
            List<InvalidPart> parts;
            if (!mBlacklistParts.TryGetValue(key, out parts))
            {
                parts = new List<InvalidPart>();
                mBlacklistParts.Add(key, parts);
            }

            parts.Add(invalid);

            InvalidPartBooter.AddInvalidPart(key, invalid);
        }

        public void ApplyBlacklistParts()
        {
            InvalidPartBooter.ParseInvalidParts<InvalidPart>();

            foreach (KeyValuePair<ResourceKey, List<InvalidPart>> value in mBlacklistParts)
            {
                foreach (InvalidPart part in value.Value)
                {
                    InvalidPartBooter.AddInvalidPart(value.Key, part);
                }
            }
        }

        public bool IsExcludedLot(Lot lot)
        {
            if (lot == null) return false;

            return mExcludedResetLots.ContainsKey(lot.LotId);
        }

        public void RemoveExcludedLot(Lot lot)
        {
            mExcludedResetLots.Remove(lot.LotId);
        }

        public void AddExcludedLot(Lot lot)
        {
            if (mExcludedResetLots.ContainsKey(lot.LotId)) return;

            mExcludedResetLots.Add(lot.LotId, true);
        }

        public void UpdateStamp()
        {
            if (mSkillStamps == null) return;

            mSkillStamp = SkillStamp.Create(mSkillStamps);
        }
    }
}
