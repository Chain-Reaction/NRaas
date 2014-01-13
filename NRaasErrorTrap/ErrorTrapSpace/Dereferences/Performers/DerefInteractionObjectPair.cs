using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefInteractionObjectPair : Dereference<InteractionObjectPair>
    {
        protected override DereferenceResult Perform(InteractionObjectPair reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInteraction", field, objects))
            {
                //Remove(ref reference.mInteraction );
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mTarget", field, objects))
            {
                Remove(ref reference.mTarget );
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
