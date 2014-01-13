using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOccultGenie : Dereference<OccultGenie>
    {
        protected override DereferenceResult Perform(OccultGenie reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mLamp", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mLamp );
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSummoner", field, objects))
            {
                Remove(ref reference.mSummoner);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
