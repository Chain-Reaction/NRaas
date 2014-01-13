using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHighChair : Dereference<HighChair>
    {
        protected override DereferenceResult Perform(HighChair reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Toddler", field, objects))
            {
                Remove(ref reference.Toddler);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
