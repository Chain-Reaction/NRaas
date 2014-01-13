using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public enum ScenarioResult
    {
        Start,
        Success,
        Failure,
        Chance,
        End
    }

    public abstract class Scenario : ManagerProgressionBase, ILocalizer, IScoringGenerator, IUpdateManager
    {
        public enum GatherResult
        {
            Update,
            Success,
            Failure
        }

        public enum ListenHandleType
        {
            Post,
            Task,
            Immediate,
        }

        public delegate void UpdateDelegate(Scenario scenario, ScenarioFrame frame);

        List<IUpdateManagerOption> mUpdaters = new List<IUpdateManagerOption>();

        StoryProgressionObject mManager = null;

        bool mPushed = false;

        int mID = 0;

        int mRescheduled = 0;

        int mPushChance = 100;

        protected bool mShouldPush = true;

        protected int mInitialReportChance = 100;

        protected int mContinueReportChance = 100;

        bool mFirst = true;

        protected Scenario()
            : base (null)
        { }
        protected Scenario(Scenario scenario)
            : base (scenario.Main)
        {
            mManager = scenario.mManager;
            mID = scenario.mID;
            //mRescheduled = scenario.mRescheduled;
            mPushChance = scenario.mPushChance;
            mShouldPush = scenario.mShouldPush;
            mInitialReportChance = scenario.mInitialReportChance;
            mContinueReportChance = scenario.mContinueReportChance;
            mUpdaters = scenario.mUpdaters;
            //mFirst = scenario.mFirst;
        }

        public override string ToString()
        {
            string text = base.ToString();

            if (Manager != null)
            {
                text += Common.NewLine + "Manager=" + Manager.UnlocalizedName;
            }

            text += Common.NewLine + "Name=" + UnlocalizedName;
            text += Common.NewLine + "PushChance=" + mPushChance;
            text += Common.NewLine + "ShouldPush=" + mShouldPush;
            text += Common.NewLine + "InitialReportChance=" + mInitialReportChance;
            text += Common.NewLine + "ContinueReportChance=" + mContinueReportChance;

            return text;
        }

        public virtual StoryProgressionObject Manager
        {
            get { return mManager; }
            set
            {
                mManager = value;
                Main = mManager.Main;

                foreach (IUpdateManagerOption updater in mUpdaters)
                {
                    updater.UpdateManager(value);
                }
            }
        }

        public void AddUpdater(IUpdateManagerOption updater)
        {
            mUpdaters.Add(updater);
        }

        public virtual List<string> GetStoryPrefixes()
        {
            List<string> prefixes = new List<string>();

            try
            {
                // This may fail if the Scenario relies on tuning to provide its name
                prefixes.Add(UnlocalizedName);
            }
            catch
            { }

            return prefixes;
        }

        public override Common.DebugLevel DebuggingLevel
        {
            get
            {
                if (mManager != null)
                {
                    return mManager.DebuggingLevel;
                }
                else if (Scenarios != null)
                {
                    return Scenarios.DebuggingLevel;
                }
                else
                {
                    return base.DebuggingLevel;
                }
            }
        }

        public override bool DebuggingEnabled
        {
            get
            {
                if (Scenarios != null)
                {
                    if (!Scenarios.DebuggingEnabled) return false;
                }

                if (mManager != null)
                {
                    return (mManager.DebuggingLevel > Common.DebugLevel.Stats);
                }
                else
                {
                    return base.DebuggingEnabled;
                }
            }
        }

        protected abstract bool Progressed
        { 
            get; 
        }

        protected virtual int PushChance
        {
            get { return mPushChance; }
        }

        public virtual bool ShouldPush
        {
            get { return mShouldPush; }
        }

        protected virtual int ContinueChance
        {
            get { return 100; }
        }

        protected virtual int InitialReportChance
        {
            get 
            {
                return mInitialReportChance;
            }
        }

        protected virtual int ContinueReportChance
        {
            get 
            {
                if (mInitialReportChance == 0) return 0;

                return mContinueReportChance; 
            }
        }

        protected virtual int ReportDelay
        {
            get
            {
                return 0;
            }
        }

        protected int ReportChance
        {
            get
            {
                if (mFirst)
                {
                    return InitialReportChance;
                }
                else
                {
                    return ContinueReportChance;
                }
            }
        }

        protected virtual bool ShouldReport
        {
            get 
            {
                int chance = ReportChance;

                if (chance == 100) return true;

                if (chance == 0) return false;

                return RandomUtil.RandomChance(chance); 
            }
        }

        protected virtual bool ImmediateReschedule
        {
            get { return AlwaysReschedule; }
        }

        protected virtual int Rescheduling
        {
            get { return 0; }
        }

        protected virtual int MaximumReschedules
        {
            get { return 4; }
        }

        protected virtual bool AlwaysReschedule
        {
            get { return false; }
        }

        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        public virtual IAlarmOwner Owner
        {
            get { return null; } 
        }

        protected virtual bool ForceAlert
        {
            get { return false; }
        }

        public virtual bool ManualSetup(StoryProgressionObject manager)
        {
            Manager = manager;
            return true;
        }

        public string Localize(string key)
        {
            return Manager.Localize(key);
        }
        public string Localize(string key, bool female, object[] parameters)
        {
            return Manager.Localize(key, female, parameters);
        }
        public bool Localize(string key, bool female, object[] parameters, out string story)
        {
            return Manager.Localize(key, female, parameters, out story);
        }

        public int AddScoring(string scoring, SimDescription scoreAgainst)
        {
            return AddScoring(scoring, scoreAgainst, Common.DebugLevel.Low);
        }
        public int AddScoring(string scoring, SimDescription scoreAgainst, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, scoreAgainst), minLevel);
        }

        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription scoreAgainst)
        {
            return AddScoring(scoring, option, type, scoreAgainst, Common.DebugLevel.Low);
        }
        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription scoreAgainst, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, option, type, scoreAgainst), minLevel);
        }

        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription scoreAgainst, SimDescription other)
        {
            return AddScoring(scoring, option, type, scoreAgainst, other, Common.DebugLevel.Low);
        }
        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription scoreAgainst, SimDescription other, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, option, type, scoreAgainst, other), minLevel);
        }

        public int AddScoring(string scoring, SimDescription scoreAgainst, SimDescription other)
        {
            return AddScoring(scoring, scoreAgainst, other, Common.DebugLevel.Low);
        }
        public int AddScoring(string scoring, SimDescription scoreAgainst, SimDescription other, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, scoreAgainst, other), minLevel);
        }

        public int AddScoring(string stat, int score)
        {
            return AddScoring(stat, score, Common.DebugLevel.Low);
        }
        public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return score;

            if (!string.IsNullOrEmpty(stat))
            {
                stat = ": " + stat;
            }

            if ((Scenarios != null) && (Scenarios.TrackingID == ID))
            {
                Scenarios.Trace = true;
                try
                {
                    Scenarios.AddScoring(GetIDName() + stat, score, minLevel);
                }
                finally
                {
                    Scenarios.Trace = false;
                }
            }

            return Manager.AddScoring(UnlocalizedName + stat, score, minLevel);
        }

        public int AddStat(string stat, int val)
        {
            return AddStat(stat, val, Common.DebugLevel.Low);
        }
        public int AddStat(string stat, int val, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return val;

            if ((Scenarios != null) && (Scenarios.TrackingID == ID))
            {
                Scenarios.Trace = true;
                try
                {
                    Scenarios.AddStat(GetIDName() + ": " + stat, val, minLevel);
                }
                finally
                {
                    Scenarios.Trace = false;
                }
            }

            return Manager.AddStat(UnlocalizedName + ": " + stat, val, minLevel);
        }
        public float AddStat(string stat, float val)
        {
            return AddStat(stat, val, Common.DebugLevel.Low);
        }
        public float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return val;

            if ((Scenarios != null) && (Scenarios.TrackingID == ID))
            {
                Scenarios.Trace = true;
                try
                {
                    Scenarios.AddStat(GetIDName() + ": " + stat, val, minLevel);
                }
                finally
                {
                    Scenarios.Trace = false;
                }
            }

            return Manager.AddStat(UnlocalizedName + ": " + stat, val, minLevel);
        }

        protected void AddTry()
        {
            if (!Common.kDebugging) return;

            Manager.AddTry(UnlocalizedName);

            Scenarios.AddTry(UnlocalizedName);
        }

        protected void AddAllowed()
        {
            if (!Common.kDebugging) return;

            Manager.IncStat("Allowed: " + UnlocalizedName);

            Scenarios.IncStat("Allowed: " + UnlocalizedName);
        }

        protected void AddSuccess()
        {
            if (!Common.kDebugging) return;

            if (Scenarios != null)
            {
                Scenarios.AddSuccess(UnlocalizedName);
            }

            Manager.AddSuccess(UnlocalizedName);
        }

        public void IncStat(string stat)
        {
            IncStat(stat, Common.DebugLevel.Low);
        }
        public void IncStat(string stat, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return;

            if ((Scenarios != null) && (Scenarios.TrackingID == ID))
            {
                Scenarios.Trace = true;
                try
                {
                    Scenarios.IncStat(GetIDName() + ": " + stat, minLevel);
                }
                finally
                {
                    Scenarios.Trace = false;
                }
            }

            if (Manager != null)
            {
                Manager.IncStat(UnlocalizedName + ": " + stat, minLevel);
            }
        }

        public virtual string GetIDName()
        {
            return mID + " " + UnlocalizedName;
        }

        public virtual bool Parse(XmlDbRow row, ref string error)
        {
            mPushChance = row.GetInt("PushChance", mPushChance);

            if (row.Exists("ShouldPush"))
            {
                mShouldPush = row.GetBool("ShouldPush");
            }

            mInitialReportChance = row.GetInt("InitialReportChance", row.GetInt("ReportChance", mInitialReportChance));
            mContinueReportChance = row.GetInt("ContinueReportChance", row.GetInt("ReportChance", mContinueReportChance));

            return true;
        }

        public virtual bool PostParse(ref string error)
        {
            return true;
        }

        public void Callback(Sim sim, float x)
        {
            try
            {
                Scenario scenario = Clone();
                if (scenario != null)
                {
                    scenario.Post(Manager, true, false);
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        protected virtual Scenario Handle(Event e, ref ListenerAction result)
        {
            return Clone();
        }

        protected virtual ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Post; }
        }

        public ListenerAction Listen(Event e)
        {
            ListenerAction result = ListenerAction.Keep;

            try
            {
                Scenario scenario = Handle(e, ref result);
                if (scenario != null)
                {
                    Spawn(scenario);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
            }
            return result;
        }

        protected void Spawn(Scenario scenario)
        {
            switch (HandleImmediately)
            {
                case ListenHandleType.Task:
                    scenario.IncStat("Handle Task");

                    scenario.AddTry();

                    Common.FunctionTask.Perform(scenario.ImmediateUpdate);
                    break;
                case ListenHandleType.Immediate:
                    scenario.IncStat("Handle Immediate");

                    scenario.AddTry();

                    if (!scenario.Test()) return;

                    scenario.AddAllowed();

                    scenario.Update(null);
                    break;
                default:
                    scenario.IncStat("Handle Post");

                    scenario.Post(Manager, true, false);
                    break;
            }
        }

        public void Fire()
        {
            try
            {
                if (!Test())
                {
                    AllowFailCleanup();
                    return;
                }

                Scenario scenario = Clone();
                if (scenario != null)
                {
                    Spawn(scenario);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(ActorObjectId, TargetObjectId, ToString(), exception);
            }
        }

        protected virtual object ActorObjectId
        {
            get { return null; }
        }

        protected virtual object TargetObjectId
        {
            get { return null; }
        }

        protected virtual bool Matches(Scenario scenario)
        {
            return (GetType () == scenario.GetType ());
        }

        public abstract Scenario Clone();

        protected abstract bool PrivateUpdate(ScenarioFrame frame);

        protected virtual void AllowFailCleanup()
        { }

        protected abstract GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random);

        protected virtual bool Push()
        {
            return true;
        }

        protected void PrintStory(bool notify)
        {
            ManagerStory.Story story = null;

            ManagerStory.StoryLogging logging = ManagerStory.StoryLogging.Log;
            if (notify)
            {
                logging |= ManagerStory.StoryLogging.Summary | ManagerStory.StoryLogging.Notice;   
            }

            story = PrintStory(null, GetTitlePrefix(PrefixType.Story), null, null, logging);
            if (story == null)
            {
                story = PrintFormattedStory(null, null, GetTitlePrefix(PrefixType.Summary), null, null, logging);
                if (story != null)
                {
                    IncStat("PrintFormattedStory Success");
                }
            }
            else
            {
                IncStat("PrintStory Success");

                story.Delay = ReportDelay;
            }

            if (story == null)
            {
                story = PrintDebuggingStory(null);
                if (story != null)
                {
                    IncStat("PrintDebuggingStory Success");
                }
            }

            if (story != null)
            {
                if ((Scenarios != null) && (Scenarios.TrackingID == ID))
                {
                    IncStat(story.ToString());
                }
            }
        }
        protected virtual ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (name == null) return null;

            if (manager == null)
            {
                manager = Manager;
            }

            if (parameters == null)
            {
                parameters = new object[0];
            }

            if (HasValue<ManagerStory.DisallowByStoryOption,string>(manager.UnlocalizedName + ":" + UnlocalizedName))
            {
                return null;
            }

            return Stories.PrintStory(manager, name, parameters, extended, ForceAlert, logging);
        }

        protected virtual ManagerStory.Story PrintDebuggingStory(object[] parameters)
        {
            if (Stories.DebuggingLevel < Common.DebugLevel.Low) return null;

            if (parameters == null)
            {
                parameters = new object[0];
            }

            return Stories.PrintFormattedStory(Manager, "(D) " + SimClock.CurrentTime() + " : " + UnlocalizedName, UnlocalizedName, parameters, null, ManagerStory.StoryLogging.Full);
        }

        protected virtual ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (text == null)
            {
                return null;
            }

            if (manager == null)
            {
                manager = Manager;
            }

            if (parameters == null)
            {
                parameters = new object[0];
            }

            if (HasValue<ManagerStory.DisallowByStoryOption, string>(manager.UnlocalizedName + ":" + UnlocalizedName))
            {
                return null;
            }

            return Stories.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }

        protected void Restart()
        {
            mRescheduled = 0;

            IncStat("Restart");
        }

        protected int Reschedule()
        {
            if ((mRescheduled > MaximumReschedules) || (Manager == null) || (Manager.Scenarios == null))
            {
                return 0;
            }

            mRescheduled++;

            int half = Rescheduling / 2;

            int delay = half + RandomUtil.GetInt(half);

            if (delay > 0)
            {
                if ((Manager.DebuggingEnabled) || (Manager.Scenarios.DebuggingEnabled))
                {
                    Manager.AddStat("Rescheduled: " + UnlocalizedName, delay, Common.DebugLevel.Stats);

                    Manager.Scenarios.AddStat("Rescheduled: " + UnlocalizedName, delay, Common.DebugLevel.Stats);
                }
            }

            return delay;
        }

        protected abstract bool UsesSim(ulong sim);

        protected virtual bool Allow()
        {
            // If the game is ended while processing, this should stop any queued scenarios from running
            if (StoryProgression.Main == null) return false;

            if (Progressed)
            {
                if (!Main.ProgressionEnabled) return false;
            }

            return true;
        }

        public bool Test()
        {
            string name = "Allow " + UnlocalizedName;
            if (!Main.SecondCycle)
            {
                name = "First " + name;
            }

            using (Common.TestSpan span = new Common.TestSpan(Scenarios, name))
            {
                return Allow();
            }
        }

        protected virtual bool Allow(bool fullUpdate, bool initialPass)
        {
            if (initialPass) 
            {
                return (MaximumReschedules == int.MaxValue);
            }

            if (!fullUpdate) return false;

            return true;
        }

        public ScenarioFrame Post(StoryProgressionObject manager, bool fullUpdate, bool initialPass)
        {
            if (!Allow(fullUpdate, initialPass))
            {
                manager.IncStat("Fail " + UnlocalizedName);
                return null;
            }

            return manager.Main.Scenarios.Post(new ScenarioRun(this, manager, ScenarioResult.Start), false);
        }

        protected ScenarioResult Run(ScenarioFrame frame)
        {
            try
            {
                if (Scenarios == null)
                {
                    return ScenarioResult.Failure;
                }

                string name = "Duration " + UnlocalizedName;
                if (!Main.SecondCycle)
                {
                    name = "First " + name;
                }

                using (Common.TestSpan span = new Common.TestSpan(Scenarios, name))
                {
                    AddTry();

                    if (!Test())
                    {
                        AllowFailCleanup();

                        return ScenarioResult.Failure;
                    }

                    AddAllowed();

                    List<Scenario> list = new List<Scenario>();

                    int continueChance = ContinueChance;
                    int maximum = 0;
                    bool random = true;

                    GatherResult result = GatherResult.Failure;

                    string gatherName = "Gather " + UnlocalizedName;
                    if (!Main.SecondCycle)
                    {
                        gatherName = "First " + gatherName;
                    }

                    using (Common.TestSpan gatherSpan = new Common.TestSpan(Scenarios, gatherName))
                    {
                        result = Gather(list, ref continueChance, ref maximum, ref random);
                    }

                    switch (result)
                    {
                        case GatherResult.Update:
                            return Update(frame);
                        case GatherResult.Failure:
                            IncStat("Gathering Failure");

                            return ScenarioResult.Failure;
                    }

                    // Stops the PushAndPrint() routine from firing
                    mPushed = true;

                    AddStat("Gathered", list.Count);

                    if (list.Count == 0)
                    {
                        return ScenarioResult.Failure;
                    }

                    if (maximum <= 0)
                    {
                        maximum = list.Count;
                    }

                    if (continueChance != 100)
                    {
                        AddStat("Chance", continueChance);
                    }

                    bool first = true;

                    int count = 0;
                    if (random)
                    {
                        while (list.Count > 0)
                        {
                            int index = RandomUtil.GetInt(list.Count - 1);

                            Scenario item = list[index];
                            list.RemoveAt(index);

                            item.mFirst = first;
                            first = false;

                            Add(frame, item, continueChance);

                            count++;
                            if (count > maximum)
                            {
                                AddStat("Maximum", count);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Scenario item in list)
                        {
                            item.mFirst = first;
                            first = false;

                            Add(frame, item, continueChance);

                            count++;
                            if (count > maximum)
                            {
                                AddStat("Maximum", count);
                                break;
                            }
                        }
                    }

                    return ScenarioResult.Success;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ToString(), e);
                return ScenarioResult.Failure;
            }
        }

        public void Add(ScenarioFrame frame, Scenario scenario, ScenarioResult result)
        {
            if (frame == null)
            {
                StoryProgression.Main.Scenarios.Post(scenario);
            }
            else
            {
                if (frame.Count == 0)
                {
                    result = ScenarioResult.Start;
                }

                frame.Add(Manager, scenario, result);
            }
        }
        public void Add(ScenarioFrame frame, Scenario scenario, int chance)
        {
            if (frame == null)
            {
                if (RandomUtil.RandomChance(chance))
                {
                    StoryProgression.Main.Scenarios.Post(scenario);
                }
            }
            else
            {
                if (frame.Count == 0)
                {
                    Add(frame, scenario, ScenarioResult.Start);
                }
                else
                {
                    frame.Add(Manager, scenario, chance);
                }
            }
        }

        private void ImmediateUpdate()
        {
            // The circumstances could have changed since this scenario are tasked
            if (!Test()) return;

            AddAllowed();

            Update(null);
        }
        protected ScenarioResult Update(ScenarioFrame frame)
        {
            try
            {
                if (Common.kDebugging)
                {
                    if (GetValue<LogScenarioOption, bool>())
                    {
                        Common.DebugWriteLog(ToString());
                    }
                }

                if (!PrivateUpdate(frame))
                {
                    return ScenarioResult.Failure;
                }

                if (!PushAndPrint())
                {
                    return ScenarioResult.Failure;
                }

                return ScenarioResult.Success;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ToString(), e);
                return ScenarioResult.Failure;
            }
        }

        public bool PushAndPrint()
        {
            if (mPushed) return true;
            mPushed = true;

            bool result = true;

            bool success = ShouldReport;

            /*
            DualSimScenario scenario = this as DualSimScenario;
            if (scenario != null)
            {
                Common.Notify(scenario.Sim, scenario.Target, "PushAndPrint: " + PushChance + Common.NewLine + ShouldPush + Common.NewLine + ToString());
            }
            */
            if (RandomUtil.RandomChance(PushChance))
            {
                if (ShouldPush) 
                {
                    if (!Push())
                    {
                        IncStat("Push Fail");

                        if (!GetValue<ManagerStory.ShowNonPushableStoryOption, bool>())
                        {
                            PrintDebuggingStory(null);

                            success = false;
                        }

                        result = false;
                    }
                    else
                    {
                        IncStat("Push Success");
                    }
                }
            }
            else
            {
                IncStat("Push Chance Fail");

                if (!GetValue<ManagerStory.ShowNonPushableStoryOption, bool>())
                {
                    PrintDebuggingStory(null);

                    success = false;
                }
            }

            PrintStory(success);

            AddSuccess();
            return result;
        }

        public class LogScenarioOption : BooleanManagerOptionItem<Main>, IDebuggingOption
        {
            public LogScenarioOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "LogScenarios";
            }
        }

        public class ScenarioRun
        {
            Scenario mScenario;

            ScenarioResult mPrecondition = ScenarioResult.Start;

            int mChance = 100;

            public ScenarioRun(Scenario scenario, StoryProgressionObject manager, ScenarioResult precondition)
            {
                mScenario = scenario;
                mScenario.Manager = manager;
                mPrecondition = precondition;
            }
            public ScenarioRun(Scenario scenario, StoryProgressionObject manager, int chance)
            {
                mPrecondition = ScenarioResult.Chance;
                mChance = chance;

                mScenario = scenario;
                mScenario.Manager = manager;
            }

            public bool IsInstant
            {
                get
                {
                    if (mPrecondition != ScenarioResult.Chance) return false;

                    return (mChance == 100f);
                }
            }

            public ScenarioResult Precondition
            {
                get { return mPrecondition; }
            }

            public int ID
            {
                get { return mScenario.ID; }
                set { mScenario.ID = value; }
            }

            public override string ToString()
            {
                if (mScenario == null)
                {
                    return "Empty";
                }
                else
                {
                    return mScenario.GetIDName();
                }
            }

            public string UnlocalizedName
            {
                get { return mScenario.UnlocalizedName; }
            }

            public void Restart()
            {
                mScenario.Restart();
            }

            public void IncStat(string msg)
            {
                mScenario.IncStat(msg);
            }

            public bool Matches(ScenarioRun run)
            {
                return (mScenario.Matches (run.mScenario));
            }

            public bool Allow()
            {
                return mScenario.Test();
            }

            public bool AlwaysReschedule
            {
                get
                {
                    return mScenario.AlwaysReschedule;
                }
            }

            public bool Delayed
            {
                get
                {
                    return mScenario.ImmediateReschedule;
                }
            }

            public int Reschedule()
            {
                return mScenario.Reschedule();
            }

            public void PushAndPrint()
            {
                mScenario.PushAndPrint();
            }

            public bool Satisfies(ScenarioResult lastResult)
            {
                if (mPrecondition == ScenarioResult.Start) 
                {
                    return true;                    
                }
                else if (lastResult == ScenarioResult.Start)
                {
                    return true;
                }
                else if (lastResult == ScenarioResult.End)
                {
                    return false;
                }
                else if (mPrecondition == ScenarioResult.Chance)
                {
                    if (lastResult != ScenarioResult.Failure)
                    {
                        if (!RandomUtil.RandomChance(mChance))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (lastResult != mPrecondition)
                    {
                        return false;
                    }
                }

                return true;
            }

            public ScenarioResult Run(ScenarioResult lastResult, ScenarioFrame frame)
            {
                if (Satisfies(lastResult))
                {
                    return mScenario.Run(frame);
                }
                else
                {
                    return ScenarioResult.End;
                }
            }

            public bool UsesSim(ulong sim)
            {
                return mScenario.UsesSim(sim);
            }
            /*
            public class RunTask : Common.WaitTask
            {
                ScenarioRun mRun;

                ScenarioResult mLastResult;

                ScenarioFrame mFrame;

                ScenarioResult mResult;

                protected RunTask(ScenarioRun run, ScenarioResult lastResult, ScenarioFrame frame)
                {
                    mRun = run;
                    mLastResult = lastResult;
                    mFrame = frame;
                }

                public static ScenarioResult Wait(ScenarioRun run, ScenarioResult lastResult, ScenarioFrame frame)
                {
                    return Wait(new RunTask(run, lastResult, frame)).mResult;
                }

                protected override void OnWaitPerform()
                {
                    mResult = mRun.Run(mLastResult, mFrame);
                }
            }*/
        }
    }
}
