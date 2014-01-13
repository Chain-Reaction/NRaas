using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSpringDanceFloor : Dereference<SpringDanceFloor>
    {
        protected override DereferenceResult Perform(SpringDanceFloor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SimDancers", field, objects))
            {
                RemoveKeys(reference.SimDancers, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
