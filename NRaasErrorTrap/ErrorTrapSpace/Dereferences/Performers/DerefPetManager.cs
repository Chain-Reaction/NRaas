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
    public class DerefPetManager : Dereference<PetManager>
    {
        protected override DereferenceResult Perform(PetManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "FavoriteSleepingSpotObject", field, objects))
            {
                Remove(ref reference.FavoriteSleepingSpotObject);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
