using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPartData : Dereference<PartData>
    {
        protected override DereferenceResult Perform(PartData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Container", field, objects))
            {
                Remove(ref reference.Container);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mContainedSim", field, objects))
            {
                Remove(ref reference.mContainedSim );
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
