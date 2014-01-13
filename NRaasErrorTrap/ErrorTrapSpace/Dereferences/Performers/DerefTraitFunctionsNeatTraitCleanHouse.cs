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
    public class DerefTraitFunctionsNeatTraitCleanHouse : Dereference<TraitFunctions.NeatTraitCleanHouse>
    {
        protected override DereferenceResult Perform(TraitFunctions.NeatTraitCleanHouse reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBlockCounts", field, objects))
            {
                RemoveKeys(reference.mBlockCounts, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
