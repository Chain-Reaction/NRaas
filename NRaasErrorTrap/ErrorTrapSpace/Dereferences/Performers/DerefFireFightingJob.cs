using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFireFightingJob : Dereference<FireFightingJob>
    {
        protected override DereferenceResult Perform(FireFightingJob reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "InitialFirefighters", field, objects))
            {
                Remove(reference.InitialFirefighters, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mRubbleTrappedSims", field, objects))
            {
                Remove(reference.mRubbleTrappedSims, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
