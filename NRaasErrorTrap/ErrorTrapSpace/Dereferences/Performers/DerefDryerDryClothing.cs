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
    public class DerefDryerDryClothing : Dereference<Dryer.DryClothing>
    {
        protected override DereferenceResult Perform(Dryer.DryClothing reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mClothingPile", field, objects))
            {
                Remove(ref reference.mClothingPile);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
