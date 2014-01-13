using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPetBedPosture : Dereference<PetBed.PetBedPosture>
    {
        protected override DereferenceResult Perform(PetBed.PetBedPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBed", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mSim.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mBed);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
