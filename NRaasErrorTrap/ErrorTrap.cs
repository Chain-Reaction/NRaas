using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.ErrorTrapSpace;
using NRaas.ErrorTrapSpace.Checks;
using NRaas.ErrorTrapSpace.Dereferences;
using NRaas.ErrorTrapSpace.Dereferences.Controllers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas
{
    public class ErrorTrapTuning
    {
        [Tunable, TunableComment("Whether to perform dereferencing cleanup on the save data")]
        public static bool kPerformDereferencing = true;

        [Tunable, TunableComment("Whether to list all object types in the statistics")]
        public static bool kFullStats = false;
    }

    public class ErrorTrapTuning3
    {
        [Tunable, TunableComment("Whether to perform log counting")]
        public static bool kLogCounting = true;
    }

    public class ErrorTrapTuning4
    {
        [Tunable, TunableComment("Scripting class on which to force referencing log")]
        public static string kForceReferenceLog = null;
    }

    public class ErrorTrapTuning5
    {
        [Tunable, TunableComment("Whether to reset stored task states on loadup")]
        public static bool kResetTaskStates = false;
    }

    public class ErrorTrapTuning6
    {
        [Tunable, TunableComment("Whether to perform speed trapping")]
        public static bool kSpeedTrap = false;
    }
   
    public class ErrorTrap : Common, Common.IStartupApp, Common.IPreLoad, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static bool sLoading = true;
        //static bool sDelayLoading = true;

        static Dictionary<string, bool> sChecked = new Dictionary<string, bool>();

        static Dictionary<string, bool> sAlreadyCaught = new Dictionary<string, bool>();

        static List<GameObject> sToBeDeleted = new List<GameObject>();
        static List<GameObject> sSilentToBeDeleted = new List<GameObject>();

        static Dictionary<ulong, SimDescription> sResidents = null;

        static Dictionary<Type, bool> sIgnoredTypes = new Dictionary<Type, bool>();

        static Dictionary<ObjectGuid, IGameObject> sObjectsOfInterest = new Dictionary<ObjectGuid, IGameObject>();

        static long sReferencesCounted = 0;

        static bool sAfterLoadup = false;

        public delegate void OnSleepFunc(string text);

        static ErrorTrap()
        {
            StatValueCount.sFullLog = true;

            sIgnoredTypes.Add(typeof(string), true);
            sIgnoredTypes.Add(typeof(ResourceKey), true);
            sIgnoredTypes.Add(typeof(AlarmHandle), true);
            sIgnoredTypes.Add(typeof(DateAndTime), true);
            sIgnoredTypes.Add(typeof(ObjectGuid), true);
            sIgnoredTypes.Add(typeof(Color), true);
            sIgnoredTypes.Add(typeof(GeneticColor), true);
            sIgnoredTypes.Add(typeof(Vector3), true);
            sIgnoredTypes.Add(typeof(Type), true);
            sIgnoredTypes.Add(typeof(Slot), true);

            Bootstrap();
        }

        public void OnStartupApp()
        {
            UIManager.mUIEventCallbackDelegate = OnProcessEventCallback;
            UIManager.mUILocaleChangedCallbackDelegate = OnUILocaleChangedCallback;
            UIManager.mUIInGameStoreDelegate = OnUIInGameStoreMessageCallback;
            UIManager.mUIResolutionChangedCallbackDelegate = OnUIResolutionChangedCallback;
            UIManager.mUIWindowShutdownCallbackDelegate = OnUIWindowShutdownCallback;
            UIManager.mUINewContentInstalledDelegate = OnUINewContentInstalled;
            UIManager.mUINewHotInstallDataCallback = OnUINewHotInstallData;

            UIManager.gUIMgr.SetUICallbacks(UIManager.mUIEventCallbackDelegate, UIManager.mUILocaleChangedCallbackDelegate, UIManager.mUIResolutionChangedCallbackDelegate, UIManager.mUIWindowShutdownCallbackDelegate, UIManager.mUINewContentInstalledDelegate, UIManager.mUIInGameStoreDelegate, UIManager.mUINewHotInstallDataCallback);
        }

        protected static bool OnProcessEventCallback(uint winHandle, uint eventType, uint srcHandle, uint dstHandle, int arg1, int arg2, int arg3, float f1, float f2, float f3, float f4, ref bool result, string text)
        {
            try
            {
                return UIManager.ProcessEventCallback(winHandle, eventType, srcHandle, dstHandle, arg1, arg2, arg3, f1, f2, f3, f4, ref result, text);
            }
            catch(Exception e)
            {
                Common.Exception("OnProcessEventCallback", e);
                return false;
            }
        }

        protected static void OnUINewContentInstalled(string message)
        {
            try
            {
                UIManager.UINewContentInstalled(message);
            }
            catch (Exception e)
            {
                Common.Exception("OnUINewContentInstalled", e);
            }
        }

        protected static void OnUINewHotInstallData(string textData)
        {
            try
            {
                UIManager.UINewHotInstallData(textData);
            }
            catch (Exception e)
            {
                Common.Exception("OnUINewHotInstallData", e);
            }
        }

        protected static void OnUIInGameStoreMessageCallback(int messageId, string text)
        {
            try
            {
                UIManager.UIInGameStoreMessageCallback(messageId, text);
            }
            catch (Exception e)
            {
                Common.Exception("OnUIInGameStoreMessageCallback", e);
            }
        }

        protected static void OnUIWindowShutdownCallback(uint winHandle)
        {
            try
            {
                UIManager.UIWindowShutdownCallback(winHandle);
            }
            catch (Exception e)
            {
                Common.Exception("OnUIWindowShutdownCallback", e);
            }
        }

        protected static void OnUILocaleChangedCallback()
        {
            try
            {
                UIManager.UILocaleChangedCallback();
            }
            catch (Exception e)
            {
                Common.Exception("OnUILocaleChangedCallback", e);
            }
        }

        protected static void OnUIResolutionChangedCallback(uint oldWidth, uint oldHeight, uint oldRefresh, bool oldFullScreen, uint newWidth, uint newHeight, uint newRefresh, bool newFullScreen, uint newUIWidth, uint newUIHeight)
        {
            try
            {
                UIManager.UIResolutionChangedCallback(oldWidth, oldHeight, oldRefresh, oldFullScreen, newWidth, newHeight, newRefresh, newFullScreen, newUIWidth, newUIHeight);
            }
            catch (Exception e)
            {
                Common.Exception("OnUIResolutionChangedCallback", e);
            }
        }

        public void OnPreLoad()
        {
            ScriptCore.ExceptionTrap.OnPrePostLoad += OnPrePostLoad;
            ScriptCore.ExceptionTrap.OnPostPostLoad += OnPostPostLoad;
            ScriptCore.ExceptionTrap.OnScriptError += OnScriptError;
            //ScriptCore.ExceptionTrap.OnLoadObject += OnLoadObject;
            ScriptCore.ExceptionTrap.OnLoadReference += OnLoadReference;
            ScriptCore.ExceptionTrap.OnLoadArrayReference += OnLoadArrayReference;
            ScriptCore.ExceptionTrap.OnRemoveObject += OnRemoveObject;
            ScriptCore.ExceptionTrap.OnGetOption += OnGetOption;

            if (ErrorTrapTuning6.kSpeedTrap)
            {
                MethodInfo sleepFunc = typeof(NRaas.ErrorTrap).GetMethod("OnSleep", BindingFlags.Public | BindingFlags.Static);

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type common = assembly.GetType("NRaas.SpeedTrap");
                    if (common == null) continue;

                    MethodInfo func = common.GetMethod("SetDelegates", BindingFlags.Public | BindingFlags.Static);
                    if (func == null) continue;

                    func.Invoke(null, new object[] { Delegate.CreateDelegate(typeof(OnSleepFunc), sleepFunc) });
                }
            }
        }

        public static int OnGetOption(string option)
        {
            switch (option)
            {
                case "ResetTaskStates":
                    return ErrorTrapTuning5.kResetTaskStates ? 1 : 0;
            }

            return 0;
        }

        public static void OnSleep(string text)
        {
            try
            {
                SleepLogger.Append(text);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception("OnSleep", e);
            }
        }

        public void OnWorldLoadFinished()
        {
            sLoading = false;

            sChecked.Clear();

            ObjectLookup.StartListener();

            foreach (ITask task in new List<ITask>(ScriptCore.Simulator.mObjHash.Values))
            //foreach (ObjectGuid guid in new List<ObjectGuid>(sAllRunning.Keys))
            {
                //ITask task = Simulator.GetTask(guid);

                IScriptProxy proxy = task as IScriptProxy;
                if (proxy != null)
                {
                    if (proxy.Target != null)
                    {
                        OneShotFunction oneShot = proxy.Target as OneShotFunction;
                        if (oneShot != null)
                        {
                            if (oneShot.mFunction != null)
                            {
                                IGameObject obj = oneShot.mFunction.Target as IGameObject;
                                if (obj != null)
                                {
                                    if (obj.HasBeenDestroyed)
                                    {
                                        Simulator.DestroyObject(task.ObjectId);

                                        LogCorrection("Removed Orphan OneShotFunction");
                                        LogCorrection(" " + oneShot.mFunction.Method.DeclaringType.ToString());
                                        LogCorrection(" " + oneShot.mFunction.Target.GetType());
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (task != null)
                {
                    Sims3.Gameplay.OneShotFunctionTask oneShot = task as Sims3.Gameplay.OneShotFunctionTask;
                    if (oneShot != null)
                    {
                        if (oneShot.mFunction != null)
                        {
                            IGameObject obj = oneShot.mFunction.Target as IGameObject;
                            if (obj != null)
                            {
                                if (obj.HasBeenDestroyed)
                                {
                                    Simulator.DestroyObject(task.ObjectId);

                                    LogCorrection("Removed Orphan OneShotFunctionTask");
                                    LogCorrection(" " + oneShot.mFunction.Method.DeclaringType.ToString());
                                    LogCorrection(" " + oneShot.mFunction.Target.GetType());
                                    continue;
                                }
                            }
                        }
                    }
                }
            }

            new Common.AlarmTask(1, TimeUnit.Minutes, OnDelayedWorldLoadFinished);
            new Common.AlarmTask(3, TimeUnit.Minutes, OnAfterLoadup);
        }

        public void OnAfterLoadup()
        {
            sAfterLoadup = true;
        }

        public void OnDelayedWorldLoadFinished()
        {
            sObjectsOfInterest.Clear();

            List<ICheck> checks = DerivativeSearch.Find<ICheck>();

            foreach (ICheck check in checks)
            {
                check.Finish();
            }

            EventTracker.sCurrentlyUpdatingDreamsAndPromisesManagers = false;

            new CheckRoleManager().Perform(RoleManager.sRoleManager, true);

            CheckRecovery();

            CheckOutfits();

            CheckLotObjects();

            Inventories.CheckInventories(LogCorrection, DebugLogCorrection, true);

            List<IDereferenceController> controllers = DerivativeSearch.Find<IDereferenceController>();

            foreach (IDereferenceController controller in controllers)
            {
                controller.Clear();

                foreach (KeyValuePair<Type, List<ObjectLookup.Item>> list in ObjectLookup.List)
                {
                    controller.Add(list.Key, list.Value);
                }

                controller.PreProcess();
            }

            if (ErrorTrapTuning3.kLogCounting)
            {
                using (TestSpan span = new TestSpan(ExternalTimeSpanLogger.sLogger, "Log Counts"))
                {
                    ObjectLookup.LogCounts();
                }
            }

            DereferenceManager.Logger.OnWorldQuit = false;

            try
            {
                DereferenceManager.Logger.sCollecting = true;

                using (Common.TestSpan totalSpan = new Common.TestSpan(TimeSpanLogger.Bin, "Dereferencing", DebugLevel.Stats))
                {
                    foreach (IDereferenceController controller in controllers)
                    {
                        controller.Perform();

                        if (DereferenceManager.Logger.OnWorldQuit) break;
                    }
                }
            }
            finally
            {
                DereferenceManager.Logger.sCollecting = false;
            }

            using (Common.TestSpan totalSpan = new Common.TestSpan(TimeSpanLogger.Bin, "Dereferencing", DebugLevel.Stats))
            {
                foreach (IDereferenceController controller in controllers)
                {
                    controller.PostPerform();

                    if (DereferenceManager.Logger.OnWorldQuit) break;
                }
            }

            // Possibly loaded by Dereference
            sResidents = null;

            ProcessToBeDeleted(sToBeDeleted, true);
            ProcessToBeDeleted(sSilentToBeDeleted, false);

            ObjectLookup.Clear();

            //sDelayLoading = false;

            RecordErrors();
        }

        protected static void ProcessToBeDeleted(List<GameObject> list, bool log)
        {
            Mausoleum mausoleum = null;

            foreach (GameObject obj in new List<GameObject>(list))
            {
                try
                {
                    Urnstone stone = obj as Urnstone;
                    if ((stone != null) && (stone.DeadSimsDescription != null))
                    {
                        if (mausoleum == null)
                        {
                            List<Mausoleum> mausoleums = new List<Mausoleum> (Sims3.Gameplay.Queries.GetObjects<Mausoleum>());
                            if (mausoleums.Count > 0)
                            {
                                mausoleum = RandomUtil.GetRandomObjectFromList(mausoleums);
                            }
                        }

                        if (mausoleum != null)
                        {
                            Urnstones.MoveToMausoleum(mausoleum, stone);

                            LogCorrection("Stone Mausoleumed: " + obj.GetType() + " (ID=" + stone.DeadSimsDescription.FullName + ")");
                        }
                    }
                    else
                    {
                        obj.Dispose();
                        obj.Destroy();

                        if (log)
                        {
                            LogCorrection("Destroyed: " + obj.GetType() + " (ID=" + obj.ObjectId + ")");
                        }
                        else
                        {
                            DebugLogCorrection("Silent Destroyed: " + obj.GetType() + " (ID=" + obj.ObjectId + ")");
                        }
                    }

                    SpeedTrap.Sleep();
                }
                catch (Exception e)
                {
                    Common.Exception(obj, e);
                }
            }

            list.Clear();
        }

        public void OnWorldQuit()
        {
            sLoading = true;
            sAfterLoadup = false;

            //sDelayLoading = true;

            DereferenceManager.Logger.OnWorldQuit = true;

            ObjectLookup.Clear();

            sObjectsOfInterest.Clear();
        }

        public static bool Loading
        {
            get { return sLoading; }
        }

        public static DebugLevel DebuggingLevel
        {
            get
            {
                if (sLoading)
                {
                    return DebugLevel.Stats;
                }
                else
                {
                    return DebugLevel.Low;
                }
            }
        }

        public static void AddObjectOfInterest(IGameObject obj)
        {
            if (sObjectsOfInterest.ContainsKey(obj.ObjectId)) return;

            sObjectsOfInterest.Add(obj.ObjectId, obj);
        }

        public static string GetQualifiedName(object obj)
        {
            string name = obj.GetType().ToString();

            SimDescription desc = obj as SimDescription;
            if (obj is Sim)
            {
                Sim sim = obj as Sim;

                desc = sim.SimDescription;
            }

            if (desc != null)
            {
                name += " (" + desc.FullName + ", V=" + desc.IsValidDescription + ")";
            }

            Delegate del = obj as Delegate;
            if (del != null)
            {
                name += " (" + del.Method + "," + del.Target + ")";
            }

            IGameObject gameObject = obj as IGameObject;
            if (gameObject != null)
            {
                name += " (ID=" + gameObject.ObjectId + ", W=" + gameObject.InWorld + ", I=" + gameObject.InInventory + ")";

                if ((gameObject.LotCurrent != null) && (!gameObject.LotCurrent.IsWorldLot))
                {
                    name += " (Lot: " + gameObject.LotCurrent.LotId + ", " + gameObject.LotCurrent.Name + ")";
                }
            }

            return name;
        }

        public static Dictionary<ulong, SimDescription> Residents
        {
            get
            {
                if (sResidents == null)
                {
                    sResidents = SimListing.GetResidents(false);
                }

                return sResidents;
            }
        }

        public static void DebugLogCorrection(string log)
        {
            if (!kDebugging) return;

            LogCorrection(log);
        }
        public static void LogCorrection(string log)
        {
            CorrectionLogger.Append(log);
        }

        public static string GetName(IGameObject obj)
        {
            if (obj == null) return "<Empty>";

            string name = null;
            try
            {
                name = obj.GetLocalizedName();
                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Trim();
                }
            }
            catch
            { }

            return name + " [" + obj.GetType().ToString() + "]";
        }

        protected static void CheckLotObjects()
        {
            foreach (Lot lot in LotManager.AllLots)
            {
                string lotName = null;

                if (lot.IsWorldLot)
                {
                    lotName = "World Lot";
                }
                else
                {
                    lotName = " " + lot.Name + " " + lot.Address;
                }

                if (string.IsNullOrEmpty(lotName.Replace(" ", "")))
                {
                    lotName = null;
                }
                else
                {
                    lotName = Common.NewLine + " Current Lot:" + lotName;
                }

                Dictionary<ObjectGuid,GameObject> lookup = new Dictionary<ObjectGuid,GameObject>();

                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    lookup.Remove(obj.ObjectId);

                    if (obj is PlumbBob) continue;

                    if (obj is SkillMeter) continue;

                    if (obj is ProgressMeter) continue;

                    if (obj is Houseboat) continue;

                    if (obj.InUse) continue;

                    if (!obj.InWorld) continue;

                    if (obj.LotCurrent == lot) continue;

                    string existingLotName = null;

                    if (obj.LotCurrent != null)
                    {
                        if (obj.LotCurrent.IsWorldLot)
                        {
                            existingLotName = "World Lot";
                        }
                        else
                        {
                            existingLotName = " " + obj.LotCurrent.Name + " " + obj.LotCurrent.Address;
                        }

                        if (string.IsNullOrEmpty(existingLotName.Replace(" ", "")))
                        {
                            existingLotName = null;
                        }
                        else
                        {
                            existingLotName = Common.NewLine + " Existing Lot:" + existingLotName;
                        }
                    }

                    obj.mLotCurrent = lot;

                    DebugLogCorrection("Reset LotCurrent " + obj.GetType() + lotName + existingLotName);
                }
            }
        }

        protected static void CheckRecovery()
        {
            foreach (SimDescription sim in Household.AllSimsLivingInWorld())
            {
                if (sim.CreatedSim == null)
                {
                    new RecoverMissingSimTask(sim, false).AddToSimulator();
                }
            }
        }

        protected static void CheckOutfits()
        {
            foreach (Sim sim in new List<Sim>(LotManager.Actors))
            {
                try
                {
                    OutfitCategories currentCategory = sim.CurrentOutfitCategory;
                    int currentIndex = sim.CurrentOutfitIndex;

                    SimOutfit outfit = sim.SimDescription.GetOutfit(currentCategory, currentIndex);
                    if ((outfit == null) || (!outfit.IsValid))
                    {
                        outfit = sim.SimDescription.GetOutfit(OutfitCategories.Everyday, 0);
                        if ((outfit != null) && (outfit.IsValid))
                        {
                            new ChangeOutfitTask(sim.SimDescription, OutfitCategories.Everyday).AddToSimulator();
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }
        }

        public static void CheckTravelData()
        {
            if (GameStates.sTravelData != null)
            {
                if (GameStates.sTravelData.mEarlyDepartures != null)
                {
                    int index = 0;
                    while (index < GameStates.sTravelData.mEarlyDepartures.Count)
                    {
                        Sim sim = GameStates.sTravelData.mEarlyDepartures[index];

                        if ((sim == null) || (sim.SimDescription == null))
                        {
                            LogCorrection("Corrupt Early Departure Sim Dropped");

                            GameStates.sTravelData.mEarlyDepartures.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
            }
        }

        public static void AddToBeDeleted(GameObject obj, bool log)
        {
            if (log)
            {
                sToBeDeleted.Add(obj);
            }
            else
            {
                sSilentToBeDeleted.Add(obj);
            }
        }

        public static void OnPrePostLoad(ScriptCore.ScriptProxy proxy, Sims3.SimIFace.IScriptLogic logic, bool postLoad)
        {
            using (TestSpan span = new TestSpan(TimeSpanLogger.Bin, "OnPrePostLoad", DebuggingLevel))
            {
                try
                {
                    List<ICheck> checks = DerivativeSearch.Find<ICheck>();

                    foreach (ICheck check in checks)
                    {
                        check.IPrePerform(logic, postLoad);
                    }
                }
                catch (Exception e)
                {
                    Exception(proxy.Target, e);
                }
            }
        }

        public static void OnPostPostLoad(ScriptCore.ScriptProxy proxy, Sims3.SimIFace.IScriptLogic logic, bool postLoad)
        {
            using (TestSpan span = new TestSpan(TimeSpanLogger.Bin, "OnPostPostLoad", DebuggingLevel))
            {
                try
                {
                    List<ICheck> checks = DerivativeSearch.Find<ICheck>();

                    foreach (ICheck check in checks)
                    {
                        check.IPostPerform(logic, postLoad);
                    }
                }
                catch (Exception e)
                {
                    Exception(proxy.Target, e);
                }
            }
        }
        
        protected static SimDescription SearchForSim(string trace)
        {
            string address = ParseStackTrace(trace, "SimDescription:Instantiate (Sims3.SimIFace.Vector3) (", 0);
            if (string.IsNullOrEmpty(address)) return null;

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if (RetrieveAddress(sim) == address)
                {
                    return sim;
                }
            }

            return null;
        }

        protected static IGameObject SearchForAutonomy(string trace)
        {
            string address = ParseStackTrace(trace, "Autonomy:RunAutonomy (bool) (", 0);
            if (string.IsNullOrEmpty(address)) return null;

            foreach (Sim sim in LotManager.Actors)
            {
                if (RetrieveAddress(sim.Autonomy) == address)
                {
                    return sim;
                }
            }

            return null;
        }

        protected static IGameObject SearchForAssignedSim(string trace)
        {
            string address = ParseStackTrace(trace, "Service:CanSimBeAssignedToLot (Sims3.Gameplay.CAS.SimDescription,Sims3.Gameplay.Core.Lot) (", 10);
            if (string.IsNullOrEmpty(address))
            {
                address = ParseStackTrace(trace, "Service:CreateNpcSim (Sims3.Gameplay.CAS.SimDescription,Sims3.SimIFace.Vector3) (", 10);
                if (string.IsNullOrEmpty(address))
                {
                    address = ParseStackTrace(trace, "SimDescription:Instantiate (Sims3.SimIFace.Vector3,Sims3.SimIFace.ResourceKey) (", 0);
                    if (string.IsNullOrEmpty(address)) return null;
                }
            }

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if (RetrieveAddress(sim) == address)
                {
                    return sim.CreatedSim;
                }
            }

            return null;
        }

        public static bool IsIgnored(object obj)
        {
            if (obj == null) return true;

            if (obj is string) return true;

            if (!ErrorTrapTuning.kFullStats)
            {
                Type type = obj.GetType();

                if (type.IsArray)
                {
                    type = type.GetElementType();
                }

                if (type.IsPrimitive) return true;

                if (type.IsEnum) return true;

                if (sIgnoredTypes.ContainsKey(type)) return true;
            }

            return false;
        }

        protected static string GetTaskName(ITask task)
        {
            string name = null;

            IScriptProxy proxy = task as IScriptProxy;
            if (proxy != null)
            {
                IScriptLogic logic = proxy.Target;
                if (logic != null)
                {
                    if (logic is Sim)
                    {
                        name = "Sim: " + (logic as Sim).FullName;
                    }
                    else if (logic is SimUpdate)
                    {
                        name = "SimUpdate: " + (logic as SimUpdate).mSim.FullName;
                    }
                }
            }

            string type = task.GetType().ToString();

            if (string.IsNullOrEmpty(name))
            {
                name = task.ToString();
            }

            if ((!string.IsNullOrEmpty(name)) && (name != type))
            {
                type += " (" + name + ")";
            }

            return type;
        }

        public static void OnRemoveObject(ObjectGuid id)
        {
            string msg = id.ToString();

            try
            {
                IGameObject obj;
                if (sObjectsOfInterest.TryGetValue(id, out obj))
                {
                    msg = "Object Of Interest Deleted: " + GetName(obj) + " (ID=" + id + ")";

                    DebugLogCorrection(msg);

                    //throw new Exception();
                }
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
        }

        public static void OnLoadArrayReference(object list, ref object parent)
        {
            if (!ErrorTrapTuning.kPerformDereferencing) return;

            using (TestSpan span = new TestSpan(TimeSpanLogger.Bin, "OnLoadArrayReferences", DebuggingLevel))
            {
                try
                {
                    if ((parent == null) || (list == null)) return;

                    if (IsIgnored(list)) return;

                    ObjectLookup.Add(list, parent, null);
                }
                catch (Exception e)
                {
                    DebugException("Parent: " + parent.GetType().ToString() + Common.NewLine + "List: " + list.GetType().ToString(), e);
                }
            }
        }

        public static void OnLoadReference(ref object child, ref object parent, FieldInfo field)
        {
            if (!ErrorTrapTuning.kPerformDereferencing) return;

            using (TestSpan span = new TestSpan(TimeSpanLogger.Bin, "OnLoadReference", DebuggingLevel))
            {
                try
                {
                    if ((parent == null) || (child == null)) return;

                    if (IsIgnored(child)) return;

                    try
                    {
                        child.GetHashCode();
                        parent.GetHashCode();
                    }
                    catch
                    {
                        return;
                    }

                    sReferencesCounted++;

                    ObjectLookup.Add(child, parent, field);
                }
                catch (Exception e)
                {
                    DebugException("Parent: " + parent.GetType().ToString() + Common.NewLine + "Child: " + child.GetType().ToString(), e);
                }
            }
        }

        /*
        public static void OnLoadObject(int index, ref object obj)
        {
            using (TestSpan span = new TestSpan(TimeSpanLogger.Bin, "OnLoadObject", DebuggingLevel))
            {
                try
                {
                    // Too many, and don't care
                    if (IsIgnored(obj)) return;
                }
                catch (Exception e)
                {
                    DebugException(obj.GetType().ToString(), e);
                }
            }
        }
        */

        public static void ChangeLoadingScreenCaption(string text)
        {
            LoadingScreenController controller = LoadingScreenController.Instance;
            if (controller == null) return;

            if (controller.mTipText == null) return;

            controller.mTipText.Caption = text;
        }

        public static void OnScriptError(ScriptCore.ScriptProxy proxy, Exception exception)
        {
            try
            {
                using (TestSpan span = new TestSpan(TimeSpanLogger.Bin, "OnScriptError", DebuggingLevel))
                {
                    bool fullReset = true;

                    bool record = !IgnoreList.IsIgnored(exception, out fullReset);

                    IScriptLogic target = null;
                    if (proxy != null)
                    {
                        target = proxy.Target;
                    }

                    StringBuilder noticeText = new StringBuilder();
                    StringBuilder logText = new StringBuilder();

                    SimUpdate update = target as SimUpdate;
                    if (update != null)
                    {
                        target = update.mSim;
                    }
                    else
                    {
                        AutonomyManager autonomy = target as AutonomyManager;
                        if (autonomy != null)
                        {
                            target = SearchForAutonomy(exception.StackTrace);
                        }
                        else
                        {
                            Services services = target as Services;
                            if (services != null)
                            {
                                target = SearchForAssignedSim(exception.StackTrace);
                            }
                        }
                    }

                    SimDescription targetSim = SearchForSim(exception.StackTrace);
                    if (targetSim != null)
                    {
                        new FixInvisibleTask(targetSim).AddToSimulator();
                    }

                    IGameObject obj = null;

                    if (targetSim != null)
                    {
                        obj = ScriptCoreLogger.Convert(targetSim, noticeText, logText);
                    }
                    else
                    {
                        obj = ScriptCoreLogger.Convert(target, noticeText, logText);
                        if (obj == null)
                        {
                            obj = target as IGameObject;
                        }
                    }

                    ObjectGuid id = ObjectGuid.InvalidObjectGuid;
                    if (obj != null)
                    {
                        id = obj.ObjectId;
                    }

                    if (record)
                    {
                        ScriptCoreLogger.Append(noticeText, logText, id, ObjectGuid.InvalidObjectGuid, exception, sAlreadyCaught);
                    }

                    /* do not use else if here */
                    if ((proxy != null) && (proxy.Target is RoleManagerTask))
                    {
                        new CheckRoleManagerTask().Perform(proxy.Target as RoleManagerTask, true);
                    }

                    Sim simObj = obj as Sim;
                    if (simObj != null)
                    {
                        try
                        {
                            if (simObj.Household == null)
                            {
                                fullReset = true;
                            }

                            if ((!fullReset) && (proxy != null))
                            {
                                proxy.OnReset();
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(proxy.Target, null, "PartialReset", e);
                            fullReset = true;
                        }

                        if (fullReset)
                        {
                            if (update != null)
                            {
                                try
                                {
                                    Simulator.DestroyObject(update.Proxy.ObjectId);
                                }
                                catch
                                { }
                            }

                            new ResetSimTask(simObj);
                            return;
                        }
                    }
                    else if (proxy != null)
                    {
                        proxy.OnReset();
                    }
                }
            }
            catch (Exception e)
            {
                Exception(proxy.Target, e);
            }
        }

        public class RunLogger : Logger<RunLogger>
        {
            readonly static RunLogger sLogger = new RunLogger();

            static string sLastText = null;
            static int sCount = 0;

            protected override string Name
            {
                get { return "Run Logs"; }
            }

            protected override RunLogger Value
            {
                get { return sLogger; }
            }

            public static void Append(string text)
            {
                text = DateTime.Now + "," + SimClock.CurrentTime() + "," + text;

                if (sLastText == text)
                {
                    sCount++;
                }
                else
                {
                    if (sLastText != null)
                    {
                        sLogger.PrivateAppend("  And " + sCount + " More");
                    }

                    sLastText = text;
                    sCount = 0;

                    sLogger.PrivateAppend(text);
                }
            }
        }

        public class TimeSpanLogger : Logger<TimeSpanLogger>, StatBin.IStatBinLogger
        {
            readonly static TimeSpanLogger sLogger = new TimeSpanLogger();

            readonly static Stats sBin = new Stats();

            static bool sChanged = false;

            public static StatBin Bin
            {
                get
                {
                    return sBin;
                }
            }

            protected override string Name
            {
                get { return "Time Span Logs"; }
            }

            protected override TimeSpanLogger Value
            {
                get { return sLogger; }
            }

            public void Append(string text)
            {
                sLogger.PrivateAppend(text);
            }
            public static void Append(string text, DateTime now, DateTime then)
            {
                long duration = (now - then).Ticks / TimeSpan.TicksPerMillisecond;
                if (duration > 0)
                {
                    sChanged = true;
                }

                Bin.AddStat(text, duration);
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (sChanged)
                {
                    sBin.Log(sLogger);
                    sChanged = false;
                }

                return base.PrivateLog(builder);
            }

            public class Stats : StatBin
            {
                public Stats()
                    : base ("Time Span")
                { }

                public override void IncStat(string stat)
                {
                    sChanged = true;

                    base.IncStat(stat);
                }

                public override float AddStat(string stat, float val)
                {
                    if (val != 0)
                    {
                        sChanged = true;
                    }

                    return base.AddStat(stat, val);
                }
            }
        }

        public class SleepLogger : Logger<SleepLogger>
        {
            readonly static SleepLogger sLogger = new SleepLogger();

            static Dictionary<ObjectGuid, TaskStats> sByGuid = new Dictionary<ObjectGuid, TaskStats>();

            static Dictionary<string, TaskStats> sByNameBefore = new Dictionary<string, TaskStats>();
            static Dictionary<string, TaskStats> sByNameAfter = new Dictionary<string, TaskStats>();

            public static void Append(string text)
            {
                ObjectGuid currentTask = Simulator.CurrentTask;

                string taskName = Common.TaskName(Simulator.GetTask(currentTask));
                if (string.IsNullOrEmpty(taskName))
                {
                    taskName = "(Empty)";
                }

                TaskStats stats = null;
                if (!sByGuid.TryGetValue(currentTask, out stats))
                {
                    stats = new TaskStats(currentTask, sAfterLoadup, taskName);
                    sByGuid.Add(currentTask, stats);
                }

                long duration = 0;
                if (text == "Begin")
                {
                    stats.mMostRecentBegin = DateTime.Now.Ticks;
                }
                else if (stats.mMostRecentBegin > 0)
                {
                    duration = DateTime.Now.Ticks - stats.mMostRecentBegin;
                    stats.Add(duration);
                }

                if (sAfterLoadup)
                {
                    if (!sByNameAfter.TryGetValue(taskName, out stats))
                    {
                        stats = new TaskStats(ObjectGuid.InvalidObjectGuid, sAfterLoadup, taskName);
                        sByNameAfter.Add(taskName, stats);
                    }
                }
                else
                {
                    if (!sByNameBefore.TryGetValue(taskName, out stats))
                    {
                        stats = new TaskStats(ObjectGuid.InvalidObjectGuid, sAfterLoadup, taskName);
                        sByNameBefore.Add(taskName, stats);
                    }
                }

                stats.Add(duration);

                //sLogger.PrivateAppend(text + "," + taskName + "," + DateTime.Now.Ticks);
            }

            protected override string Name
            {
                get { return "Sleep Logs"; }
            }

            protected override SleepLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                /*
                foreach (TaskStats stats in sByGuid.Values)
                {
                    PrivateAppend(stats.ToString());
                }

                PrivateAppend(Common.NewLine);
                */
                foreach (TaskStats stats in sByNameBefore.Values)
                {
                    PrivateAppend(stats.ToString());
                }

                foreach (TaskStats stats in sByNameAfter.Values)
                {
                    PrivateAppend(stats.ToString());
                }

                return base.PrivateLog(builder);
            }

            public class TaskStats
            {
                ObjectGuid mGuid;

                string mName;

                int mCount;

                float mDuration;

                bool mAfterLoadup;

                public long mMostRecentBegin;

                public TaskStats(ObjectGuid guid, bool afterLoadup, string name)
                {
                    mAfterLoadup = afterLoadup;
                    mGuid = guid;
                    mName = name;
                }

                public void Add(long duration)
                {
                    mCount++;
                    mDuration += duration;
                }

                public override string ToString()
                {
                    Common.StringBuilder result = new StringBuilder();

                    if (mGuid != ObjectGuid.InvalidObjectGuid)
                    {
                        result += mGuid.ToString()  + ",";
                    }

                    if (mAfterLoadup)
                    {
                        result += "After,";
                    }
                    else
                    {
                        result += "Before,";
                    }

                    result += "\"" + mName + "\"";
                    result += "," + mCount;
                    result += "," + (mDuration / TimeSpan.TicksPerMillisecond).ToString();

                    return result.ToString();
                }
            }
        }

        public class CorrectionLogger : Logger<CorrectionLogger>
        {
            readonly static CorrectionLogger sLogger = new CorrectionLogger();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Correction Logs"; }
            }

            protected override CorrectionLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (DereferenceManager.Logger.sCollecting) return 0;

                return base.PrivateLog(builder);
            }
        }

        public class ScriptCoreLogger : Logger<ScriptCoreLogger>
        {
            readonly static ScriptCoreLogger sLogger = new ScriptCoreLogger();

            public static void Append(StringBuilder noticeText, StringBuilder logText, ObjectGuid id1, ObjectGuid id2, Exception exception, Dictionary<string, bool> alreadyCaught)
            {
                sLogger.PrivateAppend(noticeText, logText, id1, id2, exception, alreadyCaught);
            }

            protected override string Name
            {
                get { return "Script Errors"; }
            }

            protected override ScriptCoreLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                sAlreadyCaught.Clear();

                return base.PrivateLog(builder);
            }
        }

        public class ExternalLogger : Logger<ExternalLogger>
        {
            static List<MethodInfo> sLogs;

            readonly static ExternalLogger sLogger = new ExternalLogger();

            protected override string Name
            {
                get { return "External Mods"; }
            }

            protected override ExternalLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (!kDebugging) return 0;

                //if (sDelayLoading) return 0;

                if (sLogs == null)
                {
                    sLogs = new List<MethodInfo>();

                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (!assembly.GetName().Name.StartsWith("NRaas")) continue;

                        Type common = assembly.GetType("NRaas.Common");
                        if (common == null) continue;

                        MethodInfo func = common.GetMethod("ExternalRecordErrors", BindingFlags.Public | BindingFlags.Static);
                        if (func == null) continue;

                        sLogs.Add(func);
                    }
                }

                foreach (MethodInfo value in sLogs)
                {
                    try
                    {
                        System.Text.StringBuilder param = new System.Text.StringBuilder();
                        value.Invoke(null, new object[] { param });
                        PrivateAppend(param.ToString());
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(value.ToString(), e);
                    }
                }

                return base.PrivateLog(builder);
            }
        }

        public class ChangeOutfitTask : Common.FunctionTask
        {
            SimDescription mSim;

            OutfitCategories mCategory;

            public ChangeOutfitTask(SimDescription sim, OutfitCategories category)
            {
                mSim = sim;
                mCategory = category;
            }

            protected override void OnPerform()
            {
                try
                {
                    Sim sim = mSim.CreatedSim;
                    if (sim == null) return;

                    if (sim.SocialComponent == null)
                    {
                        sim = ResetSimTask.Perform(sim, false);
                        if (sim == null) return;
                    }

                    if (sim.SocialComponent != null)
                    {
                        sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingOutside, mCategory, true);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, e);
                }
            }
        }
    }
}
