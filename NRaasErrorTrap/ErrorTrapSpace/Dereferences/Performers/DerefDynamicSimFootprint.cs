using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCarRoutingComponent : Dereference<CarRoutingComponent>
    {
        protected override DereferenceResult Perform(CarRoutingComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwnerVehicle", field, objects))
            {
                Remove(ref reference.mOwnerVehicle);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
