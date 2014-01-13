using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class BedCuddleEx : StartBedCuddleA, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.GetTuning<Sim, StartBedCuddleB.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            Woohooer.InjectAndReset<Sim, StartBedCuddleA.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                Actor.GreetSimOnMyLotIfPossible(Target);
                if (GetCuddleType(Actor, Target) == CuddleType.CuddleTargetOnDifferentBed)
                {
                    ChildUtils.SetPosturePrecondition(this, CommodityKind.Relaxing, new CommodityKind[] { CommodityKind.NextToTarget });
                    Actor.InteractionQueue.PushAsContinuation(this, true);
                    return true;
                }

                if (!SafeToSync())
                {
                    return false;
                }

                // Custom
                mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventMedium);
                //SocialComponent.SendCheatingEvents(Actor, Target, !Rejected);

                StartBedCuddleB entry = StartBedCuddleB.Singleton.CreateInstance(Actor, Target, GetPriority(), false, CancellableByPlayer) as StartBedCuddleB;
                LinkedInteractionInstance = entry;
                Target.InteractionQueue.Add(entry);
                if (Target.Posture.Container != Actor.Posture.Container)
                {
                    Actor.LookAtManager.SetInteractionLookAt(Target, 0xc8, LookAtJointFilter.TorsoBones);
                    Actor.Posture.CurrentStateMachine.RequestState("x", "callOver");
                }

                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = Target;
                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Started, Bed.kTimeToWaitForOtherToArriveForCuddling))
                {
                    return false;
                }

                Actor.Posture.CurrentStateMachine.RequestState("x", "ExitRelaxing");
                StartSocial("Cuddle");
                entry.Rejected = Rejected;
                Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, Bed.kTimeToWaitForOtherToArriveForCuddling))
                {
                    return false;
                }

                if (Rejected)
                {
                    if (Actor.Posture.Container == Target.Posture.Container)
                    {
                        BedMultiPart container = Actor.Posture.Container as BedMultiPart;
                        BedData partSimIsIn = container.PartComp.GetPartSimIsIn(Actor);
                        EnterStateMachine("BedSocials", "FromRelax", "x", "y");
                        mCurrentStateMachine.EnterState("y", "FromRelax");
                        mCurrentStateMachine.EnterState("bed", "FromRelax");
                        SetActor("bed", container);
                        partSimIsIn.SetPartParameters(mCurrentStateMachine);
                        mCurrentStateMachine.RequestState(false, "x", "CuddleReject");
                        mCurrentStateMachine.RequestState(false, "bed", "CuddleReject");
                        mCurrentStateMachine.RequestState(true, "y", "CuddleReject");
                        mCurrentStateMachine.RequestState(null, "ToRelax");
                        Target.RouteAway(kMinDistanceToMoveAwayWhenRejected, kMaxDistanceToMoveAwayWhenRejected, true, new InteractionPriority(InteractionPriorityLevel.Zero), false, true, true, RouteDistancePreference.NoPreference);
                    }
                    else
                    {
                        Actor.Posture.CurrentStateMachine.RequestState(true, "x", "CallOverReject");
                    }
                    return false;
                }

                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Completed, Bed.kTimeToWaitForOtherToArriveForCuddling))
                {
                    return false;
                }

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.Zero);
                CuddleRelaxing relaxing = CuddleRelaxing.Singleton.CreateInstance(Target, Actor, priority, false, CancellableByPlayer) as CuddleRelaxing;
                relaxing.IsMaster = true;
                relaxing.AdoptSocialEffect(this);
                CuddleRelaxing relaxing2 = CuddleRelaxing.Singleton.CreateInstance(Actor, Target, priority, false, CancellableByPlayer) as CuddleRelaxing;
                relaxing2.IsMaster = false;
                relaxing.LinkedInteractionInstance = relaxing2;
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, Target, "Cuddle", false, !Rejected, false, CommodityTypes.Undefined));
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Target, Actor, "Cuddle", true, !Rejected, false, CommodityTypes.Undefined));
                Actor.InteractionQueue.Add(relaxing);
                Target.InteractionQueue.Add(relaxing2);
                return !Rejected;
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

        public new class Definition : InteractionDefinition<Sim, Sim, StartBedCuddleA>, IOverrideStartInteractionBehavior
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public void StartInteraction(Sim simA, IGameObject target)
            {
                Sim sim = target as Sim;
                if ((simA.Conversation != null) && (simA.Conversation != sim.Conversation))
                {
                    simA.SocialComponent.LeaveConversation();
                }
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenWoohoo))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a == target) return false;

                    if (a.Posture == null) return false;

                    if (!a.Posture.Satisfies(CommodityKind.Relaxing, target)) return false;

                    if (!isAutonomous && SocialComponent.IsTargetUnavailableForSocialInteraction(target, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    string reason;

                    InteractionInstance currentInteraction = a.CurrentInteraction;
                    CuddleRelaxing relaxing = currentInteraction as CuddleRelaxing;
                    if ((relaxing != null) && (relaxing.Target == target))
                    {
                        return false;
                    }
                    else if (!CommonSocials.CanGetRomantic(a, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason))
                    {
                        return false;
                    }
                    else if ((currentInteraction is NestedRelaxingInteraction) || (currentInteraction is WooHoo))
                    {
                        return false;
                    }

                    if (!BedWoohoo.CanCuddleOnBedOfSimA(a, target))
                    {
                        return BedWoohoo.CanCuddleOnBedOfSimA(target, a);
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }
        }
    }
}
