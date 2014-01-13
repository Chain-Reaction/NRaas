using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOverlayComponent : Dereference<OverlayComponent>
    {
        protected override DereferenceResult Perform(OverlayComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mActiveOverlayInstances", field, objects))
            {
                Remove(reference.mActiveOverlayInstances, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mOwner", field, objects))
            {
                Remove(ref reference.mOwner);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
