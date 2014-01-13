using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCarryingGlassPosture : Dereference<Glass.CarryingGlassPosture>
    {
        protected override DereferenceResult Perform(Glass.CarryingGlassPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObjectBeingCarried", field, objects))
            {
                Remove(ref reference.mObjectBeingCarried);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
