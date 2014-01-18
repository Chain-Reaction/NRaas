using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace
{
    public class ObjectLookup
    {
        public class Item
        {
            public readonly object mChild;

            public readonly object mParent;

            public readonly FieldInfo mField;

            public Item(object child, object parent, FieldInfo field)
            {
                mChild = child;
                mParent = parent;
                mField = field;
            }

            public override string ToString()
            {
                string msg = "Child: " + mChild.GetType();
                msg += Common.NewLine + "Parent: " + mParent.GetType();
                msg += Common.NewLine + "Field: " + mField;
                return msg;
            }
        }

        static Dictionary<Type, List<Item>> sRawList = new Dictionary<Type, List<Item>>();

        static Dictionary<ReferenceWrapper, GameObjectReference> sLookup = new Dictionary<ReferenceWrapper,GameObjectReference>();

        static Dictionary<ReferenceWrapper, bool> sPostDisposed = new Dictionary<ReferenceWrapper, bool>();

        static EventListener sDisposedEvent = null;

        public static void StartListener()
        {
            // Must be immediate
            sDisposedEvent = EventTracker.AddListener(EventTypeId.kEventObjectDisposed, OnDisposed);
        }

        protected static ListenerAction OnDisposed(Event e)
        {
            try
            {
                AddPostDisposed(new ReferenceWrapper(e.TargetObject));
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
            }

            return ListenerAction.Keep;
        }

        public static void AddPostDisposed(ReferenceWrapper obj)
        {
            if (sPostDisposed.ContainsKey(obj)) return;

            sPostDisposed.Add(obj, true);
        }

        public static bool WasPostDisposed(ReferenceWrapper obj)
        {
            return sPostDisposed.ContainsKey(obj);
        }

        public static void Clear()
        {
            sRawList.Clear();

            sPostDisposed.Clear();

            if (sDisposedEvent != null)
            {
                EventTracker.RemoveListener(sDisposedEvent);
                sDisposedEvent = null;
            }
        }

        public static Dictionary<Type, List<Item>> List
        {
            get { return sRawList; }
        }

        protected static GameObjectReference BuildReference(ReferenceWrapper obj)
        {
            GameObjectReference reference = new GameObjectReference();

            List<Item> rawList;
            if (!sRawList.TryGetValue(obj.mObject.GetType(), out rawList))
            {
                rawList = new List<Item>();
                sRawList.Add(obj.mObject.GetType(), rawList);
            }

            int count = 0;

            foreach (Item item in rawList)
            {
                if (object.ReferenceEquals(obj.mObject, item.mChild))
                {
                    reference.Add(new KeyValuePair<object, FieldInfo>(item.mParent, item.mField));
                }

                count++;
                if (count > 10000)
                {
                    Common.Sleep();
                    count = 0;
                }
            }

            if (count > 0)
            {
                Common.Sleep();
            }

            return reference;
        }

        public static void Add(object child, object parent, FieldInfo field)
        {
            Type type = child.GetType();

            List<Item> rawList;
            if (!sRawList.TryGetValue(type, out rawList))
            {
                rawList = new List<Item>();
                sRawList.Add(type, rawList);
            }

            rawList.Add(new Item(child, parent, field));
        }

        public static GameObjectReference GetReference(ReferenceWrapper obj)
        {
            if (obj.mObject == null) return null;

            GameObjectReference result;
            if (!sLookup.TryGetValue(obj, out result))
            {
                result = BuildReference(obj);

                GameObjectReference otherResult;
                if (!sLookup.TryGetValue(obj, out otherResult))
                {
                    sLookup.Add(obj, result);
                }
            }

            return result;
        }

        public static void LogCounts()
        {
            if (!Common.kDebugging) return;

            List<Pair<Type, int>> sorted = new List<Pair<Type, int>>();

            foreach (KeyValuePair<Type,List<Item>> items in sRawList)
            {
                Dictionary<ReferenceWrapper, bool> lookup = new Dictionary<ReferenceWrapper, bool>();

                int count = 0;

                foreach (Item item in items.Value)
                {
                    ReferenceWrapper child = new ReferenceWrapper(item.mChild);

                    if (lookup.ContainsKey(child)) continue;
                    lookup.Add(child, true);

                    count++;
                }

                sorted.Add(new Pair<Type, int>(items.Key, count));
            }

            sorted.Sort(new Comparison<Pair<Type, int>>(OnSortObjectCount));

            int total = 0;

            foreach (Pair<Type, int> value in sorted)
            {
                string isDelegate = typeof(Delegate).IsAssignableFrom(value.First) ? "(Delegate) " : "";

                Logger.Append(value.Second + ": " + isDelegate + value.First);

                total += value.Second;
            }

            Logger.Append(Common.NewLine + "Complete Tally: " + total);

            if (!ErrorTrapTuning.kFullStats)
            {
                Logger.Append("(Partial Statistics)");
            }
        }

        public static int OnSortObjectCount(Pair<Type, int> left, Pair<Type, int> right)
        {
            if (left.Second == right.Second)
            {
                return left.First.ToString().CompareTo(right.First.ToString());
            }
            else if (left.Second > right.Second)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public class Logger : Common.Logger<Logger>
        {
            readonly static Logger sLogger = new Logger();

            public static void Append(string msg)
            {
                if (!Common.kDebugging) return;

                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Object Count"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }
        }
    }
}
