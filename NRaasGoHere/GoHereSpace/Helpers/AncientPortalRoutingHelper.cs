using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
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
    public class AncientPortalRoutingHelper : Common.IPreLoad, Common.IWorldLoadFinished, Common.IExitBuildBuy
    {
        public void OnPreLoad()
        {
            Route.PostPlanCallback -= SimRoutingComponent.PostPlanRouteCallback;
            Route.PostPlanCallback += PostPlanRouteCallback;
        }

        public void OnWorldLoadFinished()
        {
            foreach (AncientPortal portal in Sims3.Gameplay.Queries.GetObjects<AncientPortal>())
            {
                if (portal.PortalComponent == null)
                {
                    ObjectComponents.AddComponent<AncientPortalComponent>(portal, new object[0]);
                }
            }
        }

        public void OnExitBuildBuy(Lot lot)
        {
            foreach (AncientPortal portal in lot.GetObjects<AncientPortal>())
            {
                if (portal.PortalComponent == null)
                {
                    ObjectComponents.AddComponent<AncientPortalComponent>(portal, new object[0]);
                }
            }
        }

        public static void PostPlanRouteCallback(Route r, string routeType, string result)
        {
            try
            {
                if ((r.Follower != null) && (r.Follower.Target is Sim))
                {
                    if (((r.GetOption(Route.RouteOption.EnableSubwayPlanning)) || (r.GetOption2(Route.RouteOption2.EnableHoverTrainPlanning))) && !r.GetOption(Route.RouteOption.EnableUFOPlanning))
                    {
                        CheckAndUpdateRouteForPortals(r);
                    }

                    if (((routeType == "Replan") || (routeType == "ReplanFromPoint")) && r.GetOption(Route.RouteOption.PlanUsingStroller))
                    {
                        r.SetOption(Route.RouteOption.ReplanUsingStroller, false);
                    }

                    if (RoutingComponent.sPostPlanProfileCallback != null)
                    {
                        RoutingComponent.sPostPlanProfileCallback(r, routeType, result);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("PostPlanRouteCallback", e);
            }
        }

        private static void Reroute(Route r, IStation closest, IStation destination)
        {
            Sim target = r.Follower.Target as Sim;
            Vector3 currentStartPoint = r.GetCurrentStartPoint();
            float distanceRemaining = r.GetDistanceRemaining();

            Route route = target.CreateRoute();
            route.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
            route.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, false);
            route.SetOption(Route.RouteOption.EnablePlanningAsCar, r.GetOption(Route.RouteOption.EnablePlanningAsCar));
            route.SetOption(Route.RouteOption.PlanUsingStroller, r.GetOption(Route.RouteOption.PlanUsingStroller));
            route.SetOption(Route.RouteOption.ReplanUsingStroller, r.GetOption(Route.RouteOption.ReplanUsingStroller));
            route.SetOption(Route.RouteOption.BeginAsStroller, r.GetOption(Route.RouteOption.BeginAsStroller));
            Slot routeEnterEndSlot = closest.RouteEnterEndSlot;
            if (routeEnterEndSlot != Slot.None)
            {
                GameObject routingSlotEnterFootprint = closest.RoutingSlotEnterFootprint;
                if (routingSlotEnterFootprint != null)
                {
                    route.AddObjectToIgnoreForRoute(routingSlotEnterFootprint.ObjectId);
                }
                if (route.PlanToSlot(closest, routeEnterEndSlot).Succeeded())
                {
                    Slot routeExitBeginSlot = destination.RouteExitBeginSlot;
                    Vector3 slotPosition = destination.GetSlotPosition(routeExitBeginSlot);
                    GameObject routingSlotExitFootprint = destination.RoutingSlotExitFootprint;
                    if (routingSlotExitFootprint != null)
                    {
                        r.AddObjectToIgnoreForRoute(routingSlotExitFootprint.ObjectId);
                    }
                    r.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                    r.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, false);
                    if (!r.ReplanFromPoint(slotPosition).Succeeded())
                    {
                        r.ReplanFromPoint(currentStartPoint);
                    }
                    else if ((route.GetDistanceRemaining() + r.GetDistanceRemaining()) < (distanceRemaining + SimRoutingComponent.kDistanceMustSaveInOrderToUseSubway))
                    {
                        if (closest is IHoverTrainStation)
                        {
                            r.ReplanAllowed = false;
                            Route route2 = target.CreateRoute();
                            PathType elevatedTrainPath = PathType.ElevatedTrainPath;
                            List<Vector3> list = new List<Vector3>();
                            list.Add(closest.GetSlotPosition(closest.RouteEnterEndSlot));
                            list.Add(destination.GetSlotPosition(destination.RouteExitBeginSlot));
                            if (list.Count > 0)
                            {
                                route2.InsertCustomPathAtIndex(0, list.ToArray(), false, true, elevatedTrainPath);
                                route2.ReplanAllowed = false;
                                RoutePlanResult planResult = route2.PlanResult;
                                planResult.mType = RoutePlanResultType.Succeeded;
                                route2.PlanResult = planResult;
                                PathData pathData = route2.GetPathData(0);
                                pathData.ObjectId = destination.ObjectId;
                                pathData.PathType = PathType.ElevatedTrainPath;
                                route2.SetPathData(ref pathData);
                                r.InsertRouteSubPathsAtIndex(0, route2);
                            }
                        }

                        r.InsertRouteSubPathsAtIndex(0x0, route);
                        r.SetOption(Route.RouteOption.EnableSubwayPlanning, true);
                        r.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, true);
                    }
                    else
                    {
                        r.ReplanFromPoint(currentStartPoint);
                    }
                }
            }
        }

        private static void Reroute(Route r, AncientPortal closest, AncientPortal destination)
        {
            Sim target = r.Follower.Target as Sim;
            Vector3 currentStartPoint = r.GetCurrentStartPoint();
            float distanceRemaining = r.GetDistanceRemaining();

            Common.StringBuilder msg = new Common.StringBuilder();

            msg.Append("AncientPortal Reroute: " + target.FullName);

            Route routeToPortal = target.CreateRoute();
            routeToPortal.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
            routeToPortal.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, false);
            routeToPortal.SetOption(Route.RouteOption.EnablePlanningAsCar, r.GetOption(Route.RouteOption.EnablePlanningAsCar));
            routeToPortal.SetOption(Route.RouteOption.PlanUsingStroller, r.GetOption(Route.RouteOption.PlanUsingStroller));
            routeToPortal.SetOption(Route.RouteOption.ReplanUsingStroller, r.GetOption(Route.RouteOption.ReplanUsingStroller));
            routeToPortal.SetOption(Route.RouteOption.BeginAsStroller, r.GetOption(Route.RouteOption.BeginAsStroller));

            Vector3 slotPosition = closest.GetSlotPosition(closest.GetRoutingSlots()[0]);
            Vector3 slotFoward = closest.GetForwardOfSlot(closest.GetRoutingSlots()[0]);

            Vector3 farPosition = new Vector3(slotPosition);
            farPosition.x -= slotFoward.x / 4f;
            farPosition.y -= slotFoward.y / 4f;

            RoutePlanResult result = routeToPortal.PlanToPoint(farPosition);

            msg.Append(Common.NewLine + "Result: " + result);

            if (result.Succeeded())
            {
                msg.Append(Common.NewLine + "D" + Common.NewLine + Routes.RouteToString(routeToPortal));

                Route portalRoute = target.CreateRoute();
                portalRoute.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                portalRoute.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, false);
                portalRoute.SetOption(Route.RouteOption.EnablePlanningAsCar, r.GetOption(Route.RouteOption.EnablePlanningAsCar));
                portalRoute.SetOption(Route.RouteOption.PlanUsingStroller, r.GetOption(Route.RouteOption.PlanUsingStroller));
                portalRoute.SetOption(Route.RouteOption.ReplanUsingStroller, r.GetOption(Route.RouteOption.ReplanUsingStroller));
                portalRoute.SetOption(Route.RouteOption.BeginAsStroller, r.GetOption(Route.RouteOption.BeginAsStroller));

                result = portalRoute.PlanToPointFromPoint(slotPosition, farPosition);

                msg.Append(Common.NewLine + "Result: " + result);

                if (result.Succeeded())
                {
                    PathData portalData = new PathData();
                    portalData.PathType = PathType.PortalPath;
                    portalData.ObjectId = closest.ObjectId;
                    portalData.PortalStartPos = slotPosition;

                    portalRoute.SetPathData(ref portalData);

                    msg.Append(Common.NewLine + "A" + Common.NewLine + Routes.RouteToString(portalRoute));

                    slotPosition = destination.GetSlotPosition(destination.GetRoutingSlots()[0]);

                    r.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                    if (!r.ReplanFromPoint(slotPosition).Succeeded())
                    {
                        r.ReplanFromPoint(currentStartPoint);
                    }
                    else if ((routeToPortal.GetDistanceRemaining() + r.GetDistanceRemaining()) < (distanceRemaining + SimRoutingComponent.kDistanceMustSaveInOrderToUseSubway))
                    {
                        AncientPortalComponent.AddTargetPortal(target, destination);

                        msg.Append(Common.NewLine + "B" + Common.NewLine + Routes.RouteToString(r));

                        r.InsertRouteSubPathsAtIndex(0x0, portalRoute);
                        r.InsertRouteSubPathsAtIndex(0x0, routeToPortal);

                        msg.Append(Common.NewLine + "C" + Common.NewLine + Routes.RouteToString(r));

                        r.SetOption(Route.RouteOption.EnableSubwayPlanning, true);
                    }
                    else
                    {
                        r.ReplanFromPoint(currentStartPoint);
                    }
                }
            }

            Common.DebugNotify(msg, target);
            Common.DebugWriteLog(msg);
        }

        private static void CheckAndUpdateRouteForPortals(Route r)
        {
            if (r.PlanResult.Succeeded())
            {
                Sim target = r.Follower.Target as Sim;
                if (target != null)
                {
                    // Subways and Ancient portals do not work for ghosts
                    if (target.SimDescription.DeathStyle != SimDescription.DeathType.None) return;

                    bool usingHoverTrain = target.CurrentInteraction is IHoverTrainStationInteraction;
                    if (!usingHoverTrain)
                    {
                        if (Routes.Contains(r, PathType.VehiclePath))
                        {
                            Vehicle ownedAndUsableVehicle = SimEx.GetOwnedAndUsableVehicle(target, target.LotCurrent);
                            if (ownedAndUsableVehicle != null)
                            {
                                if (ownedAndUsableVehicle.DestroyOnRelease)
                                {
                                    ownedAndUsableVehicle.Destroy();
                                    ownedAndUsableVehicle = null;
                                }

                                return;
                            }
                        }
                    }

                    if (target.IsSelectable || RandomUtil.RandomChance01(SimRoutingComponent.kNPCSubwayUseChance) || usingHoverTrain)
                    {
                        bool found = false;

                        for (uint i = 0; i < r.GetNumPaths(); i++)
                        {
                            PathData data = r.GetPathData(i);
                            if (data.PathType != PathType.PortalPath) continue;

                            AncientPortal portal = GameObject.GetObject<AncientPortal>(data.ObjectId);
                            if (portal != null)
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                IStation station = GameObject.GetObject<IStation>(data.ObjectId);
                                if (station != null)
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (found) return;

                        LotLocation startLotLocation = LotLocation.Invalid;
                        LotLocation endLotLocation = LotLocation.Invalid;
                        Vector3 currentStartPoint = r.GetCurrentStartPoint();
                        Vector3 destPoint = r.GetDestPoint();
                        ulong startLot = World.GetLotLocation(currentStartPoint, ref startLotLocation);
                        ulong endLot = World.GetLotLocation(destPoint, ref endLotLocation);
                        if (((startLot != ulong.MaxValue) && (endLot != ulong.MaxValue)) && ((startLot != endLot) || (startLot == 0x0L)))
                        {
                            AncientPortal closestAP = null;
                            float closestDistAP = float.MaxValue;
                            AncientPortal destinationAP = null;
                            float destDistanceAP = float.MaxValue;
                            if (target.IsHuman)
                            {
                                foreach (AncientPortal choice in Sims3.Gameplay.Queries.GetObjects<AncientPortal>())
                                {
                                    Vector3 vector3 = choice.Position - currentStartPoint;
                                    if (vector3.Length() < closestDistAP)
                                    {
                                        closestDistAP = vector3.Length();
                                        closestAP = choice;
                                    }

                                    Vector3 vector4 = choice.Position - destPoint;
                                    if (vector4.Length() < destDistanceAP)
                                    {
                                        destDistanceAP = vector4.Length();
                                        destinationAP = choice;
                                    }
                                }
                            }

                            IStation closestSW = null;
                            float closestDistSW = float.MaxValue;
                            IStation destinationSW = null;
                            float destDistanceSW = float.MaxValue;
                            foreach (IStation subway3 in Sims3.Gameplay.Queries.GetObjects<IStation>())
                            {
                                Vector3 vector3 = subway3.Position - currentStartPoint;
                                if (vector3.Length() < closestDistSW)
                                {
                                    closestDistSW = vector3.Length();
                                    closestSW = subway3;
                                }

                                Vector3 vector4 = subway3.Position - destPoint;
                                if (vector4.Length() < destDistanceSW)
                                {
                                    destDistanceSW = vector4.Length();
                                    destinationSW = subway3;
                                }
                            }

                            float distanceRemaining = r.GetDistanceRemaining();

                            float lengthAP = float.MaxValue;
                            if (((closestAP != null) && (destinationAP != null)) && ((closestAP != destinationAP) && ((closestDistAP + destDistanceAP) <= (distanceRemaining + SimRoutingComponent.kDistanceMustSaveInOrderToUseSubway))))
                            {
                                lengthAP = closestDistAP + destDistanceAP;
                            }

                            float lengthSW = float.MaxValue;
                            if (((closestSW != null) && (destinationSW != null)) && ((closestSW != destinationSW) && ((closestDistSW + destDistanceSW) <= (distanceRemaining + SimRoutingComponent.kDistanceMustSaveInOrderToUseSubway))))
                            {
                                lengthSW = closestDistSW + destDistanceSW;
                            }

                            if (lengthAP < lengthSW)
                            {
                                if (lengthAP != float.MaxValue)
                                {
                                    Reroute(r, closestAP, destinationAP);
                                }
                            }
                            else
                            {
                                if (lengthSW != float.MaxValue)
                                {
                                    Reroute(r, closestSW, destinationSW);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
