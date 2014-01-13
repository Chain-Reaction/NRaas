using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class SimPersonality : StoryProgressionObject, IHasPersonality, IUpdateManager
    {
        string mName;

        List<OptionItem> mParsedOptions = new List<OptionItem>();

        List<IUpdateManagerOption> mUpdaters = new List<IUpdateManagerOption>();

        bool mLeaderless = false;

        SimScenarioFilter mCandidateScoring = null;
        SimScenarioFilter mLeaderRetention = null;
        SimScenarioFilter mMemberRetention = null;

        bool mFriendlyLeadership;
        bool mFriendlyMembership;

        bool mInstalled;

        ProductVersion mProductVersion = ProductVersion.BaseGame;

        MeOption mMe = null;
        UserSelectedMeOption mUserSelected = null;

        ProgressionOption mProgression = null;
        IncreasedChancePerCycleOption mIncreasedChancePerCycle = null;

        ChanceOfEventOption mChanceOfEvent = null;

        CandidateRequirementReductionOption mCandidateReduction = null;

        ManagerPersonality.LawfulnessType mLawfulness = ManagerPersonality.LawfulnessType.Neutral;

        List<string> mOpposingClans = null;

        bool mDefaultFemaleLocalization = false;

        string mDeathStory = null;
        string mLeaderResignStory = null;
        string mMemberResignStory = null;

        bool mFindReplacement = false;
        string mLeaderReplaceStory = null;

        // ------ //

        Dictionary<SimDescription, bool> mClan = new Dictionary<SimDescription, bool>();

        Common.MethodStore mLeaderCustomAccept = null;
        Common.MethodStore mMemberCustomAccept = null;

        Common.MethodStore mLeaderCustomResign = null;
        Common.MethodStore mMemberCustomResign = null;

        public SimPersonality()
            : base(null)
        { }

        public static int OnSortByName(SimPersonality left, SimPersonality right)
        {
            return left.GetLocalizedName().CompareTo(right.GetLocalizedName());
        }

        public override string ToString()
        {
            string text = "-- SimPersonality --";

            text += Common.NewLine + "Name=" + mName;
            text += Common.NewLine + "Leaderless=" + mLeaderless;
            text += Common.NewLine + "FindReplacement=" + mFindReplacement;
            text += Common.NewLine + "ProductVersion=" + mProductVersion;
            text += Common.NewLine + "DefaultFemaleLocalization=" + mDefaultFemaleLocalization;
            text += Common.NewLine + "DeathStory=" + mDeathStory;
            text += Common.NewLine + "LeaderResignStory=" + mLeaderResignStory;
            text += Common.NewLine + "LeaderReplaceStory=" + mLeaderReplaceStory;
            text += Common.NewLine + "MemberResignStory=" + mMemberResignStory;
            text += Common.NewLine + "Lawfulness=" + mLawfulness;

            foreach (string clan in mOpposingClans)
            {
                text += Common.NewLine + "OpposingClan=" + clan;
            }

            text += Common.NewLine + "CandidateScoring" + Common.NewLine + mCandidateScoring;
            text += Common.NewLine + "LeaderCustomAccept=" + mLeaderCustomAccept;
            text += Common.NewLine + "LeaderRetention" + Common.NewLine + mLeaderRetention;
            text += Common.NewLine + "LeaderCustomResign=" + mLeaderCustomResign;
            text += Common.NewLine + "MemberCustomAccept=" + mMemberCustomAccept;
            text += Common.NewLine + "MemberRetention" + Common.NewLine + mMemberRetention;
            text += Common.NewLine + "MemberCustomResign=" + mMemberCustomResign;

            text += Common.NewLine + base.ToString();

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return mName;
        }

        public bool IsLeaderless
        {
            get { return mLeaderless; }
        }

        public bool IsInstalled
        {
            get { return mInstalled; }
        }

        public bool IsFriendlyLeadership
        {
            get { return mFriendlyLeadership; }
        }

        public bool IsFriendlyMembership
        {
            get { return mFriendlyMembership; }
        }

        public SimScenarioFilter CandidateScoring
        {
            get { return mCandidateScoring; }
        }

        public SimScenarioFilter MemberRetention
        {
            get { return mMemberRetention; }
        }

        public SimPersonality Personality
        {
            get { return this; }
        }

        public int ChanceOfEvent
        {
            get
            {
                if (mChanceOfEvent == null) return 0;

                return mChanceOfEvent.Value;
            }
            set
            {
                if (mChanceOfEvent == null) return;

                mChanceOfEvent.SetValue (value);
            }
        }

        public override Common.DebugLevel DebuggingLevel
        {
            get
            {
                return Personalities.DebuggingLevel;
            }
        }

        public override bool DebuggingEnabled
        {
            get
            {
                return Personalities.DebuggingEnabled;
            }
        }

        public override bool ProgressionEnabled
        {
            get
            {
                if (!Personalities.ProgressionEnabled) return false;

                if (mProgression == null) return true;

                return mProgression.Value;
            }
        }

        public ManagerPersonality.LawfulnessType Lawfulness
        {
            get
            {
                return mLawfulness;
            }
        }

        public override bool IsFemaleLocalization()
        {
            if (Me != null)
            {
                return Me.IsFemale;
            }
            else
            {
                return mDefaultFemaleLocalization;
            }
        }

        public void SetAlertLevel(AlertLevel level)
        {
            mAlertLevel.SetValue(level, true);
        }

        public void AddUpdater(IUpdateManagerOption updater)
        {
            mUpdaters.Add(updater);
        }

        public bool IsFriendly(SimDescription simA, SimDescription simB)
        {
            if (!IsMember(simA)) return false;

            if (!IsMember(simB)) return false;

            if ((Me == simA) || (Me == simB))
            {
                return IsFriendlyLeadership;
            }
            else
            {
                return IsFriendlyMembership;
            }
        }

        public List<SimDescription> GetAlliesFor(SimDescription sim)
        {
            if (Me == sim)
            {
                if (IsFriendlyLeadership)
                {
                    return GetClanMembers(false);
                }
            }
            else if (IsMember(sim))
            {
                if (IsFriendlyMembership)
                {
                    List<SimDescription> allies = GetClanMembers(IsFriendlyLeadership);
                    allies.Remove(sim);

                    return allies;
                }
                else if (IsFriendlyLeadership)
                {
                    return MeAsList;
                }
            }

            return new List<SimDescription>();
        }
        public bool IsMember(SimDescription sim)
        {
            if (Me == sim) return true;

            return mClan.ContainsKey(sim);
        }

        public bool CanAdd(Common.IStatGenerator stats, SimDescription sim, bool steal)
        {
            return AddToClan(stats, sim, steal, true);
        }
        
        public bool AddToClan(Common.IStatGenerator stats, SimDescription sim, bool steal)
        {
            return AddToClan(stats, sim, steal, false);
        }
        private bool AddToClan(Common.IStatGenerator stats, SimDescription sim, bool steal, bool test)
        {
            SimData data = GetData(sim);

            if (Me == sim) return true;

            if (!IsLeaderless)
            {
                if (Me == null)
                {
                    stats.IncStat(UnlocalizedName + ": No Leader");
                    return false;
                }
            }

            if (!TestMemberRetention(sim))
            {
                stats.IncStat(UnlocalizedName + ": Retention Fail");
                return false;
            }

            if (!data.IsClan(this))
            {
                List<SimPersonality> opposingClans = new List<SimPersonality>();

                List<SimPersonality> clans = Personalities.GetClanMembership(sim, true);
                foreach (SimPersonality clan in clans)
                {
                    if (IsOpposing(clan))
                    {
                        opposingClans.Add(clan);
                    }
                }

                if (opposingClans.Count > 0)
                {
                    if (!steal) return false;

                    foreach (SimPersonality clan in opposingClans)
                    {
                        if (clan.Me == sim)
                        {
                            stats.IncStat(UnlocalizedName + ": Opposing Clan Leader " + clan.UnlocalizedName);
                            return false;
                        }
                    }

                    if (!test)
                    {
                        foreach (SimPersonality clan in opposingClans)
                        {
                            data.RemoveClan(clan);

                            stats.IncStat(UnlocalizedName + ": Opposing Clan Dropped " + clan.UnlocalizedName);
                        }
                    }
                }

                if (!test)
                {
                    data.AddClan(this);

                    stats.IncStat(UnlocalizedName + ": New Member");
                }
            }

            if ((!test) && (!mClan.ContainsKey(sim)))
            {
                mMemberCustomAccept.Invoke<bool>(new object[] { this, sim });

                mClan.Add(sim, true);

                GetData(sim).InvalidateCache();
            }
            return true;
        }

        public bool RemoveFromClan(SimDescription sim)
        {
            bool success = false;

            if (Me == sim)
            {
                mLeaderCustomResign.Invoke<bool>(new object[] { this, sim });

                mMe.SimDescription = null;
                success = true;
            }
            else
            {
                mMemberCustomResign.Invoke<bool>(new object[] { this, sim });
            }

            mClan.Remove(sim);

            if (GetData(sim).RemoveClan(this))
            {
                success = true;

                GetData(sim).InvalidateCache();
            }

            return success;
        }

        public List<SimDescription> GetClanMembers (bool includeLeader)
        {
            List<SimDescription> members = new List<SimDescription>(mClan.Keys);

            if ((includeLeader) && (Me != null))
            {
                members.Add(Me);
            }

            return members;
        }

        public bool IsOpposing(SimPersonality clan)
        {
            if (clan == null) return false;

            switch (Lawfulness)
            {
                case ManagerPersonality.LawfulnessType.Lawful:
                    if (clan.Lawfulness == ManagerPersonality.LawfulnessType.Unlawful) return true;
                    break;
                case ManagerPersonality.LawfulnessType.Unlawful:
                    if (clan.Lawfulness == ManagerPersonality.LawfulnessType.Lawful) return true;
                    break;
            }

            if (mOpposingClans != null)
            {
                if (mOpposingClans.Contains(clan.UnlocalizedName.ToLower())) return true;
            }

            if (clan.mOpposingClans != null)
            {
                if (clan.mOpposingClans.Contains(UnlocalizedName.ToLower())) return true;
            }

            return false;
        }

        public void SetIncreasedChangePerCycle(int value)
        {
            if (mIncreasedChancePerCycle == null) return;

            mIncreasedChancePerCycle.SetValue(value);
        }

        protected int IncreasedChancePerCycle()
        {
            if (mIncreasedChancePerCycle == null)
            {
                return 1;
            }

            int chance = mIncreasedChancePerCycle.Value;
            if (chance < 0)
            {
                chance = 0;
            }

            return chance;
        }

        protected override T GetInternalOption<T>(string name)
        {
            if (Main != null)
            {
                return base.GetInternalOption<T>(name);
            }
            else
            {
                foreach (OptionItem option in mParsedOptions)
                {
                    if (option.GetStoreKey() == name)
                    {
                        return option as T;
                    }
                }

                return null;
            }
        }

        public override bool AddOption(OptionItem option)
        {
            if (Main == null)
            {
                mParsedOptions.Add(option);
                return true;
            }
            else
            {
                return base.AddOption(option);
            }
        }

        protected static bool ToOpposingClans(string value, out string result)
        {
            if (value == null)
            {
                result = null;
                return false;
            }
            else
            {
                result = value.Trim().ToLower();
                return true;
            }
        }

        public bool Parse(XmlDbRow myRow, XmlDbTable options)
        {
            mName = myRow.GetString("Name");

            BooterLogger.AddTrace("Parsing " + mName);

            mFriendlyLeadership = myRow.GetBool("FriendlyLeadership");
            mFriendlyMembership = myRow.GetBool("FriendlyMembership");

            mFindReplacement = myRow.GetBool("FindReplacement");

            if (myRow.Exists("Lawfulness"))
            {
                mLawfulness = myRow.GetEnum<ManagerPersonality.LawfulnessType>("Lawfulness", ManagerPersonality.LawfulnessType.Undefined);
                if (mLawfulness == ManagerPersonality.LawfulnessType.Undefined)
                {
                    BooterLogger.AddError(UnlocalizedName + " Unknown Lawfulness: " + myRow.GetString("Lawfulness"));

                    mLawfulness = ManagerPersonality.LawfulnessType.Neutral;
                }
            }

            mDefaultFemaleLocalization = myRow.GetBool("DefaultFemaleLocalization");

            mDeathStory = myRow.GetString("DeathStory");
            mLeaderResignStory = myRow.GetString("LeaderResignStory");
            mMemberResignStory = myRow.GetString("MemberResignStory");
            mLeaderReplaceStory = myRow.GetString("LeaderReplaceStory");

            mLeaderCustomAccept = new Common.MethodStore(myRow.GetString("LeaderCustomAccept"), new Type[] { typeof(SimPersonality), typeof(SimDescription) });
            mMemberCustomAccept = new Common.MethodStore(myRow.GetString("MemberCustomAccept"), new Type[] { typeof(SimPersonality), typeof(SimDescription) });

            mLeaderCustomResign = new Common.MethodStore(myRow.GetString("LeaderCustomResign"), new Type[] { typeof(SimPersonality), typeof(SimDescription) });
            mMemberCustomResign = new Common.MethodStore(myRow.GetString("MemberCustomResign"), new Type[] { typeof(SimPersonality), typeof(SimDescription) });

            string error = null;

            mLeaderless = myRow.GetBool("Leaderless");
            if (!mLeaderless)
            {
                mOpposingClans = StringToList<string>.StaticConvert(myRow.GetString("OpposingClans"), ToOpposingClans);
            }

            if (!ParserFunctions.TryParseEnum<ProductVersion>(myRow.GetString("ProductVersion"), out mProductVersion, ProductVersion.BaseGame))
            {
                BooterLogger.AddError(UnlocalizedName + " ProductVersion unknown");
                return false;
            }

            List<IAutoPersonalityOption> allAutoOptions = Common.DerivativeSearch.Find<IAutoPersonalityOption>();

            foreach (IAutoPersonalityOption autoOption in allAutoOptions)
            {
                ICommonOptionItem option = autoOption.Clone();
                if (option == null)
                {
                    BooterLogger.AddError(UnlocalizedName + " AutoOption " + autoOption.GetType().Name + " failed (1)");
                    continue;
                }

                IAutoPersonalityOption personalityOption = option as IAutoPersonalityOption;
                if (personalityOption == null)
                {
                    BooterLogger.AddError(UnlocalizedName + " AutoOption " + autoOption.GetType().Name + " failed (2)");
                    continue;
                }

                error = null;
                if (!personalityOption.Parse(myRow, this, ref error))
                {
                    BooterLogger.AddError(UnlocalizedName + " AutoOption " + option.GetType ().Name + ": Error " + error);
                    return false;
                }
            }

            if ((options == null) || (options.Rows == null) || (options.Rows.Count == 0))
            {
                BooterLogger.AddError(UnlocalizedName + ": Missing Options");
                return false;
            }
            else
            {
                BooterLogger.AddTrace(UnlocalizedName + ": Options = " + options.Rows.Count);

                int index = 1;
                foreach (XmlDbRow row in options.Rows)
                {
                    Type classType = row.GetClassType("FullClassName");
                    if (classType == null)
                    {
                        BooterLogger.AddError(UnlocalizedName + ": Unknown FullClassName " + row.GetString("FullClassName"));
                        continue;
                    }

                    OptionItem option = null;
                    try
                    {
                        option = classType.GetConstructor(new Type[0]).Invoke(new object[0]) as OptionItem;
                    }
                    catch
                    { }

                    if (option == null)
                    {
                        BooterLogger.AddError(UnlocalizedName + ": Constructor Fail " + row.GetString("FullClassName"));
                    }
                    else
                    {
                        IPersonalityOption personalityOption = option as IPersonalityOption;
                        if (personalityOption == null)
                        {
                            BooterLogger.AddError(UnlocalizedName + " index " + index + " : Not Personality Option");
                        }
                        else
                        {
                            error = null;
                            if (!personalityOption.Parse(row, this, ref error))
                            {
                                BooterLogger.AddError(UnlocalizedName + " index " + index + " " + option.Name + " : Error " + error);

                                mParsedOptions.Remove(option);
                            }
                        }
                    }

                    index++;
                }

                if (GetScenarios ().Count == 0)
                {
                    BooterLogger.AddError(UnlocalizedName + ": No valid scenarios");
                    return false;
                }
            }

            if (!mLeaderless)
            {
                mCandidateScoring = new SimScenarioFilter();
                if (!mCandidateScoring.Parse(myRow, this, this, "Candidate", true, ref error))
                {
                    BooterLogger.AddError(UnlocalizedName + ": Candidate " + error);
                    return false;
                }

                mLeaderRetention = new SimScenarioFilter();
                if (!mLeaderRetention.Parse(myRow, this, this, "LeaderRetention", false, ref error))
                {
                    BooterLogger.AddError(UnlocalizedName + ": LeaderRetention " + error);
                    return false;
                }

                mMemberRetention = new SimScenarioFilter();
                if (!mMemberRetention.Parse(myRow, this, this, "MemberRetention", false, ref error))
                {
                    BooterLogger.AddError(UnlocalizedName + ": MemberRetention " + error);
                    return false;
                }
            }

            return true;
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);
        }

        public bool HasRequiredVersion()
        {
            return GameUtils.IsInstalled(mProductVersion);
        }

        public bool Install(Main main)
        {
            mInstalled = false;

            if (!HasRequiredVersion()) return false;

            Main = main;

            bool success = true;

            foreach (OptionItem option in mParsedOptions)
            {
                IPersonalityOption install = option.Clone() as IPersonalityOption;
                if (install == null) continue;

                try
                {
                    install.Install(this, true);
                }
                catch (Exception e)
                {
                    Common.Exception(install.Name, e);
                    success = false;
                }
            }

            if (!success) return false;

            foreach (IUpdateManagerOption updater in mUpdaters)
            {
                updater.UpdateManager(this);
            }

            mInstalled = true;
            return success;
        }

        public bool IsPotentialCandiate(SimDescription me)
        {
            return IsPotentialCandiate(me, TestCandidateAllow);
        }
        public bool IsPotentialCandiate(SimDescription me, SimScenarioFilter.Parameters.AllowFunc onCandidateAllow)
        {
            return mCandidateScoring.Test(new SimScenarioFilter.Parameters(this, false, mCandidateReduction.Value, onCandidateAllow), "TestLeader", me, me);
        }

        protected SimDescription ManageMe(SimDescription me)
        {
            return ManageMe(me, TestCandidateAllow);
        }
        protected SimDescription ManageMe(SimDescription me, SimScenarioFilter.Parameters.AllowFunc onCandidateAllow)
        {
            if (!ProgressionEnabled)
            {
                IncStat("Disabled");
                return me;
            }
            else if (mLeaderless)
            {
                IncStat("Leaderless");
                return null;
            }

            int delta = -mCandidateReduction.Value;
            if (delta > mCandidateScoring.ScoringMinimum / 2)
            {
                delta = mCandidateScoring.ScoringMinimum / 2;

                mCandidateReduction.SetValue(-delta);
            }

            if ((me == null) ||
                ((!mUserSelected.Value) && (!SimTypes.IsSelectable(me)) && (!IsPotentialCandiate(me, onCandidateAllow))))
            {
                SimDescription sim = null;

                ICollection<SimDescription> potentials = mCandidateScoring.Filter(new SimScenarioFilter.Parameters(this, false, -delta, onCandidateAllow), "Candidate", null, null);
                if (potentials.Count == 0)
                {
                    mCandidateReduction.AddValue(-5);
                }
                else
                {
                    mCandidateReduction.SetValue(0);
                }

                AddStat("Potentials", potentials.Count, Common.DebugLevel.Stats);

                foreach(SimDescription potential in potentials)
                {
                    sim = potential;
                    break;
                }

                if (me != sim)
                {
                    if (sim != null)
                    {
                        Stories.PrintStory(this, "Creation", new object[] { sim }, null);
                        return sim;
                    }
                    else if (me == null)
                    {
                        IncStat("No Candidate", Common.DebugLevel.Stats);
                        return null;
                    }
                }
            }
            return me;
        }

        public override void GetOptions<TOption>(List<TOption> options, bool vbUI, AllowOptionDelegate<TOption> allow)
        {
            if (Main != null)
            {
                base.GetOptions(options, vbUI, allow);
            }
            else
            {
                foreach (OptionItem option in mParsedOptions)
                {
                    TOption item = option as TOption;
                    if (item == null) continue;

                    if (allow != null)
                    {
                        if (!allow(item)) continue;
                    }

                    options.Add(item);
                }
            }
        }

        protected static bool IsVisible(WeightOption option)
        {
            return option.IsVisible;
        }

        public List<WeightOption> GetScenarios()
        {
            List<WeightOption> options = new List<WeightOption>();
            GetOptions(options, false, IsVisible);

            return options;
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((fullUpdate) && (!mLeaderless))
            {
                if (Me != null)
                {
                    CheckRetention(Me);
                }

                SimDescription sim = ManageMe(Me);

                if ((Me == null) || (Me != sim))
                {
                    if (Me != null)
                    {
                        IncStat("Leader Reset", Common.DebugLevel.Stats);
                    }
                    else
                    {
                        IncStat("Leader Empty", Common.DebugLevel.Stats);
                    }

                    SetLeader(sim, false);

                    mUserSelected.SetValue (false);

                    ChanceOfEvent = 0;
                }
            }

            if ((fullUpdate) && (ProgressionEnabled) && (!initialPass))
            {
                if ((mLeaderless) || (Me != null))
                {
                    if (RandomUtil.RandomChance(ChanceOfEvent))
                    {
                        if (Scenarios.Post(new Scenario.ScenarioRun(new RunScenario(this), this, ScenarioResult.Start), false) != null)
                        {
                            IncStat("Fired");
                        }
                    }
                    else
                    {
                        AddStat("Chance Failure", ChanceOfEvent);

                        ChanceOfEvent += IncreasedChancePerCycle();
                        if (ChanceOfEvent > 100)
                        {
                            ChanceOfEvent = 100;
                        }
                    }

                    AddStat("Members", GetClanMembers(false).Count);
                }
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();

            mClan.Clear();
        }

        public bool TestMemberRetention(SimDescription sim)
        {
            if (Me == null) return false;

            return mMemberRetention.Test(new SimScenarioFilter.Parameters(this, true, TestRetentionAllow), "MemberRetention", Me, sim);
        }

        protected bool TestCandidateMember(SimDescription sim)
        {
            if (!TestCandidateAllow(sim)) return false;

            return IsMember(sim);
        }

        protected bool TestCandidateAllow(SimDescription sim)
        {
            // Don't allow actives to gain the leadership
            if (SimTypes.IsSelectable(sim)) return false;

            if (Deaths.IsDying(sim)) return false;

            return Personalities.Allow(this, sim, Managers.Manager.AllowCheck.None);
        }

        protected bool TestRetentionAllow(SimDescription sim)
        {
            // Allow actives to hold the leadership
            if (SimTypes.IsSelectable(sim)) return true;

            return Personalities.Allow(this, sim, Managers.Manager.AllowCheck.None);
        }

        public void CheckRetention(SimDescription sim)
        {
            StoryProgressionObject storyManager = this;

            string story = null;

            SimDescription newSim = null;

            if (Me == sim) 
            {
                if (SimTypes.IsDead(sim))
                {
                    SetLeader(null, false);

                    IncStat("Dead Leader", Common.DebugLevel.Stats);

                    story = mDeathStory;
                    if (string.IsNullOrEmpty(story))
                    {
                        storyManager = Personalities;

                        story = "LeaderDead";
                    }
                }
                else if (!mLeaderRetention.Test(new SimScenarioFilter.Parameters(this, true, TestRetentionAllow), "LeaderRetention", Me, sim))
                {
                    SetLeader(null, false);

                    IncStat("Improper Leader", Common.DebugLevel.Stats);

                    if (Me != null)
                    {
                        story = mLeaderReplaceStory;

                        newSim = Me;
                    }

                    if (string.IsNullOrEmpty(story))
                    {
                        story = mLeaderResignStory;
                    }

                    if (string.IsNullOrEmpty(story))
                    {
                        storyManager = Personalities;

                        story = "LeaderResign";
                    }
                }
                else
                {
                    foreach (SimPersonality clan in Personalities.GetClanMembership(sim, true))
                    {
                        if (IsOpposing(clan))
                        {
                            clan.RemoveFromClan(sim);

                            IncStat("Improper Opposing " + clan.UnlocalizedName, Common.DebugLevel.Stats);
                        }
                    }
                }
            }

            if ((Me != null) && (GetData(sim).IsClan(this)))
            {
                if (SimTypes.IsDead(sim))
                {
                    RemoveFromClan(sim);

                    IncStat("Dead Member", Common.DebugLevel.Stats);
                }
                else if (!TestMemberRetention(sim))
                {
                    RemoveFromClan(sim);

                    story = mMemberResignStory;
                    if (string.IsNullOrEmpty(story))
                    {
                        storyManager = Personalities;

                        story = "MemberResign";
                    }

                    IncStat("Improper Member", Common.DebugLevel.Stats);
                }
                else
                {
                    foreach (SimPersonality clan in Personalities.GetClanMembership(sim, false))
                    {
                        if (IsOpposing(clan))
                        {
                            clan.RemoveFromClan(sim);

                            IncStat("Improper Opposing " + clan.UnlocalizedName, Common.DebugLevel.Stats);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(story))
            {
                Stories.PrintStory(storyManager, story, new object[] { sim, GetLocalizedName(), newSim }, new string[] { GetLocalizedName() });
            }
        }

        protected void DropClanMembers ()
        {
            List<SimDescription> members = GetClanMembers(false);

            foreach (SimDescription member in members)
            {
                RemoveFromClan(member);
            }

            IncStat("Clan Members Dropped");
        }

        public override float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return val;

            return Personalities.AddStat(UnlocalizedName + ": " + stat, val, minLevel);
        }

        public override void IncStat(string stat, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return;

            Personalities.IncStat(UnlocalizedName + ": " + stat, minLevel);
        }


        public List<SimDescription> MeAsList
        {
            get
            {
                List<SimDescription> list = new List<SimDescription>();

                if (Me != null)
                {
                    list.Add(Me);
                }

                return list;
            }
        }

        public SimDescription Me
        {
            get
            {
                if (mMe == null)
                {
                    return null;
                }
                else
                {
                    return mMe.SimDescription;
                }
            }
        }

        public void SetLeader(SimDescription sim, bool manual)
        {
            if (Me == sim) return;

            bool drop = false;

            if (sim != null)
            {
                if (!IsMember(sim))
                {
                    DropClanMembers();
                }

                mLeaderCustomAccept.Invoke<bool>(new object[] { this, sim });

                RemoveFromClan(sim);

                List<IResetOnLeaderChangeOption> options = new List<IResetOnLeaderChangeOption>();
                GetOptions(options, false, null);

                SetValue<LastLeadershipOption, string>(sim, UnlocalizedName);

                foreach (IResetOnLeaderChangeOption option in options)
                {
                    option.ResetOnLeaderChange();
                }

                mUserSelected.SetValue(manual);
            }
            else if ((!manual) && (mFindReplacement))
            {
                sim = ManageMe(null, TestCandidateMember);
                if (sim != null)
                {
                    SetLeader(sim, false);
                }
                else
                {
                    drop = true;
                }
            }
            else
            {
                drop = true;
            }

            if (drop)
            {
                mUserSelected.SetValue (false);

                DropClanMembers();
            }

            mMe.SimDescription = sim;

            if (sim != null)
            {
                GetData(sim).InvalidateCache();
            }
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            if (GameStates.IsTravelling) return;

            if ((mMe != null) && (SimID.Matches (mMe.Value, sim)))
            {
                SetLeader(null, false);

                IncStat("Leader Removed");

                //Common.DebugStackLog(new Common.StringBuilder(GetLocalizedName() + " Leader Removed"));

                ChanceOfEvent = 0;
            }
        }

        public abstract class BooleanOption : BooleanBaseManagerOptionItem<SimPersonality>, IPersonalityOption
        {
            public BooleanOption(bool defValue)
                : base(defValue)
            { }

            public virtual bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
            {
                Install(personality, true);
                return true;
            }

            public override bool ShouldDisplay()
            {
                if (Progressed)
                {
                    return Manager.Personalities.ProgressionEnabled;
                }
                else
                {
                    return true;
                }
            }
        }

        public abstract class IntegerOption : IntegerBaseManagerOptionItem<SimPersonality>, IPersonalityOption
        {
            public IntegerOption(int defValue)
                : base(defValue)
            { }

            public virtual bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
            {
                Install(personality, true);
                return true;
            }

            public override bool ShouldDisplay()
            {
                if (Progressed)
                {
                    return Manager.Personalities.ProgressionEnabled;
                }
                else
                {
                    return true;
                }
            }
        }

        public class MeOption : SimIDOption, IAutoPersonalityOption
        {
            SimPersonality mManager;

            public MeOption()
            { }

            public SimPersonality Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public bool HasRequiredVersion()
            {
                return true;
            }

            public bool Install(SimPersonality manager, bool initial)
            {
                mManager = manager;

                mManager.mMe = this;

                return mManager.AddOption(this);
            }

            public bool Parse(XmlDbRow myRow, SimPersonality personality, ref string error)
            {
                return Install(personality, true);
            }

            protected override SimScenarioFilter GetScoring()
            {
                return Manager.mCandidateScoring;
            }

            public override string GetTitlePrefix()
            {
                return "Leader";
            }

            protected override bool IsFemaleLocalization()
            {
                return Manager.IsFemaleLocalization();
            }

            public override string GetUIValue(bool pure)
            {
                return null;
            }

            public override string Name
            {
                get
                {
                    string name = base.GetUIValue(true);
                    if (!string.IsNullOrEmpty(name))
                    {
                        name = ": " + name;
                    }

                    return Common.Localize(Manager.UnlocalizedName + ":MenuName", IsFemaleLocalization()) + name;
                }
            }

            public override string GetStoreKey()
            {
                if (Manager == null) return null;

                return Manager.UnlocalizedName + base.GetStoreKey();
            }

            public override bool ShouldDisplay()
            {
                if (Manager.mLeaderless) return false;

                return true;
            }

            protected override bool Allow(SimDescription me, SimScenarioFilter scoring)
            {
                if (!base.Allow(me, scoring)) return false;

                if (scoring != null)
                {
                    int score;
                    if (!scoring.Test(new SimScenarioFilter.Parameters(Manager), "UserSelect", me, me, false, out score)) return false;
                }

                if (SimTypes.IsSpecial(me)) return false;

                return true;
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                Manager.mUserSelected.SetValue (true);
                return true;
            }

            public override SimID Value
            {
                get
                {
                    return base.Value;
                }
            }

            public override void SetValue(SimID value, bool persist)
            {
                SimDescription original = SimDescription;

                base.SetValue(value, persist);

                if (mManager != null)
                {
                    mManager.SetLeader(SimDescription, false);
                }
            }
        }

        public class MembersOption : SimIDOption, IAutoPersonalityOption, IDebuggingOption
        {
            SimPersonality mManager;

            public MembersOption()
            { }

            public SimPersonality Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public bool HasRequiredVersion()
            {
                return true;
            }

            public bool Install(SimPersonality manager, bool initial)
            {
                mManager = manager;

                return Manager.AddOption(this);
            }

            public bool Parse(XmlDbRow myRow, SimPersonality personality, ref string error)
            {
                return Install(personality, true);
            }

            public override string GetTitlePrefix()
            {
                return "Members";
            }

            public override string GetStoreKey()
            {
                if (Manager == null) return null;

                return Manager.UnlocalizedName + base.GetStoreKey();
            }

            public override bool ShouldDisplay()
            {
                if (Manager.mLeaderless) return false;

                return Manager.DebuggingEnabled;
            }

            protected override SimScenarioFilter GetScoring()
            {
                return null;
            }

            public override string GetUIValue(bool pure)
            {
                if (Sim.ActiveActor == null) return "0";

                IEnumerable<SimDescription> all = SimSelection.Create(this).All;
                if (all == null) return "0";

                return new List<SimDescription>(all).Count.ToString();
            }

            protected override bool Allow(SimDescription me, SimScenarioFilter scoring)
            {
                if (!base.Allow(me, scoring)) return false;

                if (Manager.Me == null) return false;

                if (Manager.Me == me) return false;

                return (Manager.IsMember(me));
            }
        }

        public class UserSelectedMeOption : BooleanOption, IAutoPersonalityOption, IDebuggingOption
        {
            public UserSelectedMeOption()
                : base(false)
            { }

            public override bool Install(SimPersonality manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                Manager.mUserSelected = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "UserSelectedMe";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.mLeaderless) return false;

                return base.ShouldDisplay();
            }
        }

        public class ChanceOfEventOption : IntegerOption, IAutoPersonalityOption, IDebuggingOption
        {
            public ChanceOfEventOption()
                : base (0)
            { }

            public override bool Install(SimPersonality manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                Manager.mChanceOfEvent = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "ChanceofEvent";
            }

            protected override string GetPrompt()
            {
                return Localize("Prompt", new object[] { Manager.GetLocalizedName() });
            }
        }

        public class ProgressionOption : BooleanOption, IAutoPersonalityOption
        {
            public ProgressionOption()
                : base(true)
            { }

            public override bool Install(SimPersonality manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                Manager.mProgression = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "EnableProgression";
            }
        }

        public class ResetOption : BooleanOption, IAutoPersonalityOption, INotExportableOption
        {
            public ResetOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ResetPersonality";
            }

            public override string GetUIValue(bool pure)
            {
                return null;
            }

            protected override bool PrivatePerform()
            {
                if (AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt")))
                {
                    Manager.SetLeader(null, true);

                    foreach (SimDescription sim in Manager.Sims.All)
                    {
                        if (Manager.GetValue<LastLeadershipOption, string>(sim) == Manager.UnlocalizedName)
                        {
                            Manager.SetValue<LastLeadershipOption, string>(sim, null);
                        }
                    }

                    return true;
                }

                return false;
            }

            public override bool ShouldDisplay()
            {
                if (Manager.mLeaderless) return false;

                return base.ShouldDisplay();
            }
        }

        public class DumpScenariosOption : BooleanOption, IAutoPersonalityOption, INotPersistableOption, IDebuggingOption
        {
            public DumpScenariosOption()
                : base(false)
            { }

            public override string GetUIValue(bool pure)
            {
                return "";
            }

            public override string GetTitlePrefix()
            {
                return "DumpScenarios";
            }

            protected override bool PrivatePerform()
            {
                Common.WriteLog(Manager.ToString());
                return true;
            }
        }

        public class IncreasedChancePerCycleOption : IntegerOption, IAutoPersonalityOption
        {
            public IncreasedChancePerCycleOption()
                : base(15)
            { }

            public override bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
            {
                SetValue (row.GetInt("IncreasedChancePerCycle", Value));

                return base.Parse(row, personality, ref error);
            }

            public override bool Install(SimPersonality manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                Manager.mIncreasedChancePerCycle = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "IncreasedChancePerCycle";
            }
        }

        public class CandidateRequirementReductionOption : IntegerOption, IAutoPersonalityOption, IDebuggingOption
        {
            public CandidateRequirementReductionOption()
                : base(0)
            { }

            public override bool Install(SimPersonality manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                Manager.mCandidateReduction = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "CandidateRequirementReduction";
            }
        }

        public class TicksPassedOption : TicksPassedBaseOption<SimPersonality>, IAutoPersonalityOption
        {
            public TicksPassedOption()
            { }

            public bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
            {
                return Install(personality, true);
            }
        }

        public class SpeedOption : SpeedBaseOption<SimPersonality>, IAutoPersonalityOption
        {
            public SpeedOption()
                : base(250, false)
            { }

            public bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
            {
                SetValue (row.GetInt("Speed", Value));

                return Install(personality, true);
            }
        }

        public class Updates : AlertLevelOption<SimPersonality>, IAutoPersonalityOption
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }

            public bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
            {
                return Install(personality, true);
            }
        }

        public interface IResetOnLeaderChangeOption : IOptionItem
        {
            void ResetOnLeaderChange();
        }

        public interface IAccumulatorValue
        {
            void ApplyAccumulator();
        }

        public interface IMustBeFirstChoiceOption
        { }
    }
}

