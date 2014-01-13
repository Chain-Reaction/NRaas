using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDynamicRegionBase : Dereference<DynamicRegionBase>
    {
        protected override DereferenceResult Perform(DynamicRegionBase reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDynamicFootprint", field, objects))
            {
                Remove(ref reference.mDynamicFootprint);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
