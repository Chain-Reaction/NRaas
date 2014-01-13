using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefServingContainer : Dereference<ServingContainer>
    {
        protected override DereferenceResult Perform(ServingContainer reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFoodProp", field, objects))
            {
                Remove(ref reference.mFoodProp);
                return DereferenceResult.Continue;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mDirtyBroadcaster", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mDirtyBroadcaster);
                }

                if (Performing)
                {
                    return DereferenceResult.ContinueIfReferenced;
                }
                else
                {
                    return DereferenceResult.Ignore;
                }
            }

            if (Matches(reference, "mDirtyPetBroadcaster", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mDirtyPetBroadcaster);
                }

                if (Performing)
                {
                    return DereferenceResult.ContinueIfReferenced;
                }
                else
                {
                    return DereferenceResult.Ignore;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
