using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NRaas
{
    public abstract class Common
    {
        [Tunable, TunableComment("Whether to display debug notifications")]
        public static bool kDebugging = false;

        [Tunable, TunableComment("Whether to display load log notifications")]
        private static bool kLoadLogDebugging = false;

        [Tunable, TunableComment("Whether to disable AddAllInteractions")]
        private static bool kDisableAllInteractionInjection = false;

        [Tunable, TunableComment("Whether to allow interactions in the lot menu")]
        public static bool kDisableLotMenu = false;

        // Internal use, in conjunction with kLoadLogDebugging
        protected static bool sEnableLoadLog = false;

        static bool sShowExceptionNotice = true;

        static bool sPreLoaded = false;
        static bool sFirstSave = true;

        static List<StoredNotice> sNotices = new List<StoredNotice>();

        static int sLoadupCount = 0;

        static int sLogEnumerator = 0;

        static DateTime sStartupDate = DateTime.MinValue;
        static DateTime sPreLoadDate = DateTime.MinValue;
        static DateTime sLoadFinishedDate = DateTime.MinValue;

        static ulong sBuildBuyLotId = 0;

        public static readonly string NewLine = System.Environment.NewLine;

        static Common()
        { }
        public Common()
        { }

        public static bool LoadLogging
        {
            get
            {
                if (!kDebugging) return false;

                if (!sEnableLoadLog) return false;

                return kLoadLogDebugging;
            }
        }

        protected static void Bootstrap()
        {
            try
            {
                World.sOnStartupAppEventHandler += OnStartupApp;
                LoadSaveManager.ObjectGroupsPreLoad += OnPreLoad;
                LoadSaveManager.ObjectGroupSaving += OnSaving;
                World.sOnWorldLoadFinishedEventHandler += OnWorldLoadFinished;
                World.sOnWorldQuitEventHandler += OnWorldQuit;
                World.sOnObjectPlacedInLotEventHandler += OnObjectPlacedInLot;

                LotManager.EnteringBuildBuyMode += OnEnteringBuildBuyMode;
                LotManager.ExitingBuildBuyMode += OnExitingBuildBuyMode;
            }
            catch(Exception e)
            {
                Exception("Bootstrap", e);
            }
        }

        public static string ParseStackTrace(string trace, string search, int offset)
        {
            if (string.IsNullOrEmpty(trace)) return null;

            int loc = trace.IndexOf(search);
            if (loc == -1) return null;

            return trace.Substring(loc + search.Length + offset, 8);
        }

        public static string RetrieveAddress(object obj)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                return ParseStackTrace(e.StackTrace, "RetrieveAddress (object) ([", 0);
            }
        }

        public static List<T> CloneList<T>(IEnumerable<T> old)
        {
            if (old == null) return null;

            return new List<T> (old);
        }

        public static string ObjectName(object obj)
        {
            return obj.GetType().ToString() + " - " + obj.GetType().Assembly.FullName.Replace(", Culture=neutral", "");
        }

        public static string TaskName(ITask task)
        {
            if (task == null) return null;

            string result = null;

            IScriptProxy proxy = task as IScriptProxy;
            if (proxy != null)
            {
                if (proxy.Target != null)
                {
                    result += ObjectName(proxy.Target);

                    OneShotFunction oneShot = proxy.Target as OneShotFunction;
                    if (oneShot != null)
                    {
                        if ((oneShot.mFunction != null) && (oneShot.mFunction.Method != null))
                        {
                            result = "OneShotFunction: " + oneShot.mFunction.Method.Name;
                            if (oneShot.mFunction.Target != null)
                            {
                                result += " - " + ObjectName(oneShot.mFunction.Target);
                            }
                        }
                    }
                    else
                    {
                        Sim sim = proxy.Target as Sim;
                        if (sim != null)
                        {
                            result += " - " + sim.FullName;
                        }
                        else
                        {
                            SimUpdate update = proxy.Target as SimUpdate;
                            if (update != null)
                            {
                                if (update.mSim != null)
                                {
                                    result += " - " + update.mSim.FullName;
                                }
                            }
                            else
                            {
                                Lot lot = proxy.Target as Lot;
                                if (lot != null)
                                {
                                    result += " - " + lot.Name + " - " + lot.Address;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                result += task.GetType().ToString();

                Sims3.Gameplay.OneShotFunctionTask oneGameplayShot = task as Sims3.Gameplay.OneShotFunctionTask;
                if (oneGameplayShot != null)
                {
                    if ((oneGameplayShot.mFunction != null) && (oneGameplayShot.mFunction.Method != null))
                    {
                        result = "OneShotFunctionTask: " + oneGameplayShot.mFunction.Method.Name;
                        if (oneGameplayShot.mFunction.Target != null)
                        {
                            result += " - " + ObjectName(oneGameplayShot.mFunction.Target);
                        }
                    }
                }
                else
                {
                    Sims3.UI.OneShotFunctionTask oneUIShot = task as Sims3.UI.OneShotFunctionTask;
                    if (oneUIShot != null)
                    {
                        if ((oneUIShot.mFunction != null) && (oneUIShot.mFunction.Method != null))
                        {
                            result = "OneShotFunctionTask: " + oneUIShot.mFunction.Method.Name;
                            if (oneUIShot.mFunction.Target != null)
                            {
                                result += " - " + ObjectName(oneUIShot.mFunction.Target);
                            }
                        }
                    }
                    else if (result == "NRaas.Common+FunctionTask")
                    {
                        result += ": " + task.ToString();
                    }
                }
            }
            return result;
        }

        private static void OnStartupApp(object sender, EventArgs args)
        {
            try
            {
                sStartupDate = DateTime.Now;

                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnStartupApp"))
                {
                    List<IStartupApp> helpers = DerivativeSearch.Find<IStartupApp>();
                    foreach (IStartupApp helper in helpers)
                    {
                        using (TestSpan helperSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IStartupApp"))// + helper.GetType()))
                        {
                            try
                            {
                                helper.OnStartupApp();

                                LoadLogger.Append("StartupApp " + helper.GetType().ToString());
                            }
                            catch (Exception e)
                            {
                                Exception(helper.GetType().ToString(), e);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Exception("OnStartupApp", e);
            }
        }

        private static void OnPreLoad()
        {
            try
            {
                if (sPreLoaded) return;
                sPreLoaded = true;

                sPreLoadDate = DateTime.Now;

                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnPreLoad"))
                {
                    List<IPreLoad> helpers = DerivativeSearch.Find<IPreLoad>();
                    foreach (IPreLoad helper in helpers)
                    {
                        using (TestSpan helperSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IPreLoad "))// + helper.GetType()))
                        {
                            try
                            {
                                helper.OnPreLoad();

                                LoadLogger.Append("PreLoad " + helper.GetType().ToString());
                            }
                            catch (Exception e)
                            {
                                Exception(helper.GetType().ToString(), e);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Exception("OnPreLoad", e);
            }
        }

        private static void OnEnteringBuildBuyMode()
        {
            try
            {
                sBuildBuyLotId = LotManager.sActiveBuildBuyLot.LotId;

                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnEnteringBuildBuyMode"))
                {
                    List<IEnterBuildBuy> helpers = DerivativeSearch.Find<IEnterBuildBuy>();
                    foreach (IEnterBuildBuy helper in helpers)
                    {
                        using (TestSpan helperSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IEnterBuildBuy "))// + helper.GetType()))
                        {
                            try
                            {
                                helper.OnEnterBuildBuy(LotManager.sActiveBuildBuyLot);

                                LoadLogger.Append("EnterBuildBuy " + helper.GetType().ToString());
                            }
                            catch (Exception e)
                            {
                                Exception(helper.GetType().ToString(), e);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Exception("OnEnteringBuildBuyMode", e);
            }
        }

        private static void OnExitingBuildBuyMode()
        {
            try
            {
                Lot lot = LotManager.GetLot(sBuildBuyLotId);
                if (lot == null) return;

                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnExitingBuildBuyMode"))
                {
                    List<IExitBuildBuy> helpers = DerivativeSearch.Find<IExitBuildBuy>();
                    foreach (IExitBuildBuy helper in helpers)
                    {
                        using (TestSpan helperSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IExitBuildBuy "))// + helper.GetType()))
                        {
                            try
                            {
                                helper.OnExitBuildBuy(lot);

                                LoadLogger.Append("ExitBuildBuy " + helper.GetType().ToString());
                            }
                            catch (Exception e)
                            {
                                Exception(helper.GetType().ToString(), e);
                            }
                        }
                    }

                    AddAllInteractions(lot.LotId, true);
                }
            }
            catch (Exception e)
            {
                Exception("OnExitingBuildBuyMode", e);
            }
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            try
            {
                sFirstSave = true;
                sLoadFinishedDate = DateTime.Now;

                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnWorldLoadFinished"))
                {
                    // Perform prior to calling the derivatives
                    AddAllInteractions();

                    List<IWorldLoadFinished> helpers = DerivativeSearch.Find<IWorldLoadFinished>();

                    foreach (IWorldLoadFinished helper in helpers)
                    {
                        using (TestSpan loadSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IWorldLoadFinished "))// + helper.GetType()))
                        {
                            try
                            {
                                LoadLogger.Append("WorldLoad " + helper.GetType().ToString());

                                helper.OnWorldLoadFinished();
                            }
                            catch (Exception exception)
                            {
                                Exception(helper.GetType().ToString(), exception);
                            }
                        }
                    }
                }

                sLoadupCount = RecordErrors();

                new DelayedEventListener(EventTypeId.kBoughtObject, OnNewObject);
                new DelayedEventListener(EventTypeId.kSimInstantiated, OnNewSim);

                new AlarmTask(1f, TimeUnit.Seconds, OnDelayedLoadFinished);

                new AlarmTask(2f, TimeUnit.Minutes, OnNoticeAlarm, 10f, TimeUnit.Minutes);
            }
            catch (Exception exception)
            {
                Exception("OnWorldLoadFinished", exception);
            }
        }

        private static void OnWorldQuit(object sender, EventArgs e)
        {
            try
            {
                sNotices = new List<StoredNotice>();

                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnWorldQuit", DebugLevel.Stats))
                {
                    AlarmTask.DisposeAll();

                    List<IWorldQuit> helpers = DerivativeSearch.Find<IWorldQuit>();
                    foreach (IWorldQuit helper in helpers)
                    {
                        using (TestSpan loadSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IWorldQuit "))// + helper.GetType()))
                        {
                            try
                            {
                                LoadLogger.Append("WorldQuit " + helper.GetType().ToString());

                                helper.OnWorldQuit();
                            }
                            catch (Exception exception)
                            {
                                Exception(helper.GetType().ToString(), exception);
                            }
                        }
                    }
                }

                RecordErrors();
            }
            catch (Exception exception)
            {
                WriteLog(exception);
            }
        }

        protected static void OnSaving(IScriptObjectGroup group)
        {
            try
            {
                if (sFirstSave)
                {
                    sFirstSave = false;

                    using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnPreSave", DebugLevel.Stats))
                    {
                        List<IPreSave> helpers = DerivativeSearch.Find<IPreSave>();
                        foreach (IPreSave helper in helpers)
                        {
                            using (TestSpan loadSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IPreSave "))// + helper.GetType()))
                            {
                                try
                                {
                                    LoadLogger.Append("Saving " + helper.GetType().ToString());

                                    helper.OnPreSave();
                                }
                                catch (Exception exception)
                                {
                                    Exception(helper.GetType().ToString(), exception);
                                }
                            }
                        }
                    }

                    new AlarmTask(1f, TimeUnit.Seconds, OnAfterSave);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSaving", e);
            }
        }

        protected static void OnAfterSave()
        {
            sFirstSave = true;

            using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnPostSave", DebugLevel.Stats))
            {
                List<IPostSave> helpers = DerivativeSearch.Find<IPostSave>();
                foreach (IPostSave helper in helpers)
                {
                    using (TestSpan loadSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IPostSave "))// + helper.GetType()))
                    {
                        try
                        {
                            LoadLogger.Append("Saving " + helper.GetType().ToString());

                            helper.OnPostSave();
                        }
                        catch (Exception exception)
                        {
                            Exception(helper.GetType().ToString(), exception);
                        }
                    }
                }
            }
        }

        protected static void OnObjectPlacedInLot(object sender, EventArgs e)
        {
            GameObject obj = null;

            try
            {
                World.OnObjectPlacedInLotEventArgs args = e as World.OnObjectPlacedInLotEventArgs;
                if (args != null)
                {
                    obj = GameObject.GetObject(args.ObjectId);
                    if (obj != null)
                    {
                        InteractionInjectorList.MasterList.Perform(obj);
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(obj, exception);
            }
        }

        protected static void OnNewObject(Event e)
        {
            InteractionInjectorList.MasterList.Perform(e.TargetObject as GameObject);
        }

        private static void OnNewSim(Event e)
        {
            InteractionInjectorList.MasterList.Perform(e.TargetObject as Sim);
        }

        // Externalized to ErrorTrap
        public static int ExternalRecordErrors(System.Text.StringBuilder results)
        {
            StringBuilder builder = new StringBuilder();
            int count = RecordErrors(builder, true);
            results.Append(builder.ToString());

            return count;
        }

        public static int RecordErrors()
        {
            StringBuilder builder = new StringBuilder();

            int count = RecordErrors(builder, false);

            if (builder.Count > 0)
            {
                WriteLog(builder.ToString());
            }

            return count;
        }
        public static int RecordErrors(StringBuilder builder, bool external)
        {
            List<ILogger> loggers = DerivativeSearch.Find<ILogger>();

            List<string> logs = new List<string>();

            int count = 0;

            foreach (ILogger logger in loggers)
            {
                if ((logger is IExternalLogger) != external) continue;

                count += logger.Log(builder);
            }

            if ((count > 0) && (!external))
            {
                try
                {
                    builder.Append(Common.NewLine + "Loaded Assemblies:");

                    List<string> assemblies = new List<string>();
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        string version = "";

                        try
                        {
                            Type type = assembly.GetType("NRaas.VersionStamp");
                            if (type != null)
                            {
                                FieldInfo versionField = type.GetField("sVersion", BindingFlags.Static | BindingFlags.Public);
                                if (versionField != null)
                                {
                                    version = ": Version " + versionField.GetValue(null);
                                }
                            }
                        }
                        catch
                        { }

                        assemblies.Add(assembly.FullName.Replace(", Culture=neutral", version));
                    }

                    assemblies.Sort();

                    foreach (string assembly in assemblies)
                    {
                        builder.Append(Common.NewLine + " " + assembly);
                    }
                }
                catch
                { }
            }

            return count;
        }

        private static void OnDelayedLoadFinished()
        {
            try
            {
                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "OnDelayedLoadFinished", DebugLevel.Stats))
                {
                    List<IDelayedWorldLoadFinished> helpers = DerivativeSearch.Find<IDelayedWorldLoadFinished>();

                    foreach (IDelayedWorldLoadFinished helper in helpers)
                    {
                        using (TestSpan helperSpan = new TestSpan(ExternalTimeSpanLogger.sLogger, "IDelayedWorldLoadFinished "))// + helper.GetType()))
                        {
                            try
                            {
                                LoadLogger.Append("DelayedLoad " + helper.GetType().ToString());

                                helper.OnDelayedWorldLoadFinished();
                            }
                            catch (Exception exception)
                            {
                                Exception(helper.GetType().ToString(), exception);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
        }

        private static void OnNoticeAlarm()
        {
            try
            {
                int exceptionCount = RecordErrors() + sLoadupCount;
                sLoadupCount = 0;

                if (sNotices != null)
                {
                    List<StoredNotice> notices = sNotices;

                    sNotices = null;

                    foreach (StoredNotice notice in notices)
                    {
                        Notify(notice);
                    }
                }

                if (exceptionCount > 0)
                {
                    Notify(Localize("Root:MenuName").Replace("...", "") + " Recorded: " + exceptionCount);
                }
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
        }

        public static bool IsAwayFromHomeworld()
        {
            return ((GameUtils.IsOnVacation()) || (GameUtils.IsUniversityWorld ()) || (GameUtils.IsFutureWorld()));
        }

        public static bool IsOnTrueVacation()
        {
            if (!GameStates.IsOnVacation) return false;

            switch(GameUtils.GetCurrentWorld())
            {
                case WorldName.China:
                case WorldName.Egypt:
                case WorldName.France:
                case WorldName.University:
                    return true;
            }

            return false;
        }

        protected static void AddDelayedAllInteractions()
        {
            AddAllInteractions(0, true);
        }

        public static void AddAllInteractions()
        {
            if (kDisableAllInteractionInjection) return;

            AddAllInteractions(0, false);

            Lot activeLot = LotManager.ActiveLot;
            if (activeLot != null)
            {
                AddAllInteractions(activeLot.LotId, false);
            }

            if ((Household.ActiveHousehold != null) && (Household.ActiveHousehold.LotHome != null) && (Household.ActiveHousehold.LotHome != activeLot))
            {
                AddAllInteractions(Household.ActiveHousehold.LotHome.LotId, false);
            }

            FunctionTask.Perform(AddDelayedAllInteractions);
        }
        protected static void AddAllInteractions(ulong lotId, bool full)
        {
            InteractionInjectorList list = InteractionInjectorList.MasterList;
            if (list.IsEmpty) return;

            bool localFull = full & (lotId == 0);

            foreach (KeyValuePair<Type,List<IInteractionInjector>> type in list.Types)
            {
                Array results = null;

                if (lotId == 0)
                {
                    if (!full)
                    {
                        if (!InteractionInjectorList.IsAlwaysType(type.Key)) continue;
                    }
                    else
                    {
                        SpeedTrap.Sleep();
                    }

                    results = Sims3.SimIFace.Queries.GetObjects(type.Key);
                }
                else
                {
                    if (!full)
                    {
                        // These would have been handled by the (lotId == 0) load
                        if (InteractionInjectorList.IsAlwaysType(type.Key)) continue;
                    }

                    results = Sims3.SimIFace.Queries.GetObjects(type.Key, lotId);
                }
                if (results == null) continue;

                int count = 0;

                foreach (GameObject obj in results)
                {
                    using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "AddInteraction"))
                    {
                        try
                        {
                            list.Perform(obj);
                        }
                        catch (Exception exception)
                        {
                            Exception(obj.GetType().ToString(), exception);
                        }
                    }

                    if (localFull)
                    {
                        count++;
                        if (count > 250)
                        {
                            SpeedTrap.Sleep();
                            count = 0;
                        }
                    }
                }
            }
        }

        public static string LocalizeEAString(string key)
        {
            return LocalizeEAString(false, key, new object[0]);
        }
        public static string LocalizeEAString(bool isFemale, string key)
        {
            return LocalizeEAString(isFemale, key, new object[0]);
        }
        public static string LocalizeEAString(bool isFemale, string key, object[] parameters)
        {
            string result = null;
            try
            {
                result = Localization.LocalizeString(isFemale, key, parameters);
            }
            catch (Exception e)
            {
                Common.Exception(key, e);
            }

            if (string.IsNullOrEmpty(result))
            {
                return key;
            }
            else
            {
                return result;
            }
        }

        public static bool IsAutonomous(Sim sim)
        {
            if (sim == null) return false;

            if (sim.InteractionQueue == null) return false;

            InteractionInstance interaction = sim.InteractionQueue.GetCurrentInteraction();
            if (interaction == null) return false;

            return interaction.Autonomous;
        }

        public static string Localize(string key)
        {
            return Localize(key, false, new object[0]);
        }
        public static string Localize(string key, bool isFemale)
        {
            return Localize(key, isFemale, new object[0]);
        }
        public static string Localize(string key, bool isFemale, object[] parameters)
        {
            return Localize(key, isFemale, false, parameters);
        }
        public static string Localize(string key, bool isActorFemale, bool isTargetFemale, object[] parameters)
        {
            string result;
            if (Localize(key, isActorFemale, isTargetFemale, parameters, out result))
            {
                return result;
            }
            /*
            else if (kDebugging)
            {
                return VersionStamp.sNamespace + "." + key + " (0x" + ResourceUtils.HashString64(VersionStamp.sNamespace + "." + key).ToString("X16") + ")";
            }
            */
            else
            {
                return VersionStamp.sNamespace + "." + key;
            }
        }
        public static bool Localize(string key, bool isFemale, object[] parameters, out string result)
        {
            return Localize(key, isFemale, false, parameters, out result);
        }
        public static bool Localize(string key, bool isActorFemale, bool isTargetFemale, object[] parameters, out string result)
        {
            result = null;

            try
            {
                result = Localization.LocalizeString(new bool[] { isActorFemale, isTargetFemale }, VersionStamp.sNamespace + "." + key, parameters);
            }
            catch (Exception e)
            {
                Common.Exception(key, e);
            }

            if (string.IsNullOrEmpty(result)) return false;

            if (result.StartsWith(VersionStamp.sNamespace + ".")) return false;

            return true;
        }

        public static bool DebugStackLog(string msg)
        {
            return DebugStackLog(new StringBuilder(msg));
        }
        public static bool DebugStackLog(StringBuilder msg)
        {
            if (!kDebugging) return false;

            return StackLog(msg);
        }

        public static bool StackLog(StringBuilder msg)
        {
            StackTrace trace = new StackTrace(false);

            StringBuilder text = new StringBuilder(msg);

            foreach (StackFrame frame in trace.GetFrames())
            {
                text += Common.NewLine + frame.GetMethod().DeclaringType.FullName + " : " + frame.GetMethod();
            }

            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                return Exception(text, e);
            }
        }

        public static bool DebugException(StringBuilder text, Exception exception)
        {
            if (!kDebugging) return false;

            return DebugException(text.ToString(), exception);
        }
        public static bool DebugException(string text, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception("DebugException: " + text, exception);
        }
        public static bool DebugException(SimDescription desc, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception(desc, null, "DebugException", exception);
        }
        public static bool DebugException(SimDescription a, SimDescription b, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception(a, b, "DebugException", exception);
        }
        public static bool DebugException(SimDescription a, SimDescription b, StringBuilder additional, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception(a, b, "DebugException" + Common.NewLine + additional, exception);
        }
        public static bool DebugException(IScriptLogic obj, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception(obj, null, "DebugException", exception);
        }
        public static bool DebugException(IScriptLogic a, IScriptLogic b, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception(a, b, "DebugException", exception);
        }
        public static bool DebugException(IScriptLogic a, IScriptLogic b, StringBuilder additional, Exception exception)
        {
            if (!kDebugging) return false;

            return Exception(a, b, "DebugException" + Common.NewLine + additional, exception);
        }

        public static GreyedOutTooltipCallback DebugTooltip(string text)
        {
            if (Common.kDebugging)
            {
                return delegate { return text; };
            }
            else
            {
                return null;
            }
        }

        public static void DebugNotify(string message)
        {
            if (!kDebugging) return;

            Notify(message, ObjectGuid.InvalidObjectGuid, true);
        }
        public static void DebugNotify(StringBuilder message)
        {
            if (!kDebugging) return;

            Notify(message.ToString(), ObjectGuid.InvalidObjectGuid, true);
        }
        public static void DebugNotify(string text, SimDescription sim)
        {
            if (sim == null)
            {
                DebugNotify(text, null as Sim);
            }
            else
            {
                DebugNotify(text, sim.CreatedSim);
            }
        }
        public static void DebugNotify(StringBuilder message, Sim sim)
        {
            if (!kDebugging) return;

            DebugNotify(message.ToString(), sim);
        }
        public static void DebugNotify(string message, Sim sim)
        {
            if (!kDebugging) return;

            ObjectGuid guid = ObjectGuid.InvalidObjectGuid;
            if (sim != null)
            {
                guid = sim.ObjectId;
            }

            Notify(message, guid, true);
        }
        public static void DebugNotify(string message, ObjectGuid id)
        {
            if (!kDebugging) return;

            Notify(message, id, true);
        }
        public static void DebugNotify(string text, Sim sim1, Sim sim2)
        {
            if (!Common.kDebugging) return;

            ObjectGuid id1 = ObjectGuid.InvalidObjectGuid;
            if (sim1 != null)
            {
                id1 = sim1.ObjectId;
            }

            ObjectGuid id2 = ObjectGuid.InvalidObjectGuid;
            if (sim2 != null)
            {
                id2 = sim2.ObjectId;
            }

            Notify(text, id1, id2, StyledNotification.NotificationStyle.kDebugAlert);
        }
        public static void DebugNotify(string text, ObjectGuid id1, ObjectGuid id2)
        {
            if (!Common.kDebugging) return;

            Notify(text, id1, id2, StyledNotification.NotificationStyle.kDebugAlert);
        }

        public static void Notify(StringBuilder text)
        {
            Notify(text.ToString());
        }
        public static void Notify(string text)
        {
            Notify(text, ObjectGuid.InvalidObjectGuid, true);
        }
        public static void Notify(string text, ObjectGuid id)
        {
            Notify(text, id, false);
        }
        public static void Notify(string text, ObjectGuid a, ObjectGuid b)
        {
            Notify(text, a, b, StyledNotification.NotificationStyle.kGameMessagePositive);
        }
        public static void Notify(string text, ObjectGuid id, bool debugging)
        {
            if (string.IsNullOrEmpty(text)) return;

            StyledNotification.NotificationStyle style = StyledNotification.NotificationStyle.kGameMessagePositive;
            if (debugging)
            {
                style = StyledNotification.NotificationStyle.kDebugAlert;
            }

            Notify(text, ObjectGuid.InvalidObjectGuid, id, style);
        }
        public static void Notify(string text, ObjectGuid id1, ObjectGuid id2, StyledNotification.NotificationStyle style)
        {
            Notify(text, id1, id2, style, null, ProductVersion.BaseGame);
        }
        public static void Notify(string text, ObjectGuid id1, ObjectGuid id2, StyledNotification.NotificationStyle style, string overrideImage, ProductVersion overrideVersion)
        {
            if (string.IsNullOrEmpty(text)) return;

            StoredNotice notice = new StoredNotice(text, id1, id2, style, overrideImage, overrideVersion);
            if (sNotices != null)
            {
                sNotices.Add(notice);
            }
            else
            {
                Notify(notice);
            }
        }
        public static void Notify(SimDescription me, string text)
        {
            if (me.CreatedSim != null)
            {
                Notify(me.CreatedSim, text);
            }
            else
            {
                Notify(me.FullName + ": " + text);
            }
        }
        public static void Notify(SimDescription a, SimDescription b, string text)
        {
            Sim simA = null;
            if (a != null)
            {
                simA = a.CreatedSim;
            }

            Sim simB = null;
            if (b != null)
            {
                simB = b.CreatedSim;
            }

            Notify(simA, simB, text);
        }
        public static void Notify(Sim me, string text)
        {
            if (me == null)
            {
                Notify(text);
            }
            else
            {
                Notify(text, me.ObjectId);
            }
        }
        public static void Notify(Sim a, Sim b, string text)
        {
            ObjectGuid idA = ObjectGuid.InvalidObjectGuid;
            if (a != null)
            {
                idA = a.ObjectId;
            }

            ObjectGuid idB = ObjectGuid.InvalidObjectGuid;
            if (b != null)
            {
                idB = b.ObjectId;
            }

            Notify(text, idA, idB);
        }
        protected static void Notify(StoredNotice notice)
        {
            try
            {
                sShowExceptionNotice = false;

                if (sNotices != null)
                {
                    sNotices.Add(notice);
                }
                else
                {
                    StyledNotification.NotificationStyle style = notice.mStyle;

                    bool lesson = (style == StyledNotification.NotificationStyle.kDebugAlert);
                    if (lesson)
                    {
                        style = StyledNotification.NotificationStyle.kSystemMessage;
                    }

                    StyledNotification.Format format = new StyledNotification.Format(notice.mText, notice.mID1, notice.mID2, style);

                    format.mConnectionType = StyledNotification.ConnectionType.kSpeech;

                    if (lesson)
                    {
                        format.mTNSCategory = NotificationManager.TNSCategory.Lessons;
                    }

                    StyledNotification.Show(format, notice.mOverrideImage, null, notice.mOverrideVersion, ProductVersion.BaseGame);
                }
            }
            catch
            { }
            finally
            {
                sShowExceptionNotice = true;
            }
        }

        public static bool DebugWriteLog(StringBuilder text)
        {
            if (!kDebugging) return false;

            return WriteLog(text, true);
        }
        public static bool DebugWriteLog(string text)
        {
            if (!kDebugging) return false;

            return WriteLog(text, true);
        }

        private static bool WriteLog(Exception e)
        {
            if (e == null) return false;

            return WriteLog(e.Message + Common.NewLine + e.StackTrace);
        }
        public static bool WriteLog(string text)
        {
            return WriteLog(text, true);
        }
        public static bool WriteLog(StringBuilder text)
        {
            if (text == null) return false;

            return WriteLog(text.ToString());
        }
        public static bool WriteLog(StringBuilder text, bool addHeader)
        {
            if (text == null) return false;

            return WriteLog(text.ToString(), addHeader);
        }
        public static bool WriteLog(string text, bool addHeader)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return false;

                if (addHeader)
                {
                    sLogEnumerator++;

                    string[] labels = GameUtils.GetGenericString(GenericStringID.VersionLabels).Split(new char[] { '\n' });
                    string[] data = GameUtils.GetGenericString(GenericStringID.VersionData).Split(new char[] { '\n' });

                    string header = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Common.NewLine;
                    header += "<" + VersionStamp.sNamespace + ">" + Common.NewLine;
                    header += "<ModVersion value=\"" + VersionStamp.sVersion + "\"/>" + Common.NewLine;

                    int num2 = (labels.Length > data.Length) ? data.Length : labels.Length;
                    for (int j = 0x0; j < num2; j++)
                    {
                        string label = labels[j].Replace(":", "").Replace(" ", "");

                        switch (label)
                        {
                            //case "GameVersion":
                            case "BuildVersion":
                                header += "<" + label + " value=\"" + data[j] + "\"/>" + Common.NewLine;
                                break;
                        }
                    }

                    IGameUtils utils = (IGameUtils)AppDomain.CurrentDomain.GetData("GameUtils");
                    if (utils != null)
                    {
                        ProductVersion version = (ProductVersion)utils.GetProductFlags();

                        header += "<Installed value=\"" + version + "\"/>" + Common.NewLine;
                    }

                    header += "<Enumerator value=\"" + sLogEnumerator + "\"/>" + Common.NewLine;
                    header += "<Content>" + Common.NewLine;

                    text = header + text.Replace("&", "&amp;");//.Replace(Common.NewLine, "  <br />" + Common.NewLine);

                    text += Common.NewLine + "</Content>";
                    text += Common.NewLine + "</" + VersionStamp.sNamespace + ">";
                }

                uint fileHandle = 0x0;
                string str = Simulator.CreateScriptErrorFile(ref fileHandle);
                if (fileHandle != 0x0)
                {
                    CustomXmlWriter xmlWriter = new CustomXmlWriter(fileHandle);

                    xmlWriter.WriteToBuffer(text);

                    xmlWriter.WriteEndDocument();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Exception(string text, Exception exception)
        {
            return Exception(new StringBuilder(text), new StringBuilder(text), ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, exception);
        }
        public static bool Exception(StringBuilder text, Exception exception)
        {
            return Exception(text, text, ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, exception);
        }
        public static bool Exception(object a, object b, string additionalInfo, Exception exception)
        {
            return Exception(a, b, new StringBuilder(additionalInfo), exception);
        }
        public static bool Exception(object a, object b, StringBuilder additionalInfo, Exception exception)
        {
            if (a is IMiniSimDescription)
            {
                return Exception(a as IMiniSimDescription, b as IMiniSimDescription, additionalInfo, exception);
            }
            else if ((a is IScriptLogic) || (b is IScriptLogic))
            {
                return Exception(a as IScriptLogic, b as IScriptLogic, additionalInfo, exception);
            }
            else
            {
                if (b != null)
                {
                    additionalInfo = new StringBuilder(b.ToString() + " (" + b.GetType() + ")" + Common.NewLine, additionalInfo);
                }

                if (a != null)
                {
                    additionalInfo = new StringBuilder(a.ToString() + " (" + a.GetType() + ")" + Common.NewLine, additionalInfo);
                }

                return Exception((IScriptLogic)null, (IScriptLogic)null, additionalInfo, exception);
            }
        }

        public static bool Exception(IMiniSimDescription desc, Exception exception)
        {
            return Exception(desc, null, exception);
        }
        public static bool Exception(IMiniSimDescription a, IMiniSimDescription b, Exception exception)
        {
            return Exception(a, b, null, exception);
        }
        public static bool Exception(IMiniSimDescription a, IMiniSimDescription b, StringBuilder additionalInfo, Exception exception)
        {
            StringBuilder noticeText = new StringBuilder();
            StringBuilder logText = new StringBuilder(additionalInfo);

            IGameObject objA = ExceptionLogger.Convert(a, noticeText, logText);
            IGameObject objB = ExceptionLogger.Convert(b, noticeText, logText);

            ObjectGuid idA = ObjectGuid.InvalidObjectGuid, idB = ObjectGuid.InvalidObjectGuid;

            if (objA != null)
            {
                idA = objA.ObjectId;
            }

            if (objB != null)
            {
                idB = objB.ObjectId;
            }

            return Exception(noticeText, logText, idA, idB, exception);
        }

        public static bool Exception(IScriptProxy obj, Exception exception)
        {
            return Exception((obj != null) ? obj.Target : null, null, exception);
        }
        public static bool Exception(IScriptLogic obj, Exception exception)
        {
            return Exception(obj, null, exception);
        }
        public static bool Exception(IScriptLogic a, IScriptLogic b, Exception exception)
        {
            return Exception(a, b, null, exception);
        }
        public static bool Exception(IScriptLogic a, IScriptLogic b, StringBuilder additionalInfo, Exception exception)
        {
            StringBuilder noticeText = new StringBuilder();
            StringBuilder logText = new StringBuilder(additionalInfo);

            if (logText.Count > 0)
            {
                logText += Common.NewLine;
            }

            IGameObject objA = ExceptionLogger.Convert(a, noticeText, logText);
            IGameObject objB = ExceptionLogger.Convert(b, noticeText, logText);

            ObjectGuid idA = ObjectGuid.InvalidObjectGuid, idB = ObjectGuid.InvalidObjectGuid;

            if (objA != null)
            {
                idA = objA.ObjectId;
            }

            if (objB != null)
            {
                idB = objB.ObjectId;
            }

            return Exception(noticeText, logText, idA, idB, exception);
        }

        private static bool Exception(StringBuilder noticeText, StringBuilder logText, ObjectGuid id1, ObjectGuid id2, Exception exception)
        {
            try
            {
                if (exception is ResetException)
                {
                    try
                    {
                        throw new Exception();
                    }
                    catch (Exception newException)
                    {
                        exception = newException;
                    }
                }

                ExceptionLogger.Append(noticeText, logText, id1, id2, exception, null);
                return true;
            }
            catch(Exception e)
            {
                WriteLog(exception.Message + Common.NewLine + exception.StackTrace + Common.NewLine + Common.NewLine + e.Message + Common.NewLine + e.StackTrace);
                return false;
            }
        }

        public static bool IsRootMenuObject(IGameObject obj)
        {
            if (obj is Lot)
            {
                Lot lot = obj as Lot;
                if (lot.IsBaseCampLotType)
                {
                    return true;
                }
            }
            else if (obj is Sims3.Gameplay.Objects.Electronics.Computer)
            {
                return true;
            }
            else if (obj is Sims3.Gameplay.Objects.RabbitHoles.CityHall)
            {
                return true;
            }
            else if (obj is Sims3.Gameplay.Objects.RabbitHoles.AdminstrationCenter)
            {
                return true;
            }
            else if (obj is Sims3.Gameplay.Objects.RabbitHoles.ComboCityhallPoliceMilitary)
            {
                return true;
            }
            return false;
        }

        public static void RemoveInteraction<T>(GameObject obj)
            where T : InteractionDefinition
        {
            RemoveInteraction(obj, typeof(T));
        }
        public static void RemoveInteraction(GameObject obj, Type type)
        {
            if (obj.Interactions != null)
            {
                int index = 0;
                while (index < obj.Interactions.Count)
                {
                    InteractionObjectPair pair = obj.Interactions[index];

                    if (pair.InteractionDefinition.GetType() == type)
                    {
                        obj.Interactions.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            ItemComponent itemComp = obj.ItemComp;
            if (itemComp != null)
            {
                int index = 0;
                while (index < itemComp.InteractionsInventory.Count)
                {
                    InteractionObjectPair pair = itemComp.InteractionsInventory[index];

                    if (pair.InteractionDefinition.GetType() == type)
                    {
                        itemComp.InteractionsInventory.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        public interface IAddInteraction
        {
            void AddInteraction(Common.InteractionInjectorList interactions);
        }

        public interface IStartupApp
        {
            void OnStartupApp();
        }

        public interface IPreLoad
        {
            void OnPreLoad();
        }

        public interface IEnterBuildBuy
        {
            void OnEnterBuildBuy(Lot lot);
        }

        public interface IExitBuildBuy
        {
            void OnExitBuildBuy(Lot lot);
        }

        public interface IPreSave
        {
            void OnPreSave();
        }

        public interface IPostSave
        {
            void OnPostSave();
        }

        public interface IWorldLoadFinished
        {
            void OnWorldLoadFinished();
        }

        public interface IDelayedWorldLoadFinished
        {
            void OnDelayedWorldLoadFinished();
        }

        public interface IWorldQuit
        {
            void OnWorldQuit();
        }

        public interface ILogger
        {
            int Log(StringBuilder builder);
        }

        public interface IAddLogger
        {
            void Add(string value);
        }

        public interface IExternalLogger
        {
            int Log(StringBuilder builder);
        }

        public class InteractionInjectorList
        {
            static InteractionInjectorList sMasterList = null;

            public static List<Type> sAlwaysTypes;

            Dictionary<Type, List<IInteractionInjector>> mTypes = new Dictionary<Type, List<IInteractionInjector>>();

            static InteractionInjectorList()
            {
                sAlwaysTypes = new List<Type>();

                sAlwaysTypes.Add(typeof(Sim));
                sAlwaysTypes.Add(typeof(Lot));
                sAlwaysTypes.Add(typeof(Sims3.Gameplay.Objects.RabbitHoles.CityHall));
                sAlwaysTypes.Add(typeof(Sims3.Gameplay.Objects.RabbitHoles.ComboCityhallPoliceMilitary));
                sAlwaysTypes.Add(typeof(Computer));
                sAlwaysTypes.Add(typeof(Terrain));
            }

            public InteractionInjectorList(List<IAddInteraction> interactions)
            {
                foreach (IAddInteraction interaction in interactions)
                {
                    try
                    {
                        interaction.AddInteraction(this);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(interaction.GetType().ToString(), e);
                    }
                }
            }

            public static bool IsAlwaysType(Type type)
            {
                foreach (Type alwaysType in InteractionInjectorList.sAlwaysTypes)
                {
                    if (alwaysType.IsAssignableFrom(type))
                    {
                        return true;
                    }
                }

                return false;
            }

            public void AddInjector(IInteractionInjector injector)
            {
                foreach (Type type in injector.GetTypes())
                {
                    AddType(type, injector);
                }
            }

            protected void AddType(Type type, IInteractionInjector injector)
            {
                List<IInteractionInjector> injectors;
                if (!mTypes.TryGetValue(type, out injectors))
                {
                    injectors = new List<IInteractionInjector>();
                    mTypes.Add(type, injectors);
                }

                injectors.Add(injector);
            }

            public IEnumerable<KeyValuePair<Type,List<IInteractionInjector>>> Types
            {
                get { return mTypes; }
            }

            public bool IsEmpty
            {
                get { return (mTypes.Count == 0); }
            }

            public static InteractionInjectorList MasterList
            {
                get
                {
                    if (sMasterList == null)
                    {
                        List<IAddInteraction> addInteractions = DerivativeSearch.Find<IAddInteraction>();

                        foreach (IAddInteraction interaction in addInteractions)
                        {
                            LoadLogger.Append("AddInteraction " + interaction.GetType().ToString());
                        }

                        sMasterList = new InteractionInjectorList(addInteractions);
                    }

                    return sMasterList;
                }
            }

            public void AddRoot(InteractionDefinition definition)
            {
                AddInjector(new InteractionRootInjector(definition));
            }

            public void AddCustom(IInteractionInjector injector)
            {
                AddInjector(injector);
            }

            public void AddNoDupTest<OBJ_TYPE>(InteractionDefinition definition)
                where OBJ_TYPE : IGameObject
            {
                AddInjector(new InteractionNoDupTestInjector<OBJ_TYPE>(definition));
            }
            public void Add<OBJ_TYPE>(InteractionDefinition definition)
                where OBJ_TYPE : IGameObject
            {
                AddInjector(new InteractionInjector<OBJ_TYPE>(definition, true));
            }
            public void Replace<OBJ_TYPE,T>(InteractionDefinition definition)
                where OBJ_TYPE : IGameObject
                where T : InteractionDefinition
            {
                AddInjector(new InteractionReplacer<OBJ_TYPE, T>(definition, true));
            }
            public void ReplaceNoTest<OBJ_TYPE, T>(InteractionDefinition definition)
                where OBJ_TYPE : IGameObject
                where T : InteractionDefinition
            {
                AddInjector(new InteractionReplacer<OBJ_TYPE, T>(definition, false));
            }

            public void Perform(GameObject obj)
            {
                List<IInteractionInjector> injectors = new List<IInteractionInjector>();
                foreach (KeyValuePair<Type, List<IInteractionInjector>> type in mTypes)
                {
                    if (type.Key.IsAssignableFrom(obj.GetType()))
                    {
                        injectors.AddRange(type.Value);
                    }
                }

                if (injectors.Count == 0) return;

                Perform(obj, injectors);
            }
            protected void Perform(GameObject obj, IEnumerable<IInteractionInjector> injectors)
            {
                try
                {
                    if (obj == null) return;

                    if (kDisableLotMenu)
                    {
                        if (obj is Lot) return;
                    }

                    Dictionary<Type, bool> existing = new Dictionary<Type, bool>();

                    foreach (InteractionObjectPair pair in obj.Interactions)
                    {
                        if (pair.InteractionDefinition is IWasHereDefinition) return;

                        Type type = pair.InteractionDefinition.GetType();

                        existing[type] = true;
                    }

                    if (obj.ItemComp != null)
                    {
                        foreach (InteractionObjectPair pair in obj.ItemComp.InteractionsInventory)
                        {
                            if (pair.InteractionDefinition is IWasHereDefinition) return;

                            Type type = pair.InteractionDefinition.GetType();

                            existing[type] = true;
                        }
                    }

                    foreach (IInteractionInjector injector in injectors)
                    {
                        injector.Perform(obj, existing);
                    }

                    if (!kDisableLotMenu)
                    {
                        obj.AddInteraction(IWasHereDefinition.Singleton);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(obj, e);
                }
            }
        }

        public interface IInteractionInjector
        {
            List<Type> GetTypes();

            void Perform(GameObject obj, Dictionary<Type, bool> existing);
        }

        public class InteractionInjector<OBJ_TYPE> : IInteractionInjector
            where OBJ_TYPE : IGameObject
        {
            InteractionDefinition mDefinition;

            protected InteractionInjector()
            { }
            protected InteractionInjector(InteractionDefinition definition)
                : this(definition, true)
            { }
            public InteractionInjector(InteractionDefinition definition, bool throwError)
            {
                mDefinition = definition;
                if ((throwError) && (mDefinition == null))
                {
                    throw new NullReferenceException("mDefinition");
                }
            }

            public virtual List<Type> GetTypes()
            {
                List<Type> list = new List<Type>();
                list.Add(typeof(OBJ_TYPE));
                return list;
            }

            public void Perform(GameObject obj, Dictionary<Type, bool> existing)
            {
                Perform(obj, mDefinition, existing);
            }
            protected virtual bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (obj is OBJ_TYPE)
                {
                    Type type = definition.GetType();

                    if (existing.ContainsKey(type)) return false;
                    existing.Add(type, true);

                    obj.AddInteraction(definition);
                    obj.AddInventoryInteraction(definition);
                    return true;
                }

                return false;
            }
        }

        public class InteractionNoDupTestInjector<OBJ_TYPE> : InteractionInjector<OBJ_TYPE>
            where OBJ_TYPE : IGameObject
        {
            protected InteractionNoDupTestInjector()
            { }
            public InteractionNoDupTestInjector(InteractionDefinition definition)
                : base(definition)
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (obj is OBJ_TYPE)
                {
                    obj.AddInteraction(definition);
                    obj.AddInventoryInteraction(definition);
                    return true;
                }

                return false;
            }
        }

        public class InteractionRootInjector : InteractionInjector<GameObject>
        {
            public InteractionRootInjector(InteractionDefinition definition)
                : base(definition, true)
            { }

            public override List<Type> GetTypes()
            {
                List<Type> list = new List<Type>();
                list.Add(typeof(Lot));
                list.Add(typeof(Sims3.Gameplay.Objects.Electronics.Computer));
                list.Add(typeof(Sims3.Gameplay.Objects.RabbitHoles.CityHall));
                list.Add(typeof(Sims3.Gameplay.Objects.RabbitHoles.ComboCityhallPoliceMilitary));
                list.Add(typeof(Sims3.Gameplay.Objects.RabbitHoles.AdminstrationCenter));
                return list;
            }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (!IsRootMenuObject(obj)) return false;

                return base.Perform(obj, definition, existing);
            }
        }

        public class InteractionReplacer<OBJ_TYPE, T> : InteractionInjector<OBJ_TYPE>
            where OBJ_TYPE : IGameObject
            where T : InteractionDefinition
        {
            bool mTestExistence;

            public InteractionReplacer(InteractionDefinition definition, bool testExistence)
                : base(definition)
            {
                mTestExistence = testExistence;
            }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                Type type = typeof(T);

                if (mTestExistence)
                {
                    if (!existing.ContainsKey(type)) return false;
                }

                if (!base.Perform(obj, definition, existing)) return false;

                Common.RemoveInteraction(obj, type);
                existing.Remove(type);
                return true;
            }
        }

        public abstract class Logger<T> : ILogger
            where T : Logger<T>
        {
            StringBuilder mLog = new StringBuilder();
            int mCount = 0;

            static int sCounter = 0;

            protected abstract string Name
            {
                get;
            }

            protected abstract T Value
            {
                get;
            }

            protected int Count
            {
                get { return mCount; }
            }

            public void Clear()
            {
                mCount = 0;
                mLog = new StringBuilder();
            }

            public static string GetDescription(IMiniSimDescription obj)
            {
                if (obj == null)
                {
                    return null;
                }
                else
                {
                    string logText = null;

                    logText += Common.NewLine + "SimDescription:" + Common.NewLine;
                    logText += Common.NewLine + " Name: " + obj.FullName;
                    logText += Common.NewLine + " Age: " + obj.Age;
                    logText += Common.NewLine + " Gender: " + obj.Gender;
                    logText += Common.NewLine + " SimDescriptionId: " + obj.SimDescriptionId;
                    logText += Common.NewLine + " LotHomeId: " + obj.LotHomeId;
                    logText += Common.NewLine + " HomeWorld: " + obj.HomeWorld;

                    Genealogy genealogy = obj.CASGenealogy as Genealogy;
                    if (genealogy != null)
                    {
                        if ((genealogy.mSim != null) || (genealogy.mMiniSim != null))
                        {
                            logText += Common.NewLine + " Proper Genealogy";
                        }
                        else
                        {
                            logText += Common.NewLine + " Broken Genealogy";
                        }
                    }
                    else
                    {
                        logText += Common.NewLine + " No Genealogy";
                    }

                    if (obj.IsDead)
                    {
                        logText += Common.NewLine + " Dead";
                    }

                    if (obj.IsPregnant)
                    {
                        logText += Common.NewLine + " Pregnant";
                    }

                    SimDescription desc = obj as SimDescription;
                    if (desc != null)
                    {
                        logText += Common.NewLine + " Valid: " + desc.IsValidDescription;

                        logText += Common.NewLine + " Species: " + desc.Species;

                        if (desc.Household != null)
                        {
                            logText += Common.NewLine + " Household: " + desc.Household.Name;
                        }
                        else
                        {
                            logText += Common.NewLine + " No Household";
                        }

                        if ((desc.CreatedByService != null) && (desc.CreatedByService.IsSimDescriptionInPool(desc)))
                        {
                            logText += Common.NewLine + " Service: " + desc.CreatedByService.ServiceType;
                        }

                        if (desc.AssignedRole != null)
                        {
                            logText += Common.NewLine + " Role: " + desc.AssignedRole.Type;
                        }

                        if (desc.OccultManager != null)
                        {
                            logText += Common.NewLine + " Occult: " + desc.OccultManager.CurrentOccultTypes;
                        }
                        else
                        {
                            logText += Common.NewLine + " No OccultManager";
                        }

                        logText += Common.NewLine + " Alien: " + desc.AlienDNAPercentage;

                        try
                        {
                            if (desc.Occupation != null)
                            {
                                logText += Common.NewLine + " Career: " + desc.Occupation.CareerName + " (" + desc.Occupation.CareerLevel + ")";
                            }
                            else
                            {
                                logText += Common.NewLine + " Career: <Unemployed>";
                            }
                        }
                        catch 
                        { }

                        try
                        {
                            if ((desc.CareerManager != null) && (desc.CareerManager.School != null))
                            {
                                logText += Common.NewLine + " School: " + desc.CareerManager.School.CareerName + " (" + desc.CareerManager.School.CareerLevel + ")";
                            }
                            else
                            {
                                logText += Common.NewLine + " School: <None>";
                            }
                        }
                        catch
                        { }

                        if (desc.LotHome != null)
                        {
                            logText += Common.NewLine + " Lot: " + desc.LotHome.Name;
                            logText += Common.NewLine + " Address: " + desc.LotHome.Address;
                        }

                        if (desc.SkillManager != null)
                        {
                            foreach (Skill skill in desc.SkillManager.List)
                            {
                                if (skill == null) continue;

                                string name = skill.Guid.ToString();

                                try
                                {
                                    name = skill.Name;
                                }
                                catch
                                {}

                                logText += Common.NewLine + " Skill " + name + ": " + skill.SkillLevel;
                            }
                        }

                        if (desc.TraitManager != null)
                        {
                            foreach (Trait trait in desc.TraitManager.List)
                            {
                                if (trait == null) continue;

                                string name = trait.Guid.ToString();
                                try
                                {
                                    name = trait.TraitName(desc.IsFemale);
                                }
                                catch
                                {}

                                logText += Common.NewLine + " Trait " + name;
                            }
                        }
                    }

                    return logText;
                }
            }

            public static IGameObject Convert(IMiniSimDescription obj, StringBuilder noticeText, StringBuilder logText)
            {
                if (obj == null)
                {
                    return null;
                }
                else
                {
                    SimDescription desc = obj as SimDescription;
                    if ((desc != null) && (desc.CreatedSim != null))
                    {
                        return Convert(desc.CreatedSim, noticeText, logText);
                    }

                    noticeText += obj.FullName + Common.NewLine;

                    logText += GetDescription(obj);
                    return null;
                }
            }
            public static IGameObject Convert(IScriptLogic logic, StringBuilder noticeText, StringBuilder logText)
            {
                if (logic == null)
                {
                    return null;
                }
                else
                {
                    IGameObject obj = logic as IGameObject;

                    IGameObject result = obj;

                    if (obj != null)
                    {
                        noticeText += obj.GetLocalizedName() + Common.NewLine;

                        logText += Common.NewLine + "Object:" + Common.NewLine + obj.GetObjectLocationInformation();

                        logText += Common.NewLine + " IsValid: " + Sims3.SimIFace.Objects.IsValid(obj.ObjectId);
                        logText += Common.NewLine + " World: " + GameUtils.GetCurrentWorld();
                        logText += Common.NewLine + " OnVacation: " + GameStates.IsOnVacation;

                        if (SeasonsManager.Enabled)
                        {
                            logText += Common.NewLine + " Season: " + SeasonsManager.CurrentSeason;
                        }
                        else
                        {
                            logText += Common.NewLine + " Season: None";
                        }

                        if (obj.Parent != null)
                        {
                            noticeText += Common.NewLine + "Parent:";
                            logText += Common.NewLine + "Parent:";

                            Convert(obj.Parent, noticeText, logText);
                        }

                        Sim sim = obj as Sim;
                        if (sim != null)
                        {
                            if (sim.Posture != null)
                            {
                                logText += Common.NewLine + " Posture: " + sim.Posture.GetType();
                            }
                            else
                            {
                                logText += Common.NewLine + " Posture: (None)";
                            }

                            if (sim.Household != null)
                            {
                                logText += Common.NewLine + " Household: " + sim.Household.Name;

                                if (sim.LotHome != null)
                                {
                                    logText += Common.NewLine + " LotHome: " + sim.LotHome.Name;
                                }
                                else
                                {
                                    logText += Common.NewLine + " No LotHome";
                                }

                                if (sim == Sim.ActiveActor)
                                {
                                    logText += Common.NewLine + " Active Actor";
                                }
                                else if (sim.Household == Household.ActiveHousehold)
                                {
                                    logText += Common.NewLine + " Active Household";
                                }
                            }
                            else
                            {
                                logText += Common.NewLine + " No Household";
                            }

                            SimDescription simDesc = sim.SimDescription;
                            if (simDesc != null)
                            {
                                logText += GetDescription(simDesc);

                                // Doing this messes with the ErrorTrap reset
                                //result = null;
                            }

                            logText += Common.NewLine;

                            if (sim.InteractionQueue != null)
                            {
                                logText += Common.NewLine + "Interactions:";

                                int index = 0;
                                foreach (InteractionInstance instance in sim.InteractionQueue.InteractionList)
                                {
                                    if (instance == null)
                                    {
                                        logText += Common.NewLine + "Empty Interaction";
                                    }
                                    else
                                    {
                                        InteractionInstanceParameters parameters = instance.GetInteractionParameters();

                                        index++;

                                        try
                                        {
                                            logText += Common.NewLine + index + ": " + instance.GetInteractionName();
                                        }
                                        catch
                                        {
                                            logText += Common.NewLine + index + ": error";
                                        }

                                        if (instance.InteractionDefinition != null)
                                        {
                                            logText += Common.NewLine + instance.InteractionDefinition.GetType();
                                            logText += Common.NewLine + instance.InteractionDefinition;
                                            logText += Common.NewLine + instance.GetPriority().Level;
                                        }
                                        else
                                        {
                                            logText += Common.NewLine + "Invalid Definition";
                                        }
                                    }
                                }

                                logText += Common.NewLine;
                            }
                        }
                    }
                    else
                    {
                        Household household = logic as Household;
                        if (household != null)
                        {
                            noticeText += "Household: " + household.Name + Common.NewLine;

                            logText += "Household: " + household.Name + Common.NewLine;

                            if ((household.IsServiceNpcHousehold) || (household.IsPetHousehold))
                            {
                                logText += "Service" + Common.NewLine;
                            }
                            else if (household.IsTouristHousehold)
                            {
                                logText += "Tourist" + Common.NewLine;
                            }
                            else if (household.AllSimDescriptions != null)
                            {
                                foreach (SimDescription sim in household.AllSimDescriptions)
                                {
                                    if (sim == null) continue;

                                    logText += "Member: " + sim.FullName + Common.NewLine;
                                }
                            }

                            if (household.LotHome != null)
                            {
                                logText += "Lot: " + household.LotHome.Name + Common.NewLine;
                                logText += "Address: " + household.LotHome.Address + Common.NewLine;
                            }
                            else
                            {
                                logText += "No Lot Home" + Common.NewLine;
                            }
                        }
                        else
                        {
                            logText += "Proxy: " + logic + Common.NewLine;
                        }
                    }

                    return result;
                }
            }

            protected void PrivateAppend(StringBuilder noticeText, StringBuilder logText, ObjectGuid id1, ObjectGuid id2, Exception exception, Dictionary<string, bool> alreadyCaught)
            {
                if (logText.Count == 0)
                {
                    logText += "No Proxy";
                }

                try
                {
                    sCounter++;

                    logText += Common.NewLine + " Counter: " + sCounter;
                    logText += Common.NewLine + " Sim-Time: " + SimClock.CurrentTime();

                    if (sStartupDate != DateTime.MinValue)
                    {
                        logText += Common.NewLine + " Start-Time: " + sStartupDate;
                    }

                    if (sPreLoadDate != DateTime.MinValue)
                    {
                        logText += Common.NewLine + " PreLoadup-Time: " + sPreLoadDate;
                    }

                    if (sLoadFinishedDate != DateTime.MinValue)
                    {
                        logText += Common.NewLine + " Loadup-Time: " + sLoadFinishedDate;
                    }

                    logText += Common.NewLine + " Log-Time: " + DateTime.Now + Common.NewLine;
                }
                catch
                { }

                logText += Common.NewLine + exception.ToString() + Common.NewLine;

                string finalLogText = logText.ToString();
                PrivateAppend(finalLogText);

                if ((GameUtils.IsPaused()) || (!GameStates.IsLiveState))
                {
                    WriteLog(logText);
                }
                else
                {
                    DebugWriteLog(logText);
                }

                if (id1 == ObjectGuid.InvalidObjectGuid)
                {
                    ObjectGuid id = id2;
                    id2 = id1;
                    id1 = id;
                }

                bool showNotice = sShowExceptionNotice;
                if (alreadyCaught != null)
                {
                    if (alreadyCaught.ContainsKey(finalLogText))
                    {
                        showNotice = false;
                    }
                    else
                    {
                        alreadyCaught.Add(finalLogText, true);
                    }
                }

                if (showNotice)
                {
                    Notify(VersionStamp.sNamespace + Common.NewLine + "Script Error" + Common.NewLine + noticeText, id2, id1, StyledNotification.NotificationStyle.kDebugAlert);
                }
            }
            protected void PrivateAppend(string value)
            {
                if (string.IsNullOrEmpty(value)) return;

                mLog.Append(value + Common.NewLine);
                mCount++;
            }
            protected void PrivateAppend(StringBuilder value)
            {
                if (value.Count == 0) return;

                mLog.Append(value);
                mLog.Append(Common.NewLine);
                mCount++;
            }

            protected virtual int PrivateLog(StringBuilder builder)
            {
                string log = mLog.ToString();
                mLog = new StringBuilder();

                int count = mCount;
                mCount = 0;

                if (!string.IsNullOrEmpty(log))
                {
                    builder.Append(GetHeader());
                    builder.Append(log);
                }

                return count;
            }

            protected virtual string GetHeader()
            {
                return Common.NewLine + Common.NewLine + "-- " + Name + " --" + Common.NewLine + Common.NewLine;
            }

            public int Log(StringBuilder builder)
            {
                return Value.PrivateLog(builder);
            }
        }

        public class AlarmTask
        {
            Sims3.Gameplay.Function mFunction;

            AlarmHandle mHandle;

            ObjectGuid mRunningTask = ObjectGuid.InvalidObjectGuid;

            bool mDisposeOnTimer;

            static List<AlarmTask> sTasks = new List<AlarmTask>();

            protected AlarmTask(float time, TimeUnit timeUnit)
                : this(time, timeUnit, null)
            { }
            public AlarmTask(float time, TimeUnit timeUnit, Sims3.Gameplay.Function func)
                : this(func)
            {
                mHandle = AlarmManager.Global.AddAlarm(time, timeUnit, OnTimer, "NRaasDelayedFunction", AlarmType.NeverPersisted, null);

                mDisposeOnTimer = true;
            }
            public AlarmTask(float time, TimeUnit timeUnit, float repeatTime, TimeUnit repeatTimeUnit)
                : this(time, timeUnit, null, repeatTime, repeatTimeUnit)
            { }
            public AlarmTask(float time, TimeUnit timeUnit, Sims3.Gameplay.Function func, float repeatTime, TimeUnit repeatTimeUnit)
                : this(func)
            {
                mHandle = AlarmManager.Global.AddAlarmRepeating(time, timeUnit, OnTimer, repeatTime, repeatTimeUnit, "NRaasRepeatFunction", AlarmType.NeverPersisted, null);
            }
            public AlarmTask(float hourOfDay, DaysOfTheWeek days)
                : this(hourOfDay, days, null)
            { }
            public AlarmTask(float hourOfDay, DaysOfTheWeek days, Sims3.Gameplay.Function func)
                : this(func)
            {
                mHandle = AlarmManager.Global.AddAlarmDay(hourOfDay, days, OnTimer, "NRaasDailyFunction", AlarmType.NeverPersisted, null);
            }
            protected AlarmTask(Sims3.Gameplay.Function func)
            {
                sTasks.Add(this);

                if (func == null)
                {
                    func = OnPerform;
                }

                mFunction = func;
            }

            protected virtual void OnPerform()
            { }

            public bool Valid
            {
                get { return (mHandle != AlarmHandle.kInvalidHandle); }
            }

            public void Dispose()
            {
                Simulator.DestroyObject(mRunningTask);
                mRunningTask = ObjectGuid.InvalidObjectGuid;

                AlarmManager.Global.RemoveAlarm(mHandle);
                mHandle = AlarmHandle.kInvalidHandle;

                sTasks.Remove(this);
            }

            public static float TimeTo(float hourOfDay)
            {
                float time = hourOfDay - SimClock.HoursPassedOfDay;
                if (time < 0f)
                {
                    time += 24f;
                }

                return time;
            }

            public static void DisposeAll()
            {
                List<AlarmTask> tasks = new List<AlarmTask>(sTasks);
                foreach (AlarmTask task in tasks)
                {
                    task.Dispose();
                }

                sTasks.Clear();
            }

            private void OnTimer()
            {
                try
                {
                    if (mDisposeOnTimer)
                    {
                        Dispose();
                    }

                    mRunningTask = FunctionTask.Perform(mFunction);
                }
                catch (Exception e)
                {
                    Exception(mFunction.ToString(), e);
                }
            }

            public override string ToString()
            {
                string result = null;
                if (mFunction != null)
                {
                    result += mFunction.Method.ToString();
                    if (mFunction.Target != null)
                    {
                        result = mFunction.Target.GetType() + " : " + result;
                    }
                }

                return result;
            }
        }

        public class FunctionTask : Task
        {
            Sims3.Gameplay.Function mFunction;

            protected FunctionTask()
            {
                mFunction = OnPerform;
            }
            protected FunctionTask(Sims3.Gameplay.Function func)
            {
                mFunction = func;
            }

            public static ObjectGuid Perform(Sims3.Gameplay.Function func)
            {
                return new FunctionTask(func).AddToSimulator();
            }

            public ObjectGuid AddToSimulator()
            {
                return Simulator.AddObject(this);
            }

            protected virtual void OnPerform()
            { }

            public override void Simulate()
            {
                try
                {
                    NRaas.SpeedTrap.Begin();

                    try
                    {
                        mFunction();
                    }
                    catch (ResetException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Common.Exception(ToString(), e);
                    }

                    return;
                }
                finally
                {
                    Simulator.DestroyObject(ObjectId);

                    NRaas.SpeedTrap.End();
                }
            }

            public override string ToString()
            {
                if (mFunction == null)
                {
                    return "NULL function";
                }
                else
                {
                    return (mFunction.Method.ToString() + " - " + mFunction.Method.DeclaringType.ToString() + " - " + mFunction.Method.DeclaringType.Assembly.FullName.Replace(", Culture=neutral", ""));
                }
            }             
        }

        public class DerivativeSearch
        {
            public enum Caching
            {
                Default,
                NoCache
            }

            public enum Scope
            {
                Module,
                Global
            }

            static Dictionary<Type, List<object>> sItems = new Dictionary<Type, List<object>>();

            static List<Assembly> sModules = null;

            readonly static string sModuleName = null;

            static DerivativeSearch()
            {
                sModuleName = VersionStamp.sNamespace + "Module";
            }

            public static List<T> Find<T>()
                where T : class
            {
                return Find<T>(Caching.Default, Scope.Module);
            }
            public static List<T> Find<T>(Caching caching)
                where T : class
            {
                return Find<T>(caching, Scope.Module);
            }
            public static List<T> Find<T>(Scope scope)
                where T : class
            {
                return Find<T>(Caching.Default, scope);
            }
            public static List<T> Find<T>(Caching caching, Scope scope)
                where T : class
            {
                return FindOfType<T>(typeof(T), caching, scope);
            }

            public static List<T> FindOfType<T>(Type searchType)
                where T : class
            {
                return FindOfType<T>(searchType, Caching.Default, Scope.Global);
            }
            public static List<T> FindOfType<T>(Type searchType, Caching caching, Scope scope)
                where T : class
            {
                List<T> list = new List<T>();

                List<object> existing = null;
                if ((caching == Caching.Default) && (sItems.TryGetValue(searchType, out existing)))
                {
                    foreach (T item in existing)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    Assembly myAssembly = typeof(Common).Assembly;

                    if (caching == Caching.Default)
                    {
                        existing = new List<object>();
                        sItems.Add(searchType, existing);
                    }

                    Common.StringBuilder msg = new Common.StringBuilder();

                    List<Assembly> assemblies = sModules;

                    if (scope != Scope.Module)
                    {
                        assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
                    }
                    else if (assemblies == null)
                    {
                        assemblies = new List<Assembly>();

                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if ((assembly != myAssembly) && (assembly.GetType(sModuleName) == null)) continue;

                            assemblies.Add(assembly);
                        }

                        sModules = assemblies;
                    }

                    foreach (Assembly assembly in assemblies)
                    {
                        try
                        {
                            foreach (Type type in assembly.GetTypes())
                            {
                                if (type.IsAbstract) continue;

                                if (type.IsGenericTypeDefinition) continue;

                                if (!searchType.IsAssignableFrom(type)) continue;

                                try
                                {
                                    System.Reflection.ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                                    if (constructor != null)
                                    {
                                        T item = constructor.Invoke(new object[0]) as T;
                                        if (item != null)
                                        {
                                            list.Add(item);

                                            if (existing != null)
                                            {
                                                existing.Add(item);
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    msg += Common.NewLine + type.ToString();
                                    msg += Common.NewLine + e.Message;
                                    msg += Common.NewLine + e.StackTrace;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            msg += Common.NewLine + assembly.FullName;
                            msg += Common.NewLine + e.Message;
                            msg += Common.NewLine + e.StackTrace;
                        }
                    }

                    WriteLog(msg);
                }

                return list;
            }
        }

        public enum DebugLevel : int
        {
            Quiet = 0,
            Stats = 1,
            Low = 2,
            High = 3,
            Logging = 4
        }

        public interface IAddStatGenerator
        {
            Common.DebugLevel DebuggingLevel
            {
                get;
            }

            float AddStat(string stat, float val);
            float AddStat(string stat, float val, Common.DebugLevel minLevel);
        }

        public interface IStatGenerator : IAddStatGenerator
        {
            int AddScoring(string stat, int score);
            int AddScoring(string stat, int score, Common.DebugLevel minLevel);

            int AddStat(string stat, int val);
            int AddStat(string stat, int val, Common.DebugLevel minLevel);

            void IncStat(string stat);
            void IncStat(string stat, Common.DebugLevel minLevel);
        }

        public abstract class ProtoVersionStamp
        {
            public static bool sPopupMenuStyle = false;
        }

        public abstract class TraceLogger<T> : Logger<T>
            where T : TraceLogger<T>
        {
            bool mError = false;

            protected void PrivateAddTrace(string msg)
            {
                if (string.IsNullOrEmpty(msg)) return;

                PrivateAppend(msg);

                if (LoadLogging)
                {
                    mError = true;
                }
            }

            protected void PrivateAddError(string msg)
            {
                if (string.IsNullOrEmpty(msg)) return;

                PrivateAppend("[E]" + msg);

                mError = true;
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (!mError) return 0;

                return base.PrivateLog(builder);
            }
        }

        public class ExternalTimeSpanLogger : Logger<ExternalTimeSpanLogger>, IExternalLogger, IAddStatGenerator
        {
            public readonly static ExternalTimeSpanLogger sLogger = new ExternalTimeSpanLogger();

            Dictionary<string, StatValueCount> mStats = new Dictionary<string, StatValueCount>();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(VersionStamp.sNamespace + " " + msg);
            }

            protected override string Name
            {
                get { return "External Log"; }
            }

            protected override ExternalTimeSpanLogger Value
            {
                get { return sLogger; }
            }

            public DebugLevel DebuggingLevel
            {
                get 
                {
                    if (kDebugging)
                    {
                        return DebugLevel.Stats;
                    }
                    else
                    {
                        return DebugLevel.Quiet;
                    }
                }
            }

            protected override string GetHeader()
            {
                return null;
            }

            public float AddStat(string stat, float val)
            {
                return AddStat(stat, val, DebugLevel.Stats);
            }
            public float AddStat(string stat, float val, DebugLevel minLevel)
            {
                StatValueCount item;
                if (!Value.mStats.TryGetValue(stat, out item))
                {
                    item = new StatValueCount();
                    Value.mStats.Add(stat, item);
                }

                item.Add(val);

                return val;
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                foreach (KeyValuePair<string,StatValueCount> item in mStats)
                {
                    Append(item.Key + ": " + item.Value);
                }

                mStats.Clear();

                return base.PrivateLog(builder);
            }
        }

        public class InjectionLogger : Logger<InjectionLogger>
        {
            readonly static InjectionLogger sLogger = new InjectionLogger();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Injection Log"; }
            }

            protected override InjectionLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (!LoadLogging) return 0;

                return base.PrivateLog(builder);
            }
        }

        public class LoadLogger : Logger<LoadLogger>
        {
            readonly static LoadLogger sLogger = new LoadLogger();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Load Log"; }
            }

            protected override LoadLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (!LoadLogging) return 0;

                return base.PrivateLog(builder);
            }
        }

        public class ExceptionLogger : Logger<ExceptionLogger>
        {
            readonly static ExceptionLogger sLogger = new ExceptionLogger();

            public static void Append(StringBuilder noticeText, StringBuilder logText, ObjectGuid id1, ObjectGuid id2, Exception exception, Dictionary<string, bool> alreadyCaught)
            {
                sLogger.PrivateAppend(noticeText, logText, id1, id2, exception, alreadyCaught);
            }

            protected override string Name
            {
                get { return "Script Errors"; }
            }

            protected override ExceptionLogger Value
            {
                get { return sLogger; }
            }
        }

        public class IWasHereDefinition : ImmediateInteractionDefinition<IActor, IGameObject, GameObject.DEBUG_Reset>
        {
            public static readonly InteractionDefinition Singleton = new IWasHereDefinition();

            public override bool Test(IActor actor, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return false;
            }
        }

        public class AssemblyCheck
        {
            static Dictionary<string, bool> sAssemblies = new Dictionary<string, bool>();

            public static string GetNamespace(Assembly assembly)
            {
                Type type = assembly.GetType("NRaas.VersionStamp");
                if (type == null) return null;

                FieldInfo nameSpaceField = type.GetField("sNamespace", BindingFlags.Static | BindingFlags.Public);
                if (nameSpaceField == null) return null;

                return nameSpaceField.GetValue(null) as string;
            }

            public static bool IsInstalled(string assembly)
            {
                if (string.IsNullOrEmpty(assembly)) return false;

                assembly = assembly.ToLower();

                bool loaded;
                if (sAssemblies.TryGetValue(assembly, out loaded))
                {
                    return loaded;
                }

                loaded = (FindAssembly(assembly) != null);

                sAssemblies.Add(assembly, loaded);

                return loaded;
            }

            public static Assembly FindAssembly(string name)
            {
                name = name.ToLower();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name.ToLower() == name)
                    {
                        return assembly;
                    }
                }
                return null;
            }
        }

        public class MethodStore
        {
            string mAssemblyName;
            string mClassName;
            string mRoutineName;
            Type[] mParameters;

            StringBuilder mError = new StringBuilder();

            MethodInfo mMethod;
            bool mChecked;

            public MethodStore(string assemblyName, string className, string routineName, Type[] parameters)
            {
                mAssemblyName =assemblyName;
                mClassName = className;
                mRoutineName = routineName;
                mParameters = parameters;
            }
            public MethodStore(string methodName, Type[] parameters)
            {
                if ((methodName != null) && (methodName.Contains(",")))
                {
                    string[] strArray = methodName.Split(new char[] { ',' });

                    mClassName = strArray[0];
                    mAssemblyName = strArray[1];
                    mRoutineName = strArray[2].Replace(" ", "");
                }

                mParameters = parameters;
            }

            public bool Valid
            {
                get { return LookupRoutine(); }
            }

            public string Error
            {
                get 
                {
                    LookupRoutine();
                    return mError.ToString(); 
                }
            }

            public MethodInfo Method
            {
                get
                {
                    LookupRoutine();
                    return mMethod;
                }
            }

            public override string ToString()
            {
                return mMethod + " (" + mAssemblyName + "." + mClassName + "." + mRoutineName + ")";
            }

            protected bool LookupRoutine ()
            {
                if (!mChecked)
                {
                    mError += "Assembly: " + mAssemblyName + Common.NewLine + "ClassName: " + mClassName + Common.NewLine + "RoutineName: " + mRoutineName;

                    try
                    {
                        mChecked = true;

                        if (!string.IsNullOrEmpty(mAssemblyName))
                        {
                            Assembly assembly = AssemblyCheck.FindAssembly(mAssemblyName);
                            if (assembly != null)
                            {
                                mError += Common.NewLine + " Assembly Found: " + assembly.FullName;

                                Type type = assembly.GetType(mClassName);
                                if (type != null)
                                {
                                    mError += Common.NewLine + " Class Found: " + type.ToString();

                                    if (mParameters != null)
                                    {
                                        mMethod = type.GetMethod(mRoutineName, mParameters);
                                    }
                                    else
                                    {
                                        mMethod = type.GetMethod(mRoutineName);
                                    }

                                    if (mMethod != null)
                                    {
                                        mError += Common.NewLine + " Routine Found";
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        mError += Common.NewLine + "Exception";
                        Common.Exception(mError, e);
                    }
                    finally
                    {
                        //Common.WriteLog(mError);
                    }
                }

                return (mMethod != null);
            }

            public T Invoke<T>(object[] parameters)
            {
                return Invoke<T>(null, parameters);
            }
            public T Invoke<T>(object obj, object[] parameters)
            {
                if (!Valid) return default(T);

                try
                {
                    return (T)mMethod.Invoke(obj, parameters);
                }
                catch (Exception e)
                {
                    Common.StringBuilder msg = new Common.StringBuilder(ToString());

                    int leftSize = 0;
                    if (parameters != null)
                    {
                        leftSize = parameters.Length;
                    }

                    int rightSize = 0;
                    if (mParameters != null)
                    {
                        rightSize = mParameters.Length;
                    }

                    if (leftSize != rightSize)
                    {
                        msg += Common.NewLine + " No Enough Parameters: " + leftSize + " != " + rightSize;
                    }
                    else
                    {
                        for (int i=0; i<parameters.Length; i++)
                        {
                            msg += Common.NewLine + " Param " + i + ": " + parameters[i].GetType() + " : " + mParameters[i];
                        }
                    }

                    Common.Exception(msg, e);
                    return default(T);
                }
            }
        }

        public class TestSpan : IDisposable
        {
            DateTime mThen;

            IAddStatGenerator mStats = null;

            string mStat = null;

            DebugLevel mDebugLevel;

            private TestSpan()
            {
                mThen = DateTime.Now;
            }
            public TestSpan(IAddStatGenerator stats, string stat)
                : this(stats, stat, DebugLevel.Low)
            { }
            public TestSpan(IAddStatGenerator stats, string stat, DebugLevel debugLevel)
            {
                try
                {
                    mStats = stats;
                    if (mStats != null)
                    {
                        mStat = stat;
                        if ((!string.IsNullOrEmpty(mStat)) && (mStats.DebuggingLevel != DebugLevel.Quiet))
                        {
                            mThen = DateTime.Now;
                            mDebugLevel = debugLevel;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mStat, e);
                }
            }

            public static TestSpan CreateSimple()
            {
                return new TestSpan();
            }

            public long Duration
            {
                get
                {
                    return (DateTime.Now - mThen).Ticks / TimeSpan.TicksPerMillisecond;
                }
            }

            public void Dispose()
            {
                try
                {
                    if ((mStats != null) && (!string.IsNullOrEmpty(mStat)) && (mStats.DebuggingLevel != Common.DebugLevel.Quiet))
                    {
                        mStats.AddStat(mStat, Duration, mDebugLevel);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mStat, e);
                }
            }
        }

        public abstract class EventListenerTask
        {
            public delegate void Func(Event e);

            protected Func mFunc;

            protected EventListener mListener;

            public EventListenerTask(EventTypeId id, Func func)
            {
                if (func == null)
                {
                    mFunc = OnPerform;
                }
                else
                {
                    mFunc = func;
                }
            
                mListener = EventTracker.AddListener(id, OnProcess); // Must be immediate
            }

            public EventListener Listener
            {
                get { return mListener; }
            }

            protected abstract ListenerAction OnProcess(Event e);

            protected virtual void OnPerform(Event e)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                EventTracker.RemoveListener(mListener);
            }

            public override string ToString()
            {
                return mListener.EventId + " - " + mFunc.ToString();
            }
        }

        public class ImmediateEventListener : EventListenerTask
        {
            public ImmediateEventListener(EventTypeId id, Func func)
                : base(id, func)
            { }

            protected override ListenerAction OnProcess(Event e)
            {
                try
                {
                    mFunc(e);
                }
                catch (Exception ex)
                {
                    Common.Exception(e.Actor, e.TargetObject, ex);
                }

                return ListenerAction.Keep;
            }
        }

        public class DelayedEventListener : EventListenerTask
        {
            public DelayedEventListener(EventTypeId id, Func func)
                : base(id, func)
            {}

            protected override ListenerAction OnProcess(Event e)
            {
                DelayTask.Perform(e, mFunc);

                return ListenerAction.Keep;
            }

            public class DelayTask : Common.FunctionTask
            {
                Event mEvent;

                Func mFunc;

                protected DelayTask(Event e, Func func)
                {
                    mEvent = e;
                    mFunc = func;
                }

                public static void Perform(Event e, Func func)
                {
                    new DelayTask(e, func).AddToSimulator();
                }

                protected override void OnPerform()
                {
                    try
                    {
                        mFunc(mEvent);
                    }
                    catch (Exception exception)
                    {
                        Common.Exception(mEvent.Actor, mEvent.TargetObject, exception);
                    }
                }
            }
        }

        public class StatValueCount
        {
            public static bool sFullLog = false;

            float mValue = 0;
            int mCount = 0;

            public void Add(float value)
            {
                mValue += value;
                mCount++;
            }

            public float Average
            {
                get
                {
                    if (mCount == 0) return 0;

                    return (mValue / mCount);
                }
            }

            public int Count
            {
                get { return mCount; }
            }

            public override string ToString()
            {
                return ToString(0);
            }
            public string ToString(float maximum)
            {
                if (mCount == 0)
                {
                    if (!sFullLog)
                    {
                        return null;
                    }
                    else
                    {
                        return ",,,,,,,,";
                    }
                }

                if (mValue != mCount)
                {
                    float meanTotal = mValue;
                    float meanAverage = Average;
                    if ((mCount > 1) && (maximum != 0))
                    {
                        meanTotal -= maximum;
                        meanAverage = ((meanTotal) / (mCount - 1));
                    }

                    string result = mCount.ToString();
                    result += ",Tot," + mValue.ToString(((mValue > -1) && (mValue < 1)) ? "F2" : "F0") + ",Avg," + Average.ToString(((Average > -1) && (Average < 1)) ? "F2" : "F0") + ",MeanTot," + meanTotal.ToString(((meanTotal > -1) && (meanTotal < 1)) ? "F2" : "F0") + ",MeanAvg," + meanAverage.ToString(((meanAverage > -1) && (meanAverage < 1)) ? "F2" : "F0");
                    return result;
                }
                else
                {
                    if (sFullLog)
                    {
                        return mValue.ToString() + ",,,,,,,,";
                    }
                    else
                    {
                        return mValue.ToString();
                    }
                }
            }
        }

        public class StringBuilder
        {
            List<string> mStrings = new List<string>();

            public StringBuilder()
            { }
            public StringBuilder(StringBuilder builder)
            {
                if (builder != null)
                {
                    mStrings.AddRange(builder.mStrings);
                }
            }
            public StringBuilder(string firstLine)
            {
                Append(firstLine);
            }
            public StringBuilder(string firstLine, StringBuilder builder)
                : this(firstLine)
            {
                Append(builder);
            }

            public int Count
            {
                get { return mStrings.Count; }
            }

            public void AddXML(string fieldName, object value)
            {
                Append(XML(fieldName, value));
            }

            public static string XML(string fieldName, object value)
            {
                return Common.NewLine + "<" + fieldName + ">" + value + "</" + fieldName + ">";
            }

            public static StringBuilder operator +(StringBuilder builder, object obj)
            {
                if (obj == null) return null;

                builder.Append(obj.ToString());
                return builder;
            }
            public static StringBuilder operator +(StringBuilder builder, string line)
            {
                builder.Append(line);
                return builder;
            }
            public static StringBuilder operator +(StringBuilder builder, StringBuilder line)
            {
                builder.Append(line);
                return builder;
            }

            public void Append(string line)
            {
                mStrings.Add(line);
            }
            public void Append(StringBuilder builder)
            {
                if (builder == null) return;

                mStrings.AddRange(builder.mStrings);
            }

            public override string ToString()
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                foreach (string text in mStrings)
                {
                    if (text == null) continue;

                    builder.Append(text);
                }

                return builder.ToString();
            }
        }

        public class StoredNotice
        {
            public readonly ObjectGuid mID1;
            public readonly ObjectGuid mID2;

            public readonly string mText;

            public readonly StyledNotification.NotificationStyle mStyle;

            public readonly string mOverrideImage;

            public readonly ProductVersion mOverrideVersion;

            public StoredNotice(string text, ObjectGuid id1, ObjectGuid id2, StyledNotification.NotificationStyle style, string overrideImage, ProductVersion overrideVersion)
            {
                mText = text;
                mID1 = id1;
                mID2 = id2;
                mStyle = style;
                mOverrideImage = overrideImage;
                mOverrideVersion = overrideVersion;
            }
        }
   }
}