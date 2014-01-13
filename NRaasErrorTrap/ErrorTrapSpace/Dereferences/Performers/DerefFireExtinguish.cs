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
    public class DerefFireExtinguish : Dereference<Fire.Extinguish>
    {
        protected override DereferenceResult Perform(Fire.Extinguish reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCurrentExtinguishable", field, objects))
            {
                Remove(ref reference.mCurrentExtinguishable);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
