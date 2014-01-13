using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFishingSpot : Dereference<FishingSpot>
    {
        protected override DereferenceResult Perform(FishingSpot reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mData", field, objects))
            {
                if (Performing)
                {
                    if (reference.HasBeenDestroyed)
                    {
                        Remove(ref reference.mData);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
