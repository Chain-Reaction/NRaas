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
    public class DerefDoCarpoolPickupAndDropoffDefinition : Dereference<CarpoolManager.CarpoolData.DoCarpoolPickupAndDropoff.Definition>
    {
        protected override DereferenceResult Perform(CarpoolManager.CarpoolData.DoCarpoolPickupAndDropoff.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "RiderList", field, objects))
            {
                Remove(reference.RiderList, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
