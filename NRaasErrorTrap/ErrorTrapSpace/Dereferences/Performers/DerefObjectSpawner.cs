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
    public class DerefObjectSpawner : Dereference<ObjectSpawner>
    {
        protected override DereferenceResult Perform(ObjectSpawner reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mObjectSet", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    RemoveKeys(reference.mObjectSet, objects);
                }

                return DereferenceResult.End;
            }

            DereferenceResult reason = MatchAndRemove(reference, "mObjectDisposedListener", field, ref reference.mObjectDisposedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
