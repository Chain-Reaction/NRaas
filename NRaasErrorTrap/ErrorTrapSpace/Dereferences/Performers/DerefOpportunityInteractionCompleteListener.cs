using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOpportunityInteractionCompleteListener : Dereference<OpportunityInteractionCompleteListener>
    {
        protected override DereferenceResult Perform(OpportunityInteractionCompleteListener reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTargets", field, objects))
            {
                Remove(reference.mTargets, objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mTargetsUsed", field, objects))
            {
                Remove(reference.mTargetsUsed, objects);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
