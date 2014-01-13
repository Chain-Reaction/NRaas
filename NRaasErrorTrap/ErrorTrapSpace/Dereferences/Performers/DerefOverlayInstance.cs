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
    public class DerefOverlayInstance : Dereference<OverlayInstance>
    {
        protected override DereferenceResult Perform(OverlayInstance reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult result = DereferenceResult.Failure;

            if (Matches(reference, "mOverlay", field, objects))
            {
                Remove(ref reference.mOverlay);
                result = DereferenceResult.Continue;
            }

            if (Matches(reference, "Actor", field, objects))
            {
                Remove(ref reference.Actor );
                result = DereferenceResult.Continue;
            }

            return result;
        }
    }
}
