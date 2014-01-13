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
    public class DerefObjectComponent : Dereference<ObjectComponent>
    {
        protected override DereferenceResult Perform(ObjectComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mScriptObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    //Remove(ref reference.mScriptObject );
                    return DereferenceResult.ContinueIfReferenced;
                }
                else
                {
                    return DereferenceResult.Ignore;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
