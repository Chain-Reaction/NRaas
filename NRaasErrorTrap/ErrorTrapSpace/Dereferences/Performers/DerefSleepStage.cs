using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSleepStage : Dereference<SleepStage>
    {
        protected override DereferenceResult Perform(SleepStage reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
