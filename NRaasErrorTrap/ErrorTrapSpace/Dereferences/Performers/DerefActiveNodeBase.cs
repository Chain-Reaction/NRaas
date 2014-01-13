using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefActiveNodeBase : Dereference<ActiveNodeBase>
    {
        protected override DereferenceResult Perform(ActiveNodeBase reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mOwner", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    //Remove(ref reference.mOwner);
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
