using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBonFire : Dereference<BonFire>
    {
        protected override DereferenceResult Perform(BonFire reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mHerbToAdd", field, objects))
            {
                Remove(ref reference.mHerbToAdd);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
