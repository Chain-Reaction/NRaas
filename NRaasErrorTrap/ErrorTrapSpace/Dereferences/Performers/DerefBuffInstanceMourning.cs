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
    public class DerefBuffInstanceMourning : Dereference<BuffMourning.BuffInstanceMourning>
    {
        protected override DereferenceResult Perform(BuffMourning.BuffInstanceMourning reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "MissedSim", field, objects))
            {
                Remove(ref reference.MissedSim);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
