using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDoor : Dereference<Door>
    {
        protected override DereferenceResult Perform(Door reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "VelvetRope", field, objects))
            {
                Remove(ref reference.VelvetRope);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
