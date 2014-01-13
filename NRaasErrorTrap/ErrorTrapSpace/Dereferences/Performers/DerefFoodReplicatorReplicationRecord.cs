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
    public class DerefFoodReplicatorReplicationRecord : Dereference<FoodReplicator.ReplicationRecord>
    {
        protected override DereferenceResult Perform(FoodReplicator.ReplicationRecord reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "CookingProcess", field, objects))
            {
                Remove(ref reference.CookingProcess);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
