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
    public class DerefCarrySystemPutDownCallback : Dereference<CarrySystem.PutDownCallback>
    {
        protected override DereferenceResult Perform(CarrySystem.PutDownCallback reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTempObject", field, objects))
            {
                Remove(ref reference.mTempObject);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
