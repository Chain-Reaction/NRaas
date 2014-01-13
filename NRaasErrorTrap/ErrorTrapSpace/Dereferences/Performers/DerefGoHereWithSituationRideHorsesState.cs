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
    public class DerefGoHereWithSituationRideHorsesState : Dereference<GoHereWithSituation.RideHorsesState>
    {
        protected override DereferenceResult Perform(GoHereWithSituation.RideHorsesState reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFollowersAndLeader", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mFollowersAndLeader, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPotentialCarriers", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(reference.mPotentialCarriers, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
