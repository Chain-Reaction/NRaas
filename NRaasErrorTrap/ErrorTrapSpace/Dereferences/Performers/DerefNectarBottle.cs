using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefNectarBottle : Dereference<NectarBottle>
    {
        protected override DereferenceResult Perform(NectarBottle reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mBottleInfo", field, objects, out result) == MatchResult.CorrectField)
            {
                Remove(ref reference.mBottleInfo.mCreator);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
