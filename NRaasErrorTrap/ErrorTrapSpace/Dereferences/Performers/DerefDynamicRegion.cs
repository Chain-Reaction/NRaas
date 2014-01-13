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
    public class DerefDynamicRegion : Dereference<DynamicRegion>
    {
        protected override DereferenceResult Perform(DynamicRegion reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mMemberObjects", field, objects))
            {
                Remove(reference.mMemberObjects, objects);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
