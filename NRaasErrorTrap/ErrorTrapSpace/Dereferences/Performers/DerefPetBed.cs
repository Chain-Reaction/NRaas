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
    public class DerefPetBed : Dereference<PetBed>
    {
        protected override DereferenceResult Perform(PetBed reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActorX", field, objects))
            {
                Remove(ref reference.mActorX);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mActorY", field, objects))
            {
                Remove(ref reference.mActorY );
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
