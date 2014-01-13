using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFire : Dereference<Fire>
    {
        protected override DereferenceResult Perform(Fire reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObjectsBeingBurned", field, objects))
            {
                Remove(reference.mObjectsBeingBurned,objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
