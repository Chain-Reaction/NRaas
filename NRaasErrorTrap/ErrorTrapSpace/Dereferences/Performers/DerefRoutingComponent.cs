using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRoutingComponent : Dereference<RoutingComponent>
    {
        protected override DereferenceResult Perform(RoutingComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            /*
            if (Matches(reference, "mRouteActions", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }

                    Remove(reference.mRouteActions, objects);
                }
                return DereferenceResult.End;
            }
            */

            ReferenceWrapper result;
            if (Matches(reference, "OnRouteActionsFinished", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        RoutingComponent.RouteActionsFinishedDelegate callback = FindLast<RoutingComponent.RouteActionsFinishedDelegate>(objects);
                        if (callback != null)
                        {
                            reference.OnRouteActionsFinished -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "OnRoutingObstaclesEncounteredEvent", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.OnReset();
                        }
                        catch
                        { }

                        RoutingComponent.RouteEventObstaclesEncounteredCallback callback = FindLast<RoutingComponent.RouteEventObstaclesEncounteredCallback>(objects);
                        if (callback != null)
                        {
                            reference.OnRoutingObstaclesEncounteredEvent -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "OnRoutingEvent", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }

                    RoutingComponent.RouteEventCallback callback = FindLast<RoutingComponent.RouteEventCallback>(objects);
                    if (callback != null)
                    {
                        reference.OnRoutingEvent -= callback;
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPreviousRouteAction", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.OnReset();
                    }
                    catch
                    { }

                    Remove(ref reference.mPreviousRouteAction);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
