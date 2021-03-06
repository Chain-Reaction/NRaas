﻿using NRaas.CommonSpace.Booters;
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
    public class DerefCar : Dereference<Car>
    {
        protected override DereferenceResult Perform(Car reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPassengers", field, objects))
            {
                Remove(reference.mPassengers, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
