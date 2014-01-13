using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFishTank : Dereference<FishTank>
    {
        protected override DereferenceResult Perform(FishTank reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mItems", field, objects))
            {
                Remove(reference.mItems, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
