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
    public class DerefTransferToFryingPanHelper : Dereference<Stove.TransferToFryingPanHelper>
    {
        protected override DereferenceResult Perform(Stove.TransferToFryingPanHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFryingPan", field, objects))
            {
                Remove(ref reference.mFryingPan);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mOldFoodProp", field, objects))
            {
                Remove(ref reference.mOldFoodProp);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mNewFoodProp", field, objects))
            {
                Remove(ref reference.mNewFoodProp);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
