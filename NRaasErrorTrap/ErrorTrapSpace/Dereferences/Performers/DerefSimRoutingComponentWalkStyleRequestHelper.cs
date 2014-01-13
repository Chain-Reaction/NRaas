using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimRoutingComponentWalkStyleRequestHelper : Dereference<SimRoutingComponent.WalkStyleRequestHelper>
    {
        protected override DereferenceResult Perform(SimRoutingComponent.WalkStyleRequestHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mWalkStyleRequests", field, objects))
            {
                RemoveKeys(reference.mWalkStyleRequests, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
