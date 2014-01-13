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
    public class DerefBuffInstancePestilencePlague : Dereference<BuffPestilencePlague.BuffInstancePestilencePlague>
    {
        protected override DereferenceResult Perform(BuffPestilencePlague.BuffInstancePestilencePlague reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPlaguedSim", field, objects))
            {
                Remove(ref reference.mPlaguedSim);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
