using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Skills
{
    [Persistable]
    public abstract class CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE> : Skill
        where T : CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE>
        where MAJOR_STAT_TYPE : CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE>.MajorStat
        where MINOR_STAT_TYPE : CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE>.MinorStat
        where OPP_TYPE : CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE>.CommonOpportunity
    {
        static SkillNames sStaticGuid = SkillNames.None;

        [Persistable(false)]
        List<ITrackedStat> mTrackedStats;

        [Persistable(false)]
        List<MINOR_STAT_TYPE> mMinorStats;

        [Persistable(false)]
        List<ILifetimeOpportunity> mLifetimeOpportunities;

        [Persistable(false)]
        List<OPP_TYPE> mMinorOpportunities;

        [Persistable(false)]
        List<OPP_TYPE> mAllOpportunities;

        Dictionary<string, bool> mIsNew = new Dictionary<string, bool>();

        int mCashMade = 0;

        public CommonSkill()
        {
            mSkillGuid = sStaticGuid;
        }
        public CommonSkill(SkillNames guid)
            : base(guid)
        {
            if (sStaticGuid == SkillNames.None)
            {
                // All GUID must be uint in order to bypass an error in SkillManager:ImportContent()
                sStaticGuid = (SkillNames)(uint)guid;
            }

            mSkillGuid = sStaticGuid;
        }

        public static SkillNames StaticGuid
        {
            get { return sStaticGuid; }
        }

        public int GetCashMade()
        {
            return mCashMade;
        }

        public override bool IsVisibleInUI()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                if (e.StackTrace.Contains("CanTutorSkill")) return false;
            }

            try
            {
                return base.IsVisibleInUI();
            }
            catch (Exception e)
            {
                Common.Exception(SkillOwner, e);
                return false;
            }
        }

        public override uint GetSkillHash()
        {
            return (uint)mSkillGuid;
        }

        public override List<ILifetimeOpportunity> LifetimeOpportunities
        {
            get { return mLifetimeOpportunities; }
        }

        public List<OPP_TYPE> AllOpportunities
        {
            get { return mAllOpportunities; }
        }

        public override void MoneyEarned(int money)
        {
            try
            {
                mCashMade += money;

                UpdateXpForEarningMoney(money);

                if (SkillOwner.CareerManager != null)
                {
                    SkillOwner.CareerManager.UpdateCareerUI();
                }

                base.MoneyEarned(money);
            }
            catch (Exception e)
            {
                Common.Exception(SkillOwner, e);
            }
        }

        public override List<ITrackedStat> TrackedStats
        {
            get { return mTrackedStats; }
        }

        protected virtual List<MINOR_STAT_TYPE> MinorStats
        {
            get { return mMinorStats; }
        }

        public override List<Sims3.UI.ObjectPicker.HeaderInfo> HeaderInfo(int page)
        {
            List<ObjectPicker.HeaderInfo> list = new List<ObjectPicker.HeaderInfo>();
            list.Add(new ObjectPicker.HeaderInfo("Gameplay/Skills/" + LocalizationKey + ":CompletedTitle", "Gameplay/Skills/" + LocalizationKey + ":CompletedTitle", 200));
            list.Add(new ObjectPicker.HeaderInfo("Gameplay/Skills/" + LocalizationKey + ":CountTitle", "Gameplay/Skills/" + LocalizationKey + ":CountTitle", 50));
            list.Add(new ObjectPicker.HeaderInfo("Gameplay/Skills/" + LocalizationKey + ":DescriptionTitle", "Gameplay/Skills/" + LocalizationKey + ":DescriptionTitle"));
            return list;
        }

        public override List<ObjectPicker.TabInfo> SecondaryTabs
        {
            get
            {
                List<ObjectPicker.TabInfo> tabs = new List<ObjectPicker.TabInfo>();

                List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

                foreach (OPP_TYPE opp in mMinorOpportunities)
                {
                    List<ObjectPicker.ColumnInfo> columns = new List<ObjectPicker.ColumnInfo>();

                    if (opp.Completed)
                    {
                        columns.Add(new ObjectPicker.ImageAndTextColumn("skill_journal_full_star_r2", opp.Title));
                        //columns.Add(new ObjectPicker.ThumbAndTextColumn(new ThumbnailKey(IconKey, ThumbnailSize.Medium), opp.Title));
                    }
                    else
                    {
                        columns.Add(new ObjectPicker.ImageAndTextColumn("", opp.Title));
                        //columns.Add(new ObjectPicker.TextColumn(opp.Title));
                    }

                    columns.Add(new ObjectPicker.TextColumn(EAText.GetNumberString(opp.CurrentValue)));
                    columns.Add(new ObjectPicker.TextColumn(opp.RewardDescription));

                    rowInfo.Add(new ObjectPicker.RowInfo(opp, columns));
                }

                if (rowInfo.Count > 0)
                {
                    tabs.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("MinorOpportunities"), rowInfo));
                }

                return tabs;
            }
        }

        public static ObjectPicker.RowInfo CreateRow(MINOR_STAT_TYPE stat)
        {
            return CreateRow(stat.Title, stat.Count, stat.Description);
        }
        public static ObjectPicker.RowInfo CreateRow(string title, int count, string description)
        {
            List<ObjectPicker.ColumnInfo> columns = new List<ObjectPicker.ColumnInfo>();

            columns.Add(new ObjectPicker.TextColumn(title));
            columns.Add(new ObjectPicker.TextColumn(EAText.GetNumberString(count)));
            columns.Add(new ObjectPicker.TextColumn(description));

            return new ObjectPicker.RowInfo(null, columns);
        }

        public override bool OnLoadFixup()
        {
            // Guids are exported as uint, even though the enum is ulong, meaning the IDs specified for custom skills
            //   do not persist properly, set the value now
            mSkillGuid = StaticGuid;

            return base.OnLoadFixup();
        }

        public override bool ExportContent(IPropertyStreamWriter writer)
        {
            try
            {
                uint skillHash = GetSkillHash() - 1;

                writer.WriteInt32(skillHash, mCashMade);
                skillHash--;

                string[] newKeys = new string[mIsNew.Count];
                bool[] newValues = new bool[mIsNew.Count];

                int index = 0;
                foreach (KeyValuePair<string, bool> isNew in mIsNew)
                {
                    newKeys[index] = isNew.Key;
                    newValues[index] = isNew.Value;
                    index++;
                }

                writer.WriteString(skillHash, newKeys);
                skillHash--;

                writer.WriteBool(skillHash, newValues);
                skillHash--;

                return base.ExportContent(writer);
            }
            catch (Exception e)
            {
                Common.Exception("ExportContent", e);
                return false;
            }
        }

        public override bool ImportContent(IPropertyStreamReader reader)
        {
            try
            {
                uint skillHash = GetSkillHash() - 1;

                reader.ReadInt32(skillHash, out mCashMade, 0);
                skillHash--;

                string[] newKeys;
                bool[] newValues;

                bool found = reader.ReadString(skillHash, out newKeys);
                skillHash--;

                found = found & reader.ReadBool(skillHash, out newValues);
                skillHash--;

                mIsNew.Clear();

                if (found)
                {
                    for (int i = 0; i < newKeys.Length; i++)
                    {
                        mIsNew.Add(newKeys[i], newValues[i]);
                    }
                }

                return base.ImportContent(reader);
            }
            catch (Exception e)
            {
                Common.Exception("ImportContent", e);
                return false;
            }
        }

        public override void CreateSkillJournalInfo()
        {
            try
            {
                mTrackedStats = new List<ITrackedStat>();

                List<MAJOR_STAT_TYPE> majorStats = Common.DerivativeSearch.Find<MAJOR_STAT_TYPE>();
                majorStats.Sort(new Comparison<MAJOR_STAT_TYPE>(MajorStat.OnSortByOrder));

                foreach (MAJOR_STAT_TYPE stat in majorStats)
                {
                    if ((SkillOwner != null) && (!stat.Allow(SkillOwner))) continue;

                    MAJOR_STAT_TYPE newStat = stat.Clone();
                    if (newStat == null) continue;

                    newStat.Skill = this as T;

                    mTrackedStats.Add(newStat);
                }

                if (mTrackedStats.Count == 0)
                {
                    mTrackedStats = null;
                }

                mMinorStats = new List<MINOR_STAT_TYPE>();

                List<MINOR_STAT_TYPE> minorStats = Common.DerivativeSearch.Find<MINOR_STAT_TYPE>();

                foreach (MINOR_STAT_TYPE stat in minorStats)
                {
                    if ((SkillOwner != null) && (!stat.Allow(SkillOwner))) continue;

                    MINOR_STAT_TYPE newStat = stat.Clone();
                    if (newStat == null) continue;

                    newStat.Skill = this as T;

                    mMinorStats.Add(newStat);
                }

                mLifetimeOpportunities = new List<ILifetimeOpportunity>();

                mMinorOpportunities = new List<OPP_TYPE>();

                mAllOpportunities = new List<OPP_TYPE>();

                List<OPP_TYPE> opportunities = Common.DerivativeSearch.Find<OPP_TYPE>();

                foreach (OPP_TYPE opportunity in opportunities)
                {
                    if ((SkillOwner != null) && (!opportunity.Allow(SkillOwner))) continue;

                    OPP_TYPE newOpp = opportunity.Clone();
                    if (newOpp == null) continue;

                    newOpp.Skill = this as T;

                    mAllOpportunities.Add(newOpp);

                    if (opportunity.IsMajor)
                    {
                        mLifetimeOpportunities.Add(newOpp);
                    }
                    else
                    {
                        mMinorOpportunities.Add(newOpp);
                    }
                }

                mMinorOpportunities.Sort(new Comparison<OPP_TYPE>(CommonOpportunity.OnSortByTitle));
            }
            catch (Exception e)
            {
                Common.Exception(SkillOwner, e);
            }
        }

        public override void MergeTravelData(Skill paramSkill)
        {
            base.MergeTravelData(paramSkill);

            CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE> skill = paramSkill as CommonSkill<T, MAJOR_STAT_TYPE, MINOR_STAT_TYPE, OPP_TYPE>;

            mCashMade = skill.mCashMade;
        }

        protected abstract string LocalizationKey
        {
            get;
        }

        protected string LocalizeString(string name)
        {
            return LocalizeString(name, new object[0]);
        }
        protected new string LocalizeString(string name, params object[] parameters)
        {
            return Common.LocalizeEAString(false, "Gameplay/Skills/" + LocalizationKey + ":" + name, parameters);
        }

        public static uint GetSkillLevel(SimDescription sim)
        {
            if (StaticGuid == SkillNames.None) return 0;

            return Skill.GetSkillLevel(sim, StaticGuid);
        }
        public static uint GetSkillLevel(Sim sim)
        {
            if (StaticGuid == SkillNames.None) return 0;

            return Skill.GetSkillLevel(sim, StaticGuid);
        }

        public static T EnsureSkill(Sim sim)
        {
            return EnsureSkill(sim.SimDescription);
        }
        public static T EnsureSkill(SimDescription sim)
        {
            if (StaticGuid == SkillNames.None) return null;

            Skill skill = sim.SkillManager.GetElement(StaticGuid);
            if (skill == null)
            {
                skill = sim.SkillManager.AddElement(StaticGuid);
            }

            return (skill as T);
        }

        public abstract class CommonStat : ITrackedStat
        {
            protected T mSkill;

            public T Skill
            {
                set { mSkill = value; }
            }

            protected abstract string LocalizationKey
            {
                get;
            }

            public abstract int Count
            {
                get;
            }

            public abstract string Description
            {
                get;
            }
        }

        public abstract class MajorStat : CommonStat
        {
            public abstract int Order
            {
                get;
            }

            public static int OnSortByOrder(MAJOR_STAT_TYPE left, MAJOR_STAT_TYPE right)
            {
                if (left.Order < right.Order) return -1;

                if (left.Order > right.Order) return 1;

                return 0;
            }

            public override string Description
            {
                get { return mSkill.LocalizeString(LocalizationKey, new object[] { Count }); }
            }

            public abstract bool Allow(SimDescription sim);

            public abstract MAJOR_STAT_TYPE Clone();
        }

        public abstract class MinorStat : CommonStat
        {
            public string Title
            {
                get { return mSkill.LocalizeString(LocalizationKey + "Title"); }
            }

            public override string Description
            {
                get { return mSkill.LocalizeString(LocalizationKey + "Description"); }
            }

            public abstract bool Allow(SimDescription sim);

            public abstract MINOR_STAT_TYPE Clone();
        }

        public abstract class CommonOpportunity : ILifetimeOpportunity
        {
            protected T mSkill;

            public CommonOpportunity()
            { }

            public T Skill
            {
                set { mSkill = value; }
            }

            public virtual bool IsMajor
            {
                get { return false; }
            }

            protected abstract string LocalizationKey
            {
                get;
            }

            public abstract int MinValue
            {
                get;
            }

            public abstract int CurrentValue
            {
                get;
            }

            public virtual bool Completed
            {
                get { return (CurrentValue >= MinValue); }
            }

            protected SimDescription SkillOwner
            {
                get { return mSkill.SkillOwner; }
            }

            public bool IsNew
            {
                get 
                {
                    if (!mSkill.mIsNew.ContainsKey(LocalizationKey)) return false;

                    return mSkill.mIsNew[LocalizationKey]; 
                }
                set 
                {
                    mSkill.mIsNew[LocalizationKey] = value; 
                }
            }

            public virtual string AchievedDescription
            {
                get { return mSkill.LocalizeString("Achieved" + LocalizationKey, new object[] { mSkill.SkillOwner }); }
            }

            public virtual string RewardDescription
            {
                get
                {
                    if (Completed)
                    {
                        return AchievedDescription;
                    }
                    else
                    {
                        string result = mSkill.LocalizeString("Description" + LocalizationKey, new object[] { MinValue, mSkill.SkillOwner });

                        if (Common.kDebugging)
                        {
                            result += " (Current: " + CurrentValue + ")";
                        }

                        return result;
                    }
                }
            }

            public string Title
            {
                get { return mSkill.LocalizeString("Title" + LocalizationKey, new object[] { mSkill.SkillOwner }); }
            }

            public abstract bool Allow(SimDescription sim);

            public abstract OPP_TYPE Clone();

            public static int OnSortByTitle(OPP_TYPE left, OPP_TYPE right)
            {
                try
                {
                    return left.Title.CompareTo(right.Title);
                }
                catch (Exception e)
                {
                    Common.Exception("Left:" + Common.NewLine + left.GetType() + Common.NewLine + left + Common.NewLine + "Right:" + Common.NewLine + right.GetType() + Common.NewLine + right, e);
                    return 0;
                }
            }
        }
    }
}
