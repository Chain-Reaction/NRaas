using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefAncientPortal : Dereference<AncientPortal>
    {
        protected override DereferenceResult Perform(AncientPortal reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRoutingSims", field, objects))
            {
                Remove(reference.mRoutingSims, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
