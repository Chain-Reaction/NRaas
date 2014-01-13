using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRabbitHoleSlotInfo : Dereference<RabbitHole.SlotInfo>
    {
        protected override DereferenceResult Perform(RabbitHole.SlotInfo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Footprint", field, objects))
            {
                Remove(ref reference.Footprint);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
