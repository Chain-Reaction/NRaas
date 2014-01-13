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
    public class DerefAnimationEventHelper : Dereference<AnimationEventHelper>
    {
        protected override DereferenceResult Perform(AnimationEventHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
