using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCurtain : Dereference<Curtain>
    {
        protected override DereferenceResult Perform(Curtain reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCachedWindow", field, objects))
            {
                Remove(ref reference.mCachedWindow);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
