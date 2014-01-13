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
    public class DerefSteeringAdaptor : Dereference<SteeringAdaptor>
    {
        protected override DereferenceResult Perform(SteeringAdaptor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRouteAction", field, objects))
            {
                //Remove(ref reference.mRouteAction);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
