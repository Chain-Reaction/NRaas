using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DereferenceManagerTuning
    {
        [Tunable, TunableComment("Whether to display log for Found reference test")]
        public static bool kShowFoundReferences = false;
    }

    public class DereferenceManager
    {
        [Tunable, TunableComment("The number of references to store before writing a log")]
        public static int kRecursionDebugging = 0;

        static List<IDereference> sDereferences;

        public static List<IDereference> Dereferences
        {
            get
            {
                if (sDereferences == null)
                {
                    sDereferences = Common.DerivativeSearch.Find<IDereference>();

                    if (Common.kDebugging)
                    {
                        foreach (IDereference dereference in sDereferences)
                        {
                            ErrorTrap.LogCorrection("Dereferencer: " + dereference.GetType());
                        }
                    }
                }
                return sDereferences;
            }
        }

        public static bool HasBeenDestroyed(IGameObject obj)
        {
            if (obj == null) return true;

            Sim sim = obj as Sim;
            if (sim != null)
            {
                // Must be checked prior to HasBeenDestroyed to stop a script error
                if (sim.mSimDescription == null) return true;
            }

            return obj.HasBeenDestroyed;
        }

        public static bool Perform(object obj, GameObjectReference references, bool perform, bool reportFound)
        {
            bool immediate = false;

            bool report = true;
            if ((obj is RabbitHoleDoorJig) || (obj is PhoneCell))
            {
                report = ErrorTrap.kDebugging;
            }
            else if (obj is Lot)
            {
                immediate = true;
            }
            else if (obj is Sim)
            {
                Sim sim = obj as Sim;
                if (sim.LotHome != null)
                {
                    if ((RecoverMissingSimTask.FindPlaceholderForSim(sim.SimDescription) == null) &&
                        ((sim.SimDescription.CreatedSim == null) || (sim.SimDescription.CreatedSim == sim)))
                    {
                        immediate = true;
                    }
                }
            }

            ReferenceLog log = null;

            if (!perform)
            {
                log = new ReferenceLog("Potential Object Found: " + ErrorTrap.GetQualifiedName(obj), report, immediate);
            }

            if (references == null) return false;

            if ((!ErrorTrap.kDebugging) && (references.Count == 0))
            {
                return false;
            }

            if (perform)
            {
                log = new ReferenceLog("Destroyed Object Found: " + ErrorTrap.GetQualifiedName(obj), report, immediate);
            }

            log.Perform = perform;
            log.ReportFound = reportFound;

            if ((!ErrorTrap.kDebugging) && (obj is Lot))
            {
                return false;
            }

            List<DereferenceStack> stack = new List<DereferenceStack>();

            foreach (KeyValuePair<object, FieldInfo> pair in references)
            {
                stack.Add(new DereferenceStack(pair, new Dictionary<ReferenceWrapper, bool>(), new List<ReferenceWrapper>(), new ReferenceWrapper(obj), false, false));
            }

            bool firstFailure = true;

            int index = 0;
            while (index < stack.Count)
            {
                DereferenceStack stackObject = stack[index];
                index++;

                ReferenceWrapper key = new ReferenceWrapper(stackObject.mReference.Key);

                bool recursion = false;
                if (stackObject.ContainsKey(key))
                {
                    recursion = true;
                }

                if ((perform) && (stackObject.Count == 1))
                {
                    log.DumpLog(false);
                }

                object reference = stackObject.mReference.Key;

                string depth = "";

                for (int i = 0; i < stackObject.Count - 1; i++)
                {
                    depth += " ";
                }

                bool reset = false, success = false;

                foreach (IDereference dereference in DereferenceManager.Dereferences)
                {
                    DereferenceResult result = dereference.IPerform(stackObject.mReference, stackObject.mList, perform);
                    if (result == DereferenceResult.Found)
                    {
                        log.SetFoundSuccess();
                    }

                    if (result != DereferenceResult.Failure)
                    {
                        stackObject.mResult = depth + "  Reference " + result +  ": " + reference.GetType() + " " + stackObject.mReference.Value + " (Using " + dereference.GetType().Name + ")";
                        if (report)
                        {
                            log.Add(stackObject.mResult);
                        }                           

                        success = true;

                        bool checkSuccess = false;

                        if (reference is GameObject)
                        {
                            stackObject.mEndFound = true;

                            checkSuccess = true;

                            GameObject gameObject = reference as GameObject;
                            if ((perform) && (!HasBeenDestroyed(gameObject)) && (!reset) && (!(reference is Sim)))
                            {
                                try
                                {
                                    reset = true;
                                    gameObject.SetObjectToReset();

                                    log.Add(depth + "  Object Reset: " + ErrorTrap.GetQualifiedName(gameObject));
                                }
                                catch //(Exception e)
                                {
                                    //Common.DebugException(gameObject, e);
                                }
                            }
                        }
                        else 
                        {
                            switch (result)
                            {
                                case DereferenceResult.End:
                                case DereferenceResult.Found:
                                    stackObject.mEndFound = true;
                                    break;
                                case DereferenceResult.Continue:
                                case DereferenceResult.ContinueIfReferenced:
                                case DereferenceResult.Ignore:
                                    bool unimportant = stackObject.mUnimportant;

                                    switch (result)
                                    {
                                        case DereferenceResult.ContinueIfReferenced:
                                            unimportant = true;
                                            break;
                                        case DereferenceResult.Ignore:
                                            unimportant = true;

                                            stackObject.mEndFound = true;
                                            break;
                                    }

                                    if (!recursion)
                                    {
                                        if (ShouldRecurse(reference))
                                        {
                                            bool referenced = false;

                                            GameObjectReference gameReference = ObjectLookup.GetReference(key);
                                            if (gameReference != null)
                                            {
                                                foreach (KeyValuePair<object, FieldInfo> pair in gameReference)
                                                {
                                                    stack.Insert(index, new DereferenceStack(pair, stackObject, key, unimportant, stackObject.mEndFound));
                                                    referenced = true;
                                                }
                                            }

                                            if (!referenced)
                                            {
                                                checkSuccess = true;

                                                if (result == DereferenceResult.ContinueIfReferenced)
                                                {
                                                    stackObject.mEndFound = true;
                                                }
                                            }
                                            else
                                            {
                                                stackObject.mBridge = true;
                                            }
                                        }
                                        else
                                        {
                                            stackObject.mEndFound = true;
                                        }
                                    }
                                    else if (result == DereferenceResult.ContinueIfReferenced)
                                    {
                                        stackObject.mEndFound = true;
                                    }
                                    else
                                    {
                                        checkSuccess = true;
                                    }
                                    break;
                                default:
                                    if (recursion)
                                    {
                                        checkSuccess = true;
                                    }
                                    break;
                            }
                        }

                        if (checkSuccess)
                        {
                            switch (result)
                            {
                                case DereferenceResult.End:
                                case DereferenceResult.Ignore:
                                    break;
                                default:
                                    if (recursion)
                                    {
                                        if (!object.ReferenceEquals(key.mObject, obj))
                                        {
                                            log.SetFailure("A");
                                        }
                                        else
                                        {
                                            stackObject.mEndFound = true;
                                        }
                                    }
                                    else
                                    {
                                        log.SetFailure("B");
                                    }
                                    break;
                            }
                        }
                    }
                }

                if (!success)
                {
                    bool mustRecurse = false;

                    string priority = "UNHANDLED";
                    if (recursion)
                    {
                        priority = "Recursion";

                        if (object.ReferenceEquals(key.mObject, obj))
                        {
                            stackObject.mEndFound = true;
                        }
                        else
                        {
                            stackObject.mBridge = true;
                        }
                    }
                    else if ((reference.GetType().IsArray) || 
                        (reference is IList) || 
                        (reference is IDictionary) || 
                        (reference is ICollection) ||
                        (reference.GetType().Name.Contains("ForgetfulList")) || 
                        (reference.GetType().Name.Contains("PairedListDictionary")))
                    {
                        priority = "Bridge";
                        mustRecurse = true;

                        stackObject.mBridge = true;
                    }
                    else if (stackObject.mUnimportant)
                    {
                        priority = "Ancillary";

                        if ((reference is GameObject) || (reference is SimDescription))
                        {
                            recursion = true;
                        }
                        else
                        {
                            mustRecurse = true;
                        }

                        stackObject.mBridge = true;
                    }
                    else
                    {
                        log.ImportantFound ();

                        log.SetFailure("C");
                    }

                    stackObject.mResult = depth + "  " + priority + " Reference: " + ErrorTrap.GetQualifiedName(reference) + " " + stackObject.mReference.Value;

                    if (report)
                    {
                        log.Add(stackObject.mResult);
                    }

                    bool referenced = false;
                    if (!recursion)
                    {
                        if (ShouldRecurse(reference))
                        {
                            GameObjectReference gameReference = ObjectLookup.GetReference(key);
                            if (gameReference != null)
                            {
                                foreach (KeyValuePair<object, FieldInfo> pair in gameReference)
                                {
                                    stack.Insert(index, new DereferenceStack(pair, stackObject, key, stackObject.mUnimportant, stackObject.mEndFound));
                                    referenced = true;
                                }
                            }
                        }
                        else
                        {
                            stackObject.mEndFound = true;

                            mustRecurse = false;
                        }
                    }

                    if (!referenced)
                    {
                        if (mustRecurse)
                        {
                            log.SetFailure("D");
                        }
                    }
                    else
                    {
                        stackObject.mBridge = true;
                    }
                }

                if ((!perform) && (!log.IsTotalSuccess) && (firstFailure))
                {
                    firstFailure = false;
                    log.Add(depth + "  RETAINED " + log.FailureReason);
                }
            }

            bool first = true;
            foreach (DereferenceStack stackObject in stack)
            {
                if (stackObject.mEndFound) continue;

                if (stackObject.mBridge) continue;

                log.EndMissing();

                if (first)
                {
                    first = false;

                    log.Add("  OPEN ENDED");
                }

                log.Add(stackObject.mResult);
            }

            log.DumpLog(false);

            Logger.ForceRecord();

            return log.IsTotalSuccess;
        }

        protected static bool ShouldRecurse(object obj)
        {
            if (obj is GameObject)
            {
                return false;
            }
            else if (obj is SimDescription)
            {
                return false;
            }
            else if (obj is MiniSimDescription)
            {
                return false;
            }
            else if (obj is Household)
            {
                return false;
            }
            else if (obj is Relationship)
            {
                return false;
            }
            else if (obj is Genealogy)
            {
                return false;
            }

            return true;
        }

        public class DereferenceStack
        {
            public readonly KeyValuePair<object, FieldInfo> mReference;

            readonly Dictionary<ReferenceWrapper, bool> mLookup;

            public readonly List<ReferenceWrapper> mList;

            public readonly bool mUnimportant;

            public bool mEndFound;
            
            public bool mBridge;

            public string mResult;

            public DereferenceStack(KeyValuePair<object, FieldInfo> reference, DereferenceStack frame, ReferenceWrapper obj, bool unimportant, bool endFound)
                : this(reference, frame.mLookup, frame.mList, obj, unimportant, endFound)
            { }
            public DereferenceStack(KeyValuePair<object, FieldInfo> reference, Dictionary<ReferenceWrapper, bool> lookup, List<ReferenceWrapper> list, ReferenceWrapper obj, bool unimportant, bool endFound)
            {
                mReference = reference;
                mLookup = new Dictionary<ReferenceWrapper, bool>(lookup);
                mLookup.Add(obj, true);

                mList = new List<ReferenceWrapper>(list);
                mList.Add(obj);

                mUnimportant = unimportant;
                mEndFound = endFound;
            }

            public int Count
            {
                get { return mList.Count; }
            }

            public bool ContainsKey(ReferenceWrapper obj)
            {
                return mLookup.ContainsKey(obj);
            }
        }

        public class ReferenceLog
        {
            string mPrimaryText;

            string mFailureReason;

            bool mReport = false;

            bool mEndMissing = false;
            bool mImportantFound = false;

            bool mPerform = false;
            bool mTotalSuccess = true;

            bool mFoundSuccess;
            bool mReportFound;

            List<string> mAdditionalInfo = new List<string>();

            public ReferenceLog(string primaryText, bool report, bool immediate)
            {
                mPrimaryText = primaryText;
                mReport = report;

                if (immediate)
                {
                    Logger.Append(mPrimaryText);
                    mPrimaryText = null;
                }
            }

            public void ImportantFound()
            {
                mImportantFound = true;
            }

            public void EndMissing()
            {
                mEndMissing = true;
            }

            public void SetFailure(string reason)
            {
                mTotalSuccess = false;
                mFailureReason = reason;
            }

            public bool IsTotalSuccess
            {
                get { return (mTotalSuccess) && (!mFoundSuccess); }
            }

            public string FailureReason
            {
                get { return mFailureReason; }
            }

            public bool Perform
            {
                set { mPerform = value; }
            }

            public bool ReportFound
            {
                set { mReportFound = value; }
            }

            public void SetFoundSuccess()
            {
                mFoundSuccess = true;
                mFailureReason = "FOUND SUCCESS";
            }

            public void Add(string text)
            {
                mAdditionalInfo.Add(text);

                if ((kRecursionDebugging > 0) && (mAdditionalInfo.Count > kRecursionDebugging))
                {
                    DumpLog(Common.kDebugging);

                    Logger.ForceRecord();
                }
            }

            public void DumpLog(bool force)
            {
                if ((force) || (!mFoundSuccess) /*|| ((!mPerform) && (mTotalSuccess) && (ErrorTrap.kDebugging))*/ || (mReportFound) || (DereferenceManagerTuning.kShowFoundReferences))
                {
                    if ((mImportantFound) || (mEndMissing) || (ErrorTrap.kDebugging))
                    {
                        if ((ErrorTrap.kDebugging) || ((mReport) && (mAdditionalInfo.Count > 0)))
                        {
                            if (mPrimaryText != null)
                            {
                                Logger.Append(mPrimaryText);
                                mPrimaryText = null;
                            }

                            foreach (string text in mAdditionalInfo)
                            {
                                Logger.Append(text);
                            }
                        }

                        mAdditionalInfo.Clear();
                    }
                }

                mImportantFound = false;
                mEndMissing = false;
                mAdditionalInfo.Clear();
            }
        }

        public class Logger : Common.Logger<Logger>
        {
            readonly static Logger sLogger = new Logger();

            public static bool sForceLog;

            public static bool sCollecting;

            static bool sOnWorldQuit;

            public static void Append(string msg)
            {
                if (string.IsNullOrEmpty(msg)) return;

                sLogger.PrivateAppend(msg);

                //ForceRecord();
            }

            public static void ForceRecord()
            {
                if ((kRecursionDebugging > 0) && (StaticCount > kRecursionDebugging))
                {
                    sForceLog = true;
                    try
                    {
                        Common.RecordErrors();
                    }
                    finally
                    {
                        sForceLog = false;
                    }
                }
            }

            protected override string Name
            {
                get { return "Dereferencing"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }

            public static int StaticCount
            {
                get
                {
                    return sLogger.Count;
                }
            }

            public static bool OnWorldQuit
            {
                get { return sOnWorldQuit; }
                set
                {
                    sOnWorldQuit = value;
                    if (value)
                    {
                        sLogger.Clear();
                    }
                }
            }

            protected override int PrivateLog(Common.StringBuilder builder)
            {
                if (sOnWorldQuit) return 0;

                if ((sCollecting) && (!sForceLog)) return 0;

                return base.PrivateLog(builder);
            }
        }
    }
}
