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
    public class DerefRepairableComponent : Dereference<RepairableComponent>
    {
        protected override DereferenceResult Perform(RepairableComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBrokenBroadcaster", field, objects))
            {
                Remove(ref reference.mBrokenBroadcaster);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
