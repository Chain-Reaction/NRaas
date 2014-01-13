using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlayCatchObjectPlayCatchInteraction : Dereference<PlayCatchObject.PlayCatchInteraction>
    {
        protected override DereferenceResult Perform(PlayCatchObject.PlayCatchInteraction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mJig", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    //Remove(ref reference.mJig );
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
