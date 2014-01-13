using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTV : Dereference<TV>
    {
        protected override DereferenceResult Perform(TV reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsWatchingTv", field, objects))
            {
                Remove(reference.mSimsWatchingTv, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
