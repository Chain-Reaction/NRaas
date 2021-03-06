﻿using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefShowFloor : Dereference<ShowFloor>
    {
        protected override DereferenceResult Perform(ShowFloor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSlottedObjects", field, objects))
            {
                RemoveValues(reference.mSlottedObjects, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
