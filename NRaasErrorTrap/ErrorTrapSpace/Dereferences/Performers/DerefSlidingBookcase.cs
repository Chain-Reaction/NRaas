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
    public class DerefSlidingBookcase : Dereference<SlidingBookcase>
    {
        protected override DereferenceResult Perform(SlidingBookcase reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mHaveInspectedSims", field, objects))
            {
                Remove(reference.mHaveInspectedSims, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mFogLocator", field, objects))
            {
                Remove(ref reference.mFogLocator);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
