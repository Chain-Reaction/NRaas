using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class StoryProgressionObject : ManagerProgressionBase, ILocalizer, IStoryProgressionUpdater, IScoringGenerator, IInstallationBase
    {
        public enum AlertLevel : int
        {
            None = 0,
            Friends = 0x01,
            Blood = 0x02,
            Romantic = 0x04,
            Enemies = 0x08,
            PersonalityLeaders = 0x10,
            PersonalityMembers = 0x20,
            Spouses = 0x040,
            All = 0xff,
            OnlyResidents = 0x100,
            PortraitPanel = 0x200,
        }

        static Common.MethodStore sIsSimListed = new Common.MethodStore("NRaasPortraitPanel", "NRaas.PortraitPanel", "IsSimListed", new Type[] { typeof(SimDescription) });

        private List<OptionItem> mOptions = new List<OptionItem>();

        protected IDebugLevelOption mDebugLevel = null;
        protected ISpeedBaseOption mSpeedOption = null;
        protected ITicksPassedBaseOption mTicksPassed = null;
        protected IAlertLevelOption mAlertLevel = null;

        private EventDictionary mListeners = new EventDictionary();

        private AlarmDictionary mAlarms = new AlarmDictionary();

        public StoryProgressionObject(Main manager)
            : base (manager)
        { }

        public string GetLocalizedName ()
        {
            return Localize("MenuName");
        }

        public int Speed
        {
            get
            {
                if (mSpeedOption == null) return 0;

                return mSpeedOption.Value;
            }
        }

        public override string ToString()
        {
            string text = base.ToString();
            
            foreach (OptionItem option in mOptions)
            {
                text += Common.NewLine + Common.NewLine + "-- Option --";
                text += Common.NewLine + option;
                text += Common.NewLine + "-- End Option --";
            }

            return text;
        }

        public string Localize(string key)
        {
            return Common.Localize(UnlocalizedName + ":" + key, IsFemaleLocalization(), new object[0]);
        }
        public string Localize(string key, bool female, object[] parameters)
        {
            return Common.Localize(UnlocalizedName + ":" + key, female, parameters);
        }
        public bool Localize(string key, bool female, object[] parameters, out string story)
        {
            return Common.Localize(UnlocalizedName + ":" + key, female, parameters, out story);
        }

        public virtual bool IsFemaleLocalization()
        {
            return false;
        }

        public void Startup()
        {
            foreach (OptionItem option in mOptions)
            {
                IStoryProgressionUpdater updater = option as IStoryProgressionUpdater;
                if (updater != null)
                {
                    updater.Startup();
                }
            }
        }

        public virtual void Startup(PersistentOptionBase options)
        {
            BooterLogger.AddTrace("Startup " + UnlocalizedName);

            mAlarms.Manager = this;

            foreach (OptionItem option in mOptions)
            {
                options.Restore(option);
            }

            Startup();
        }

        protected virtual void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            foreach (OptionItem option in mOptions)
            {
                IStoryProgressionUpdater updater = option as IStoryProgressionUpdater;
                if (updater == null) continue;

                updater.Update(fullUpdate, initialPass);
            }
        }

        public void Update(bool fullUpdate, bool initialPass)
        {
            using (Common.TestSpan span = new Common.TestSpan(Scenarios, "Manager: " + UnlocalizedName))
            {
                bool bFullUpdate = mTicksPassed.InitialPass;
                if ((mSpeedOption == null) || (mTicksPassed.Value > mSpeedOption.Value))
                {
                    mTicksPassed.SetValue(0);

                    bFullUpdate = true;
                }

                try
                {
                    PrivateUpdate(bFullUpdate, mTicksPassed.InitialPass);
                }
                catch (ResetException)
                { }
                catch (Exception e)
                {
                    Common.Exception(UnlocalizedName, e);
                }

                mTicksPassed.SetValue(mTicksPassed.Value + 10);

                mTicksPassed.InitialPass = false;
            }
        }

        public virtual void Shutdown()
        {
            BooterLogger.AddTrace("Shutdown " + UnlocalizedName);

            foreach (Common.AlarmTask alarm in mAlarms.Keys)
            {
                alarm.Dispose();
            }
            mAlarms.Clear();

            foreach (EventListener listener in mListeners.Keys)
            {
                EventTracker.RemoveListener(listener);
            }
            mListeners.Clear();

            foreach (OptionItem option in mOptions)
            {
                IStoryProgressionUpdater updater = option as IStoryProgressionUpdater;
                if (updater == null) continue;

                updater.Shutdown();
            }

            mOptions.Clear();
        }

        public void Notify(string story, Sim sim, string text)
        {
            if (HasValue<ManagerStory.DisallowByStoryOption, string>(UnlocalizedName + ":" + story)) return;

            Notify(sim, text);
        }
        public void Notify(Sim sim, string text)
        {
            if ((SimTypes.IsSelectable(sim)) || (MatchesAlertLevel(sim)))
            {
                Common.Notify(sim, text);
            }
        }

        public bool MatchesAlertLevel(IEnumerable<SimDescription> sims)
        {
            foreach (SimDescription sim in sims)
            {
                if (MatchesAlertLevel(sim)) return true;
            }
            return false;
        }
        public bool MatchesAlertLevel(Sim sim)
        {
            if (sim == null) return false;

            return MatchesAlertLevel(sim.SimDescription);
        }
        public bool MatchesAlertLevel(SimDescription sim)
        {
            if (sim == null) return false;

            if (SimTypes.IsSelectable(sim))
            {
                return Stories.DebuggingEnabled;
            }

            if (GetValue<ShowStoriesOption, bool>(sim))
            {
                return true;
            }
            else if (!GetValue<AllowStoryOption, bool>(sim))
            {
                return false;
            }

            if (mAlertLevel == null) return false;

            if ((GetValue<ManagerStory.ShowWholeHouseholdOption, bool>()) &&
                (sim.Household != null) &&
                (!SimTypes.IsSpecial(sim)))
            {
                foreach (SimDescription member in HouseholdsEx.All(sim.Household))
                {
                    if (mAlertLevel.Matches(member))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (mAlertLevel.Matches(sim))
                {
                    return true;
                }
            }

            return false;
        }

        public static void RemoveSim(List<SimDescription> sims, ulong id)
        {
            if (sims == null) return;

            foreach (SimDescription sim in sims)
            {
                if (sim.SimDescriptionId == id)
                {
                    sims.Remove(sim);
                    return;
                }
            }
        }

        public virtual bool ProgressionEnabled
        {
            get
            {
                Managers.Main manager = NRaas.StoryProgression.Main;
                if (manager == null) return false;

                return manager.ProgressionEnabled;
            }
        }

        public override Common.DebugLevel DebuggingLevel
        {
            get
            {
                if ((mDebugLevel == null) || (!DebuggingEnabled))
                {
                    return Common.DebugLevel.Quiet;
                }
                else
                {
                    return mDebugLevel.Value;
                }
            }
        }

        public override bool DebuggingEnabled
        {
            get
            {
                if (!Main.DebuggingEnabled) return false;

                if (mDebugLevel == null) return true;

                return (mDebugLevel.Value != Common.DebugLevel.Quiet);
            }
        }

        public virtual void RemoveSim(ulong sim)
        { }

        public delegate bool AllowOptionDelegate<TOption>(TOption option) where TOption : class, IOptionItem;

        public virtual void GetOptions<TOption>(List<TOption> options, bool forUI, AllowOptionDelegate<TOption> allow)
            where TOption : class, IOptionItem
        {
            bool showingHidden = GetValue<Main.ShowHiddenOption, bool>();

            bool debugging = DebuggingEnabled;

            foreach (OptionItem option in mOptions)
            {
                TOption item = option as TOption;
                if (item == null) continue;

                if (allow != null)
                {
                    if (!allow(item)) continue;
                }

                if (forUI) 
                {
                    if ((!showingHidden) && (!item.ShouldDisplay())) continue;

                    if ((!debugging) && (item is IDebuggingOption)) continue;
                }

                options.Add(item);
            }

            if ((debugging) && (forUI))
            {
                TOption option = new DebugListingOption(this) as TOption;
                if (option != null)
                {
                    options.Add(option);
                }
            }
        }

        public ScenarioFrame Post(Scenario scenario)
        {
            return Scenarios.Post(scenario, this, false);
        }

        public AlarmManagerReference AddAlarm(IAlarmScenario scenario)
        {
            return AddAlarm(scenario, true);
        }
        public AlarmManagerReference AddAlarm(IAlarmScenario scenario, bool testAllow)
        {
            if (scenario == null) return null;

            scenario.Manager = this;

            if ((testAllow) && (!scenario.Test())) return null;

            return scenario.SetupAlarm(mAlarms);
        }

        public void RemoveAlarm(Common.AlarmTask alarm)
        {
            if (alarm == null) return;

            alarm.Dispose();

            mAlarms.Remove(alarm);
        }

        public bool AddListener(IEventScenario scenario)
        {
            if (scenario == null) return false;

            scenario.Manager = this;

            return scenario.SetupListener(mListeners);
        }
        protected bool AddListener(EventTypeId vID, ProcessEventDelegate func)
        {
            EventListener listener = EventTracker.AddListener(vID, func);
            if (listener == null) return false;

            mListeners.Add(listener, null);
            return true;
        }

        public int AddScoring(string scoring, SimDescription sim)
        {
            return AddScoring(scoring, sim, Common.DebugLevel.Low);
        }
        public int AddScoring(string scoring, SimDescription sim, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim), minLevel);
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

        public int AddScoring(string scoring, SimDescription target, SimDescription sim)
        {
            return AddScoring(scoring, target, sim, Common.DebugLevel.Low);
        }
        public int AddScoring(string scoring, SimDescription target, SimDescription sim, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, target, sim), minLevel);
        }

        public int AddScoring(string stat, int score)
        {
            return AddScoring(stat, score, Common.DebugLevel.Low);
        }
        public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return score;

            if (score > 0)
            {
                AddStat(stat + " Scoring Pos", score, minLevel);
            }
            else if (score < 0)
            {
                AddStat(stat + " Scoring Neg", score, minLevel);
            }
            else
            {
                AddStat(stat + " Scoring Zero", score, minLevel);
            }

            return score;
        }

        public int AddStat(string stat, int val)
        {
            return AddStat(stat, val, Common.DebugLevel.Low);
        }
        public int AddStat(string stat, int val, Common.DebugLevel minLevel)
        {
            if (!Common.kDebugging) return val;

            return (int)AddStat(stat, (float)val, minLevel);
        }
        public float AddStat(string stat, float val)
        {
            return AddStat(stat, val, Common.DebugLevel.Low);
        }
        public abstract float AddStat(string stat, float val, Common.DebugLevel minLevel);

        public void AddTry(string stat)
        {
            if (!Common.kDebugging) return;

            IncStat("Try " + stat, Common.DebugLevel.Stats);
        }

        public void AddSuccess(string stat)
        {
            if (!Common.kDebugging) return;

            IncStat("Success " + stat, Common.DebugLevel.Stats);
        }

        public void IncStat(string stat)
        {
            IncStat(stat, Common.DebugLevel.Low);
        }
        public abstract void IncStat(string stat, Common.DebugLevel minLevel);

        public virtual bool AddOption(OptionItem option)
        {
            if (Main == null) return false;

            mOptions.Add(option);

            return Main.AddToLookup(option);
        }

        public abstract class SpeedBaseOption<TManager> : IntegerManagerOptionItem<TManager>, ISpeedBaseOption
            where TManager : StoryProgressionObject
        {
            bool mAgeSpanAdjust = false;

            public SpeedBaseOption(int defValue, bool ageSpanAdjust)
                : base(defValue)
            {
                mAgeSpanAdjust = ageSpanAdjust;
            }

            public override bool Install(TManager main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                Manager.mSpeedOption = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "Speed";
            }

            public bool AdjustsForAgeSpan
            {
                get { return mAgeSpanAdjust; }
            }

            public bool AdjustsForSpeed
            {
                get { return mAgeSpanAdjust; }
            }

            public override bool ShouldDisplay()
            {
                if (Progressed)
                {
                    // Running off Main
                    return Manager.Main.ProgressionEnabled;
                }
                else
                {
                    return true;
                }
            }

            public override int Value
            {
                get
                {
                    return base.Value;
                }
            }

            public override void SetValue(int value, bool persist)
            {
                if (value < 10)
                {
                    base.SetValue(10, persist);
                }
                else
                {
                    base.SetValue(value, persist);
                }
            }

            protected override string GetPrompt()
            {
                return Localize("Prompt", new object[] { Manager.GetLocalizedName() });
            }
        }

        public abstract class TicksPassedBaseOption<TManager> : IntegerManagerOptionItem<TManager>, ITicksPassedBaseOption, INotExportableOption, IDebuggingOption
            where TManager : StoryProgressionObject
        {
            bool mInitialPass = true;

            public TicksPassedBaseOption()
                : base(-1)
            { }

            public override bool Install(TManager main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                Manager.mTicksPassed = this;
                return true;
            }

            public bool InitialPass
            {
                get
                {
                    return mInitialPass;
                }
                set
                {
                    mInitialPass = value;
                }
            }

            public override string GetTitlePrefix()
            {
                return "TicksPassed";
            }

            public override bool ShouldDisplay()
            {
                return Manager.DebuggingEnabled;
            }
        }

        public abstract class DebugLevelOption<TManager> : EnumManagerOptionItem<TManager, Common.DebugLevel>, IDebugLevelOption
            where TManager : StoryProgressionObject
        {
            public DebugLevelOption(Common.DebugLevel value)
                : base(value, value)
            { }

            public override bool Install(TManager main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                Manager.mDebugLevel = this;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "Debugging";
            }

            protected override string GetLocalizationValueKey()
            {
                return "DebugLevel";
            }

            protected override Common.DebugLevel Convert(int value)
            {
                return (Common.DebugLevel)value;
            }

            protected override Common.DebugLevel Combine(Common.DebugLevel original, Common.DebugLevel add, out bool same)
            {
                same = (original == add);
                return add;
            }

            protected override bool Allow(Common.DebugLevel value)
            {
                if (value == Common.DebugLevel.Logging) return false;

                return base.Allow(value);
            }

            public override bool ShouldDisplay()
            {
                return Manager.Main.DebuggingEnabled;
            }
        }

        protected class EventDictionary : Dictionary<EventListener, IEventScenario>, IEventHandler
        {
            public bool AddListener(IEventScenario scenario, EventTypeId vID)
            {
                EventListener listener = EventTracker.AddListener(vID, scenario.Listen);
                if (listener == null) return false;

                Add(listener, scenario);
                return true;
            }
        }

        protected class AlarmDictionary : Dictionary<Common.AlarmTask, IAlarmScenario>, IAlarmHandler
        {
            protected StoryProgressionObject mManager = null;

            public AlarmDictionary()
            { }

            public StoryProgressionObject Manager
            {
                set { mManager = value; }
            }

            public AlarmManagerReference AddAlarm(IAlarmScenario scenario, float time)
            {
                return AddAlarm(scenario, time, TimeUnit.Hours);
            }
            public AlarmManagerReference AddAlarm(IAlarmScenario scenario, float time, TimeUnit units)
            {
                scenario.AddStat("Alarm (" + units + ")", time);

                Common.AlarmTask alarm = new Common.AlarmTask(time, units, scenario.Fire);
                Add(alarm, scenario);

                return new AlarmManagerReference(mManager, alarm);
            }
            public AlarmManagerReference AddAlarmDelayed(IAlarmScenario scenario, float repeatTime, TimeUnit repeatUnit)
            {
                scenario.AddStat("AlarmRepeating (" + repeatUnit + ")", repeatTime);

                Common.AlarmTask alarm = new Common.AlarmTask(repeatTime, repeatUnit, scenario.Fire, repeatTime, repeatUnit);
                Add(alarm, scenario);
                return new AlarmManagerReference(mManager, alarm);
            }
            public AlarmManagerReference AddAlarmImmediate(IAlarmScenario scenario, float repeatTime, TimeUnit repeatUnit)
            {
                scenario.AddStat("AlarmImmediate (" + repeatUnit + ")", repeatTime);

                Common.AlarmTask alarm = new Common.AlarmTask(1f, TimeUnit.Seconds, scenario.Fire, repeatTime, repeatUnit);
                Add(alarm, scenario);
                return new AlarmManagerReference(mManager, alarm);
            }
            public AlarmManagerReference AddAlarmDay(IAlarmScenario scenario, float hourOfDay)
            {
                return AddAlarmDay(scenario, hourOfDay, DaysOfTheWeek.All);
            }
            public AlarmManagerReference AddAlarmDay(IAlarmScenario scenario, float hourOfDay, DaysOfTheWeek days)
            {
                scenario.AddStat("AlarmDay (" + days + ")", hourOfDay);

                Common.AlarmTask alarm = new Common.AlarmTask(hourOfDay, days, scenario.Fire);
                Add(alarm, scenario);
                return new AlarmManagerReference(mManager, alarm);
            }
        }

        public class DebugListingOption : NestingManagerOptionItem<StoryProgressionObject, IDebuggingOption>
        {
            public DebugListingOption(StoryProgressionObject manager)
                : base(manager)
            { }

            public override string GetTitlePrefix()
            {
                return "DebugListing";
            }
        }

        public abstract class AlertLevelOption<TManager> : EnumManagerOptionItem<TManager, AlertLevel>, IAlertLevelOption
            where TManager : StoryProgressionObject
        {
            public AlertLevelOption(AlertLevel value)
                : base(value, value)
            { }

            public override bool Install(TManager main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (UpdatesOnAll())
                {
                    Manager.mAlertLevel = this;
                }

                return true;
            }

            protected override int NumSelectable
            {
                get { return 0; }
            }

            protected override string GetLocalizationValueKey()
            {
                return "AlertLevel";
            }

            public virtual bool UpdatesOnAll()
            {
                return true;
            }

            public override AlertLevel Value
            {
                get
                {
                    if (!ShouldDisplay()) return AlertLevel.None;

                    return base.Value;
                }
            }

            protected override AlertLevel Convert(int value)
            {
                return (AlertLevel)value;
            }

            protected override AlertLevel Combine(AlertLevel original, AlertLevel add, out bool same)
            {
                AlertLevel result = original | add;

                same = (result == original);

                return result;
            }

            protected override List<AlertLevel> ConvertToList(AlertLevel value)
            {
                if (value == AlertLevel.All)
                {
                    List<AlertLevel> results = new List<AlertLevel>();
                    results.Add(value);
                    return results;
                }
                else
                {
                    return base.ConvertToList(value);
                }
            }

            public override string GetUIValue(bool pure)
            {
                if (Value == AlertLevel.None)
                {
                    return LocalizeValue("None");
                }

                int value;
                if ((Value.ToString().Contains(",")) || (int.TryParse(Value.ToString(), out value)))
                {
                    return LocalizeValue("Custom");
                }
                else
                {
                    return base.GetUIValue(pure);
                }
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected override List<IGenericValueOption<AlertLevel>> GetAllOptions()
            {
                List<IGenericValueOption<AlertLevel>> results = base.GetAllOptions();

                results.Add(new ListItem(this, AlertLevel.None));

                return results;
            }

            protected override bool PrivatePerform(List<AlertLevel> values)
            {
                foreach (AlertLevel level in values)
                {
                    if (level == AlertLevel.None)
                    {
                        values.Clear();

                        SetValue(AlertLevel.None);
                        break;
                    }
                    else if ((AlertLevel.All & level) == level)
                    {
                        RemoveValue(AlertLevel.All);
                        break;
                    }
                }

                if (!base.PrivatePerform(values)) return false;

                if ((Value == AlertLevel.None) && (Manager != null))
                {
                    Manager.Stories.Purge(Manager);
                }

                return true;
            }

            protected override bool Allow(AlertLevel value)
            {
                switch (value)
                {
                    case AlertLevel.PortraitPanel:
                        return Common.AssemblyCheck.IsInstalled("NRaasPortraitPanel");
                    case AlertLevel.None:
                        return false;
                }
    
                return base.Allow(value);
            }

            public bool Matches(List<SimDescription> sims)
            {
                foreach (SimDescription sim in sims)
                {
                    if (Matches(sim)) return true;
                }

                return false;
            }
            public bool Matches(SimDescription sim)
            {
                if (Value == AlertLevel.None)
                {
                    return false;
                }
                else if (((Value & AlertLevel.OnlyResidents) == AlertLevel.OnlyResidents) && (sim.LotHome == null))
                {
                    return false;
                }
                else if ((Value == AlertLevel.OnlyResidents) || ((Value & AlertLevel.All) == AlertLevel.All) || (sim.Household == Household.ActiveHousehold))
                {
                    return true;
                }
                else if (((Value & AlertLevel.PersonalityLeaders) == AlertLevel.PersonalityLeaders) && (Manager.Personalities.GetClanLeadership(sim).Count > 0))
                {
                    return true;
                }
                else if (((Value & AlertLevel.PersonalityMembers) == AlertLevel.PersonalityMembers) && (Manager.Personalities.GetClanMembership(sim, false).Count > 0))
                {
                    return true;
                }
                else if (((Value & AlertLevel.PortraitPanel) == AlertLevel.PortraitPanel) && (sIsSimListed.Invoke<bool>(new object[] { sim })))
                {
                    return true;
                }
                else
                {
                    Household house = Household.ActiveHousehold;
                    if (house != null)
                    {
                        foreach (SimDescription active in HouseholdsEx.All(house))
                        {
                            if (!SimTypes.IsSelectable(active)) continue;

                            if ((Value & AlertLevel.Blood) == AlertLevel.Blood)
                            {
                                if (Relationships.IsCloselyRelated(active, sim, false)) return true;
                            }

                            if (Matches(Relationship.Get(active, sim, false))) return true;

                            if (((Value & AlertLevel.Spouses) == AlertLevel.Spouses) && (sim.Partner != null))
                            {
                                if (Matches(Relationship.Get(active, sim.Partner, false))) return true;
                            }
                        }
                    }

                    return false;
                }
            }

            protected bool Matches(Relationship relation)
            {
                if (relation != null)
                {
                    if ((Value & AlertLevel.Friends) == AlertLevel.Friends)
                    {
                        if (relation.AreFriends()) return true;
                    }

                    if ((Value & AlertLevel.Enemies) == AlertLevel.Enemies)
                    {
                        if (relation.AreEnemies()) return true;
                    }

                    if ((Value & AlertLevel.Romantic) == AlertLevel.Romantic)
                    {
                        if (relation.AreRomantic()) return true;
                    }
                }

                return false;
            }
        }
    }
}
