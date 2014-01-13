using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefToyBox : Dereference<ToyBox>
    {
        protected override DereferenceResult Perform(ToyBox reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimPlayingInToyBox", field, objects))
            {
                Remove(ref reference.mSimPlayingInToyBox);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
