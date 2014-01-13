using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlantDisposeDeadPlant : Dereference<Plant.DisposeDeadPlant>
    {
        protected override DereferenceResult Perform(Plant.DisposeDeadPlant reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTrashPile", field, objects))
            {
                Remove(ref reference.mTrashPile);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "IgnorePlants", field, objects))
            {
                Remove(reference.IgnorePlants, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
