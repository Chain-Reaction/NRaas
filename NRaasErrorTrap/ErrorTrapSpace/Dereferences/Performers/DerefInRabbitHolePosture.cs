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
    public class DerefInRabbitHolePosture : Dereference<InRabbitHolePosture>
    {
        protected override DereferenceResult Perform(InRabbitHolePosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mRabbitHole", field, objects))
            {
                Remove(ref reference.mRabbitHole );
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
