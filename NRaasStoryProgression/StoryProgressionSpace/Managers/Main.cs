using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using Sims3.Gameplay;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class Main : Manager, IInstallationBase
    {
        [PersistableStatic]
        private static OptionStore sOptions = null;

        Common.AlarmTask mTask = null;

        bool mFirst = true;
        bool mRunning = false;
        bool mInTimer = false;

        bool mSecondCycle = false;

        SleepWatch mWatch = new SleepWatch();

        Dictionary<Type, OptionItemLookup> mOptionLookup = new Dictionary<Type, OptionItemLookup>();

        static bool sInitialInstall = true;

        List<Manager> mManagers = null;

        List<string> mStoryPrefixes = null;

        Dictionary<Type, Manager> mManagerLookup = null;
        Dictionary<string, Manager> mManagerByName = null;

        ManagerLot mLotManager = null;
        ManagerSim mSimManager = null;
        ManagerDeath mDeathManager = null;
        ManagerStory mStoryManager = null;
        ManagerCareer mCareerManager = null;
        ManagerCaste mCasteManager = null;
        ManagerFlirt mFlirtManager = null;
        ManagerMoney mMoneyManager = null;
        ManagerSkill mSkillManager = null;
        ManagerRomance mRomanceManager = null;
        ManagerScoring mScoringManager = null;
        ManagerScenario mScenarioManager = null;
        ManagerSituation mSituationManager = null;
        ManagerHousehold mHouseholdManager = null;
        ManagerPregnancy mPregnancyManager = null;
        ManagerFriendship mFriendManager = null;
        ManagerPersonality mPersonalityManager = null;

        public class OptionItemLookup
        {
            private OptionItem mOption;

            private Dictionary<string, OptionItem> mOptions = new Dictionary<string, OptionItem>();

            public OptionItemLookup()
            { }

            public OptionItem Get(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return mOption;
                }
                else
                {
                    try
                    {
                        return mOptions[name];
                    }
                    catch
                    {
                        Common.DebugStackLog("Missing Option=" + name);
                        return null;
                    }
                }
            }

            public bool Add(OptionItem option)
            {
                string key = option.GetStoreKey();

                try
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        mOptions.Add(key, option);
                    }

                    mOption = option;
                    return true;
                }
                catch
                {
                    BooterLogger.AddError("Identical Key=" + key);
                    return false;
                }
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

                if (mOption != null)
                {
                    builder.Append(Common.NewLine + "Default: " + mOption.GetStoreKey());
                    builder.Append(Common.NewLine + "  " + mOption.GetType());
                    builder.Append(Common.NewLine + "  " + mOption.DisplayValue);
                }

                foreach (KeyValuePair<string, OptionItem> options in mOptions)
                {
                    builder.Append(Common.NewLine + options.Key);
                    builder.Append(Common.NewLine + "  " + options.Value.GetType ());
                    builder.Append(Common.NewLine + "  " + options.Value.DisplayValue);
                }

                return builder.ToString();
            }
        }

#if _NEXTPHASE
        public static readonly bool sNextPhase = true;
#else
        public static readonly bool sNextPhase = false;
