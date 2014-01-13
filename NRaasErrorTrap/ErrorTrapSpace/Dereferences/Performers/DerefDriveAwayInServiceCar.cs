using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDriveAwayInServiceCar : Dereference<DriveAwayInServiceCar.Definition>
    {
        protected override DereferenceResult Perform(DriveAwayInServiceCar.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Car", field, objects))
            {
                Remove(ref reference.Car);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
