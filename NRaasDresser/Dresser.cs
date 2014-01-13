using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.DresserSpace;
using NRaas.DresserSpace.Helpers;
using NRaas.DresserSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Dresser : Common, Common.IPreLoad, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static Common.AlarmTask sTask = null;

        [PersistableStatic]
        static DresserSettings sSettings;

        static Dresser()
        {
            Bootstrap();
        }

        public static DresserSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new DresserSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnPreLoad()
        {
            InvalidPartBooter.ParseParts<DresserInvalidPart>();

            //BooterLogger.AddError("Check");
        }

        public void OnWorldLoadFinished()
        {
            RestartAlarm();

            Settings.CleanupSims();

            kDebugging = Settings.Debugging;
        }

        public static string GetStoreOutfitKey(OutfitCategories category, bool maternity)
        {
            return VersionStamp.sNamespace + category + maternity;
        }

        public static void RestartAlarm()
        {
            if (sTask != null)
            {
                sTask.Dispose();
                sTask = null;
            }

            if (Settings.NightlyRandomChangeOutfit)
            {
                sTask = new Common.AlarmTask(Settings.NightlyChangeOutfitHour, DaysOfTheWeek.All, OnTimer);
            }
        }

        protected static void OnTimer()
        {
            foreach (Sim sim in LotManager.Actors)
            {
                DresserSpace.Helpers.SwitchOutfits.PerformRotation(sim, false, false);
            }
        }

        [Persistable]
        public class DresserSettings
        {
            [Tunable, TunableComment("Whether to change all sims outfits to a new selection each night")]
            public static bool kNightlyRandomChangeOutfit = false;
            [Tunable, TunableComment("Hour each night to switch the outfits on sims")]
            public static float kNightlyChangeOutfitHour = 7f;
            [Tunable, TunableComment("Whether to change the outfits of the active sims during nightly rotation")]
            public static bool kRotationAffectActive = false;
            [Tunable, TunableComment("The percent chance a sim will consider switching outfits")]
            public static int kChanceOfSwitch = 75;
            [Tunable, TunableComment("Whether to rotate the martial arts outfit category")]
            public static bool kRotateMartialOutfit = false;
            [Tunable, TunableComment("Whether to check outfits on age-up")]
            public static bool kCheckOutfits = true;
            [Tunable, TunableComment("Whether to display an notification whenever a sim's outfit is altered")]
            public static bool kNotifyOnCheckOutfits = true;
            [Tunable, TunableComment("Whether to force a sim to switch out of Sleepwear or Athletic upon entering the outdoors")]
            public static bool kSwitchOnOutside = true;

            [Tunable, TunableComment("Whether the mod is allowed to use clothing not marked ValidForRandom when replacing invalids")]
            public static bool kIgnoreValidForRandomClothing = false;
            [Tunable, TunableComment("Whether the mod is allowed to use hair not marked ValidForRandom when replacing invalids")]
            public static bool kIgnoreValidForRandomHair = false;
            [Tunable, TunableComment("Whether the mod is allowed to use accessories not marked ValidForRandom when replacing invalids")]
            public static bool kIgnoreValidForRandomAccessories = true;
            [Tunable, TunableComment("Whether the mod is allowed to use makeup not marked ValidForRandom when replacing invalids")]
            public static bool kIgnoreValidForRandomMakeup = true;
            [Tunable, TunableComment("Whether to switch out of formal wear upon leaving a lot")]
            public static bool kSwitchFormal = false;
            [Tunable, TunableComment("Whether to change the outfits of the active sims during room transitions")]
            public static bool kTransitionAffectActive = false;
            [Tunable, TunableComment("Whether to change the outfits of the active sims during age-up checks")]
            public static bool kCheckAffectActive = false;
            [Tunable, TunableComment("Whether to switch out of sleepwear upon siwtching rooms")]
            public static bool kSwitchSleepwear = false;
            [Tunable, TunableComment("Whether to use the same hair for all categories when rerolling")]
            public static bool kSameHairForAllCategories = true;
            [Tunable, TunableComment("Whether to allow hats when rerolling")]
            public static bool kAllowHats = true;
            [Tunable, TunableComment("Whether to add accessories to the martial arts outfits")]
            public static bool kMartialArtsAccessories = false;
            [Tunable, TunableComment("Whether to rotate the outfits of skin jobs")]
            public static bool kRotateOccult = false;
            [Tunable, TunableComment("Whether to reroll hair on adult age-up")]
            public static bool kAllowRollAgeupHair = true;
            [Tunable, TunableComment("Whether to reroll makeup on adult age-up")]
            public static bool kAllowRollAgeupMakeup = true;
            [Tunable, TunableComment("Whether to ignore the gender on body hair parts")]
            public static bool kIgnoreBodyHairGender = false;
            [Tunable, TunableComment("Whether to use the same makeup for all categories when rerolling")]
            public static bool kSameMakeupForAllCategories = true;
            [Tunable, TunableComment("Whether to add accessories to pets during Check Outfits")]
            public static bool kAllowPetAccessories = true;
            [Tunable, TunableComment("Whether to display the mod's sim menu")]
            public static bool kSimMenuVisibility = true;

            [Tunable, TunableComment("Whether to use the Master Controller blacklist")]
            public static bool kUseMasterControllerBlackList = false;
            [Tunable, TunableComment("Defines the first index to use while rotating outfits on vacation")]
            public static int kVacationOutfitIndex = 0;
            [Tunable, TunableComment("Whether to reroll body hair on adult age-up")]
            public static bool kAllowRollAgeupBodyHair = false;
            [Tunable, TunableComment("Whether to reroll accessories on adult age-up")]
            public static bool kAllowRollAgeupAccessories = false;

            [Tunable, TunableComment("Percent chance of adding a beard to a sim")]
            public static int kRandomBeardChance = 25;
            [Tunable, TunableComment("Whether to reroll hair on general age-up")]
            public static bool kAllowRollAgeupHairGeneral = true;
            [Tunable, TunableComment("Whether to reroll body hair on general age-up")]
            public static bool kAllowRollAgeupBodyHairGeneral = true;
            [Tunable, TunableComment("Whether to choose a brand new outfit when a sim ages into Adult")]
            public static bool kFullAdultReroll = false;
            [Tunable, TunableComment("Whether to add accessories to stray animals")]
            public static bool kAllowStrayPetAccessories = false;
            [Tunable, TunableComment("Whether to change the outfits of the inactive sims during nightly rotation")]
            public static bool kRotationAffectInactive = true;
            [Tunable, TunableComment("Whether to inherit body hair from a sim's parents")]
            public static bool kInheritBodyHair = true;

            [Tunable, TunableComment("Product versions whose parts are deemed invalid for random")]
            public static ProductVersion[] kInvalidProductVersionGeneral = new ProductVersion[0];

            [Tunable, TunableComment("Product versions whose parts are deemed invalid for makeup")]
            public static ProductVersion[] kInvalidProductVersionMakeup = new ProductVersion[] { ProductVersion.EP7 };
            [Tunable, TunableComment("Product versions whose parts are deemed invalid for accessories")]
            public static ProductVersion[] kInvalidProductVersionAccessories = new ProductVersion[0];
            [Tunable, TunableComment("Product versions whose parts are deemed invalid for body hair")]
            public static ProductVersion[] kInvalidProductVersionBodyHair = new ProductVersion[] { ProductVersion.EP7 };
            [Tunable, TunableComment("Product versions whose parts are deemed invalid for head hair/hats")]
            public static ProductVersion[] kInvalidProductVersionHair = new ProductVersion[0];

            [Tunable, TunableComment("The minimum number of random accessories to add to males which have none")]
            public static int kMinRandomMaleAccessories = 1;
            [Tunable, TunableComment("The maximum number of random accessories to add to males which have none")]
            public static int kMaxRandomMaleAccessories = 1;
            [Tunable, TunableComment("The minimum number of random accessories to add to females which have none")]
            public static int kMinRandomFemaleAccessories = 1;
            [Tunable, TunableComment("The maximum number of random accessories to add to females which have none")]
            public static int kMaxRandomFemaleAccessories = 4;
            [Tunable, TunableComment("Percent chance of adding accessories to a male sim")]
            public static int kRandomMaleAccessoriesChance = 75;
            [Tunable, TunableComment("Percent chance of adding accessories to a female sim")]
            public static int kRandomFemaleAccessoriesChance = 75;
            [Tunable, TunableComment("Whether to apply accessories to a specific age (Baby,Toddler,Child,Teen,YoungAdult,Adult,Elder)")]
            public static CASAgeGenderFlags[] kAccessoriesByAge = new CASAgeGenderFlags[] { CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

            [Tunable, TunableComment("The minimum number of random makeup to add to males which have none")]
            public static int kMinRandomMaleMakeup = 0;
            [Tunable, TunableComment("The maximum number of random makeup to add to males which have none")]
            public static int kMaxRandomMaleMakeup = 0;
            [Tunable, TunableComment("The minimum number of random makeup to add to females which have none")]
            public static int kMinRandomFemaleMakeup = 1;
            [Tunable, TunableComment("The maximum number of random makeup to add to females which have none")]
            public static int kMaxRandomFemaleMakeup = 2;
            [Tunable, TunableComment("Percent chance of adding makeup to a male sim")]
            public static int kRandomMaleMakeupChance = 75;
            [Tunable, TunableComment("Percent chance of adding makeup to a female sim")]
            public static int kRandomFemaleMakeupChance = 75;
            [Tunable, TunableComment("Whether to apply makeup to a specific age (Baby,Toddler,Child,Teen,YoungAdult,Adult,Elder)")]
            public static CASAgeGenderFlags[] kMakeupByAge = new CASAgeGenderFlags[] { CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

            [Tunable, TunableComment("The minimum number of random body hair to add to males which have none")]
            public static int kMinRandomMaleBodyHair = 3;
            [Tunable, TunableComment("The maximum number of random body hair to add to males which have none")]
            public static int kMaxRandomMaleBodyHair = 6;
            [Tunable, TunableComment("The minimum number of random body hair to add to females which have none")]
            public static int kMinRandomFemaleBodyHair = 0;
            [Tunable, TunableComment("The maximum number of random body hair to add to females which have none")]
            public static int kMaxRandomFemaleBodyHair = 0;
            [Tunable, TunableComment("Percent chance of adding body hair to a male sim")]
            public static int kRandomMaleBodyHairChance = 100;
            [Tunable, TunableComment("Percent chance of adding body hair to a female sim")]
            public static int kRandomFemaleBodyHairChance = 50;
            [Tunable, TunableComment("Whether to apply body hair to a specific age")]
            public static CASAgeGenderFlags[] kBodyHairByAge = new CASAgeGenderFlags[] { CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

            [Tunable, TunableComment("Categories that should not be allowed to roll hats (Everyday,Formalwear,Athletic,Swimwear,Sleepwear)")]
            public static OutfitCategories[] kInvalidHatCategories = new OutfitCategories[] { OutfitCategories.Sleepwear, OutfitCategories.Swimwear };

            [Tunable, TunableComment("Whether the mod is allowed to use body hair not marked ValidForRandom when replacing invalids")]
            public static bool kIgnoreValidForRandomBodyHair = true;

            private bool mNightlyRandomChangeOutfit = kNightlyRandomChangeOutfit;
            private float mNightlyChangeOutfitHour = kNightlyChangeOutfitHour;
            public bool mRotationAffectActive = kRotationAffectActive;
            public bool mRotationAffectInactive = kRotationAffectInactive;
            private float mChanceOfSwitch = kChanceOfSwitch;
            public bool mRotateMartialOutfit = kRotateMartialOutfit;
            public bool mCheckOutfits = kCheckOutfits;
            public bool mNotifyOnCheckOutfits = kNotifyOnCheckOutfits;
            public int mRandomBeardChance = kRandomBeardChance;
            public bool mSwitchOnOutside = kSwitchOnOutside;
            public bool mIgnoreValidForRandomHair = kIgnoreValidForRandomHair;
            public bool mIgnoreValidForRandomClothing = kIgnoreValidForRandomClothing;
            public bool mIgnoreValidForRandomAccessories = kIgnoreValidForRandomAccessories;
            public bool mIgnoreValidForRandomMakeup = kIgnoreValidForRandomMakeup;
            public bool mSwitchFormal = kSwitchFormal;
            public bool mTransitionAffectActive = kTransitionAffectActive;
            public bool mCheckAffectActive = kCheckAffectActive;
            public bool mSwitchSleepwear = kSwitchSleepwear;
            public bool mSameHairForAllCategories = kSameHairForAllCategories;
            public bool mSameMakeupForAllCategories = kSameMakeupForAllCategories;
            public bool mAllowHats = kAllowHats;
            public bool mMartialArtsAccessories = kMartialArtsAccessories;
            public bool mRotateOccult = kRotateOccult;
            public bool mAllowRollAgeupHair = kAllowRollAgeupHair;
            public bool mAllowRollAgeupMakeup = kAllowRollAgeupMakeup;
            public bool mIgnoreBodyHairGender = kIgnoreBodyHairGender;
            public bool mAllowPetAccessories = kAllowPetAccessories;
            public bool mSimMenuVisibility = kSimMenuVisibility;

            public bool mUseMasterControllerBlackList = kUseMasterControllerBlackList;

            public GenderMinMax mRandomAccessories = new GenderMinMax(kAccessoriesByAge, kMinRandomMaleAccessories, kMaxRandomMaleAccessories, kRandomMaleAccessoriesChance, kMinRandomFemaleAccessories, kMaxRandomFemaleAccessories, kRandomFemaleAccessoriesChance);
            public GenderMinMax mRandomMakeup = new GenderMinMax(kMakeupByAge, kMinRandomMaleMakeup, kMaxRandomMaleMakeup, kRandomMaleMakeupChance, kMinRandomFemaleMakeup, kMaxRandomFemaleMakeup, kRandomFemaleMakeupChance);
            public GenderMinMax mRandomBodyHair = new GenderMinMax(kBodyHairByAge, kMinRandomMaleBodyHair, kMaxRandomMaleBodyHair, kRandomMaleBodyHairChance, kMinRandomFemaleBodyHair, kMaxRandomFemaleBodyHair, kRandomFemaleBodyHairChance);

            public int mVacationOutfitIndex = kVacationOutfitIndex;
            public bool mAllowRollAgeupBodyHair = kAllowRollAgeupBodyHair;
            public bool mAllowRollAgeupAccessories = kAllowRollAgeupAccessories;

            public bool mAllowRollAgeupHairGeneral = kAllowRollAgeupHairGeneral;
            public bool mAllowRollAgeupBodyHairGeneral = kAllowRollAgeupBodyHairGeneral;
            public bool mFullAdultReroll = kFullAdultReroll;
            public bool mAllowStrayPetAccessories = kAllowStrayPetAccessories;
            public bool mInheritBodyHair = kInheritBodyHair;

            public List<ProductVersion> mInvalidProductVersionGeneral = new List<ProductVersion>(kInvalidProductVersionGeneral);
            public List<ProductVersion> mInvalidProductVersionMakeup = new List<ProductVersion>(kInvalidProductVersionMakeup);
            public List<ProductVersion> mInvalidProductVersionAccessories = new List<ProductVersion>(kInvalidProductVersionAccessories);
            public List<ProductVersion> mInvalidProductVersionBodyHair = new List<ProductVersion>(kInvalidProductVersionBodyHair);
            public List<ProductVersion> mInvalidProductVersionHair = new List<ProductVersion>(kInvalidProductVersionHair);

            public List<OutfitCategories> mInvalidHatCategories = new List<OutfitCategories>(kInvalidHatCategories);

            Dictionary<ulong, CheckOutfitTask.ProcessOptions> mTested = new Dictionary<ulong, CheckOutfitTask.ProcessOptions>();

            Dictionary<ulong, int> mOutfitCount = new Dictionary<ulong, int>();

            protected bool mDebugging = Common.kDebugging;

            public bool mIgnoreValidForRandomBodyHair = kIgnoreValidForRandomBodyHair;

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

            public bool IsProtected(SimDescription sim)
            {
                bool existed = false;
                return ((GetTested(sim, out existed) & CheckOutfitTask.ProcessOptions.Protected) != CheckOutfitTask.ProcessOptions.None);
            }

            public void SetProtected(SimDescription sim, bool set)
            {
                if (set)
                {
                    AddTested(sim, CheckOutfitTask.ProcessOptions.Protected);
                }
                else
                {
                    RemoveTested(sim, CheckOutfitTask.ProcessOptions.Protected);
                }
            }

            public CheckOutfitTask.ProcessOptions GetTested(SimDescription sim, out bool existed)
            {
                CheckOutfitTask.ProcessOptions result;
                if (!mTested.TryGetValue(sim.SimDescriptionId, out result)) 
                {
                    existed = false;
                    return CheckOutfitTask.ProcessOptions.None;
                }
                else
                {
                    existed = true;
                    return result;
                }
            }

            public void AddTested(SimDescription sim, CheckOutfitTask.ProcessOptions test)
            {
                bool existed;
                CheckOutfitTask.ProcessOptions tested = GetTested(sim, out existed);

                mTested[sim.SimDescriptionId] = tested | test;
            }

            public void RemoveTested(SimDescription sim, CheckOutfitTask.ProcessOptions test)
            {
                bool existed;
                CheckOutfitTask.ProcessOptions tested = GetTested(sim, out existed);

                tested &= ~test;

                mTested[sim.SimDescriptionId] = tested;
            }

            public int GetOutfitCount(SimDescription sim)
            {
                int count;
                if (!mOutfitCount.TryGetValue(sim.SimDescriptionId, out count))
                {
                    return 0;
                }

                return count;
            }

            public void SetOutfitCount(SimDescription sim, int count)
            {
                mOutfitCount[sim.SimDescriptionId] = count;
            }

            public void CleanupSims()
            {
                Dictionary<ulong, List<SimDescription>> allSims = SimListing.AllSims<SimDescription>(null, true);

                List<ulong> remove = new List<ulong>();
                foreach (ulong sim in mTested.Keys)
                {
                    if (allSims.ContainsKey(sim)) continue;

                    remove.Add(sim);
                }

                foreach (ulong sim in remove)
                {
                    mTested.Remove(sim);
                }

                remove.Clear();
                foreach (ulong sim in mOutfitCount.Keys)
                {
                    if (allSims.ContainsKey(sim)) continue;

                    remove.Add(sim);
                }

                foreach (ulong sim in remove)
                {
                    mOutfitCount.Remove(sim);
                }
            }

            public bool ShouldCheck(SimDescription sim)
            {
                if (!mCheckOutfits) return false;

                if (IsProtected(sim)) return false;

                if ((sim.Household == Household.ActiveHousehold) && (!mCheckAffectActive))
                {
                    return false;
                }

                if ((sim.LotHome == null) && (!SimTypes.IsServiceOrRole(sim, false)) && (!SimTypes.IsTourist(sim)))
                {
                    return false;
                }

                return true;
            }

            public bool AllowAccessories(SimDescription sim)
            {
                if (!sim.IsHuman)
                {
                    if (!Dresser.Settings.mAllowPetAccessories)
                    {
                        return false;
                    }
                    else if (!Dresser.Settings.mAllowStrayPetAccessories)
                    {
                        if (SimTypes.IsService(sim))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            public bool NightlyRandomChangeOutfit
            {
                get
                {
                    return mNightlyRandomChangeOutfit;
                }
                set
                {
                    mNightlyRandomChangeOutfit = value;

                    RestartAlarm();
                }
            }

            public float NightlyChangeOutfitHour
            {
                get
                {
                    return mNightlyChangeOutfitHour;
                }
                set
                {
                    mNightlyChangeOutfitHour = value;

                    if (mNightlyChangeOutfitHour < 0)
                    {
                        mNightlyChangeOutfitHour = 0;
                    }
                    else if (mNightlyChangeOutfitHour >= 24)
                    {
                        mNightlyChangeOutfitHour = 0;
                    }

                    RestartAlarm();
                }
            }

            public float ChanceOfSwitch
            {
                get 
                { 
                    return mChanceOfSwitch; 
                }
                set
                {
                    mChanceOfSwitch = value;

                    if (mChanceOfSwitch < 0)
                    {
                        mChanceOfSwitch = 0;
                    }
                    else if (mChanceOfSwitch > 100)
                    {
                        mChanceOfSwitch = 100;
                    }
                }
            }
        }

        [Persistable]
        public class GenderMinMax
        {
            public List<CASAgeGenderFlags> mByAge;

            public int mMinMale;
            public int mMaxMale;
            public int mMaleChance;

            public int mMinFemale;
            public int mMaxFemale;
            public int mFemaleChance;

            public GenderMinMax()
            { }
            public GenderMinMax(CASAgeGenderFlags[] byAge, int minMale, int maxMale, int maleChance, int minFemale, int maxFemale, int femaleChance)
            {
                mByAge = new List<CASAgeGenderFlags>(byAge);

                mMinMale = minMale;
                mMaxMale = maxMale;
                mMaleChance = maleChance;

                mMinFemale = minFemale;
                mMaxFemale = maxFemale;
                mFemaleChance = maleChance;
            }

            public int GetRandomAmount(bool female, CASAgeGenderFlags age)
            {
                if (!mByAge.Contains(age)) return 0;

                if (female)
                {
                    if (!RandomUtil.RandomChance(mFemaleChance)) return 0;

                    return RandomUtil.GetInt(mMinFemale, mMaxFemale);
                }
                else
                {
                    if (!RandomUtil.RandomChance(mMaleChance)) return 0;

                    return RandomUtil.GetInt(mMinMale, mMaxMale);
                }
            }
        }
    }
}
