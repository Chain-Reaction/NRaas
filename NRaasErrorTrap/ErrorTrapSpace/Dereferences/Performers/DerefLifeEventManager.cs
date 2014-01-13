using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefLifeEventManager : Dereference<LifeEventManager>
    {
        protected override DereferenceResult Perform(LifeEventManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwnerDescription", field, objects))
            {
                //Remove(ref reference.mOwnerDescription );
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mActiveNodes", field, objects))
            {
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
