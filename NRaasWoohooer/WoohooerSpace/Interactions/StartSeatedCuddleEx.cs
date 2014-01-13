using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class StartSeatedCuddleEx : StartSeatedCuddleA, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, StartSeatedCuddleA.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public override bool Run()
        {
            try
            {
                StartSeatedCuddleB eb;
                Actor.GreetSimOnMyLotIfPossible(Target);
                Seat.EnsureLivingChairPosture(Actor);
                IHasSeatingGroup container = Actor.Posture.Container as IHasSeatingGroup;
                Seat seat = container.SeatingGroup[Actor];
                Seat seat2 = container.SeatingGroup[Target];
                if (!SafeToSync())
                {
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                Actor.LookAtManager.SetInteractionLookAt(Target, kTargetLookAtPriority, LookAtJointFilter.TorsoBones);
                if ((seat2 != null) && !seat2.IsAdjacentTo(seat))
                {
                    Seat emptyAdjacentSeat = seat2.GetEmptyAdjacentSeat(Actor);
                    if (emptyAdjacentSeat != null)
                    {
                        InteractionInstance instance = seat.CreateChangeSeatInteraction(emptyAdjacentSeat, GetPriority(), Autonomous, CancellableByPlayer);
                        StartSeatedCuddleA ea = Singleton.CreateInstance(Target, Actor, GetPriority(), Autonomous, CancellableByPlayer) as StartSeatedCuddleA;
                        Actor.InteractionQueue.PushAsContinuation(instance, true);
                        Actor.InteractionQueue.PushAsContinuation(ea, true);
                        return true;
                    }
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                if (seat is IHotTubSeat)
                {
                    eb = StartSeatedCuddleB.HotTubSingleton.CreateInstance(Actor, Target, GetPriority(), false, CancellableByPlayer) as StartSeatedCuddleB;
                }
                else
                {
                    eb = StartSeatedCuddleB.Singleton.CreateInstance(Actor, Target, GetPriority(), false, CancellableByPlayer) as StartSeatedCuddleB;
                }

                eb.SeatX = seat;
                eb.SeatY = seat2;
                LinkedInteractionInstance = eb;
                Target.InteractionQueue.Add(eb);

                // Custom
                mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventMedium);
                //SocialComponent.SendCheatingEvents(Actor, Target, !Rejected);

                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = Target;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Started, (float)kHowLongToWaitForOtherToArrive))
                {
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                StartSocial("Cuddle");
                eb.Reject = Rejected;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, (float)kHowLongToWaitForOtherToArrive))
                {
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                if (Rejected)
                {
                    if (Seat.SimsAreAdjacent(Actor, Target))
                    {
                        Seat.EnsureLivingChairPosture(Target);
                        bool paramValue = seat2 == seat.Right;
                        EnterStateMachine("SeatingCuddle", "EnterSitting", "x", "y");
                        mCurrentStateMachine.EnterState("y", "EnterSitting");
                        mCurrentStateMachine.EnterState("SeatX", "EnterSitting");
                        mCurrentStateMachine.EnterState("SeatY", "EnterSitting");
                        SetActor("SeatX", seat.Host);
                        SetParameter("SuffixX", seat.IKSuffix);
                        SetActor("SeatY", seat2.Host);
                        SetParameter("SuffixY", seat2.IKSuffix);
                        SetActor("surface", seat.Host);
                        SetParameter("IsMirrored", paramValue);
                        bool flag2 = seat is IHotTubSeat;
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
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                Actor.SynchronizationLevel = Sim.SyncLevel.Completed;
                if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Completed, (float)kHowLongToWaitForOtherToArrive))
                {
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                if (!Seat.SimsAreAdjacent(Actor, Target))
                {
                    Actor.Posture.CurrentStateMachine.RequestState("x", "routeFail");
                    PushRelaxIfHottub(seat, Autonomous, CancellableByPlayer);
                    return false;
                }

                if (seat2 == null)
                {
                    seat2 = container.SeatingGroup[Target];
                }

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.Zero);
                CuddleSeated entry = CuddleSeated.Singleton.CreateInstance(Target, Actor, priority, false, CancellableByPlayer) as CuddleSeated;
                entry.AdoptSocialEffect(this);
                entry.SeatX = seat;
                entry.SeatY = seat2;
                CuddleSeated seated2 = CuddleSeated.Singleton.CreateInstance(Actor, Target, priority, false, CancellableByPlayer) as CuddleSeated;
                seated2.SeatX = seat;
                seated2.SeatY = seat2;
                entry.LinkedInteractionInstance = seated2;
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, Target, "Cuddle", false, !Rejected, false, CommodityTypes.Undefined));
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Target, Actor, "Cuddle", true, !Rejected, false, CommodityTypes.Undefined));
                EventTracker.SendEvent(EventTypeId.kCuddled, Actor, Target);
                EventTracker.SendEvent(EventTypeId.kCuddled, Target, Actor);
                Actor.InteractionQueue.Add(entry);
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

        public new class Definition : InteractionDefinition<Sim, Sim, StartSeatedCuddleEx>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
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
                    InteractionInstance currentInteraction = a.CurrentInteraction;
                    if (((currentInteraction != null) && (currentInteraction.Target == target)) && ((currentInteraction is CuddleSeated) || (currentInteraction is NestedCuddleInteraction)))
                    {
                        return false;
                    }

                    IHasSeatingGroup container = a.Posture.Container as IHasSeatingGroup;
                    if (container != null)
                    {
                        Seat seat = container.SeatingGroup[a];
                        if ((seat != null) && (seat is IHotTubSeat))
                        {
                            if (target.CurrentOutfitCategory == OutfitCategories.SkinnyDippingTowel)
                            {
                                greyedOutTooltipCallback = new GrayedOutTooltipHelper(target.IsFemale, "ClothesStolenTooltip", null).GetTooltip;
                                return false;
                            }
                        }
                    }

                    string reason;
                    if (!CommonSocials.CanGetRomantic(a, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason))
                    {
                        return false;
   
                    }

                    if (!StartSeatedCuddleA.Definition.CanCuddle(a, target, ref greyedOutTooltipCallback, true))
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }
        }
    }
}