#endif

        public Main()
            : base (null)
        {
            Main = this;

            BooterLogger.AddTrace("Main Constructed");
        }

        public bool IsRunning
        {
            get { return mRunning; }
        }

        public override bool ProgressionEnabled
        {
            get
            {
                return GetValue<ProgressionOption, bool>();
            }
        }

        public override bool DebuggingEnabled
        {
            get
            {
                return Common.kDebugging;
            }
        }

        public override OptionStore Options
        {
            get
            {
                if (sOptions == null)
                {
                    sOptions = new OptionStore();
                }
                return sOptions;
            }
        }

        public override ManagerMoney Money
        {
            get
            {
                return mMoneyManager;
            }
            protected set
            {
                mMoneyManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerCaste Castes
        {
            get
            {
                return mCasteManager;
            }
            protected set
            {
                mCasteManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerSituation Situations
        {
            get
            {
                return mSituationManager;
            }
            protected set
            {
                mSituationManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerSim Sims
        {
            get
            {
                return mSimManager;
            }
            protected set
            {
                mSimManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerDeath Deaths
        {
            get
            {
                return mDeathManager;
            }
            protected set
            {
                mDeathManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerPersonality Personalities
        {
            get
            {
                return mPersonalityManager;
            }
            protected set
            {
                mPersonalityManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerPregnancy Pregnancies
        {
            get
            {
                return mPregnancyManager;
            }
            protected set
            {
                mPregnancyManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerSkill Skills
        {
            get
            {
                return mSkillManager;
            }
            protected set
            {
                mSkillManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerCareer Careers
        {
            get
            {
                return mCareerManager;
            }
            protected set
            {
                mCareerManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerFriendship Friends
        {
            get
            {
                return mFriendManager;
            }
            protected set
            {
                mFriendManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerFlirt Flirts
        {
            get
            {
                return mFlirtManager;
            }
            protected set
            {
                mFlirtManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerRomance Romances
        {
            get
            {
                return mRomanceManager;
            }
            protected set
            {
                mRomanceManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerHousehold Households
        {
            get
            {
                return mHouseholdManager;
            }
            protected set
            {
                mHouseholdManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerLot Lots
        {
            get
            {
                return mLotManager;
            }
            protected set
            {
                mLotManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerScenario Scenarios
        {
            get
            {
                return mScenarioManager;
            }
            protected set
            {
                mScenarioManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerScoring Scoring
        {
            get
            {
                return mScoringManager;
            }
            protected set
            {
                mScoringManager = value;
                mManagers.Add(value);
            }
        }

        public override ManagerStory Stories
        {
            get
            {
                return mStoryManager;
            }
            protected set
            {
                mStoryManager = value;
                mManagers.Add(value);
            }
        }

        public void Sleep(string scope)
        {
            mWatch.Sleep();
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<Main>(this).Perform(initial);
        }

        public T GetManager<T>(string managerName)
            where T : StoryProgressionObject
        {
            Manager manager;
            if (mManagerByName.TryGetValue(managerName, out manager))
            {
                return (manager as T);
            }
            else
            {
                return null;
            }
        }
        public T GetManager<T>()
            where T : class
        {
            Manager manager;
            if (mManagerLookup.TryGetValue(typeof(T), out manager))
            {
                return (manager as T);
            }
            else
            {
                return null;
            }
        }

        protected List<Manager> Managers
        {
            get
            {
                return mManagers;
            }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Main"; 
        }

        public bool SecondCycle
        {
            get { return mSecondCycle; }
        }

        public void InitialStartup()
        {
            Startup(null);
        }
        public override void Startup(PersistentOptionBase vOptions)
        {
            try
            {
                mManagers = new List<Manager>();

                Castes = new ManagerCaste(this);

                Lots = new ManagerLot(this);
                
                Households = new ManagerHousehold(this);

                Sims = new ManagerSim(this);

                Money = new ManagerMoney(this);

                Deaths = new ManagerDeath(this);

                Skills = new ManagerSkill(this);

                Careers = new ManagerCareer(this);

                Pregnancies = new ManagerPregnancy(this);

                Romances = new ManagerRomance(this);

                Flirts = new ManagerFlirt(this);

                Friends = new ManagerFriendship(this);

                Situations = new ManagerSituation(this);

                Personalities = new ManagerPersonality(this);

                Scenarios = new ManagerScenario(this);

                Scoring = new ManagerScoring(this);

                Stories = new ManagerStory(this);

                mManagerLookup = new Dictionary<Type,Manager>();
                mManagerLookup.Add(GetType(), this);

                mManagerByName = new Dictionary<string, Manager>();
                mManagerByName.Add(GetTitlePrefix(PrefixType.Pure), this);

                foreach (Manager manager in mManagers)
                {
                    mManagerLookup.Add(manager.GetType(), manager);
                    mManagerByName.Add(manager.GetTitlePrefix(PrefixType.Pure), manager);
                }

                InstallOptions(sInitialInstall);

                foreach (Manager manager in mManagers)
                {
                    manager.InstallOptions(sInitialInstall);
                }

                if (Common.IsAwayFromHomeworld())
                {
                    // Stop persistence for the moment
                    OptionStore optionStore = sOptions;
                    sOptions = null;

                    List<IAdjustForVacationOption> options = new List<IAdjustForVacationOption>();
                    GetOptions(options, false, null);

                    foreach (IAdjustForVacationOption option in options)
                    {
                        option.AdjustForVacationTown();
                    }

                    sOptions = optionStore;
                }

                Options.Restore();

                Common.kDebugging = GetValue<DebuggingOption, bool>();

                base.Startup(Options);

                RemoveStats();

                foreach (Manager manager in mManagers)
                {
                    try
                    {
                        manager.Startup(Options);
                    }
                    catch (Exception exception)
                    {
                        BooterLogger.AddError(exception.ToString());
                    }
                }

                if (DebuggingEnabled)
                {
                    TestOptionNameUniqueness();
                }

                sInitialInstall = false;

                mFirst = true;
                mTask = new Common.AlarmTask(1f, TimeUnit.Seconds, OnTimer, 10f, TimeUnit.Minutes);
            }
            catch (Exception exception)
            {
                Common.Exception("Startup", exception);
            }
        }

        public List<string> GetStoryPrefixes()
        {
            if (mStoryPrefixes == null)
            {
                mStoryPrefixes = new List<string>();

                GetStoryPrefixes(mStoryPrefixes);
            }

            return mStoryPrefixes;
        }

        public override void GetStoryPrefixes(List<string> prefixes)
        {
            foreach (Manager manager in mManagers)
            {
                manager.GetStoryPrefixes(mStoryPrefixes);
            }
        }

        public bool AddToLookup(OptionItem option)
        {
            OptionItemLookup lookup;
            if (!mOptionLookup.TryGetValue(option.GetType(), out lookup))
            {
                lookup = new OptionItemLookup();
                mOptionLookup.Add(option.GetType(), lookup);
            }

            return lookup.Add(option);
        }

        protected override T GetInternalOption<T>(string name)
        {
            OptionItemLookup lookup;
            if (!mOptionLookup.TryGetValue(typeof(T), out lookup))
            {
                return null;
            }

            return lookup.Get(name) as T;
        }

        public string OptionsToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<Type, OptionItemLookup> lookup in mOptionLookup)
            {
                builder.Append(Common.NewLine + lookup.Key.ToString());
                builder.Append(lookup.Value.ToString());
            }

            return builder.ToString();
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (!initialPass)
            {
                mSecondCycle = true;
            }
#if _NEXTPHASE
            if (initialPass)
            {
                SimpleMessageDialog.Show("NEXTPHASE", "This is a Next Phase build of StoryProgression.");
            }
#endif
            if (ProgressionEnabled)
            {
                StoryProgressionServiceEx.DisableCreateHousehold();
            }
            else
            {
                StoryProgressionServiceEx.EnableCreateHousehold();
            }

            if ((ProgressionEnabled) && (StoryProgressionService.sService != null))
            {
                if (StoryProgressionService.sService.mStoryProgressionAlarm != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(StoryProgressionService.sService.mStoryProgressionAlarm);
                    StoryProgressionService.sService.mStoryProgressionAlarm = AlarmHandle.kInvalidHandle;

                    Common.Notify(Localize("ProgressionDisabled"));
                }
            }

            if (mFirst)
            {
                if (ProgressionEnabled)
                {
                    PetAdoptions.Cleanup(IncStat);

                    PetAdoptions.Stop(IncStat);

                    Common.Notify(Localize("ProgressionEnabled"));
                    mFirst = false;
                }
            }
            
            if (!IntroTutorial.IsRunning)
            {
                foreach (Manager manager in mManagers)
                {
                    manager.Update(fullUpdate, initialPass);
                }

                base.PrivateUpdate(fullUpdate, initialPass);
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();

            mFirst = true;

            if (mTask != null)
            {
                mTask.Dispose();
                mTask = null;
            }

            foreach (Manager manager in mManagers)
            {
                try
                {
                    manager.Shutdown();
                }
                catch (Exception e)
                {
                    Common.DebugException(manager.UnlocalizedName, e);
                }
            }

            mRunning = false;

            mManagers = null;
            mManagerLookup = null;

            mScenarioManager = null;
            mSimManager = null;
            mHouseholdManager = null;
            mLotManager = null;

            sOptions = null;
        }

        public override void RemoveSim(ulong sim)
        {
            if (!IsRunning) return;

            base.RemoveSim(sim);

            if (mManagers != null)
            {
                foreach (Manager manager in mManagers)
                {
                    manager.RemoveSim(sim);
                }
            }

            // Retain the data for the sim until the user exits the game, so inheritance can use the data
            //Options.RemoveSim(sim);
        }

        public override void GetOptions<TOption>(List<TOption> options, bool vbUI, AllowOptionDelegate<TOption> allow)
        {
            base.GetOptions(options, vbUI, allow);

            foreach (Manager manager in Managers)
            {
                if (vbUI)
                {
                    List<TOption> subOptions = new List<TOption>();
                    manager.GetOptions(subOptions, vbUI, allow);

                    if (subOptions.Count > 0)
                    {
                        TOption listing = new MasterListingOption(manager) as TOption;
                        if (listing != null)
                        {
                            options.Add(listing);
                        }
                    }
                }
                else
                {
                    manager.GetOptions(options, vbUI, allow);
                }
            }
        }

        protected static bool AdjustsForAgeSpan(ISpeedBaseOption option)
        {
            return option.AdjustsForAgeSpan;
        }

        protected void AdjustSpeedForAgeSpan(float factor)
        {
            List<ISpeedBaseOption> allOptions = new List<ISpeedBaseOption>();
            GetOptions(allOptions, false, AdjustsForAgeSpan);

            foreach (ISpeedBaseOption option in allOptions)
            {
                int value = (int)(option.Value * factor);
                if (value <= 0)
                {
                    value = 1;
                }

                option.SetValue(value);
            }
        }

        protected static bool AdjustsForSpeed(ISpeedBaseOption option)
        {
            return option.AdjustsForSpeed;
        }

        protected void AdjustSpeedForSpeed(float factor)
        {
            List<ISpeedBaseOption> allOptions = new List<ISpeedBaseOption>();
            GetOptions(allOptions, false, AdjustsForSpeed);

            foreach (ISpeedBaseOption option in allOptions)
            {
                int value = (int)(option.Value * factor);
                if (value <= 0)
                {
                    value = 1;
                }

                option.SetValue(value);
            }
        }

        protected bool TestOptionNameUniqueness ()
        {
            Dictionary<string, OptionItem> optionLookup = new Dictionary<string, OptionItem>();
            return GetOptionLookup(optionLookup);
        }

        public bool GetOptionLookup(Dictionary<string, OptionItem> optionLookup)
        {
            List<OptionItem> allOptions = new List<OptionItem>();
            GetOptions(allOptions, false, PersistenceEx.IsExportable);

            foreach (OptionItem option in Options.GetOptions(this, "Town", false))
            {
                allOptions.Add(option);
            }

            foreach (OptionItem option in Options.GetImmigrantOptions(this))
            {
                allOptions.Add(option);
            }

            bool bUnique = true;
            foreach (OptionItem option in allOptions)
            {
                List<string> names = PersistenceEx.GetExportKey(option);
                if (names == null) continue;

                for (int i = 0; i < names.Count; i++)
                {
                    string name = names[i];

                    if (optionLookup.ContainsKey(name))
                    {
                        if ((i == 0) && (DebuggingEnabled))
                        {
                            Common.Notify("Duplicate name: '" + name + "'");
                            bUnique = false;
                        }
                    }
                    else
                    {
                        optionLookup.Add(name, option);
                    }
                }
            }

            return bUnique;
        }

        public void OnTimer()
        {
            try
            {
                if (StoryProgression.Main == null) return;

                Scenarios.IncStat("OnTimer Try");

                if (mInTimer) return;

                mWatch.Restart();

                try
                {
                    mInTimer = true;

                    mRunning = true;

                    if (GetValue<ShowStartScreenOption, bool>())
                    {
                        FilePersistence.ImportFromTuning("NRaas.StoryProgression.Tuning");

                        if (!GetValue<ProgressionOption, bool>())
                        {
                            if (AcceptCancelDialog.Show(Localize("Welcome")))
                            {
                                GetOption<ProgressionOption>().SetValue(true);

                                new InitialHomelessScenario().Post(Households, true, false);
                            }
                        }
                    }

                    GetOption<ShowStartScreenOption>().SetValue (false);

                    if (GetValue<ProgressionOption, bool>())
                    {
                        int currentInterval = LifeSpan.GetHumanAgeSpanLength();

                        if (GetValue<LastAskedIntervalOption, int>() != currentInterval)
                        {
                            if (AcceptCancelDialog.Show(Localize("AgeSpan")))
                            {
                                AdjustSpeedForAgeSpan((float)currentInterval / GetValue<SavedIntervalOption, int>());

                                GetOption<SavedIntervalOption>().SetValue (currentInterval);
                            }

                            GetOption<LastAskedIntervalOption>().SetValue (currentInterval);
                        }
                    }

                    Update(false, false);
                }
                finally
                {
                    mInTimer = false;
                }

                Scenarios.IncStat("OnTimer Complete");
            }
            catch (Exception exception)
            {
                Common.Exception(UnlocalizedName, exception);
            }
        }

        public class MasterListingOption : NestingManagerOptionItem<StoryProgressionObject, OptionItem>
        {
            public MasterListingOption(StoryProgressionObject manager)
                : base(manager)
            { }

            public override string GetTitlePrefix()
            {
                return "MasterOption";
            }

            public override string Name
            {
                get
                {
                    return Localize("MenuName", new object[] { Common.Localize(Manager.UnlocalizedName + ":MenuName", IsFemaleLocalization()) });
                }
            }

            protected override bool Allow(OptionItem option)
            {
                if (option is INotRootLevelOption) return false;

                return true;
            }
        }

        public class DebuggingOption : BooleanManagerOptionItem<Main>
        {
            public DebuggingOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowDebugging";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override void SetValue(bool value, bool persist)
            {
                base.SetValue(value, persist);

                StoryProgression.kDebugging = value;

                if (value)
                {
                    Manager.TestOptionNameUniqueness();
                }
            }
        }

        public class DumpAllOptions : BooleanManagerOptionItem<Main>, IDebuggingOption
        {
            public DumpAllOptions()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "DumpAllOptions";
            }

            public override string GetUIValue(bool pure)
            {
                return null;
            }

            protected override bool PrivatePerform()
            {
                List<OptionItem> options = new List<OptionItem>();
                Manager.GetOptions(options, true, null);

                Common.StringBuilder builder = new Common.StringBuilder();

                int index = 0;
                while (index < options.Count)
                {
                    OptionItem option = options[index];
                    index++;

                    builder.Append(Common.NewLine + option.Name);

                    MasterListingOption master = option as MasterListingOption;
                    if (master != null)
                    {
                        foreach (OptionItem subItem in master.GetOptions())
                        {
                            options.Insert(index, subItem);
                        }
                    }
                }

                Common.WriteLog(builder);
                return true;
            }
        }

        public class ShowHiddenOption : BooleanManagerOptionItem<Main>, IDebuggingOption
        {
            public ShowHiddenOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowHiddenOptions";
            }

            public override bool Value
            {
                get
                {
                    if (!ShouldDisplay()) return false;

                    return base.Value;
                }
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                return Manager.GetValue<DebuggingOption, bool>();
            }
        }

        public class ReapplyInteractionsOption : BooleanManagerOptionItem<Main>, INotPersistableOption
        {
            public ReapplyInteractionsOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ReapplyInteractions";
            }

            public override string GetUIValue(bool pure)
            {
                return "";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected override bool PrivatePerform()
            {
                Common.AddAllInteractions();
                return true;
            }
        }

        public class ResetOption : BooleanManagerOptionItem<Main>, INotPersistableOption
        {
            public ResetOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "TotalReset";
            }

            public override string GetUIValue(bool pure)
            {
                return "";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected override bool PrivatePerform()
            {
                if (!AcceptCancelDialog.Show(Localize("Prompt")))
                {
                    return false;
                }

                StoryProgression.Reset();
                return true;
            }
        }

        public class SavedIntervalOption : IntegerManagerOptionItem<Main>
        {
            public SavedIntervalOption()
                : base(90)
            { }

            public override string GetTitlePrefix()
            {
                return "SavedInterval";
            }

            public override bool ShouldDisplay()
            {
                return false;
            }
        }

        public class LastAskedIntervalOption : IntegerManagerOptionItem<Main>
        {
            public LastAskedIntervalOption()
                : base(90)
            { }

            public override string GetTitlePrefix()
            {
                return "LastAskedInterval";
            }

            public override bool ShouldDisplay()
            {
                return false;
            }
        }

        public class ShowStartScreenOption : BooleanManagerOptionItem<Main>, INotExportableOption
        {
            public ShowStartScreenOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowStartScreen";
            }

            public override bool ShouldDisplay()
            {
                return false;
            }
        }

        public class ProgressionOption : BooleanManagerOptionItem<Main>
        {
            public ProgressionOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "EnableProgression";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                if (!Value)
                {
                    if (StoryProgressionService.sService != null)
                    {
                        StoryProgressionService.sService.Schedule();

                        Common.Notify(Localize("EAProgressionEnabled"));
                    }
                }

                return true;
            }
        }

        public enum AdjustProgressionLevel : int
        {
            Snail = 16,
            Slow = 8,
            Default = 4,
            Fast = 2,
            Rabbit = 1 
        }

        public class AdjustProgression : EnumManagerOptionItem<Main,AdjustProgressionLevel>
        {
            public AdjustProgression()
                : base(AdjustProgressionLevel.Default, AdjustProgressionLevel.Default)
            { }

            public override string GetTitlePrefix()
            {
                return "AdjustSpeed";
            }

            protected override string GetLocalizationValueKey()
            {
                return "AdjustProgressionLevel";
            }

            protected override AdjustProgressionLevel Convert(int value)
            {
                return (AdjustProgressionLevel)value;
            }

            protected override AdjustProgressionLevel Combine(AdjustProgressionLevel original, AdjustProgressionLevel add, out bool same)
            {
                same = (original == add);
                return add;
            }

            protected override bool PrivatePerform()
            {
                AdjustProgressionLevel original = Value;

                if (!base.PrivatePerform()) return false;

                Manager.AdjustSpeedForSpeed((float)Value / (float)original);

                return true;
            }
        }

        protected class ChangeAllUpdates : AlertLevelOption<Main>
        {
            public ChangeAllUpdates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "AllStories";
            }

            protected static bool UpdatesOnAll(IAlertLevelOption option)
            {
                return option.UpdatesOnAll();
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                if (AcceptCancelDialog.Show(Localize("Prompt", new object[] { DisplayValue })))
                {
                    List<IAlertLevelOption> allOptions = new List<IAlertLevelOption>();
                    Manager.GetOptions(allOptions, false, UpdatesOnAll);

                    foreach (IAlertLevelOption option in allOptions)
                    {
                        option.SetValue(Value);
                    }
                }

                if (Value == AlertLevel.None)
                {
                    Manager.Stories.Clear();
                }

                return true;
            }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<Main>
        {
            public TicksPassedOption()
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<Main>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        public class Version : IntegerManagerOptionItem<Main>, INotPersistableOption
        {
            public Version()
                : base(VersionStamp.sVersion)
            { }

            public override string GetTitlePrefix()
            {
                return "Version";
            }

            public override string Name
            {
                get { return Common.Localize("Version:MenuName"); }
            }

            public override int Value
            {
                get { return VersionStamp.sVersion; }
            }

            public override object PersistValue
            {
                get
                {
                    return null;
                }
                set
                {
                    return;
                }
            }

            protected virtual string Prompt
            {
                get { return Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { VersionStamp.sVersion }); }
            }

            protected override bool PrivatePerform()
            {
                SimpleMessageDialog.Show(Name, Prompt);
                return true;
            }
        }

        public class FireScenarioOption : GenericOptionItem<Scenario>, IInstallable<Main>, INotPersistableOption, IDebuggingOption
        {
            Main mManager = null;

            public FireScenarioOption()
                : base(null, null)
            { }

            public virtual bool HasRequiredVersion()
            {
                return true;
            }

            public Main Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public bool Install(Main manager, bool initial)
            {
                if (!HasRequiredVersion()) return false;

                mManager = manager;

                return mManager.AddOption(this);
            }

            public override string GetTitlePrefix()
            {
                return "FireScenario";
            }

            protected override string GetLocalizationValueKey()
            {
                return null;
            }

            public override object PersistValue
            {
                get
                {
                    return null;
                }
                set
                {
                    return;
                }
            }

            protected override bool PrivatePerform()
            {
                List<IScenarioOptionItem> options = new List<IScenarioOptionItem>();
                mManager.GetOptions(options, false, null);

                List<ScenarioOption> scenarios = new List<ScenarioOption>();

                //Common.StringBuilder msg = new Common.StringBuilder();

                foreach (IScenarioOptionItem option in options)
                {
                    Scenario scenario = option.GetScenario();
                    if (scenario == null) continue;

                    ScenarioOption item = new ScenarioOption(scenario, option.GetManager());

                    /*
                    msg += Common.NewLine + "Option: " + option.GetType();
                    msg += Common.NewLine + "Scenario: " + scenario.GetType();
                    msg += Common.NewLine + "Name: " + item.Name;
                    */
                    scenarios.Add(item);
                }

                //Common.WriteLog(msg);

                if (scenarios.Count == 0) return false;

                bool okayed = false;
                List<ScenarioOption> selection = OptionItem.ListOptions(scenarios, Name, scenarios.Count, out okayed);
                if ((selection == null) || (selection.Count == 0)) return false;

                foreach (ScenarioOption item in selection)
                {
                    item.Perform(); 
                }

                return true;
            }

            public override bool ShouldDisplay()
            {
                return true;
            }

            protected class ScenarioOption : GenericOptionItem<Scenario>, IScenarioOptionItem, INotPersistableOption
            {
                StoryProgressionObject mManager = null;

                protected ScenarioOption()
                { }
                public ScenarioOption(Scenario scenario, StoryProgressionObject manager)
                    : base(scenario, scenario)
                {
                    scenario.Manager = manager;

                    mManager = manager;
                }

                public void Install(IInstallationBase main)
                { }

                public Scenario GetScenario()
                {
                    return Value;
                }

                public StoryProgressionObject GetManager()
                {
                    return mManager;
                }

                public override string GetTitlePrefix()
                {
                    return null;
                }

                public bool Allowed
                {
                    get
                    {
                        try
                        {
                            return Value.Test();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(Name, e);
                            return false;
                        }
                    }
                }

                public override string GetUIValue(bool pure)
                {
                    return Common.Localize("Disallowed:" + (!Allowed).ToString());
                }

                protected override string GetLocalizationValueKey()
                {
                    return null;
                }

                public override string Name
                {
                    get 
                    {
                        return Value.UnlocalizedName + " - " + mManager.UnlocalizedName;
                    }
                }

                public override object PersistValue
                {
                    get
                    {
                        return null;
                    }
                    set
                    {
                        return;
                    }
                }

                public override bool ShouldDisplay()
                {
                    return true;
                }

                protected override bool PrivatePerform()
                {
                    if (!Value.ManualSetup(GetManager())) return false;

                    mManager.Main.Scenarios.Post(new Scenario.ScenarioRun(Value, GetManager(), ScenarioResult.Start), true);
                    return true;
                }
            }
        }

        public class Logger : Common.TraceLogger<Logger>
        {
            static Logger sLogger = new Logger();

            static int sCount = 0;

            public static void AddTrace(string msg)
            {
                sCount++;
                sLogger.PrivateAddTrace(msg);
            }

            protected override string Name
            {
                get { return "Trace"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(Common.StringBuilder builder)
            {
                if (sCount < 50) return 0;

                sCount = 0;

                return base.PrivateLog(builder);
            }
        }
    }
}

