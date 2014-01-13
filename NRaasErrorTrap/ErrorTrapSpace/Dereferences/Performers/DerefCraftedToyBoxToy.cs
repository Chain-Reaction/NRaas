using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCraftedToyBoxToy : Dereference<CraftedToyBoxToy>
    {
        protected override DereferenceResult Perform(CraftedToyBoxToy reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInventor", field, objects))
            {
                Remove(ref reference.mInventor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
