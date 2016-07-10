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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Seasons;
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
    public class RoutingComponentEx
    {
        public static void LogFailure(GameObject routingObject, RouteAction.FailureExplanation explanation, RoutePlanResult planResult, string errorText)
        {
            RoutingComponent.mnNumDoRouteFailures += (ulong)0x1L;
            if (RoutingComponent.mbLogFailures)
            {
                Common.StringBuilder msg = new Common.StringBuilder("Explanation: " + explanation);
                msg += Common.NewLine + "Result: " + planResult;
                msg += Common.NewLine + "Error: " + errorText;

                if (routingObject.LotCurrent != null)
                {
                    msg += Common.NewLine + "Address A: " + routingObject.LotCurrent.Address;
                }

                Sim sim = routingObject as Sim;
                if (sim != null)
                {
                    InteractionInstance currentInteraction = sim.CurrentInteraction;
                    InteractionInstance nextInteraction = sim.InteractionQueue.GetNextInteraction();
                    if (((currentInteraction != null) && (currentInteraction.Target != null)) && (currentInteraction.Target.LotCurrent != null))
                    {
                        msg += Common.NewLine + "Address B: " + currentInteraction.Target.LotCurrent.Address;
                    }

                    if (((nextInteraction != null) && (nextInteraction.Target != null)) && (nextInteraction.Target.LotCurrent != null))
                    {
                        msg += Common.NewLine + "Address C: " + nextInteraction.Target.LotCurrent.Address;
                    }
                }

                Common.DebugNotify(msg, sim);
            }
        }

        public static bool DoRoute(RoutingComponent ths, Route r)
        {
            Sim owner = ths.Owner as Sim;
            if ((!r.ExecutionFromNonSimTaskIsSafe && (owner != null)) && (Simulator.CurrentTask != ths.Owner.ObjectId))
            {
                Simulator.CurrentTask.ObjectFromId<GameObject>();
                return false;
            }

            IBoat boat = ths.Owner as IBoat;
            if (ths.RoutingParent != null)
            {
                if (!(ths.RoutingParent is IBoat) || (r.GetOption2(Route.RouteOption2.EndAsBoat) && (!r.PlanResult.Succeeded() || !RoutingComponent.PlannedRouteHasNonBoatSubpath(r))))
                {
                    // Custom
                    if (ths.RoutingParent.RoutingComponent is CarRoutingComponent)
                    {
                        return CarRoutingComponentEx.DoRoute(ths.RoutingParent.RoutingComponent as CarRoutingComponent, r);
                    }
                    else
                    {
                        return ths.RoutingParent.RoutingComponent.DoRoute(r);
                    }
                }
            }
            else if (((boat != null) && r.PlanResult.Succeeded()) && (!r.GetOption2(Route.RouteOption2.EndAsBoat) || RoutingComponent.PlannedRouteHasNonBoatSubpath(r)))
            {
                Sim driver = boat.GetDriver();
                if (driver != null)
                {
                    return driver.RoutingComponent.DoRoute(r);
                }
            }

            Common.StringBuilder msg = new Common.StringBuilder("RoutingComponentEx:DoRoute" + Common.NewLine);
            if (owner != null)
            {
                msg += Common.NewLine + owner.FullName + Common.NewLine;
            }

            ths.mRouteActions.Clear();
            bool routeSuccess = true;

            if (!GoHere.Settings.mAllowBoatRouting)
            {
                r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, false);
                r.SetOption2(Route.RouteOption2.BeginAsBoat, false);
                r.SetOption(Route.RouteOption.EnableWaterPlanning, true);
                r.SetOption(Route.RouteOption.DoNotEmitDegenerateRoutesForRadialRangeGoals, true);
                r.SetOption(Route.RouteOption.DisallowGoalsOnBridges, true);
                r.Replan();
            }

            try
            {
                RouteAction.FailureExplanation explanation = RouteAction.FailureExplanation.None;
                RoutingComponent.mnNumDoRouteCalls += (ulong)0x1L;
                if (r.PlanResult.Succeeded())
                {
                    ths.OnAboutToStartFollowingRoute(r);
                    if (r.PlanResult.Succeeded())
                    {
                        ths.PushActionsForRoute(r, r);
                        if (owner != null)
                        {
                            owner.PushBackpackPostureIfNecessary();
                            owner.PushCanePostureIfNecessary();
                            owner.PushJetpackPostureIfNecessary();
                        }
                    }
                    else
                    {
                        routeSuccess = false;
                    }
                }
                else
                {
                    routeSuccess = false;
                }

                msg.Append("A");

                if (!routeSuccess)
                {
                    explanation |= RouteAction.FailureExplanation.InitialPlanFailure;
                    LogFailure(ths.Owner, RouteAction.FailureExplanation.InitialPlanFailure, r.PlanResult, "Initial Path Plan Failure (WILL NOT play route fail)");
                    r.SetRouteActionResult(0x2);
                    return false;
                }

                msg.Append("B");

                int failures = 0;

                if ((ths.RoutingParent != null) && (ths.RoutingParent is IBoat))
                {
                    ths.UpdateDynamicFootprint(ths.RoutingParent as GameObject, false);
                }
                else
                {
                    ths.UpdateDynamicFootprint(ths.Owner, false);
                }

                Vector3 originalPosition = ths.Owner.Position;
                int loops = 0;

                bool success = true;
                Route processedPath = r;
                bool isOutside = ths.Owner.IsOutside;
                ths.mNumWaterEntriesThisRoute = 0;
                while (ths.mRouteActions.Count != 0x0)
                {
                    // Custom
                    if (ths.Owner.Position == originalPosition)
                    {
                        loops++;
                    }

                    if ((loops > 30) || (ths.mRouteActions.Count > 30))
                    {
                        success = false;
                        ths.mRouteActions.Clear();
                        break;
                    }

                    if (ths.mRouteActions[0] is GetInBoatRouteAction)
                    {
                        ths.mNumWaterEntriesThisRoute++;
                        if (ths.mNumWaterEntriesThisRoute > 10)
                        {
                            if (owner != null)
                            {
                                owner.SetToResetAndSendHome();
                            }
                            else
                            {
                                ths.Owner.SetObjectToReset();
                            }
                            return false;
                        }
                    }

                    if ((owner != null) && owner.IsOutside)
                    {
                        owner.PushHoverboardPostureIfNecessary();
                    }

                    // Custom
                    RouteAction routeAction = ths.mRouteActions[0x0];

                    msg.Append(Common.NewLine + routeAction.GetType());

                    RouteAction.ActionResult actionResult = RouteAction.ActionResult.Continue;

                    //using (BuffsChangedProxy buffsProxy = new BuffsChangedProxy(owner))
                    {
                        try
                        {
                            if (routeAction is ObstacleEncounteredRouteInterruptAction)
                            {
                                actionResult = ObstacleEncounteredRouteInterruptActionEx.PerformAction(routeAction as ObstacleEncounteredRouteInterruptAction);
                            }
                            else
                            {
                                actionResult = routeAction.PerformAction();
                            }
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (SacsErrorException e)
                        {
                            Common.DebugException(owner, null, msg, e);

                            failures++;

                            if (failures > 10)
                            {
                                actionResult = RouteAction.ActionResult.Terminate;
                            }
                            else
                            {
                                actionResult = RouteAction.ActionResult.ContinueAndPopPathAndReplan;
                            }

                            routeAction.OnReset();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(owner, null, msg.ToString(), e);

                            string stackTrace = e.StackTrace.ToString();

                            if (stackTrace.Contains("OccultManager:DisallowClothesChange"))
                            {
                                OccultTypeHelper.ValidateOccult(owner.SimDescription, null);

                                actionResult = RouteAction.ActionResult.Terminate;
                            }
                            else if (stackTrace.Contains("CommonDoor:IsAllowedThrough"))
                            {
                                Corrections.CleanupCommonDoors(null);
                                actionResult = RouteAction.ActionResult.Terminate;
                            }
                            else if (stackTrace.Contains("SimRoutingComponent:RequestWalkStyle"))
                            {
                                actionResult = RouteAction.ActionResult.Terminate;
                            }
                            else
                            {
                                failures++;

                                if (failures > 10)
                                {
                                    actionResult = RouteAction.ActionResult.Terminate;
                                }
                                else
                                {
                                    actionResult = RouteAction.ActionResult.ContinueAndPopPathAndReplan;
                                }
                            }

                            routeAction.OnReset();
                        }
                    }

                    msg.Append(Common.NewLine + "C");

                    // Custom
                    if ((ths.mRouteActions.Count > 0) && (ths.mRouteActions[0x0].ForciblyReplanned) && (actionResult == RouteAction.ActionResult.ContinueAndPopPath))
                    {
                        actionResult = RouteAction.ActionResult.ContinueAndFollowPath;
                    }

                    msg.Append(Common.NewLine + actionResult + Common.NewLine);

                    if (GoHere.Settings.mDetailedRouting)
                    {
                        if (Sim.ActiveActor == owner)
                        {
                            Common.DebugNotify(msg);
                        }
                    }

                    if (ths.mRouteActions.Count == 0x0)
                    {
                        explanation |= RouteAction.FailureExplanation.InternalPlanFailure;
                        success = false;
                        break;
                    }

                    msg.Append("D");

                    ths.mPreviousRouteAction = ths.mRouteActions[0x0];
                    switch(actionResult)
                    {
                        case RouteAction.ActionResult.Continue:
                        case RouteAction.ActionResult.ContinueAndPopPath:
                        case RouteAction.ActionResult.ContinueAndPopPathAndReplan:
                        case RouteAction.ActionResult.ContinueAndPopPathAndRestart:
                        case RouteAction.ActionResult.ContinueAndFollowPath:
                            msg.Append("E");

                            ths.mRouteActions.RemoveAt(0x0);
                            bool flag3 = (processedPath.GetNumPaths() > 0x1) || (processedPath.IsLoop());
                            if (((actionResult == RouteAction.ActionResult.ContinueAndPopPath) && flag3) || ((actionResult == RouteAction.ActionResult.ContinueAndPopPathAndRestart) || (actionResult == RouteAction.ActionResult.ContinueAndPopPathAndReplan)))
                            {
                                if (flag3)
                                {
                                    processedPath.PopPath();
                                }

                                msg.Append("F");

                                if (actionResult == RouteAction.ActionResult.ContinueAndPopPathAndReplan)
                                {
                                    Vector3 outPosition = new Vector3();
                                    if ((ths.RoutingParent != null) && (ths.RoutingParent is IBoat))
                                    {
                                        outPosition = ths.RoutingParent.PositionOnFloor;
                                    }
                                    else if (ths.Owner != null)
                                    {
                                        if ((owner != null) && (owner.Posture is SwimmingInPool))
                                        {
                                            outPosition = ths.Owner.Position;
                                        }
                                        else
                                        {
                                            outPosition = ths.Owner.PositionOnFloor;
                                        }
                                    }
                                    else
                                    {
                                        processedPath.GetPathStartPoint(0x0, ref outPosition);
                                    }
                                    processedPath.ReplanFromPoint(outPosition);
                                }

                                msg.Append("G");

                                if (actionResult == RouteAction.ActionResult.ContinueAndPopPathAndRestart)
                                {
                                    Vector3 positionOnFloor = new Vector3();
                                    if (ths.Owner != null)
                                    {
                                        positionOnFloor = ths.Owner.PositionOnFloor;
                                    }
                                    else
                                    {
                                        processedPath.GetPathStartPoint(0x0, ref positionOnFloor);
                                    }
                                    processedPath.SetOption(Route.RouteOption.EnablePlanningAsCar, false);
                                    processedPath.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                                    processedPath.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, false);
                                    processedPath.ReplanFromPoint(positionOnFloor);
                                    explanation |= RouteAction.FailureExplanation.CancelledByScript;
                                    success = false;
                                    ths.mRouteActions.Clear();
                                }
                                else if (processedPath.PlanResult.Succeeded())
                                {
                                    ths.PushActionsForRoute(r, processedPath);
                                }
                                else
                                {
                                    explanation |= RouteAction.FailureExplanation.ReplanFailure;
                                    success = false;
                                    ths.mRouteActions.Clear();
                                }
                            }
                            else if ((actionResult == RouteAction.ActionResult.ContinueAndFollowPath) && (r.GetNumPaths() > 0x0))
                            {
                                msg.Append("H");

                                if (r.PlanResult.Succeeded())
                                {
                                    ths.PushActionsForRoute(r, processedPath);
                                }
                                else
                                {
                                    explanation |= RouteAction.FailureExplanation.InternalPlanFailure;
                                    success = false;
                                    ths.mRouteActions.Clear();
                                }
                            }
                            break;
                        default:
                            msg.Append("I");

                            explanation |= ths.mPreviousRouteAction.FailureExplanations;
                            success = false;
                            ths.mRouteActions.Clear();
                            break;
                    }
                }

                msg.Append("J");

                ths.mRouteActions.Clear();
                r.SetRouteActionResult((uint)explanation);
                if (!success)
                {
                    if (ths.mPreviousRouteAction == null)
                    {
                        LogFailure(ths.Owner, explanation, r.PlanResult, "(Null mPreviousRouteAction failure)");
                    }
                    else
                    {
                        LogFailure(ths.Owner, explanation, r.PlanResult, "(failure in " + ths.mPreviousRouteAction.ToString() + ")");
                    }
                }

                msg.Append("K");

                if ((ths.RoutingParent != null) && (ths.RoutingParent is IBoat))
                {
                    ths.UpdateDynamicFootprint(ths.RoutingParent as GameObject, true);
                }
                else
                {
                    ths.UpdateDynamicFootprint(ths.Owner, true);
                }

                routeSuccess = (success && (processedPath == r)) && r.PlanResult.Succeeded();
                ths.mPreviousRouteAction = null;
                if (ths.OnRouteActionsFinished != null)
                {
                    ths.OnRouteActionsFinished(ths.Owner, routeSuccess);
                }

                if (((SeasonsManager.Enabled && routeSuccess) && (isOutside && (owner != null))) && ((owner.IsHuman && owner.SimDescription.ChildOrAbove) && (!owner.IsOutside && (owner.CurrentOutfitCategory == OutfitCategories.Outerwear))))
                {
                    owner.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.RemovingOuterwear);
                }

                if (owner != null)
                {
                    SwimmingInPool posture = owner.Posture as SwimmingInPool;
                    if ((posture != null) && posture.UsedShortEntryAnimation)
                    {
                        posture.UsedShortEntryAnimation = false;
                        if (owner.SimDescription.IsMatureMermaid)
                        {
                            OccultMermaid.PutOnTailWithSpin(owner.SimDescription);
                        }
                    }
                }
            }
            catch (ResetException)
            {
                routeSuccess = false;
                if (ths.mRouteActions.Count > 0x0)
                {
                    RouteAction action = ths.mRouteActions[0x0];
                    if (action != null)
                    {
                        action.OnReset();
                    }
                }
                ths.mRouteActions.Clear();
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(owner, null, msg.ToString(), e);
                routeSuccess = false;

                if (ths.mRouteActions.Count > 0x0)
                {
                    RouteAction action = ths.mRouteActions[0x0];
                    if (action != null)
                    {
                        action.OnReset();
                    }
                }
                ths.mRouteActions.Clear();
            }
            finally
            {
                ths.mRouteActions.Clear();
                if (owner != null)
                {
                    owner.RestoreUsedHoverboard();
                }

                if ((owner != null) && (owner.FirstName == "Roberto"))
                {
                    Common.DebugStackLog(msg);
                }
            }

            return routeSuccess;
        }

        /* Laundry Buff issue resolved in Patch 1.55
        public class BuffsChangedProxy : IDisposable
        {
            BuffManager mBuffManager;

            BuffManager.BuffsChangedCallback mBuffsChanged;

            bool mCalled = false;

            public BuffsChangedProxy(Sim sim)
            {
                if (sim != null)
                {
                    mBuffManager = sim.BuffManager;

                    if (mBuffManager != null)
                    {
                        mBuffsChanged = mBuffManager.BuffsChanged;

                        mBuffManager.BuffsChanged = OnBuffChanged;
                    }
                }
            }

            protected void OnBuffChanged()
            {
                mCalled = true;

                Common.DebugNotify("BuffsChangedProxy:OnBuffChanged", mBuffManager.Actor);
            }

            public void Dispose()
            {
                if (mBuffManager != null)
                {
                    mBuffManager.BuffsChanged += mBuffsChanged;
                    mBuffManager.BuffsChanged -= OnBuffChanged;

                    if ((mCalled) && (mBuffManager.BuffsChanged != null))
                    {
                        mBuffManager.BuffsChanged();
                    }
                }
            }
        }
        */
    }
}
