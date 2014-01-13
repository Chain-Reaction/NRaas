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
    public class DerefDynamicSimFootprint : Dereference<DynamicSimFootprint>
    {
        protected override DereferenceResult Perform(DynamicSimFootprint reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mParentSim", field, objects))
            {
                Remove(ref reference.mParentSim);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
