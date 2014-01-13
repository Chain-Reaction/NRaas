using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDJTurntable : Dereference<DJTurntable>
    {
        protected override DereferenceResult Perform(DJTurntable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mStateMachine", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        try
                        {
                            reference.SetObjectToReset();
                        }
                        catch
                        { }

                        Remove(ref reference.mStateMachine);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
