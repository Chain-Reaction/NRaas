using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBathtub : Dereference<Bathtub>
    {
        protected override DereferenceResult Perform(Bathtub reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimInBath", field, objects))
            {
                Remove(ref reference.mSimInBath);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
