using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Rewards;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFoodReplicator : Dereference<FoodReplicator>
    {
        protected override DereferenceResult Perform(FoodReplicator reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCounterUtilsList", field, objects))
            {
                Remove(reference.mCounterUtilsList, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mStoredRecipes", field, objects))
            {
                Remove(reference.mStoredRecipes, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
