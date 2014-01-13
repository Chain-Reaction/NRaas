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
    public class DerefTraitFunctionsStolenObject : Dereference<TraitFunctions.StolenObject>
    {
        protected override DereferenceResult Perform(TraitFunctions.StolenObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mChild", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mChild);
                    return DereferenceResult.ContinueIfReferenced;
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
