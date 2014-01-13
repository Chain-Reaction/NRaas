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
    public class DerefBuffInstanceScared : Dereference<BuffScared.BuffInstanceScared>
    {
        protected override DereferenceResult Perform(BuffScared.BuffInstanceScared reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "ScaryObject", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.ScaryObject);
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
