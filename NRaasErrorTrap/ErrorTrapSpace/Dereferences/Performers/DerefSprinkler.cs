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
    public class DerefSprinkler : Dereference<Sprinkler>
    {
        protected override DereferenceResult Perform(Sprinkler reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "SimsUsingSlots", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.SimsUsingSlots, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mDynamicFootprint", field, objects))
            {
                Remove(ref reference.mDynamicFootprint);

                if (Performing)
                {
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
