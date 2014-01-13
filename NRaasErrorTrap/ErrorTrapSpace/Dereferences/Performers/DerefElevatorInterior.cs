using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefElevatorInterior : Dereference<ElevatorInterior>
    {
        protected override DereferenceResult Perform(ElevatorInterior reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult result = DereferenceResult.Failure;

            if (Matches(reference, "mElevatorInteriorAbove", field, objects))
            {
                Remove(ref reference.mElevatorInteriorAbove);
                result = DereferenceResult.Continue;
            }

            if (Matches(reference, "mElevatorInteriorBelow", field, objects))
            {
                Remove(ref reference.mElevatorInteriorBelow);
                result = DereferenceResult.Continue;
            }

            return result;
        }
    }
}
