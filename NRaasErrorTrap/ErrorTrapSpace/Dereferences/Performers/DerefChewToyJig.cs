using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefChewToyJig : Dereference<ChewToyJig>
    {
        protected override DereferenceResult Perform(ChewToyJig reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mToy", field, objects))
            {
                Remove(ref reference.mToy);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
