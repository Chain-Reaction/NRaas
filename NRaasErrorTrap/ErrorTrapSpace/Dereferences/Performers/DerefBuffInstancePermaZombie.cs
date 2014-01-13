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
    public class DerefBuffInstancePermaZombie : Dereference<BuffPermaZombie.BuffInstancePermaZombie>
    {
        protected override DereferenceResult Perform(BuffPermaZombie.BuffInstancePermaZombie reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult reason = MatchAndRemove(reference, "mListener", field, ref reference.mListener, objects, DereferenceResult.Continue);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
