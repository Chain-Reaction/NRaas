using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("The maximum amount of funds allowed to transfer during a Shakedown")]
        protected static int kMaxShakedown = 500;

        [Tunable, TunableComment("The amount of relationship lost during a shakedown")]
        protected static int kShakedownRelationChange = -50;

        [Tunable, TunableComment("The level for busker at which a sim can perform concerts")]
        protected static int kBuskerLevelToGetPaidForConcerts = 7;

        [Tunable, TunableComment("Allow the mod to create a broken object during the 'Find Broken' interaction")]
        protected static bool kRepairAllowToBreak = true;

        [Tunable, TunableComment("How much cash per homemaker mark should be paid daily")]
        protected static int kHomemakerPayPerMark = 1;

        [Tunable, TunableComment("The Homemaker level at which a sim is immune to Stir Crazy")]
        protected static int kHomemakerLevelStirCrazy = 2;
        [Tunable, TunableComment("The Homemaker level at which purchases are discounted")]
        protected static int kHomemakerLevelDiscount = 3;
        [Tunable, TunableComment("The Homemaker level at which lifetime rewards are provided")]
        protected static int kHomemakerLevelLifetimeRewards = 5;

        [Tunable, TunableComment("The Homemaker discount level as a percent")]
        protected static int kHomemakerDiscountRate = 20;

        [Tunable, TunableComment("The amount of performance gained per Home Schooling homework submission")]
        protected static int kPerformancePerHomework = 20;

        [Tunable, TunableComment("Length in Sim days of 1 term of homeworld University")]
        protected static int kHomeworldUniversityTermLength = 7;

        public int mMaxShakedown = kMaxShakedown;
        public int mShakedownRelationChange = kShakedownRelationChange;

        public int mBuskerLevelToGetPaidForConcerts = kBuskerLevelToGetPaidForConcerts;

        public bool mRepairAllowToBreak = kRepairAllowToBreak;

        public int mHomemakerPayPerMark = kHomemakerPayPerMark;
        public int mHomemakerLevelStirCrazy = kHomemakerLevelStirCrazy;
        public int mHomemakerLevelDiscount = kHomemakerLevelDiscount;
        public int mHomemakerLevelLifetimeRewards = kHomemakerLevelLifetimeRewards;
        public int mHomemakerDiscountRate = kHomemakerDiscountRate;

        public int mPerformancePerHomework = kPerformancePerHomework;

        public int mHomeworldUniversityTermLength = kHomeworldUniversityTermLength;

        public Dictionary<OccupationNames, CareerSettings> mCareerSettings = new Dictionary<OccupationNames, CareerSettings>();

        protected bool mDebugging = Common.kDebugging;

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

        public void UpdateCareerSettings(CareerSettings settings)
        {
            if (mCareerSettings.ContainsKey(settings.mName))
            {
                mCareerSettings[settings.mName] = settings;
            }
            else
            {
                mCareerSettings.Add(settings.mName, settings);
            }

            SetCareerData(settings);
        }

        public void SetCareerData(CareerSettings settings)
        {
            if (settings != null)
            {
                if (settings.mName == OccupationNames.Any)
                {
                    foreach (Career career in CareerManager.CareerList)
                    {
                        career.SharedData.MinCoworkers = settings.mMinCoworkers;
                        career.SharedData.MaxCoworkers = settings.mMaxCoworkers;
                    }
                }
                else
                {
                    Career career = CareerManager.GetStaticCareer(settings.mName);
                    if (career != null)
                    {
                        career.SharedData.MinCoworkers = settings.mMinCoworkers;
                        career.SharedData.MaxCoworkers = settings.mMaxCoworkers;

                        foreach (string branch in career.CareerLevels.Keys)
                        {
                            foreach (KeyValuePair<int, Sims3.Gameplay.Careers.CareerLevel> levelData in career.CareerLevels[branch])
                            {
                                CareerLevelSettings levelSettings = settings.GetSettingsForLevel(branch, levelData.Key, false);

                                if (levelSettings != null)
                                {
                                    levelData.Value.PayPerHourBase = levelSettings.mPayPerHourBase;
                                    levelData.Value.CarpoolType = levelSettings.mCarpoolType;
                                }
                            }
                        }
                    }
                }

                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.Occupation != null)
                    {
                        sim.Occupation.RescheduleCarpool();
                    }
                }
            }
        }

        public CareerSettings GetCareerSettings(OccupationNames name, bool create)
        {
            CareerSettings result;
            if (mCareerSettings.TryGetValue(name, out result))
            {
                return result;
            }
            else
            {
                if (create)
                {
                    CareerSettings settings = new CareerSettings(name);
                    CareerSettings cloned = settings.mDefaults.Clone();
                    cloned.SetDefaults();
                    return cloned;
                }
            }

            return null;
        }

        [Persistable]
        public class CareerSettings
        {
            public OccupationNames mName;

            public int mMinCoworkers = 0;
            public int mMaxCoworkers = 0;

            public CareerSettings mDefaults = null;

            Dictionary<string, Dictionary<int, CareerLevelSettings>> mLevelSettings = new Dictionary<string, Dictionary<int, CareerLevelSettings>>();

            public CareerSettings()
            {
            }
            public CareerSettings(OccupationNames name)
            {
                mName = name;                
                SetDefaults();
            }

            public void SetDefaults()
            {
                Career career = CareerManager.GetStaticCareer(mName);
                if (career != null)
                {
                    CareerSettings settings = new CareerSettings();
                    settings.mName = mName;

                    settings.mMinCoworkers = career.SharedData.MinCoworkers;
                    settings.mMaxCoworkers = career.SharedData.MaxCoworkers;

                    mDefaults = settings;
                }
            }

            public void RevertToDefault()
            {
                if (mDefaults != null)
                {
                    Career career = CareerManager.GetStaticCareer(mName);
                    if (career != null)
                    {
                        career.SharedData.MinCoworkers = mDefaults.mMinCoworkers;
                        career.SharedData.MaxCoworkers = mDefaults.mMaxCoworkers;
                    }

                    foreach (KeyValuePair<string, Dictionary<int, CareerLevelSettings>> settings in mLevelSettings)
                    {
                        foreach (KeyValuePair<int, CareerLevelSettings> settings2 in settings.Value)
                        {
                            settings2.Value.RevertToDefaults(mName);
                        }
                    }
                }
            }

            public CareerLevelSettings GetSettingsForLevel(string branch, int level, bool create)
            {
                if (!mLevelSettings.ContainsKey(branch))
                {
                    mLevelSettings.Add(branch, new Dictionary<int, CareerLevelSettings>());
                }

                if (!mLevelSettings[branch].ContainsKey(level))
                {
                    if (create)
                    {
                        CareerLevelSettings settings = new CareerLevelSettings(mName, branch, level);
                        CareerLevelSettings cloned = settings.mDefaults.Clone();
                        cloned.SetDefaults(mName);
                        mLevelSettings[branch].Add(level, cloned);
                        return cloned;
                    }
                    else
                    {
                        return null;
                    }
                }

                return mLevelSettings[branch][level];
            }

            public CareerSettings Clone()
            {
                return this.MemberwiseClone() as CareerSettings;
            }
        }

        [Persistable]
        public class CareerLevelSettings
        {
            public string mBranch;
            public int mLevel;

            public float mPayPerHourBase;
            public CarNpcManager.NpcCars mCarpoolType;

            public CareerLevelSettings mDefaults = null;

            public CareerLevelSettings()
            {
            }
            public CareerLevelSettings(OccupationNames career, string branchName, int level)
            {
                mBranch = branchName;
                mLevel = level;
                SetDefaults(career);
            }

            public void SetDefaults(OccupationNames name)
            {
                Career career = CareerManager.GetStaticCareer(name);
                if (career != null)
                {
                    foreach (string branch in career.CareerLevels.Keys)
                    {
                        if (branch != mBranch) continue;

                        foreach (KeyValuePair<int, CareerLevel> level in career.CareerLevels[branch])
                        {
                            if (level.Key == mLevel)
                            {
                                CareerLevelSettings settings = new CareerLevelSettings();
                                settings.mBranch = mBranch;
                                settings.mLevel = mLevel;
                                settings.mPayPerHourBase = level.Value.PayPerHourBase;
                                settings.mCarpoolType = level.Value.CarpoolType;

                                mDefaults = settings;
                            }
                        }
                    }
                }
            }

            public void RevertToDefaults(OccupationNames name)
            {
                Career career = CareerManager.GetStaticCareer(name);
                if (career != null && mDefaults != null)
                {
                    foreach (string branch in career.CareerLevels.Keys)
                    {
                        if (branch != mBranch) continue;

                        foreach (KeyValuePair<int, CareerLevel> level in career.CareerLevels[branch])
                        {
                            if (level.Key == mLevel)
                            {
                                level.Value.PayPerHourBase = mDefaults.mPayPerHourBase;
                                level.Value.CarpoolType = mDefaults.mCarpoolType;
                            }
                        }
                    }
                }
            }

            public CareerLevelSettings Clone()
            {
                return this.MemberwiseClone() as CareerLevelSettings;
            }
        }
    }
}
