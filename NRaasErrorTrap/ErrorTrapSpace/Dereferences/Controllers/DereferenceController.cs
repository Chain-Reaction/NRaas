using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences.Controllers
{
    public interface IDereferenceController
    {
        void Clear();

        bool Add(Type type, List<ObjectLookup.Item> list);

        void PreProcess();

        void Perform();

        void PostPerform();
    }

    public abstract class DereferenceController<T> : IDereferenceController
        where T : class
    {
        protected Type mType = typeof(T);

        List<Pair<Type, List<ObjectLookup.Item>>> mList = new List<Pair<Type, List<ObjectLookup.Item>>>();

        protected delegate void OnProcess(T obj, object parent, FieldInfo field);

        public virtual void Clear()
        {
            mList.Clear();
        }

        public bool Add(Type type, List<ObjectLookup.Item> list)
        {
            if (mType == null) return false;

            if (!mType.IsAssignableFrom(type)) return false;

            mList.Add(new Pair<Type, List<ObjectLookup.Item>>(type, list));
            return true;
        }

        public void PreProcess()
        {
            //Common.DebugWriteLog("PreProcess: " + GetType());

            Perform(PreProcess);
        }

        protected abstract void PreProcess(T obj, object parent, FieldInfo field);

        protected virtual void SubPerform(T obj, object parent, FieldInfo field, Dictionary<ReferenceWrapper, bool> performed, OnProcess process)
        {
            ReferenceWrapper child = new ReferenceWrapper(obj);

            if (ObjectLookup.WasPostDisposed(child)) return;

            if (performed.ContainsKey(child)) return;
            performed.Add(child, true);

            process(obj, parent, field);
        }

        protected void Perform(OnProcess process)
        {
            //using (Common.TestSpan span = new Common.TestSpan(ErrorTrap.TimeSpanLogger.Bin, mType.ToString(), Common.DebugLevel.Stats))
            {
                foreach (Pair<Type, List<ObjectLookup.Item>> pair in mList)
                {
                    Dictionary<ReferenceWrapper, bool> performed = new Dictionary<ReferenceWrapper, bool>();

                    foreach (ObjectLookup.Item item in pair.Second)
                    {
                        try
                        {
                            T obj = item.mChild as T;
                            if (obj == null) continue;

                            SubPerform(obj, item.mParent, item.mField, performed, process);

                            if (DereferenceManager.Logger.OnWorldQuit) return;
                        }
                        catch (Exception e)
                        {
                            if (item.mChild is SimDescription)
                            {
                                Common.Exception(item.mChild as SimDescription, e);
                            }
                            else
                            {
                                Common.Exception(item.mChild as GameObject, e);
                            }
                        }
                    }
                }
            }
        }

        public virtual void Perform()
        {
            //Common.DebugWriteLog("Perform: " + GetType());

            Perform(Perform);
        }

        protected abstract void Perform(T obj, object parent, FieldInfo field);

        public virtual void PostPerform()
        { }
    }
}
