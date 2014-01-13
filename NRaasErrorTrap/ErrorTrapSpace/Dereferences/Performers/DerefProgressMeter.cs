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
    public class DerefProgressMeter : Dereference<ProgressMeter>
    {
        protected override DereferenceResult Perform(ProgressMeter reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSim", field, objects))
            {
                Remove(ref reference.mSim);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mSmc", field, objects))
            {
                Remove(ref reference.mSmc);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mOwnerInteraction", field, objects))
            {
                Remove(ref reference.mOwnerInteraction);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
