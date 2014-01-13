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
    public class DerefMooringPost : Dereference<MooringPost>
    {
        protected override DereferenceResult Perform(MooringPost reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mReservedVehicle", field, objects))
            {
                try
                {
                    if (Performing)
                    {
                        reference.StopTetherEffect();
                        reference.DisableWaterFootprint();
                    }
                }
                catch
                { }

                Remove(ref reference.mReservedVehicle);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
