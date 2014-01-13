using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class StartSaunaSeatedCuddleAEx : SaunaClassic.StartSaunaSeatedCuddleA, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;
        
        public void OnPreLoad()
        {
            Tunings.Inject<Sim, SaunaClassic.StartSaunaSeatedCuddleA.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                Actor.GreetSimOnMyLotIfPossible(Target);
                Seat.EnsureLivingChairPosture(Actor);
                IHasSeatingGroup container = Actor.Posture.Container as IHasSeatingGroup;
                Seat otherSeat = container.SeatingGroup[Actor];
                Seat seat2 = container.SeatingGroup[Target];
                if (!SafeToSync())
                {
                    return false;
                }

                Actor.LookAtManager.SetInteractionLookAt(Target, kTargetLookAtPriority, LookAtJointFilter.TorsoBones);
                if ((seat2 != null) && !seat2.IsAdjacentTo(otherSeat))
                {
                    Seat emptyAdjacentSeat = seat2.GetEmptyAdjacentSeat(Actor);
                    if (emptyAdjacentSeat != null)
                    {
                        InteractionInstance instance = otherSeat.CreateChangeSeatInteraction(emptyAdjacentSeat, GetPriority(), Autonomous, CancellableByPlayer);
                        SaunaClassic.StartSaunaSeatedCuddleA ea = Singleton.CreateInstance(Target, Actor, GetPriority(), Autonomous, CancellableByPlayer) as SaunaClassic.StartSaunaSeatedCuddleA;
                        Actor.InteractionQueue.PushAsContinuation(instance, true);
                        Actor.InteractionQueue.PushAsContinuation(ea, true);
                        return true;
                    }
                    return false;
                }

                SaunaClassic.StartSaunaSeatedCuddleB entry = SaunaClassic.StartSaunaSeatedCuddleB.Singleton.CreateInstance(Actor, Target, GetPriority(), false, CancellableByPlayer) as SaunaClassic.StartSaunaSeatedCuddleB;
                entry.SeatX = otherSeat;
                entry.SeatY = seat2;
                LinkedInteractionInstance = entry;
                Target.InteractionQueue.Add(entry);

                // Custom
                mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventMedium);
                //SocialComponent.SendCheatingEvents(Actor, Target, !Rejected);

                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = Target;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Started, (float)kHowLongToWaitForOtherToArrive))
                {
                    return false;
                }

                StartSocial("Cuddle");
                entry.Reject = Rejected;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, (float)kHowLongToWaitForOtherToArrive))
                {
                    return false;
                }

                if (Rejected)
                {
                    if (Seat.SimsAreAdjacent(Actor, Target))
                    {
                        Seat.EnsureLivingChairPosture(Target);
                        bool paramValue = seat2 == otherSeat.Right;
                        EnterStateMachine("SeatingCuddle", "EnterSitting", "x", "y");
                        mCurrentStateMachine.EnterState("y", "EnterSitting");
                        mCurrentStateMachine.EnterState("SeatX", "EnterSitting");
                        mCurrentStateMachine.EnterState("SeatY", "EnterSitting");
                        SetActor("SeatX", otherSeat.Host);
                        SetParameter("SuffixX", otherSeat.IKSuffix);
                        SetActor("SeatY", seat2.Host);
                        SetParameter("SuffixY", seat2.IKSuffix);
                        SetActor("surface", otherSeat.Host);
                        SetParameter("IsMirrored", paramValue);
                        bool flag2 = otherSeat is IHotTubSeat;
                        SetParameter("IsHottub", flag2);
                        mCurrentStateMachine.RequestState(false, "x", "CuddleReject");
                        mCurrentStateMachine.RequestState(false, "SeatX", "CuddleReject");
                        mCurrentStateMachine.RequestState(false, "SeatY", "CuddleReject");
                        mCurrentStateMachine.RequestState(true, "y", "CuddleReject");
                        mCurrentStateMachine.RequestState(null, "ExitSitting");
                        Target.RouteAway(kMinDistanceToMoveAwayWhenRejected, kMaxDistanceToMoveAwayWhenRejected, true, new InteractionPriority(InteractionPriorityLevel.Zero), false, true, true, RouteDistancePreference.NoPreference);
                    }
                    else if (Actor.TraitManager.HasElement(TraitNames.HotHeaded))
                    {
                        Actor.PlayReaction(ReactionTypes.JealousIntense, ReactionSpeed.Immediate);
                    }
                    else if (Actor.TraitManager.HasElement(TraitNames.OverEmotional))
                    {
                        Actor.PlayReaction(ReactionTypes.JealousMild, ReactionSpeed.Immediate);
                    }
                    else if (Actor.TraitManager.HasAnyElement(new TraitNames[] { TraitNames.Flirty, TraitNames.GreatKisser }))
                    {
                        Actor.PlayReaction(ReactionTypes.Shrug, ReactionSpeed.Immediate);
                    }
                    else
                    {
                        Actor.PlayReaction(ReactionTypes.Awkward, ReactionSpeed.Immediate);
                    }
                    return false;
                }

                Actor.SynchronizationLevel = Sim.SyncLevel.Completed;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Completed, (float)kHowLongToWaitForOtherToArrive))
                {
                    return false;
                }

                if (!Seat.SimsAreAdjacent(Actor, Target))
                {
                    Actor.Posture.CurrentStateMachine.RequestState("x", "routeFail");
                    return false;
                }

                if (seat2 == null)
                {
                    seat2 = container.SeatingGroup[Target];
                }

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.Zero);
                CuddleSeated seated = CuddleSeated.Singleton.CreateInstance(Target, Actor, priority, false, CancellableByPlayer) as CuddleSeated;
                seated.AdoptSocialEffect(this);
                seated.SeatX = otherSeat;
                seated.SeatY = seat2;
                CuddleSeated seated2 = CuddleSeated.Singleton.CreateInstance(Actor, Target, priority, false, CancellableByPlayer) as CuddleSeated;
                seated2.SeatX = otherSeat;
                seated2.SeatY = seat2;
                seated.LinkedInteractionInstance = seated2;
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, Target, "Cuddle", false, !Rejected, false, CommodityTypes.Undefined));
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Target, Actor, "Cuddle", true, !Rejected, false, CommodityTypes.Undefined));
                EventTracker.SendEvent(EventTypeId.kCuddled, Actor, Target);
                EventTracker.SendEvent(EventTypeId.kCuddled, Target, Actor);
                Actor.InteractionQueue.Add(seated);
                Target.InteractionQueue.Add(seated2);
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

        public new class Definition : SaunaClassic.StartSaunaSeatedCuddleA.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new StartSaunaSeatedCuddleAEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                /*
                if (a.SimDescription.IsVisuallyPregnant)
                {
                    return false;
                }
                if (target.SimDescription.IsVisuallyPregnant)
                {
                    return false;
                }
                */

                InteractionInstance currentInteraction = a.CurrentInteraction;
                if (((currentInteraction != null) && (currentInteraction.Target == target)) && ((currentInteraction is CuddleSeated) || (currentInteraction is NestedCuddleInteraction)))
                {
                    return false;
                }

                string reason;
                return (CommonSocials.CanGetRomantic(a, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason) && CanCuddle(a, target, ref greyedOutTooltipCallback, true));
            }
        }
    }
}


