using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPetAdoptionNeighborAdoption : Dereference<PetAdoption.NeighborAdoption>
    {
        protected override DereferenceResult Perform(PetAdoption.NeighborAdoption reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPetsToAdopt", field, objects))
            {
                Remove(reference.mPetsToAdopt, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
