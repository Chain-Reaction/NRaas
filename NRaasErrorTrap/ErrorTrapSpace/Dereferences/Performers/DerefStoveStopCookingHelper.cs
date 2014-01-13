using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStoveStopCookingHelper : Dereference<Stove.StopCookingHelper>
    {
        protected override DereferenceResult Perform(Stove.StopCookingHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTarget", field, objects))
            {
                Remove(ref reference.mTarget);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
