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
    public class DerefShowFoodInContainerHelper : Dereference<ShowFoodInContainerHelper>
    {
        protected override DereferenceResult Perform(ShowFoodInContainerHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mContainer", field, objects))
            {
                Remove(ref reference.mContainer);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mFoodProp", field, objects))
            {
                Remove(ref reference.mFoodProp);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mObjectTransferredFrom", field, objects))
            {
                Remove(ref reference.mObjectTransferredFrom);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
