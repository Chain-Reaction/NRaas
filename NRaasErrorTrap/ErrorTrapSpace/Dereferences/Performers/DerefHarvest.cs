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
    public class DerefHarvest : Dereference<HarvestPlant.Harvest>
    {
        protected override DereferenceResult Perform(HarvestPlant.Harvest reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "IgnorePlants", field, objects))
            {
                Remove(reference.IgnorePlants, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mDummyIk", field, objects))
            {
                //Remove(ref reference.mDummyIk);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
