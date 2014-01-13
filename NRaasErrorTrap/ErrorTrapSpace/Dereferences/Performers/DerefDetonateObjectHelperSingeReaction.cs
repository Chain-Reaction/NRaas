using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDetonateObjectHelperSingeReaction : Dereference<DetonateObjectHelper.SingeReaction>
    {
        protected override DereferenceResult Perform(DetonateObjectHelper.SingeReaction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObj", field, objects))
            {
                Remove(ref reference.mObj);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
