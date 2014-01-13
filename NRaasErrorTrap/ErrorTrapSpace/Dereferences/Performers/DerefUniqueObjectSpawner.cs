using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public abstract class DerefUniqueObjectSpawner<T,U> : Dereference<U>
        where T : UniqueObject
        where U : UniqueObjectSpawner<T>
    {
        protected override DereferenceResult Perform(U reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSpawnedObject", field, objects))
            {
                Remove(ref reference.mSpawnedObject);
                return DereferenceResult.End;
            }

            DereferenceResult reason = MatchAndRemove(reference, "mOnSimSelectedEventListener", field, ref reference.mOnSimSelectedEventListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
