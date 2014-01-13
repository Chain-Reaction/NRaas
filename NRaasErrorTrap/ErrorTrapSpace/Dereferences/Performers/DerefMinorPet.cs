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
    public class DerefMinorPet : Dereference<MinorPet>
    {
        protected override DereferenceResult Perform(MinorPet reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "MyTerrarium", field, objects))
            {
                Remove(ref reference.MyTerrarium);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
