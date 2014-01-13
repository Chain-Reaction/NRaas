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
    public class DerefMinorPetTerrarium : Dereference<MinorPetTerrarium>
    {
        protected override DereferenceResult Perform(MinorPetTerrarium reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mMournBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.TurnOffMournBroadcaster();
                    }
                    catch
                    { }

                    Remove(ref reference.mMournBroadcaster);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
