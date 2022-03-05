using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class UseBusinessPlannerEx : AcademicBusinessPlanner.UseBusinessPlanner, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<AcademicBusinessPlanner, AcademicBusinessPlanner.UseBusinessPlanner.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<AcademicBusinessPlanner, AcademicBusinessPlanner.UseBusinessPlanner.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                if (interactionDefinition == null)
                {
                    return false;
                }
                if (Autonomous && (interactionDefinition.mUsageType == BusinessPlanning.None))
                {
                    interactionDefinition.mUsageType = (BusinessPlanning)RandomUtil.GetInt(0, 1);
                }
                IInteractionNameCanBeOverriden overriden = this;
                if (overriden != null)
                {
                    overriden.SetInteractionName(interactionDefinition.GetAutonomousInteractionName());
                    UpdateCaption = true;
                }
                if (this.mRunFromInventory && Actor.Inventory.Contains(Target))
                {
                    Vector3 vector;
                    Vector3 vector2;
                    FindGoodLocationBooleans constraints = FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.PreferEmptyTiles;
                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(Actor.Position, constraints);
                    if (!GlobalFunctions.FindGoodLocation(Target, fglParams, out vector, out vector2))
                    {
                        return false;
                    }
                    Actor.Inventory.TryToRemove(Target);
                    Target.SetPosition(vector);
                    Target.SetForward(vector2);
                    Target.AddToWorld();
                    Target.SetOpacity(0f, 0f);
                }
                if (!Target.Line.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), AcademicBusinessPlanner.kTimeToWaitInLine))
                {
                    Actor.Inventory.TryToAdd(Target);
                    return false;
                }
                if (!Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0))
                {
                    Actor.Inventory.TryToAdd(Target);
                    return false;
                }
                StandardEntry();
                Target.EnableFootprintAndPushSims(AcademicBusinessPlanner.sFootprintHash, Actor);
                if (this.mRunFromInventory)
                {
                    Actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                    Target.SetOpacityTransition(0f, 1f, 1f);
                }
                AcquireStateMachine("BusinessPlanner");
                SetActor("x", Actor);
                SetActor("planner", Target);
                List<AcademicDegreeNames> namesNeeded = new List<AcademicDegreeNames>();
                namesNeeded.Add(AcademicDegreeNames.Business);
                BeginCommodityUpdatesAccountingForAcademicPerformance(namesNeeded);
                Target.mCurrentBusinessPlanningType = interactionDefinition.mUsageType;
                mCurrentStateMachine.EnterState("x", "Enter");
                if (!Target.PlannerIsOpen)
                {
                    AnimateSim("Open");
                    Target.PlannerIsOpen = true;
                }
                float kXPIncreasePerWorking = 0f;
                ReactionBroadcaster broadcaster = null;
                if (interactionDefinition.mUsageType == BusinessPlanning.WorkOnBusinessPlan)
                {
                    AnimateSim("Work");
                    this.mWaitTime = RandomUtil.GetFloat(kMinWorkLength, kMaxWorkLength);
                    kXPIncreasePerWorking = AcademicBusinessPlanner.UseBusinessPlanner.kXPIncreasePerWorking;
                }
                else
                {
                    AnimateSim("Practice");
                    this.mWaitTime = RandomUtil.GetFloat(kMinPresentationLength, kMaxPresentationLength);
                    kXPIncreasePerWorking = kXPIncreasePerPresentation;
                    broadcaster = new ReactionBroadcaster(Target, kPresentationBroadcastParams, new ReactionBroadcaster.BroadcastCallback(this.BroadcastCallback));
                }
                DoTimedLoop(this.mWaitTime, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                bool flag2 = false;
                foreach (InteractionInstance instance in Actor.InteractionQueue.InteractionList)
                {
                    if ((instance != this) && (instance is AcademicBusinessPlanner.UseBusinessPlanner))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2 && Target.PlannerIsOpen)
                {
                    AnimateSim("Close");
                    Target.PlannerIsOpen = false;
                }
                AnimateSim("Exit");
                Target.DisableFootprint(AcademicBusinessPlanner.sFootprintHash);
                EndCommodityUpdates(true);
                StandardExit();
                if (!Actor.HasExitReason(ExitReason.Canceled))
                {
                    if (interactionDefinition.mUsageType == BusinessPlanning.PresentBusinessPlan)
                    {
                        EventTracker.SendEvent(EventTypeId.kPresentedBusinessPlan, Actor, Target.LotCurrent);
                    }
                    else if (interactionDefinition.mUsageType == BusinessPlanning.WorkOnBusinessPlan)
                    {
                        EventTracker.SendEvent(EventTypeId.kWorkedOnBusinessPlan, Actor, Target.LotCurrent);
                    }
                }
                if (broadcaster != null)
                {
                    broadcaster.EndBroadcast();
                    broadcaster.Dispose();
                    broadcaster = null;
                }

                // Custom, removed IsUniversityWorld() check

                Business occupationAsCareer = Actor.OccupationAsCareer as Business;
                if (occupationAsCareer != null)
                {
                    occupationAsCareer.UpdatePerformanceOrExperience(kXPIncreasePerWorking);
                }

                AcademicCareer occupationAsAcademicCareer = Actor.OccupationAsAcademicCareer;
                if (((occupationAsAcademicCareer != null) && (occupationAsAcademicCareer.DegreeInformation != null)) && (occupationAsAcademicCareer.DegreeInformation.AcademicDegreeName == AcademicDegreeNames.Business))
                {
                    occupationAsAcademicCareer.UpdatePerformanceOrExperience(kXPIncreasePerWorking);
                }

                if (!Target.PlannerIsOpen)
                {
                    Actor.WaitForExitReason(2f, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (!Actor.Inventory.Contains(Target))
                    {
                        Actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                        Actor.Inventory.TryToAdd(Target);
                    }
                }
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : AcademicBusinessPlanner.UseBusinessPlanner.Definition
        {
            public Definition()
            { }
            public Definition(AcademicBusinessPlanner.UseBusinessPlanner.BusinessPlanning type)
                : base(type)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new UseBusinessPlannerEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, AcademicBusinessPlanner target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(AcademicBusinessPlanner.UseBusinessPlanner.BusinessPlanning.PresentBusinessPlan), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(AcademicBusinessPlanner.UseBusinessPlanner.BusinessPlanning.WorkOnBusinessPlan), iop.Target));
            }

            public override string GetInteractionName(Sim a, AcademicBusinessPlanner target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
