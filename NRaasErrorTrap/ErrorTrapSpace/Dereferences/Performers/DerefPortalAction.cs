using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPortalAction : Dereference<PortalAction>
    {
        protected override DereferenceResult Perform(PortalAction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPortalObject", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }
                }
                Remove(ref reference.mPortalObject);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mRoutingSim", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }
                }

                Remove(ref reference.mRoutingSim);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
