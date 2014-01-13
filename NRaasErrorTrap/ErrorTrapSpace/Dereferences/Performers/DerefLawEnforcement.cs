using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefLawEnforcement : Dereference<LawEnforcement>
    {
        protected override DereferenceResult Perform(LawEnforcement reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBeeper", field, objects))
            {
                Remove(ref reference.mBeeper);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mPartner", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mBeeper);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
