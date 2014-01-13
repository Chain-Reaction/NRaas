using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGetInCarpoolDefinition : Dereference<CarpoolManager.GetInCarpool.Definition>
    {
        protected override DereferenceResult Perform(CarpoolManager.GetInCarpool.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Riders", field, objects))
            {
                Remove(reference.Riders, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
