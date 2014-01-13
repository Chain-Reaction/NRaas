using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefObjectWithOrientation : Dereference<ObjectWithOrientation>
    {
        protected override DereferenceResult Perform(ObjectWithOrientation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mGameObject", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mGameObject);
                    return DereferenceResult.ContinueIfReferenced;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
