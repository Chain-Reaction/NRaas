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
    public class DerefSkatingRinkSkate : Dereference<SkatingRink.Skate>
    {
        protected override DereferenceResult Perform(SkatingRink.Skate reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mCouplesSpinJig", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mCouplesSpinJig);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSingleSpinJig", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mSingleSpinJig);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
