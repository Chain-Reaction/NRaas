using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStove : Dereference<Stove>
    {
        protected override DereferenceResult Perform(Stove reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCounterUtilsList", field, objects))
            {
                Remove(reference.mCounterUtilsList, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
