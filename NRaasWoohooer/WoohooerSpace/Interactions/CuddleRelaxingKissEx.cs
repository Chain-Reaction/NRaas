using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class CuddleRelaxingKissEx : CuddleRelaxingKiss, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, CuddleRelaxingKiss.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public override bool Run()
        {
            try
            {
                if (!StartSync())
                {
                    return false;
                }

                // Custom
                mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventMedium);
                //SocialComponent.SendCheatingEvents(Actor, Target, !Rejected);

                StandardEntry(false);
                BeginCommodityUpdates();
                if (IsMaster)
                {
                    ReturnInstance.EnsureMaster();
                    StartSocial("Kiss");
                    InitiateSocialUI(Actor, Target);
                    if ((Actor.Posture.Container as Bed).IsTent)
                    {
                        ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.BalloonData("balloon_lips");
                        Actor.ThoughtBalloonManager.ShowBalloon(bd);
                        Target.ThoughtBalloonManager.ShowBalloon(bd);
                    }

                    (LinkedInteractionInstance as NestedRelaxingInteraction).Rejected = Rejected;
                    if (Rejected)
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "KissReject");
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "ToRelax");
                        FinishSocial("Kiss", true);
                    }
                    else
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "Kiss");
                        FinishSocial("Kiss", true);
                    }
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                FinishLinkedInteraction(IsMaster);
                EndCommodityUpdates(!Rejected);
                StandardExit(false, false);
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, Target, "Kiss", !IsMaster, !Rejected, false, CommodityTypes.Undefined));
                if (!Rejected)
                {
                    Actor.SimDescription.SetFirstKiss(Target.SimDescription);
                    DoResume();
                    if (SimClock.IsNightTime() && SimClock.IsFullMoon())
                    {
                        Actor.BuffManager.AddElement(BuffNames.KissedUnderFullMoon, Origin.None);
                        EventTracker.SendEvent(EventTypeId.kKissedUnderFullMoon, Actor);
                    }
                }

                WaitForSyncComplete();
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

        public new class Definition : CuddleRelaxingKiss.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CuddleRelaxingKissEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
