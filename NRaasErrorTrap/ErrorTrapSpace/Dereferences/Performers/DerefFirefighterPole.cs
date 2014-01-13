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
    public class DerefFirefighterPole : Dereference<FirefighterPole>
    {
        protected override DereferenceResult Perform(FirefighterPole reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFloorCutoutRim", field, objects))
            {
                Remove(ref reference.mFloorCutoutRim);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
