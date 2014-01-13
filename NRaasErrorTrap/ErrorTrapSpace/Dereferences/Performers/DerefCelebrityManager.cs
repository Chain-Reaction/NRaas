using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCelebrityManager : Dereference<CelebrityManager>
    {
        protected override DereferenceResult Perform(CelebrityManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDisgracefulActionQueue", field, objects))
            {
                Remove(reference.mDisgracefulActionQueue, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
