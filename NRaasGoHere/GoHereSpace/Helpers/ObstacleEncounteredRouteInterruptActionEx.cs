using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class ObstacleEncounteredRouteInterruptActionEx
    {
        public static RouteAction.ActionResult PerformAction(ObstacleEncounteredRouteInterruptAction ths)
        {
            try
            {
                ths.mRoutingSim.SimRoutingComponent.EnableDynamicFootprint();
                ths.mRoutingSim.SimRoutingComponent.StopPushImmunity();
                if ((ths.mRoutingSim.SimRoutingComponent.UsingStroller && (ths.mRoutingSim.CarryingChildPosture != null)) && (ths.mRoutingSim.CarryingChildPosture.Stroller != null))
                {
                    ths.mRoutingSim.CarryingChildPosture.Stroller.SetPosition(ths.mRoutingSim.CarryingChildPosture.Stroller.Position);
                }

                bool option = ths.mInterruptedPath.GetOption(Route.RouteOption.BlockedByPeople);
                ths.mInterruptedPath.SetOption(Route.RouteOption.BlockedByPeople, true);
                if (ths.mbForceInitialReplan)
                {
                    RoutePlanResult result = ths.mInterruptedPath.Replan();
                    if (!result.Succeeded())
                    {
                        GameObject obj2 = new ObjectGuid(result.mBlockerObjectId).ObjectFromId<GameObject>();
                        ths.mRoutingSim.SimRoutingComponent.PlayRouteFailureIfAppropriate(obj2);
                        return RouteAction.ActionResult.Terminate;
                    }
                }

                /* This is duplicated in StandAndWaitController, I believe it is an EA error since there is no "Exit"
                StateMachineClient client = StateMachineClient.Acquire(ths.mRoutingSim, "StandAndWait");
                client.SetActor("x", ths.mRoutingSim);
                client.SetParameter("isMermaid", OccultMermaid.IsMatureMermaidAndWearingTail(base.mRoutingSim));
                client.EnterState("x", "Enter");
                */

                SpeedTrap.Sleep();
                ths.mnCheckFrequency = 0x1;
                ths.mnCheckOffset = 0x0;
                try
                {
                    StandAndWaitController controller = new StandAndWaitController();
                    controller.AllowZeroCycle = ths.mbAllowZeroCycle;
                    controller.Duration = SimRoutingComponent.DefaultStandAndWaitDuration;
                    controller.OnCycle = ths.StandAndWaitCycleHandler;
                    controller.Run(ths.mRoutingSim);
                }
                catch (SacsErrorException e)
                {
                    Common.DebugException("ObstacleEncounteredRouteInterruptActionEx:PerformAction", e);
                }

                ths.mRoutingSim.SimRoutingComponent.DisableDynamicFootprint();
                if (!ths.mbElectedToExit)
                {
                    ths.mReturnValue = RouteAction.ActionResult.Terminate;
                }

                if (ths.mReturnValue == RouteAction.ActionResult.Terminate)
                {
                    int numObstacles = 0x0;
                    int numPushableObstacles = 0x0;
                    int numWorldObstacles = 0x0;
                    int numStaticObstacles = 0x0;
                    int numReplannableObstacles = 0x0;
                    ObjectGuid[] guidArray = ths.CollectObstacles(ths.mRoutingSim, ref numObstacles, ref numPushableObstacles, ref numReplannableObstacles, ref numStaticObstacles, ref numWorldObstacles);
                    int num6 = numObstacles - numWorldObstacles;
                    if (numObstacles > 0x0)
                    {
                        if ((ths.mRoutingSim.LotCurrent != null) && !ths.mRoutingSim.LotCurrent.IsWorldLot)
                        {
                            if ((num6 == 0x0) && (numWorldObstacles > 0x0))
                            {
                                ths.mRoutingSim.SimRoutingComponent.StartIgnoringObstacles();
                                return RouteAction.ActionResult.ContinueAndFollowPath;
                            }
                            else if ((num6 >= 0x3) && (Sims3.Gameplay.Queries.CountObjects<Sim>(ths.mRoutingSim.Position, 1f) >= 0x4))
                            {
                                ths.mRoutingSim.SimRoutingComponent.StartIgnoringObstacles();
                                return RouteAction.ActionResult.ContinueAndFollowPath;
                            }
                        }
                        else if (numWorldObstacles > 0x0)
                        {
                            ths.mRoutingSim.SimRoutingComponent.StartIgnoringObstacles();
                            return RouteAction.ActionResult.ContinueAndFollowPath;
                        }
                    }
                    else
                    {
                        ths.mRoutingSim.SimRoutingComponent.StartPushImmunity(SimRoutingComponent.OnRouteStartedImmuneToPushesDuration);
                    }

                    if ((!ths.mRoutingSim.HasExitReason(ExitReason.CancelExternal | ExitReason.MidRoutePushRequested) && ths.mInterruptedPath.DoRouteFail) && !ths.mInterruptedPath.PlanResult.Succeeded())
                    {
                        foreach (ObjectGuid guid in guidArray)
                        {
                            GameObject obj3 = guid.ObjectFromId<GameObject>();
                            if (ths.mRoutingSim.SimRoutingComponent.PlayRouteFailureIfAppropriate(obj3))
                            {
                                break;
                            }
                        }
                    }
                }

                ths.mInterruptedPath.SetOption(Route.RouteOption.BlockedByPeople, option);
                return ths.mReturnValue;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception("ObstacleEncounteredRouteInterruptActionEx:PerformAction", e);
                throw;
            }
        }
    }
}
