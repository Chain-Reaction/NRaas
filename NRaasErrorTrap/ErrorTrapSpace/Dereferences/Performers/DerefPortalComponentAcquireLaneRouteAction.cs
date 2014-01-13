using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPortalComponentAcquireLaneRouteAction : Dereference<PortalComponent.AcquireLaneRouteAction>
    {
        protected override DereferenceResult Perform(PortalComponent.AcquireLaneRouteAction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPortalObject", field, objects))
            {
                Remove(ref reference.mPortalObject);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mRoutingSim", field, objects))
            {
                Remove(ref reference.mRoutingSim);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
