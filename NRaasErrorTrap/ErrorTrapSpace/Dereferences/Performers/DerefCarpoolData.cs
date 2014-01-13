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
    public class DerefCarpoolData : Dereference<CarpoolManager.CarpoolData>
    {
        protected override DereferenceResult Perform(CarpoolManager.CarpoolData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCar", field, objects))
            {
                Remove(ref reference.mCar);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mDriver", field, objects))
            {
                Remove(ref reference.mDriver);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
