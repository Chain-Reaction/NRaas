using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Telemetry;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Interactions
{
    public class TimePortalActivateDeactivateEx : TimePortal.ActivateDeactivate, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<TimePortal, TimePortal.ActivateDeactivate.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();

            tuning = Tunings.GetTuning<TimePortal, TimePortal.SummonTimeTraveler.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            tuning = Tunings.GetTuning<TimePortal, TimePortal.ResetTimeContinuum.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<TimePortal, TimePortal.ActivateDeactivate.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!TimePortal.sTimeTravelerHasBeenSummoned && Actor.CreateRoute().PlanToSlot(Target, Slot.RoutingSlot_0).Succeeded())
                {
                    if (!TimeTravelerSituation.Create(Actor.LotHome, Actor.ObjectId, Target.ObjectId))
                    {
                        return false;
                    }
                    CauseEffectService.OpportunityPortal = Target;
                    mTimeTravlerArrivalHandled = false;
                }
                if ((Target.InUse && !Target.IsActorUsingMe(Actor)) || !Actor.RouteToSlot(Target, Slot.RoutingSlot_0, false))
                {
                    return false;
                }
                if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    return false;
                }

                Definition interactionDefinition = InteractionDefinition as Definition;
                if (Target.Active)
                {
                    interactionDefinition.curState = TimePortal.PortalState.Active;
                }
                else
                {
                    interactionDefinition.curState = TimePortal.PortalState.Inactive;
                }

                Actor.SkillManager.AddElement(SkillNames.Future);
                StandardEntry(!Target.IsActorUsingMe(Actor));
                BeginCommodityUpdates();
                Target.mTimeToStepBack = false;
                EnterStateMachine("timeportal", "Enter", "x");
                SetActor("portal", Target);
                mCurrentStateMachine.AddOneShotScriptEventHandler(0x65, SwitchActiveState);
                mCurrentStateMachine.AddOneShotScriptEventHandler(0x66, PositionTimeTraveler);
                AnimateSim("Mess With");
                if (!TimePortal.sTimeTravelerHasBeenSummoned)
                {                    
                    Common.DebugNotify("A");

                    mCurrentStateMachine.RequestState(false, "x", "FirstTimeReact");
                    emergencyStopWatch = StopWatch.Create(StopWatch.TickStyles.Seconds);
                    int num = 0x2d;

                    mTimeTraveler = CauseEffectService.GetInstance().GetTimeTraveler();
                    while ((mTimeTraveler == null) && (emergencyStopWatch.GetElapsedTime() < num))
                    {
                        SpeedTrap.Sleep(0x1);

                        mTimeTraveler = CauseEffectService.GetInstance().GetTimeTraveler();

                        /*
                        // Custom
                        SimDescription simDesc = SimDescription.Find(CauseEffectService.sPersistableData.TimeTravelerSimID);
                        if (simDesc == null)
                        {                            
                            CauseEffectService.sPersistableData.TimeTravelerSimID = 0;
                            CauseEffectService.GetInstance().RequestTimeTravelerSimDesc(Household.NpcHousehold);
                        }
                        else
                        {                            
                            mTimeTraveler = Instantiation.PerformOffLot(simDesc, Target.LotCurrent, null);
                        }
                         */
                    }

                    Common.DebugNotify("B");

                    if (mTimeTraveler != null)
                    {                        
                        InteractionQueue interactionQueue = mTimeTraveler.InteractionQueue;
                        mLinkedInteraction = TimePortal.BeSummoned.Singleton.CreateInstance(Actor, mTimeTraveler, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false) as TimePortal.BeSummoned;
                        mLinkedInteraction.SyncTarget = Actor;
                        interactionQueue.AddNext(mLinkedInteraction);
                        Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                        Actor.SynchronizationTarget = mTimeTraveler;
                        Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                        if (Actor.WaitForSynchronizationLevelWithSim(mTimeTraveler, Sim.SyncLevel.Started, 40f))
                        {
                            Common.DebugNotify("C");

                            SetActorAndEnter("y", mTimeTraveler, "TTEnter");
                            AnimateJoinSims("TimeTravelerExit");
                            Actor.SynchronizationLevel = Sim.SyncLevel.Completed;

                            TimeTravelerSituation situation = ServiceSituation.FindServiceSituationInvolving(mTimeTraveler) as TimeTravelerSituation;
                            if (situation != null)
                            {
                                situation.OnAppearComplete(mTimeTraveler, 0f);

                                while (!Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                                {
                                    Actor.LoopIdle();
                                    SpeedTrap.Sleep(0xa);
                                }
                            }
                            else
                            {
                                Target.ActorsUsingMe.Clear();

                                TimePortal.sTimeTravelerHasBeenSummoned = true;
                            }
                        }
                        else
                        {
                            AnimateSim("Exit");
                        }

                        Common.DebugNotify("D");

                        mTimeTravlerArrivalHandled = true;
                    }
                    else
                    {                        
                        AnimateSim("Exit");
                    }
                }
                else
                {                   
                    if (!GameUtils.IsFutureWorld())
                    {
                        MiniSimDescription simDesc = MiniSimDescription.Find(CauseEffectService.sPersistableData.TimeTravelerSimID);
                        if (simDesc == null)
                        {                            
                            CauseEffectService.sPersistableData.TimeTravelerSimID = 0;
                            CauseEffectService.GetInstance().RequestTimeTravelerSimDesc(Household.NpcHousehold);
                        }                        
                    }

                    AnimateSim("Exit");
                    Vector3 v = Target.Position + ((Vector3)(2f * Target.ForwardVector));
                    Actor.RouteToPoint(v);
                    Actor.RouteTurnToFace(Target.Position);
                }

                EndCommodityUpdates(true);
                StandardExit();
                if (Target.Active)
                {
                    EventTracker.SendEvent(EventTypeId.kInspectedTimePortal, Actor);
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
            }
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
                //Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
            }

            return false;
        }

        public new class Definition : TimePortal.ActivateDeactivate.Definition
        {
            public override string GetInteractionName(Sim actor, TimePortal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TimePortalActivateDeactivateEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
