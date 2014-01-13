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
    public class DerefBuffInstanceNegligent : Dereference<BuffNegligent.BuffInstanceNegligent>
    {
        protected override DereferenceResult Perform(BuffNegligent.BuffInstanceNegligent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "MissedSims", field, objects))
            {
                Remove(reference.MissedSims, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
