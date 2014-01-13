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
    public class DerefCane : Dereference<Cane>
    {
        protected override DereferenceResult Perform(Cane reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPropCane", field, objects))
            {
                Remove(ref reference.mPropCane);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
