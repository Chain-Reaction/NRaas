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
    public class DerefPathAdaptor : Dereference<PathAdaptor>
    {
        protected override DereferenceResult Perform(PathAdaptor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mCallbackObject", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCallbackObject);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mRouteAction", field, objects))
            {
                //Remove(ref reference.mRouteAction);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
