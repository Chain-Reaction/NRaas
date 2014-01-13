using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDoCarpoolPickupAndDropoff : Dereference<CarpoolManager.CarpoolData.DoCarpoolPickupAndDropoff>
    {
        protected override DereferenceResult Perform(CarpoolManager.CarpoolData.DoCarpoolPickupAndDropoff reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRiderList", field, objects))
            {
                try
                {
                    reference.Cleanup();
                }
                catch
                { }

                Remove(reference.mRiderList, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mData", field, objects))
            {
                try
                {
                    reference.Cleanup();
                }
                catch
                { }

                Remove(ref reference.mData);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
