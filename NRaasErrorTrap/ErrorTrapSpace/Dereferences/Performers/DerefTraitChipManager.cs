using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTraitChipManager : Dereference<TraitChipManager>
    {
        protected override DereferenceResult Perform(TraitChipManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mValues", field, objects))
            {
                if (Performing)
                {
                    RemoveValues(reference.mValues, objects);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mTraitChipSlots", field, objects))
            {
                if (Performing)
                {
                    Remove(reference.mTraitChipSlots, objects);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
