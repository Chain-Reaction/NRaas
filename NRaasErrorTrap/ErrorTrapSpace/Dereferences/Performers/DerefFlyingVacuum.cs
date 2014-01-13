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
    public class DerefFlyingVacuum : Dereference<FlyingVacuum>
    {
        protected override DereferenceResult Perform(FlyingVacuum reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mReservingSim", field, objects))
            {
                Remove(ref reference.mReservingSim);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
