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
    public class DerefFollowRouteAction : Dereference<FollowRouteAction>
    {
        protected override DereferenceResult Perform(FollowRouteAction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSteeringAdaptor", field, objects))
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

                //Remove(ref reference.mSteeringAdaptor);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mScriptAdaptor", field, objects))
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

                //Remove(ref reference.mScriptAdaptor);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
