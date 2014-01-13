using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBarProfessionalTendingPosture : Dereference<BarProfessional.TendingPosture>
    {
        protected override DereferenceResult Perform(BarProfessional.TendingPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActor", field, objects))
            {
                // Causes issues on reset
                //Remove(ref reference.mActor);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mContainer", field, objects))
            {
                Remove(ref reference.mContainer);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
