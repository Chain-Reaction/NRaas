using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefLookAtManager : Dereference<LookAtManager>
    {
        protected override DereferenceResult Perform(LookAtManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mLastReactTime", field, objects))
            {
                RemoveKeys(reference.mLastReactTime, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mOwner", field, objects))
            {
                Remove(ref reference.mOwner);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
