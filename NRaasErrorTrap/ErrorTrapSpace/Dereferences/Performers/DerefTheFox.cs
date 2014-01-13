using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Rewards;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTheFox : Dereference<TheFox>
    {
        protected override DereferenceResult Perform(TheFox reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mThief", field, objects))
            {
                Remove(ref reference.mThief);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
