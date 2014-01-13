using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDanceOnCounterOrTable : Dereference<DanceOnCounterOrTable>
    {
        protected override DereferenceResult Perform(DanceOnCounterOrTable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFadedOutObjects", field, objects))
            {
                Remove(reference.mFadedOutObjects, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
