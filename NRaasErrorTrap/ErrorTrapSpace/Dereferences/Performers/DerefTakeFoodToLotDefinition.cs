using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTakeFoodToLotDefinition : Dereference<TakeFoodToLot.Definition>
    {
        protected override DereferenceResult Perform(TakeFoodToLot.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "FoodToBring", field, objects))
            {
                Remove(ref reference.FoodToBring);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
