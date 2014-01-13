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
    public class DerefHarvestPlant : Dereference<HarvestPlant>
    {
        protected override DereferenceResult Perform(HarvestPlant reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Seed", field, objects))
            {
                Remove(ref reference.Seed);
                if (Performing)
                {
                    return DereferenceResult.Continue;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mSoil", field, objects))
            {
                Remove(ref reference.mSoil);

                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
