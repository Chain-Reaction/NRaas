using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefObjectOwnerComponent : Dereference<ObjectOwnerComponent>
    {
        protected override DereferenceResult Perform(ObjectOwnerComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwner", field, objects))
            {
                Remove(ref reference.mOwner );
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mThief", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mThief);
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
