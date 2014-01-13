using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DishwasherUseDishwasher : Dereference<Dishwasher.UseDishwasher>
    {
        protected override DereferenceResult Perform(Dishwasher.UseDishwasher reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mThingToWash", field, objects))
            {
                //Remove(ref reference.mThingToWash);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
