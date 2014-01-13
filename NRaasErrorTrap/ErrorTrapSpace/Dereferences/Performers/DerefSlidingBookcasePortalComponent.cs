using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Door;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSlidingBookcasePortalComponent : Dereference<SlidingBookcase.SlidingBookcasePortalComponent>
    {
        protected override DereferenceResult Perform(SlidingBookcase.SlidingBookcasePortalComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCurrentlyRoutingSim", field, objects))
            {
                Remove(ref reference.mCurrentlyRoutingSim);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
