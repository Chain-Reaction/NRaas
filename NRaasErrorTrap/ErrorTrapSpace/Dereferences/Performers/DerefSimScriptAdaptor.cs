using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimScriptAdaptor : Dereference<SimScriptAdaptor>
    {
        protected override DereferenceResult Perform(SimScriptAdaptor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRoutingSim", field, objects))
            {
                Remove(ref reference.mRoutingSim);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mTeenCurfewHelper", field, objects))
            {
                //Remove(ref reference.mTeenCurfewHelper);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
