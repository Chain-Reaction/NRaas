using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCaregiverRoutingMonitor : Dereference<CaregiverRoutingMonitor>
    {
        protected override DereferenceResult Perform(CaregiverRoutingMonitor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "LastOnLotPositions", field, objects))
            {
                RemoveKeys(reference.LastOnLotPositions,objects);

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
