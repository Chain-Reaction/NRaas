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
    public class DerefBuffInstanceMyLove : Dereference<BuffMyLove.BuffInstanceMyLove>
    {
        protected override DereferenceResult Perform(BuffMyLove.BuffInstanceMyLove reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Lover", field, objects))
            {
                Remove(ref reference.Lover);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
