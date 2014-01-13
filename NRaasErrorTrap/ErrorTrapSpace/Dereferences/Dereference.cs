using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public interface IDereference
    {
        DereferenceResult IPerform(KeyValuePair<object, FieldInfo> reference, List<ReferenceWrapper> objects, bool perform);
    }

    public enum DereferenceResult
    {
        Failure,
        End,
        Ignore,
        Continue,
        ContinueIfReferenced,
        Found,
    }

    public abstract class Dereference<T> : IDereference
    {
        protected enum MatchResult
        {
            Success,
            Failure,
            CorrectField,
        }

        protected bool mPerform;

        protected bool Performing
        {
            get { return mPerform; }
        }

        public virtual DereferenceResult IPerform(KeyValuePair<object, FieldInfo> reference, List<ReferenceWrapper> objects, bool perform)
        {
            if (reference.Key is T)
            {
                mPerform = perform;
                return Perform((T)reference.Key, reference.Value, objects);
            }
            else
            {
                return DereferenceResult.Failure;
            }
        }

        protected S FindLast<S>(ICollection<ReferenceWrapper> objects)
        {
            S result = default(S);

            foreach (ReferenceWrapper obj in objects)
            {
                if (obj.mObject is S)
                {
                    result = (S)obj.mObject;
                }
            }

            return result;
        }

        protected S Find<S>(ICollection<ReferenceWrapper> objects)
        {
            foreach (ReferenceWrapper obj in objects)
            {
                if (obj.mObject is S) return ((S)obj.mObject);
            }

            return default(S);
        }

        protected DereferenceResult MatchAndRemove(object parent, string fieldName, FieldInfo matchField, ref EventListener listener, List<ReferenceWrapper> objects, DereferenceResult reason)
        {
            ReferenceWrapper result;
            if (Matches(parent, fieldName, matchField, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        Remove(ref listener);
                    }

                    return reason;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }
            else
            {
                return DereferenceResult.Failure;
            }
        }

        protected bool Matches(object parent, string fieldName, FieldInfo matchField, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            return (Matches(parent, fieldName, matchField, objects, out result) == MatchResult.Success);
        }
        protected MatchResult Matches(object parent, string fieldName, FieldInfo matchField, List<ReferenceWrapper> objects, out ReferenceWrapper result)
        {
            FieldInfo actualField;
            return Matches(parent, fieldName, matchField, objects, out result, out actualField);
        }
        protected MatchResult Matches(object parent, string fieldName, FieldInfo matchField, List<ReferenceWrapper> objects, out ReferenceWrapper result, out FieldInfo actualField)
        {
            result = ReferenceWrapper.Empty;

            actualField = parent.GetType().GetField(fieldName, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance);

            object fieldValue = null;

            try
            {
                if (actualField != null)
                {
                    fieldValue = actualField.GetValue(parent);
                }
            }
            catch (Exception e)
            {
                Common.Exception(fieldName + Common.NewLine + parent.GetType(), e);
                return MatchResult.Failure;
            }

            if ((fieldValue == null) && (actualField == matchField))
            {
                return MatchResult.Success;
            }

            if (objects.Count > 0)
            {
                result = objects[objects.Count-1];
            }

            if ((result != null) && (result.Equals(fieldValue)))
            {
                return MatchResult.Success;
            }
            else if (actualField == matchField)
            {
                return MatchResult.CorrectField;
            }
            else
            {
                return MatchResult.Failure;
            }
        }

        protected void RemoveDelegate<S>(ref S reference, ICollection<ReferenceWrapper> objects)
            where S : class
        {
            if (!Performing) return;

            Delegate del = FindLast<S>(objects) as Delegate;
            if (del != null)
            {
                reference = Delegate.Remove(reference as Delegate, del) as S;
            }
        }

        protected void Remove(ref EventListener obj)
        {
            if (!Performing) return;

            try
            {
                EventTracker.RemoveListener(obj);
            }
            catch
            { }

            obj = null;
        }

        protected void Remove<S>(ref S obj)
        {
            if (!Performing) return;

            obj = default(S);
        }

        protected void Remove<S>(S[] list, ICollection<ReferenceWrapper> objects)
        {
            if (!Performing) return;

            if (list == null) return;

            for (int index = 0; index < list.Length; index++)
            {
                S item = list[index];

                if (objects.Contains(new ReferenceWrapper(item)))
                {
                    list[index] = default(S);
                }
            }
        }

        protected void Remove(ArrayList list, ICollection<ReferenceWrapper> objects)
        {
            if (!Performing) return;

            if (list == null) return;

            for (int index = list.Count-1; index >= 0; index--)
            {
                object item = list[index];

                if (objects.Contains(new ReferenceWrapper(item)))
                {
                    list.RemoveAt(index);
                }
            }
        }

        protected bool Remove<S>(List<S> list, ICollection<ReferenceWrapper> objects)
        {
            if (list == null) return false;

            bool success = false;

            for (int i=list.Count-1; i>=0; i--)
            {
                if (objects.Contains(new ReferenceWrapper(list[i])))
                {
                    if (Performing)
                    {
                        list.RemoveAt(i);
                    }
                    success = true;
                }
            }

            return success;
        }

        protected bool Remove<S>(ListSorted<S> list, ICollection<ReferenceWrapper> objects)
            where S : IComparable<S>
        {
            if (list == null) return false;

            bool success = false;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (objects.Contains(new ReferenceWrapper(list[i])))
                {
                    if (Performing)
                    {
                        list.RemoveAt(i);
                    }
                    success = true;
                }
            }

            return success;
        }

        protected List<object> RemoveKeys(Hashtable lookup, ICollection<ReferenceWrapper> objects)
        {
            if (lookup == null) return new List<object>();

            List<object> remove = new List<object>();

            if (Performing)
            {
                foreach (object key in lookup.Keys)
                {
                    if (objects.Contains(new ReferenceWrapper(key)))
                    {
                        remove.Add(key);
                    }
                }
            }

            List<object> results = new List<object>();

            foreach (object key in remove)
            {
                results.Add(lookup[key]);

                lookup.Remove(key);
            }

            return results;
        }
        protected List<S> RemoveKeys<U, S>(IDictionary<U, S> lookup, ICollection<ReferenceWrapper> objects)
        {
            if (lookup == null) return new List<S>();

            List<KeyValuePair<U, S>> remove = new List<KeyValuePair<U, S>>();

            if (Performing)
            {
                foreach (KeyValuePair<U, S> pair in lookup)
                {
                    if (objects.Contains(new ReferenceWrapper(pair.Key)))
                    {
                        remove.Add(pair);
                    }
                }
            }

            List<S> results = new List<S>();

            foreach (KeyValuePair<U, S> pair in remove)
            {
                lookup.Remove(pair.Key);

                results.Add(pair.Value);
            }

            return results;
        }

        protected List<U> RemoveValues<U, S>(IDictionary<U, S> lookup, ICollection<ReferenceWrapper> objects)
        {
            if (lookup == null) return new List<U>();

            List<KeyValuePair<U, S>> remove = new List<KeyValuePair<U, S>>();

            if (Performing)
            {
                foreach (KeyValuePair<U, S> pair in lookup)
                {
                    if (objects.Contains(new ReferenceWrapper(pair.Value)))
                    {
                        remove.Add(pair);
                    }
                }
            }

            List<U> results = new List<U>();

            foreach (KeyValuePair<U, S> pair in remove)
            {
                lookup.Remove(pair.Key);

                results.Add(pair.Key);
            }

            return results;
        }

        protected List<KeyValuePair<object, FieldInfo>> GetReferences(object obj)
        {
            GameObjectReference reference = ObjectLookup.GetReference(new ReferenceWrapper(obj));
            if (reference == null)
            {
                return new List<KeyValuePair<object, FieldInfo>>();
            }
            else
            {
                return reference.GetFullReferenceList();
            }
        }

        protected abstract DereferenceResult Perform(T reference, FieldInfo field, List<ReferenceWrapper> objects);
    }
}
