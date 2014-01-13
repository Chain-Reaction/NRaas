using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMapTag : Dereference<MapTag>
    {
        protected override DereferenceResult Perform(MapTag reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTargetObject", field, objects))
            {
                Remove(ref reference.mTargetObject);

                if (Performing)
                {
                    return DereferenceResult.ContinueIfReferenced;
                }
                else
                {
                    return DereferenceResult.Ignore;
                }
            }

            if (Matches(reference, "mOwner", field, objects))
            {
                Remove(ref reference.mOwner );
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
