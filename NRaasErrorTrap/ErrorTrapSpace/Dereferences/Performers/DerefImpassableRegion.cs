using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefImpassableRegion : Dereference<ImpassableRegion>
    {
        protected override DereferenceResult Perform(ImpassableRegion reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mMemberObjects", field, objects))
            {
                Remove(reference.mMemberObjects, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
