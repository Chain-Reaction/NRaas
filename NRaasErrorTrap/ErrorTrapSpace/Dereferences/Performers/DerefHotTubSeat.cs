using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHotTubSeat : Dereference<HotTubSeat>
    {
        protected override DereferenceResult Perform(HotTubSeat reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "DrinkRef", field, objects))
            {
                Remove(ref reference.DrinkRef);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
