using NRaas.CareerSpace;
using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Tones
{
    [Persistable]
    public class CareerToneEx : CareerTone
    {
        List<SkillRate> mSkills = new List<SkillRate>();

        List<MotiveRate> mMotives = new List<MotiveRate>();

        List<MetricRate> mMetrics = new List<MetricRate>();

        float mPerformanceModifier = 0f;

        ProductVersion mVersion = ProductVersion.BaseGame;

        string mBranch = null;

        int mMinLevel = 0;
        int mMaxLevel = 100;

        bool mOnlySkill = true;
        bool mOnlyMetric = false;
        bool mMustHaveVisibleSkill = true;

        public CareerToneEx()
        { }

        public override void BeginCareerTone(InteractionInstance interactionInstance)
        {
            try
            {
                foreach (SkillRate skill in mSkills)
                {
                    Skill element = Career.OwnerDescription.SkillManager.AddElement(skill.mSkill);
                    if (element != null)
                    {
                        Career.OwnerDescription.SkillManager.StartSkillGain(skill.mSkill, skill.mRate);
                    }
                }

                foreach (MotiveRate motive in mMotives)
                {
                    motive.mStored = interactionInstance.AddMotiveDelta(motive.mKind, motive.mRate);
                }

                foreach (MetricRate rate in mMetrics)
                {
                    rate.mStartTime = SimClock.ElapsedTime(TimeUnit.Hours);
                }

                Career.PerformanceBonusPerHour += mPerformanceModifier;
            }
            catch (Exception e)
            {
                Common.Exception(Career.OwnerDescription, e);
            }
        }

        public override void EndCareerTone(InteractionInstance interactionInstance)
        {
            try
            {
                foreach (SkillRate skill in mSkills)
                {
                    Career.OwnerDescription.SkillManager.StopSkillGain(skill.mSkill);
                }

                foreach (MotiveRate motive in mMotives)
                {
                    interactionInstance.RemoveMotiveDelta(motive.mStored);
                }

                Career.PerformanceBonusPerHour -= mPerformanceModifier;
            }
            catch (Exception e)
            {
                Common.Exception(Career.OwnerDescription, e);
            }
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            try
            {
                foreach (MetricRate rate in mMetrics)
                {
                    float hoursElapsed = SimClock.ElapsedTime(TimeUnit.Hours) - rate.mStartTime;

                    if (hoursElapsed < rate.mHoursUntilChange) continue;

                    rate.mStartTime = SimClock.ElapsedTime(TimeUnit.Hours);

                    switch(rate.mMetric)
                    {
                        case MetricRate.MetricType.ConcertsPerformed:
                            Music music = OmniCareer.Career<Music>(Career);
                            if (music != null)
                            {
                                music.ConcertsPerformed += rate.mRate;
                            }
                            break;
                        case MetricRate.MetricType.Journals:
                            OmniCareer journals = Career as OmniCareer;
                            if (journals != null)
                            {
                                journals.FinishedJournal();
                            }
                            break;
                        case MetricRate.MetricType.Recruitment:
                            OmniCareer recruit = Career as OmniCareer;
                            if (recruit != null)
                            {
                                recruit.AddToRecruits();
                            }
                            break;
                        case MetricRate.MetricType.Reports:
                            LawEnforcement law = OmniCareer.Career<LawEnforcement>(Career);
                            if (law != null)
                            {
                                law.ReportsCompltetedToday += rate.mRate;
                            }
                            break;
                        case MetricRate.MetricType.StoriesAndReviews:
                            Journalism journalism = OmniCareer.Career<Journalism>(Career);
                            if (journalism != null)
                            {
                                journalism.StoriesWrittenToday += rate.mRate;
                            }
                            break;
                        case MetricRate.MetricType.WinLossRecord:
                            ProSports proSports = OmniCareer.Career<ProSports>(Career);
                            if (proSports != null)
                            {
                                if (rate.mRate > 0)
                                {
                                    proSports.mWinRecord += rate.mRate;
                                    proSports.mTotalWins += rate.mRate;
                                }
                                else
                                {
                                    proSports.mLossRecord -= rate.mRate;
                                    proSports.mTotalLoss -= rate.mRate;
                                }
                            }
                            break;
                        case MetricRate.MetricType.MeetingsHeld:
                            Business business = OmniCareer.Career<Business>(Career);
                            if (business != null)
                            {
                                business.MeetingsHeldToday += rate.mRate;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Career.OwnerDescription, e);
            }
        }

        public virtual void Set(XmlDbRow row)
        {
            try
            {
                mPerformanceModifier = row.GetFloat("PerformanceModifier", mPerformanceModifier);

                if (!string.IsNullOrEmpty(row.GetString("OnlySkill")))
                {
                    mOnlySkill = row.GetBool("OnlySkill");
                }

                if (!string.IsNullOrEmpty(row.GetString("MustHaveVisibleSkill")))
                {
                    mMustHaveVisibleSkill = row.GetBool("MustHaveVisibleSkill");
                }

                row.TryGetEnum<ProductVersion>("ProductVersoin", out mVersion, ProductVersion.BaseGame);

                mBranch = row.GetString("CareerBranch");

                mMinLevel = row.GetInt("CareerMinLevel", mMinLevel);

                mMaxLevel = row.GetInt("CareerMaxLevel", mMaxLevel);

                int count = row.GetInt("SkillCount");
                for (int i = 1; i <= count; i++)
                {
                    float rate = 1f;
                    if (row.Exists("SkillRate" + i))
                    {
                        rate = row.GetFloat("SkillRate" + i);
                    }

                    string skillName = row.GetString("Skill" + i);
                    if (skillName == null) continue;

                    SkillNames skill = SkillManager.sSkillEnumValues.ParseEnumValue(skillName);
                    if (skill == SkillNames.None) continue;

                    mSkills.Add(new SkillRate (skill, rate));
                }

                count = row.GetInt("MotiveCount");
                for (int i = 1; i <= count; i++)
                {
                    float rate = 1f;
                    if (row.Exists("MotiveRate" + i))
                    {
                        rate = row.GetFloat("MotiveRate" + i);
                    }

                    CommodityKind motive = CommodityKind.None;

                    string motiveName = row.GetString("Motive" + i);
                    if (motiveName == null) continue;

                    ParserFunctions.TryParseEnum<CommodityKind>(motiveName, out motive, CommodityKind.None);

                    if (motive == CommodityKind.None) continue;

                    mMotives.Add(new MotiveRate(motive, rate));
                }

                count = row.GetInt("MetricCount");
                for (int i = 1; i <= count; i++)
                {
                    int rate = row.GetInt("MetricRate" + i, 1);

                    MetricRate.MetricType metric = row.GetEnum<MetricRate.MetricType>("MetricType" + i, MetricRate.MetricType.None);

                    int hoursUntilChange = row.GetInt("MetricHoursUntilChange" + i, 1);

                    mMetrics.Add(new MetricRate(metric, rate, hoursUntilChange));
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name(), exception);
            }
        }

        public override bool ShouldAddTone(Career career)
        {
            try
            {
                if (!GameUtils.IsInstalled(mVersion))
                {
                    return false;
                }

                if (!base.ShouldAddTone(career))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(mBranch))
                {
                    if (career.CurLevelBranchName != mBranch)
                    {
                        return false;
                    }
                }

                if (mMinLevel > career.CareerLevel)
                {
                    return false;
                }

                if (mMaxLevel < career.CareerLevel)
                {
                    return false;
                }

                if (career.OwnerDescription == null)
                {
                    return false;
                }

                if (career.OwnerDescription.SkillManager == null)
                {
                    return false;
                }

                if (mMetrics.Count > 0)
                {
                    foreach (MetricRate rate in mMetrics)
                    {
                        switch (rate.mMetric)
                        {
                            case MetricRate.MetricType.Reports:
                                LawEnforcement law = OmniCareer.Career<LawEnforcement>(career);
                                if (law != null)
                                {
                                    if (!OmniCareer.HasMetric<LawEnforcement.MetricReportsWritten>(career)) return false;
                                }
                                break;
                            case MetricRate.MetricType.StoriesAndReviews:
                                Journalism journalism = OmniCareer.Career<Journalism>(career);
                                if (journalism != null)
                                {
                                    if (!OmniCareer.HasMetric<Journalism.MetricStoriesAndReviews>(career)) return false;
                                }
                                break;
                        }
                    }
                }

                if (mSkills.Count > 0)
                {
                    bool needed = false;

                    foreach (SkillRate skill in mSkills)
                    {
                        if (mMustHaveVisibleSkill)
                        {
                            if (!career.OwnerDescription.SkillManager.IsPlayerVisible(skill.mSkill))
                            {
                                return false;
                            }
                        }

                        if (skill.mSkill == SkillNames.Athletic)
                        {
                            if ((career.OwnerDescription.CreatedSim != null) &&
                                (career.OwnerDescription.CreatedSim.BuffManager != null) &&
                                (career.OwnerDescription.CreatedSim.BuffManager.HasElement(BuffNames.Fatigued)))
                            {
                                return false;
                            }
                        }

                        if (mOnlyMetric)
                        {
                            bool found = false;
                            foreach (PerfMetric metric in career.CurLevel.Metrics)
                            {
                                MetricSkillX skillMetric = metric as MetricSkillX;
                                if (skillMetric == null) continue;

                                if (skillMetric.SkillGuid == skill.mSkill)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                return false;
                            }
                        }

                        Skill simSkill = career.OwnerDescription.SkillManager.GetSkill<Skill>(skill.mSkill);
                        if ((simSkill == null) || (simSkill.SkillLevel < simSkill.MaxSkillLevel))
                        {
                            needed = true;
                        }
                    }

                    if ((!needed) && (mOnlySkill))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Common.Exception(career.OwnerDescription, exception);
            }

            return false;
        }

        public override bool Test(InteractionInstance ii, out StringDelegate reason)
        {
            reason = null;

            return ShouldAddTone(Career);
        }

        public void GetSkills (List<SkillNames> skills)
        {
            if (mSkills == null) return;

            foreach (SkillRate skill in mSkills)
            {
                skills.Add(skill.mSkill);
            }
        }

        [Persistable]
        public class SkillRate
        {
            public SkillNames mSkill = SkillNames.None;
            public float mRate = 0;

            public SkillRate()
            { }
            public SkillRate(SkillNames skill, float rate)
            {
                mSkill = skill;
                mRate = rate;
            }
        }

        [Persistable]
        public class MotiveRate
        {
            public CommodityKind mKind = CommodityKind.None;
            public float mRate = 0;

            public MotiveDelta mStored = null;

            public MotiveRate()
            { }
            public MotiveRate(CommodityKind kind, float rate)
            {
                mKind = kind;
                mRate = rate;
            }
        }

        [Persistable]
        public class MetricRate
        {
            public MetricType mMetric = MetricType.None;
            public int mRate = 0;
            public int mHoursUntilChange = 1;

            public float mStartTime = 0;

            public MetricRate()
            { }
            public MetricRate(MetricType metric, int rate, int hoursUntilChange)
            {
                mMetric = metric;
                mRate = rate;
                mHoursUntilChange = hoursUntilChange;
            }

            public enum MetricType
            {
                None = 0,
                ConcertsPerformed,
                Journals,
                Recruitment,
                Reports,
                StoriesAndReviews,
                WinLossRecord,
                MeetingsHeld
            }
        }
    }
}
