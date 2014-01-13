using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefEggHuntController : Dereference<EggHuntController>
    {
        protected override DereferenceResult Perform(EggHuntController reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPlacedEggs", field, objects))
            {
                Remove(reference.mPlacedEggs, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
