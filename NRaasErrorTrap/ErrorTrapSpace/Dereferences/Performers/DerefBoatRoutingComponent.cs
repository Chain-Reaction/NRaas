using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBoatRoutingComponent : Dereference<BoatRoutingComponent>
    {
        protected override DereferenceResult Perform(BoatRoutingComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mDynamicFootprint", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.OnReset();
                        }
                        catch
                        { }

                        Remove(ref reference.mDynamicFootprint);
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mOwnerVehicle", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }

                    Remove(ref reference.mOwnerVehicle);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
