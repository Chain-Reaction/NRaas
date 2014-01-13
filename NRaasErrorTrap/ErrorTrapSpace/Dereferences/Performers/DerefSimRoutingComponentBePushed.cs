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
    public class DerefSimRoutingComponentBePushed : Dereference<SimRoutingComponent.BePushed>
    {
        protected override DereferenceResult Perform(SimRoutingComponent.BePushed reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Pusher", field, objects))
            {
                //Remove(ref reference.Pusher);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
