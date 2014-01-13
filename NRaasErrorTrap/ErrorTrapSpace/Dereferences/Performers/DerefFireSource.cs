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
    public class DerefFireSource : Dereference<FireSource>
    {
        protected override DereferenceResult Perform(FireSource reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsWatchingFire", field, objects))
            {
                Remove(reference.mSimsWatchingFire, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
