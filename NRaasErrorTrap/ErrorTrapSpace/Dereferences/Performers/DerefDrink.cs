using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDrink : Dereference<Bartending.Drink>
    {
        protected override DereferenceResult Perform(Bartending.Drink reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mIngredients", field, objects))
            {
                Remove(reference.mIngredients, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
