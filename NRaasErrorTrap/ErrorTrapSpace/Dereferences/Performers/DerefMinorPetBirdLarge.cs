using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.MinorPets;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMinorPetBirdLarge : Dereference<MinorPetBirdLarge>
    {
        protected override DereferenceResult Perform(MinorPetBirdLarge reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mGreetNonhouseholdSimBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.StopBroadcasters();
                    }
                    catch
                    { }

                    Remove(ref reference.mGreetNonhouseholdSimBroadcaster);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
