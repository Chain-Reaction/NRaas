using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefAthleticGameObjectWorkOut : Dereference<AthleticGameObject.WorkOut>
    {
        protected override DereferenceResult Perform(AthleticGameObject.WorkOut reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mWorkoutJig", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    //Remove(ref reference.mWorkoutJig);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
