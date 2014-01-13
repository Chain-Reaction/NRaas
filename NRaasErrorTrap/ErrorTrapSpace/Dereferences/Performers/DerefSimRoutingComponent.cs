using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimRoutingComponent : Dereference<SimRoutingComponent>
    {
        protected override DereferenceResult Perform(SimRoutingComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "LockedDoorsDuringPlan", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }

                    Remove(reference.LockedDoorsDuringPlan, objects);
                }
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mPusherSim", field, objects, out result) != MatchResult.Failure)
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

                        Remove(ref reference.mPusherSim);
                    }
                }
                return DereferenceResult.End;
            }

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

            if (Matches(reference, "mOwnerSim", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }

                    Remove(ref reference.mOwnerSim);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
