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
    public class DerefTendGardenHarvestPlant : Dereference<Plant.TendGarden<HarvestPlant>>
    {
        protected override DereferenceResult Perform(Plant.TendGarden<HarvestPlant> reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mWrappedInteraction", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    Remove(ref reference.mWrappedInteraction);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
