using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public abstract class DereferenceGeneric<T> : Dereference<T>
    {
        protected abstract bool Matches(string type);

        public override DereferenceResult IPerform(KeyValuePair<object, FieldInfo> reference, List<ReferenceWrapper> objects, bool perform)
        {
            try
            {
                if (Matches(reference.Key.GetType().ToString()))
                {
                    mPerform = perform;
                    return Perform((T)reference.Key, reference.Value, objects);
                }
            }
            catch (Exception e)
            {
                Common.Exception("Object Type: " + reference.Key.GetType(), e);
            }
            return DereferenceResult.Failure;
        }

        protected void Remove<S>(FieldInfo field, ref S reference)
        {
            Remove<S>(field, null, ref reference);
        }
        protected void Remove<S>(FieldInfo field, object defValue, ref S reference)
        {
            field.SetValue(reference, defValue);
        }

        protected DereferenceResult MatchAndRemove(object parent, string fieldName, FieldInfo matchField, List<ReferenceWrapper> objects, DereferenceResult reason)
        {
            FieldInfo actualField;
            ReferenceWrapper result;
            if (Matches(parent, fieldName, matchField, objects, out result, out actualField) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        Remove(actualField, ref parent);
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
    }
}
