using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.GoHereSpace.Helpers
{
    public class SimRoutingComponentEx : SimRoutingComponent
    {
        public SimRoutingComponentEx()
        { }
        public SimRoutingComponentEx(GameObject obj)
            : base(obj)
        { }

        public override Route CreateRoute()
        {
            // Custom
            return CommonSpace.Helpers.SimRoutingComponentEx.CreateRoute(this);
        }

        private static InteractionInstance CreateGoHereWithInteractionInstance(SimRoutingComponent ths, Route r, List<Sim> followers, InteractionPriority priority, bool cancellableByPlayer, out InteractionInstance ownerInteraction, GoHereWithSituation.OnFailBehavior failureBehavior, Vector3 teleportLocation)
        {
            Sim properLeader = ths.GetProperLeader(ths.OwnerSim, followers);
            InteractionInstanceParameters parameters = new InteractionInstanceParameters(new InteractionObjectPair(LeaderGoHereWith.Singleton, Terrain.Singleton), properLeader, priority, false, cancellableByPlayer);
            LeaderGoHereWith with = LeaderGoHereWith.Singleton.CreateInstanceFromParameters(ref parameters) as LeaderGoHereWith;
            with.SetOwner(ths.OwnerSim);

            with.OnFailBehavior = failureBehavior;
            if (teleportLocation != Vector3.Invalid)
            {
                with.TeleportDestination = teleportLocation;
            }
            else if (failureBehavior == GoHereWithSituation.OnFailBehavior.Teleport)
            {
                with.TeleportDestination = r.GetDestPoint();
            }

            if (properLeader != ths.OwnerSim)
            {
                followers = new List<Sim>(followers);
                followers.Remove(properLeader);
                Route route = r.ShallowCopy();
                route.ExitReasonsInterrupt = r.ExitReasonsInterrupt;
                ths.UpdateRoutingOptionsFromLeader(properLeader, route);
                route.Follower = properLeader.Proxy;
                route.Replan();
                with.SetRouteToFollow(route);
            }
            else
            {
                with.SetRouteToFollow(r);
            }

            with.SetFollowers(followers);
            GoHereWithSituationEx.CreateSituation(with);
            if (properLeader != ths.OwnerSim)
            {
                ownerInteraction = new SlaveLeaderGoHereWith.Definition(with.Situation).CreateInstance(Terrain.Singleton, ths.OwnerSim, priority, false, cancellableByPlayer);
            }
            else
            {
                ownerInteraction = null;
            }

            return with;
        }

        private bool DoSingleRouteEx(Route r, bool bAllowOverlays)
        {
            Common.StringBuilder msg = new Common.StringBuilder("DoSingleRouteEx");

            try
            {
                mbRouteLeadInPlaying = false;
                if ((bAllowOverlays && (r.GetDistanceRemaining() > TraitTuning.RouteDistanceForIdle)) && !r.GetOption(Route.RouteOption.DisableRouteLeadIns))
                {
                    if (!mOwnerSim.IsHoldingAnything() && RandomUtil.RandomChance01(TraitTuning.ChanceForRouteLeadIn))
                    {
                        mOwnerSim.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayUpperbody);
                        uint segmentAtDistanceBeforeEnd = r.GetSegmentAtDistanceBeforeEnd(SimScriptAdaptor.DistanceToStopOverlay, true);
                        r.RegisterCallback(OnAboutToFinishRoute, RouteCallbackType.TriggerOnce, RouteCallbackConditions.EnteringSegment(segmentAtDistanceBeforeEnd));
                        r.RegisterCallback(OnRouteFinished, RouteCallbackType.TriggerOnce, new RouteCallbackCondition(RouteCallbackConditions.Ended));
                        mbRouteLeadInPlaying = mOwnerSim.IdleManager.PlayRouteLeadIn();
                        if (mbRouteLeadInPlaying)
                        {
                            OverlayComponent overlayComponent = mOwnerSim.OverlayComponent;
                            overlayComponent.OverlaysEnded += OnOverlaysEnded;
                        }
                    }
                    if (mOwnerSim.IsHuman)
                    {
                        mOwnerSim.OverlayComponent.PlayReaction(ReactionTypes.FacialAutoSelect, null);
                    }
                }

                if (mOwnerSim.IsHorse || mOwnerSim.IsDeer)
                {
                    r.RegisterCallback(HorseDeerRoutingEffectsCallback, RouteCallbackType.TriggerWhileTrue, new RouteCallbackCondition(RouteCallbackConditions.AnimationTriggeredEvent));
                }
                else if ((mOwnerSim.IsPuppy || mOwnerSim.IsKitten) && SeasonsManager.Enabled)
                {
                    r.RegisterCallback(PuppyKittenSnowLevelCallback, RouteCallbackType.TriggerOnTrue, new RouteCallbackCondition(RouteCallbackConditions.InDeepSnow));
                    r.RegisterCallback(PuppyKittenSnowLevelCallback, RouteCallbackType.TriggerOnTrue, new RouteCallbackCondition(RouteCallbackConditions.InShallowSnow));
                }

                bool flag = false;
                IHasScriptProxy destObj = r.DestObj;
                try
                {
                    flag = RoutingComponentEx.DoRoute(this, r);
                    float maxMinutesToWaitForRouteLeadIn = SimScriptAdaptor.MaxMinutesToWaitForRouteLeadIn;
                    DateAndTime previousDateAndTime = SimClock.CurrentTime();

                    while (mbRouteLeadInPlaying && (SimClock.ElapsedTime(TimeUnit.Minutes, previousDateAndTime) < maxMinutesToWaitForRouteLeadIn))
                    {
                        SpeedTrap.Sleep();
                    }

                    if (UsingStroller)
                    {
                        new GetOutOfStrollerRouteAction(mOwnerSim, r).PerformAction();
                    }
                }
                finally
                {
                    if (mbRouteLeadInPlaying)
                    {
                        mbRouteLeadInPlaying = false;
                        OverlayComponent component2 = mOwnerSim.OverlayComponent;
                        component2.OverlaysEnded -= OnOverlaysEnded;
                        mOwnerSim.OverlayComponent.StopAllOverlays();
                    }

                    try
                    {
                        mOwnerSim.IdleManager.StopFacialIdle(true);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(mOwnerSim, e);
                    }

                    if (UsingStroller)
                    {
                        new GetOutOfStrollerRouteAction(mOwnerSim, r).PerformAction();
                    }
                }

                if (!flag || !mOwnerSim.HasExitReason(ExitReason.ObjectStateChanged))
                {
                    return flag;
                }

                if (r.DoRouteFail)
                {
                    InteractionInstance currentInteraction = mOwnerSim.CurrentInteraction;
                    if ((currentInteraction != null) && (currentInteraction.Target != null))
                    {
                        mOwnerSim.PlayRouteFailure(currentInteraction.Target.GetThoughtBalloonThumbnailKey());
                    }
                    else
                    {
                        mOwnerSim.PlayRouteFailure();
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(mOwnerSim, e);
            }

            return false;
        }
        /*
        public static void DebugMsg(Sim sim, Common.StringBuilder msg, string text)
        {
            if (text != null)
            {
                msg += Common.NewLine + text;
            }

            if ((sim != null) && (sim.FirstName == "Roberto"))
            {
                Common.Notify(msg);
            }
        }
        */
        public override bool DoRoute(Route r)
        {
            Sim routeSim = null;

            if (!GoHere.Settings.mAllowCarRouting)
            {
                r.SetOption(Route.RouteOption.EnablePlanningAsCar, false);
                r.SetOption(Route.RouteOption.BeginAsCar, false);
            }            

            Common.StringBuilder msg = new Common.StringBuilder("DoRoute");

            try
            {
                if (r.Follower != null)
                {
                    routeSim = r.Follower.Target as Sim;
                }

                /*
                if (!GoHere.Settings.mAllowBoatRouting)
                {
                    Houseboat boat;
                    if (!Houseboat.IsPointOnHouseboat(mOwnerSim.Position, out boat))
                    {
                        r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, false);
                        r.SetOption2(Route.RouteOption2.BeginAsBoat, false);
                        r.SetOption2(Route.RouteOption2.EndAsBoat, false);

                        bool water = false;
                        if (mOwnerSim.InteractionQueue != null)
                        {
                            Terrain.GoHere interaction = mOwnerSim.InteractionQueue.GetHeadInteraction() as Terrain.GoHere;

                            if (interaction != null && interaction.mDestinationType == Terrain.GoHere.DestinationType.OnSeaWater)
                            {
                                water = true;
                            }
                        }

                        if (mOwnerSim.Posture is Ocean.PondAndOceanRoutingPosture || mOwnerSim.Posture is SwimmingInPool)
                        {
                            water = true;
                        }

                        if (water)
                        {
                            r.SetOption(Route.RouteOption.EnableWaterPlanning, true);
                            r.SetOption(Route.RouteOption.DoNotEmitDegenerateRoutesForRadialRangeGoals, true);
                            r.SetOption(Route.RouteOption.DisallowGoalsOnBridges, true);
                        }
                    }
                }
                 */

                if (!GoHere.Settings.mAllowMermaidRouting)
                {
                    r.SetOption2(Route.RouteOption2.RouteAsMermaid, false);
                }

                // why did he use routeSim here? It seems to always be null...
                if ((routeSim != null) && (routeSim.Occupation != null))
                {
                    if (routeSim.SimDescription.GetOutfitCount(OutfitCategories.Career) == 0)
                    {
                        routeSim.Occupation.SetOccupationOutfitForCurrentLevel();
                    }
                }

                if (!r.PlanResult.Succeeded())
                {
                    msg += Common.NewLine + "A: " + r.PlanResult + " False";

                    if (r.PlanResult.mType == RoutePlanResultType.FailedBecauseDestinationPortalLocked)
                    {
                        r.DoRouteFail = false;
                    }

                    DoRouteFailureBehavior(r);
                    mOwnerSim.AddExitReason(ExitReason.RouteFailed);
                    return false;
                }

                bool flag = false;
                IGameObject destObj = r.DestObj as IGameObject;
                try
                {
                    if (destObj != null)
                    {
                        destObj.AddToRoutingReferenceList(mOwnerSim);
                        mOwnerSim.LookAtManager.SetRoutingLookAt(destObj as GameObject);
                    }

                    mbIgnoreAllObstaclesStartTimeValid = false;
                    if (mOwnerSim.IsActiveSim)
                    {
                        Route.SetFadePriority(mOwnerSim.ObjectId, 0x186a3);
                    }
                    else if (mOwnerSim.IsInActiveHousehold)
                    {
                        Route.SetFadePriority(mOwnerSim.ObjectId, 0x186a2);
                    }
                    else
                    {
                        Route.SetFadePriority(mOwnerSim.ObjectId, 0x186a1);
                    }

                    while (true)
                    {
                        flag = false;
                        if (mbPushRequested)
                        {
                            if ((!(mOwnerSim.Posture is SittingInVehicle) && !(mOwnerSim.Posture is SittingInBoat)) && (mOwnerSim.RoutingComponent.RoutingParent == null))
                            {
                                try
                                {
                                    MidRouteBePushed(r.ExecutionFromNonSimTaskIsSafe);
                                }
                                catch (ResetException)
                                {
                                    throw;
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(routeSim, e);
                                }

                                r.Replan();
                            }
                            else
                            {
                                mbPushRequested = false;
                            }

                            continue;
                        }

                        StartPushImmunity(OnRouteStartedImmuneToPushesDuration);
                        flag = DoSingleRouteEx(r, true);
                        flag &= r.PlanResult.Succeeded();
                        if (mbPushRequested)
                        {
                            continue;
                        }

                        if (r.GetOption(Route.RouteOption.CheckForFootprintsNearGoals))
                        {
                            ObjectGuid guid = r.IsPointObstructedBySim(mOwnerSim.PositionOnFloor);
                            if (guid != ObjectGuid.InvalidObjectGuid)
                            {
                                ObjectGuid runningTargetId = ObjectGuid.InvalidObjectGuid;
                                if (((mOwnerSim.InteractionQueue != null) && (mOwnerSim.InteractionQueue.RunningInteraction != null)) && (mOwnerSim.InteractionQueue.RunningInteraction.Target != null))
                                {
                                    runningTargetId = mOwnerSim.InteractionQueue.RunningInteraction.Target.ObjectId;
                                }

                                if ((guid != runningTargetId) && !PushSims(mOwnerSim.PositionOnFloor, 0.5f, false))
                                {
                                    bool option = r.GetOption(Route.RouteOption.BlockedByPeople);
                                    r.SetOption(Route.RouteOption.BlockedByPeople, true);
                                    r.Replan();
                                    r.SetOption(Route.RouteOption.BlockedByPeople, option);
                                    if (r.PlanResult.Succeeded() && (r.IsPointObstructedBySim(mOwnerSim.PositionOnFloor) == ObjectGuid.InvalidObjectGuid))
                                    {
                                        continue;
                                    }

                                    StartPushImmunity(OnEmergencyGetAwayRouteStartedImmuneToPushesDuration);
                                    Route route = PlanRouteForPush(mOwnerSim, null, PushSimsAwayDistanceMin, PushSimsAwayDistanceMax);
                                    DoSingleRouteEx(route, false);
                                    flag = false;
                                    if (r.DoRouteFail)
                                    {
                                        Sim sim = guid.ObjectFromId<Sim>();
                                        PlayRouteFailureIfAppropriate(sim);
                                    }

                                    RouteFailureTurnJigBlocker = ObjectGuid.InvalidObjectGuid;
                                    HasRouteFailureFromTurnJig = false;

                                    msg += Common.NewLine + "B: " + r.PlanResult + " " + flag;

                                    return flag;
                                }
                            }
                        }

                        break;
                    }

                    if (HasRouteFailureFromTurnJig && r.DoRouteFail)
                    {
                        GameObject obj3 = GameObject.GetObject(RouteFailureTurnJigBlocker);
                        PlayRouteFailureIfAppropriate(obj3);
                    }
                }
                finally
                {
                    if (destObj != null)
                    {
                        destObj.RemoveFromRoutingReferenceList(mOwnerSim);
                    }

                    mbIgnoreAllObstaclesStartTimeValid = false;
                    mbPushImmunityStartTimeValid = false;
                    Route.SetFadePriority(mOwnerSim.ObjectId, 0x186a0);
                    mOwnerSim.LookAtManager.SetRoutingLookAt(null);
                }

                RouteFailureTurnJigBlocker = ObjectGuid.InvalidObjectGuid;
                HasRouteFailureFromTurnJig = false;

                msg += Common.NewLine + "C: " + flag;

                return flag;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(routeSim, e);
                throw;
            }
            finally
            {
                //Common.DebugNotify(msg, routeSim);
            }
        }

        public static bool DoRouteWithFollowers(SimRoutingComponent ths, Route r, List<Sim> followers)
        {
            return DoRouteWithFollowers(ths, r, followers, GoHereWithSituation.OnFailBehavior.None, Vector3.Invalid);
        }
        public static bool DoRouteWithFollowers(SimRoutingComponent ths, Route r, List<Sim> followers, GoHereWithSituation.OnFailBehavior failBehavior, Vector3 teleportLocation)
        {
            if ((followers != null) && (followers.Count != 0))
            {
                if (ths.OwnerSim.Autonomy.SituationComponent.InSituationOfType(typeof(GoHereWithSituation)))
                {
                    return false;
                }
                bool flag = r.PlanResult.Succeeded();
                if (!flag)
                {
                    RidingPosture ridingPosture = ths.OwnerSim.RidingPosture;
                    if (ridingPosture != null)
                    {
                        ObjectGuid objectId = ridingPosture.Mount.ObjectId;
                        r.AddObjectToIgnoreForRoute(objectId);
                        r.Replan();
                        r.RemoveObjectToIgnoreForRoute(objectId);
                        flag = r.PlanResult.Succeeded();
                    }
                }
                if (flag)
                {
                    InteractionInstance instance;
                    bool flag2 = false;
                    InteractionPriority priority = ths.OwnerSim.InheritedPriority();

                    // Custom
                    InteractionInstance entry = CreateGoHereWithInteractionInstance(ths, r, followers, priority, true, out instance, failBehavior, teleportLocation);
                    try
                    {
                        ths.mbAllowBikes = false;
                        if (instance != null)
                        {
                            entry.InstanceActor.InteractionQueue.Add(entry);
                        }
                        else
                        {
                            instance = entry;
                        }

                        InteractionInstance currentInteraction = ths.OwnerSim.CurrentInteraction;
                        currentInteraction.PartOfGoHereSituation = true;
                        instance.GroupId = currentInteraction.GroupId;
                        flag2 = instance.RunInteractionWithoutCleanup();
                        flag2 = flag2 && !entry.InstanceActor.HasExitReason(ExitReason.RouteFailed);
                    }
                    finally
                    {
                        ths.mbAllowBikes = true;
                        instance.Cleanup();
                    }
                    return flag2;
                }
                if ((ths.OwnerSim.IsInRidingPosture && (r.Follower.Target == ths.OwnerSim)) && (r.DoRouteFail && ths.mOwnerSim.CheckPlayRouteFailFrequency()))
                {
                    ths.PlayRouteFailureIfAppropriate(null);
                    return false;
                }
            }
            return ths.DoRoute(r);
        }

        public class Loader : Common.IWorldLoadFinished
        {
            protected static void ReplaceComponent(Sim sim)
            {
                if (sim.GetComponent<SimRoutingComponentEx>() != null) return;

                SimRoutingComponent oldComponent = sim.SimRoutingComponent;

                sim.RemoveComponent<SimRoutingComponent>();

                ObjectComponents.AddComponent<SimRoutingComponentEx>(sim, new object[0]);

                if (oldComponent != null)
                {
                    SimRoutingComponent newComponent = sim.SimRoutingComponent;
                    if (newComponent != null)
                    {
                        newComponent.mWalkStyleRequests = oldComponent.mWalkStyleRequests;
                        newComponent.mSimWalkStyle = oldComponent.mSimWalkStyle;
                        if (sim.SimDescription != null && sim.SimDescription.IsPlantSim)
                        {
                            newComponent.SetUpOccultWalkingEffects(OccultTypes.PlantSim);
                        }
                    }
                }
            }

            public void OnWorldLoadFinished()
            {
                new Common.DelayedEventListener(EventTypeId.kSimInstantiated, OnInstantiated);

                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.OccultManager == null) continue;

                    ReplaceComponent(sim);
                }
            }

            public static void OnInstantiated(Event e)
            {
                Sim sim = e.TargetObject as Sim;
                if (sim == null) return;

                ReplaceComponent(sim);
            }
        }
    }
}
