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
    public class DerefFoodTransferHelper : Dereference<FoodProcessor.FoodTransferHelper>
    {
        protected override DereferenceResult Perform(FoodProcessor.FoodTransferHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mContainer", field, objects))
            {
                Remove(ref reference.mContainer);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
