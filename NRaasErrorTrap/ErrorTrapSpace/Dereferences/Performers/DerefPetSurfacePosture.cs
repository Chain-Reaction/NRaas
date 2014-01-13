using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPetSurfacePosture : Dereference<PetSurfacePosture>
    {
        protected override DereferenceResult Perform(PetSurfacePosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mBedJig", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.mSim.Posture = null;
                        }
                        catch
                        { }

                        //Remove(ref reference.mBedJig);
                    }
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
