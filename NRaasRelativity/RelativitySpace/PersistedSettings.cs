using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RelativitySpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Overriding speed")]
        public static int kSpeedOverride = 0;

        [Tunable, TunableComment("Whether to display the change of speed notice")]
        public static bool kShowNotice = true;

        [Tunable, TunableComment("Whether to apply a skill gain multiple based on life-span")]
        public static bool kApplyLifeSpanSkillFactor = false;

        [Tunable, TunableComment("Number of cycles between updates")]
        public static int kCyclesPerUpdate = 0;

        [Tunable, TunableComment("Skill Gain Factors.  Format <Skill>:<Factor>,<Skill>:<Factor>")]
        public static string[] kSkillGainFactors = new string[] { "None:1" };

        [Tunable, TunableComment("Motive Factors.  OccultType and/or AgeSpecies are optional.  Format <OccultType>:<AgeSpecies>:<Motive>:<Factor>,<OccultType>:<AgeSpecies>:<Motive>:<Factor>")]
        public static string[] kMotiveDeltaFactors = new string[] { "Fun:1", "Social:1", "Hunger:1", "VampireThirst:1", "Hygiene:1", "Bladder:1", "Energy:1", "HorseExercise:1", "HorseThirst:1", "CatScratch:1", "DogDestruction:1" };

        [Tunable, TunableComment("Motive Decay Factors.  OccultType and/or AgeSpecies are optional.  Format <OccultType>:<AgeSpecies>:<Motive>:<Factor>,<OccultType>:<AgeSpecies>:<Motive>:<Factor>")]
        public static string[] kMotiveDecayDeltaFactors = new string[] { "Fun:1", "Social:1", "Hunger:1", "VampireThirst:1", "Hygiene:1", "Bladder:1", "HorseExercise:1", "HorseThirst:1", "CatScratch:1", "DogDestruction:1" };

        [Tunable, TunableComment("Comma separated list of motives that are relative")]
        public static CommodityKind[] kRelativeMotives = new CommodityKind[] { CommodityKind.Fun, CommodityKind.Social, CommodityKind.Hunger, CommodityKind.VampireThirst, CommodityKind.Hygiene, CommodityKind.Bladder };

        [Tunable, TunableComment("Comma separated list of skills that are absolute, and not subject to relativity")]
        public static SkillNames[] kAbsoluteSkills = new SkillNames[0];

        [Tunable, TunableComment("Whether to pause the game when the active sim runs out of interactions")]
        public static bool kPauseOnCompletion = false;

        [Tunable, TunableComment("The speed to set the mod when running in non-Normal game-speed")]
        public static int kSwitchSpeedOnFast = 0;

        [Tunable, TunableComment("The minimum number of sims allowed in the active household before the ActiveSimFactorReduction kicks in")]
        public static int kActiveSimFactorMinimum = 5;

        [Tunable, TunableComment("The percent reduction in speed for each active sim over the ActiveSimFactorMinimum")]
        public static int kActiveSimFactorReduction = 0;

        [Tunable, TunableComment("Whether to only count humans when checking whether to fast forward while sleeping")]
        public static bool kSkipOnHumanSleep = false;

        [Tunable, TunableComment("Whether to alter tuning based on the relative speed of the mod")]
        public static bool kPerformRelativeTuningAlterations = true;

        public static float sRelativeFactor = 1f;

        [Persistable(false)]
        public bool mDisable = false;

        public Dictionary<SkillNames, float> mSkillGains = new Dictionary<SkillNames,float>();

        Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> mMotiveDeltasV2 = new Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>>();
        Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> mMotiveDecayDeltasV2 = new Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>>();

        public Dictionary<CommodityKind, bool> mRelativeMotives = new Dictionary<CommodityKind, bool>();

        public Dictionary<SkillNames, bool> mRelativeSkills = new Dictionary<SkillNames, bool>();

        public int mSpeedOverride = kSpeedOverride;

        public bool mShowNotice = kShowNotice;

        protected bool mDebugging = false;

        public int mCyclesPerUpdate = kCyclesPerUpdate;

        public bool mApplyLifeSpanSkillFactor = kApplyLifeSpanSkillFactor;

        List<SpeedInterval> mIntervals = null;

        static Dictionary<WorldName, Dictionary<DaysOfTheWeek, Dictionary<int, int>>> sSpeeds = null;

        public bool mPauseOnCompletion = kPauseOnCompletion;

        public int mSwitchSpeedOnFast = kSwitchSpeedOnFast;

        public int mActiveSimFactorMinimum = kActiveSimFactorMinimum;

        public int mActiveSimFactorReduction = kActiveSimFactorReduction;

        public bool mSkipOnHumanSleep = kSkipOnHumanSleep;

        public bool mSkippingSleep = false;
        public bool mPausingOnCompletion = false;

        public bool mPerformRelativeTuningAlterations = kPerformRelativeTuningAlterations;

        public PersistedSettings()
        {
            foreach (CommodityKind kind in kRelativeMotives)
            {
                mRelativeMotives.Add(kind, true);
            }

            List<SkillNames> absoluteSkills = new List<SkillNames>(kAbsoluteSkills);

            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                if (absoluteSkills.Contains(skill.Guid)) continue;

                if (mRelativeSkills.ContainsKey(skill.Guid)) continue;

                mRelativeSkills.Add(skill.Guid, true);
            }

            foreach (string entry in kSkillGainFactors)
            {
                string[] entries = entry.Split(':');

                SkillNames skill = SkillNames.None;
                if (entries.Length >= 1)
                {
                    if (entries[0] != "All")
                    {
                        skill = SkillManager.sSkillEnumValues.ParseEnumValue(entries[0]);
                        if (skill == SkillNames.None) continue;
                    }
                }

                float value = 1f;
                if (entries.Length == 2)
                {
                    if (!float.TryParse(entries[1], out value)) continue;
                }

                mSkillGains.Remove(skill);
                mSkillGains.Add(skill, value);
            }

            ParseMotiveDeltas("kMotiveDeltaFactors", mMotiveDeltasV2, kMotiveDeltaFactors);
            ParseMotiveDeltas("kMotiveDecayDeltaFactors", mMotiveDecayDeltasV2, kMotiveDecayDeltaFactors);
        }

        public void EnsureBaseMotives()
        {
            foreach (CommodityKind kind in TuningAlterations.sCommodities)
            {
                MotiveKey key = new MotiveKey(kind);

                bool exactMatch;
                GetMotiveFactor(key, false, out exactMatch);
                if (!exactMatch)
                {
                    SetMotiveFactor(key, 1f);
                }

                GetMotiveDecayFactor(key, out exactMatch);
                if (!exactMatch)
                {
                    SetMotiveDecayFactor(key, 1f);
                }
            }
        }

        protected static void ParseMotiveDeltas(string logName, Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> dstDeltas, string[] srcDeltas)
        {
            foreach (string entry in srcDeltas)
            {
                string[] entries = entry.Split(':');

                CASAgeGenderFlags ageSpecies = CASAgeGenderFlags.None;

                OccultTypes occult = OccultTypes.None;

                int length = entries.Length;

                CommodityKind motive = CommodityKind.None;

                string motiveStr = null;
                string valueStr = null;
                string ageSpeciesStr = null;
                string occultStr = null;
                switch (length)
                {
                    case 1:
                        motiveStr = entries[0];
                        break;
                    case 2:
                        motiveStr = entries[0];
                        valueStr = entries[1];
                        break;
                    case 3:
                        ageSpeciesStr = entries[0];
                        motiveStr = entries[1];
                        valueStr = entries[2];
                        break;
                    case 4:
                        occultStr = entries[0];
                        ageSpeciesStr = entries[1];
                        motiveStr = entries[2];
                        valueStr = entries[3];
                        break;
                }

                if (string.IsNullOrEmpty(motiveStr))
                {
                    BooterLogger.AddError(logName + ": Missing CommodityKind");
                    continue;
                }

                if (!ParserFunctions.TryParseEnum<CommodityKind>(motiveStr, out motive, CommodityKind.None))
                {
                    BooterLogger.AddError(logName + ": Unknown CommodityKind '" + motiveStr + "'");
                    continue;
                }

                float value = 1f;

                if (!string.IsNullOrEmpty(valueStr))
                {
                    if (!float.TryParse(valueStr, out value))
                    {
                        BooterLogger.AddError(logName + ": Unknown Value '" + valueStr + "'");
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(occultStr))
                {
                    if (!ParserFunctions.TryParseEnum<OccultTypes>(occultStr, out occult, OccultTypes.None))
                    {
                        BooterLogger.AddError(logName + ": Unknown OccultTypes '" + occultStr + "'");
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(ageSpeciesStr))
                {
                    if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(ageSpeciesStr, out ageSpecies, CASAgeGenderFlags.None))
                    {
                        BooterLogger.AddError(logName + ": Unknown AgeSpecies '" + ageSpeciesStr + "'");
                        continue;
                    }
                }

                Dictionary<OccultTypes, Dictionary<CommodityKind, float>> occults;
                if (!dstDeltas.TryGetValue(ageSpecies, out occults))
                {
                    occults = new Dictionary<OccultTypes, Dictionary<CommodityKind, float>>();
                    dstDeltas.Add(ageSpecies, occults);
                }

                Dictionary<CommodityKind, float> commodities;
                if (!occults.TryGetValue(occult, out commodities))
                {
                    commodities = new Dictionary<CommodityKind, float>();
                    occults.Add(occult, commodities);
                }

                commodities.Remove(motive);
                commodities.Add(motive, value);
            }
        }

        public static void ResetSpeeds()
        {
            sSpeeds = null;
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

        public Dictionary<WorldName, Dictionary<DaysOfTheWeek, Dictionary<int, int>>> Speeds
        {
            get
            {
                if (sSpeeds == null)
                {
                    sSpeeds = new Dictionary<WorldName, Dictionary<DaysOfTheWeek, Dictionary<int, int>>>();

                    foreach (SpeedInterval interval in Intervals)
                    {
                        Dictionary<DaysOfTheWeek, Dictionary<int, int>> dayLookup;
                        if (!sSpeeds.TryGetValue(interval.mWorld, out dayLookup))
                        {
                            dayLookup = new Dictionary<DaysOfTheWeek, Dictionary<int, int>>();
                            sSpeeds.Add(interval.mWorld, dayLookup);
                        }

                        foreach (DaysOfTheWeek day in interval.mDays)
                        {
                            Dictionary<int, int> hours;
                            if (!dayLookup.TryGetValue(day, out hours))
                            {
                                hours = new Dictionary<int, int>();
                                dayLookup.Add(day, hours);
                            }

                            int hour = interval.mStartHour;
                            while (hour != interval.mEndHour)
                            {
                                if (hours.ContainsKey(hour))
                                {
                                    hours.Remove(hour);
                                }

                                hours.Add(hour, interval.mSpeed);

                                hour++;
                                if (hour > 24)
                                {
                                    hour = 0;
                                }

                                if (hour == interval.mStartHour) break;
                            }
                        }
                    }
                }

                return sSpeeds;
            }
        }

        public float GetSkillFactor(SkillNames skill)
        {
            return (GetConstantSkillFactor(skill) * GetDynamicSkillFactor(skill));
        }

        public float GetConstantSkillFactor(SkillNames skill)
        {
            float multiple;
            if (!mSkillGains.TryGetValue(skill, out multiple))
            {
                if (!mSkillGains.TryGetValue(SkillNames.None, out multiple))
                {
                    multiple = 1f;
                }
            }

            return multiple;
        }

        public float GetDynamicSkillFactor(SkillNames skill)
        {
            float multiple = 1f;

            bool value;
            if (!mRelativeSkills.TryGetValue(skill, out value))
            {
                if (!mRelativeSkills.TryGetValue(SkillNames.None, out value))
                {
                    value = true;
                }
            }

            if (value)
            {
                multiple *= sRelativeFactor;
            }

            if (mApplyLifeSpanSkillFactor)
            {
                multiple *= (90f / LifeSpan.GetHumanAgeSpanLength());
            }

            if (multiple < 0)
            {
                multiple = 0;
            }

            return multiple;
        }

        public List<MotiveKeyValue> GetMotiveDecayList()
        {
            return MotiveKeyValue.ConvertToList(mMotiveDecayDeltasV2);
        }

        public void ClearMotiveDecayFactors()
        {
            mMotiveDecayDeltasV2.Clear();
        }

        public float GetMotiveDecayFactor(MotiveKey key)
        {
            bool exactMatch;
            return GetMotiveDecayFactor(key, out exactMatch);
        }
        public float GetMotiveDecayFactor(MotiveKey key, out bool exactMatch)
        {
            float multiple = key.GetMotiveFactor(mMotiveDecayDeltasV2, out exactMatch);

            /*
            if ((relative) && (mRelativeDecayMotives.ContainsKey(kind)))
            {
                multiple *= sRelativeFactor;
            }
            */
            if (multiple < 0)
            {
                multiple = 0;
            }

            return multiple;
        }

        public void SetMotiveDecayFactor(MotiveKey key, float factor)
        {
            key.SetMotiveFactor(mMotiveDecayDeltasV2, factor);
        }

        public List<MotiveKeyValue> GetMotiveDeltaList()
        {
            return MotiveKeyValue.ConvertToList(mMotiveDeltasV2);
        }

        public void ClearMotiveFactors()
        {
            mMotiveDeltasV2.Clear();
        }

        public float GetMotiveFactor(MotiveKey key, bool relative)
        {
            bool exactMatch;
            return GetMotiveFactor(key, relative, out exactMatch);
        }
        public float GetMotiveFactor(MotiveKey key, bool relative, out bool exactMatch)
        {
            float multiple = key.GetMotiveFactor(mMotiveDeltasV2, out exactMatch);

            if ((relative) && (mRelativeMotives.ContainsKey(key.mKind)))
            {
                multiple *= sRelativeFactor;
            }

            if (multiple < 0)
            {
                multiple = 0;
            }

            return multiple;
        }

        public void SetMotiveFactor(MotiveKey key, float factor)
        {
            key.SetMotiveFactor(mMotiveDeltasV2, factor);
        }

        public void Add(SpeedInterval interval)
        {
            mIntervals.Add(interval);

            mIntervals.Sort(new Comparison<SpeedInterval>(SpeedInterval.OnSort));

            ResetSpeeds();
        }

        public void Remove(SpeedInterval interval)
        {
            mIntervals.Remove(interval);

            ResetSpeeds();
        }

        public int GetSpeed(DateAndTime time, int overrideSpeed)
        {
            return GetSpeed(time.DayOfWeek, (int)time.Hour, overrideSpeed);
        }
        public int GetSpeed(DaysOfTheWeek day, int hour, int overrideSpeed)
        {
            if (mDisable)
            {
                return Relativity.sOneMinute;
            }

            if (overrideSpeed > 0)
            {
                return overrideSpeed;
            }

            int speed = 0;

            if (mSpeedOverride != 0)
            {
                if (mSpeedOverride < 0)
                {
                    speed = 0;
                }
                else
                {
                    speed = mSpeedOverride;
                }
            }
            else
            {
                Dictionary<DaysOfTheWeek, Dictionary<int, int>> days;

                if ((!Speeds.TryGetValue(GameUtils.GetCurrentWorld(), out days)) && (!Speeds.TryGetValue(WorldName.Undefined, out days)))
                {
                    speed = Relativity.sOneMinute;
                }
                else
                {
                    Dictionary<int, int> hours;
                    if (!days.TryGetValue(day, out hours))
                    {
                        speed = Relativity.sOneMinute;
                    }
                    else if (!hours.TryGetValue(hour, out speed))
                    {
                        speed = Relativity.sOneMinute;
                    }
                }
            }

            int activeSize = Households.NumSims(Household.ActiveHousehold) - mActiveSimFactorMinimum;
            if (activeSize > 0)
            {
                speed -= (int)(speed * activeSize * (mActiveSimFactorReduction / 100f));
                if (speed <= 0)
                {
                    speed = 1;
                }
            }

            return speed;
        }

        public void ParseIntervals()
        {
            mIntervals = new List<SpeedInterval>();

            BooterHelper.DataBootFile speedFile = new BooterHelper.DataBootFile("NRaas.RelativityTuning", "NRaas.RelativityTuning", false);
            if (!speedFile.IsValid) return;

            BooterHelper.DataBootTable table = new BooterHelper.DataBootTable(speedFile, "Speeds");
            if (!table.IsValid) return;

            table.Load(Perform);

            mIntervals.Sort(new Comparison<SpeedInterval>(SpeedInterval.OnSort));
        }

        public void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            WorldName world;
            row.TryGetEnum<WorldName>("World", out world, WorldName.Undefined);

            List<DaysOfTheWeek> days = ParserFunctions.ParseDayList(row.GetString("Days"));
            if ((days == null) || (days.Count == 0))
            {
                days = ParserFunctions.ParseDayList("MTWRFSU");
            }

            int startHour = row.GetInt("StartHour");
            int endHour = row.GetInt("EndHour");

            if (startHour > endHour) return;

            int speed = row.GetInt("Speed");
            if (speed <= 0)
            {
                speed = Relativity.sOneMinute;
            }

            mIntervals.Add(new SpeedInterval(world, days, startHour, endHour, speed));
        }

        public List<SpeedInterval> Intervals
        {
            get
            {
                if (mIntervals == null)
                {
                    ParseIntervals();
                }

                return mIntervals;
            }
        }
    }
}
