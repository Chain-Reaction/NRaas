using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBuffInstanceFrozenSolid : Dereference<BuffFrozenSolid.BuffInstancFrozenSolid>
    {
        protected override DereferenceResult Perform(BuffFrozenSolid.BuffInstancFrozenSolid reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mLODModelSim", field, objects))
            {
                Remove(ref reference.mLODModelSim);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
