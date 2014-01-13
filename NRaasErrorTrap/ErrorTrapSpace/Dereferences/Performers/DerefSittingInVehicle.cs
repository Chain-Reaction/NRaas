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
    public class DerefSittingInVehicle : Dereference<SittingInVehicle>
    {
        protected override DereferenceResult Perform(SittingInVehicle reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSim", field, objects))
            {
                try
                {
                    reference.mContainer.SetObjectToReset();
                }
                catch
                { }

                //Remove(ref reference.mSim);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mContainer", field, objects))
            {
                try
                {
                    reference.mSim.Posture = null;
                }
                catch
                { }

                //Remove(ref reference.mContainer);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
