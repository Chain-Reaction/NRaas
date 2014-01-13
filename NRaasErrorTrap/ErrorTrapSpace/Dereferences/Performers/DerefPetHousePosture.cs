using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPetHousePosture : Dereference<PetHouse.PetHousePosture>
    {
        protected override DereferenceResult Perform(PetHouse.PetHousePosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mHouse", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Sim.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mHouse);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mContainer", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Sim.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mContainer);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
