using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOneShotFunction : Dereference<OneShotFunction>
    {
        protected override DereferenceResult Perform(OneShotFunction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFunction", field, objects))
            {
                //Remove(ref reference.mFunction);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
