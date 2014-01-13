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
    public class DerefBonehildaCoffin : Dereference<BonehildaCoffin>
    {
        protected override DereferenceResult Perform(BonehildaCoffin reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBonehildaSim", field, objects))
            {
                Remove(ref reference.mBonehildaSim);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mBonehilda", field, objects))
            {
                Remove(ref reference.mBonehilda);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
