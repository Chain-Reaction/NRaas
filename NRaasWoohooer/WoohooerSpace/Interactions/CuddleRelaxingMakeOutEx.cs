using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class CuddleRelaxingMakeOutEx : CuddleRelaxingMakeOut, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, CuddleRelaxingMakeOut.Definition, Definition>(false);

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
                mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                //SocialComponent.SendCheatingEvents(Actor, Target, !Rejected);

                StandardEntry(false);
                BeginCommodityUpdates();
                if (IsMaster)
                {
                    ReturnInstance.EnsureMaster();
                    StartSocial("Make Out");
                    InitiateSocialUI(Actor, Target);
                    (LinkedInteractionInstance as NestedRelaxingInteraction).Rejected = Rejected;
                    if (Rejected)
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "MakeOutReject");
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "ToRelax");
                        FinishSocial("Make Out", true);
                    }
                    else
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "MakeOut");
                        StartStages();
                        if ((Actor.Posture.Container as Bed).IsTent)
                        {
                            ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.BalloonData("balloon_lips");
                            Actor.ThoughtBalloonManager.ShowBalloon(bd);
                            Target.ThoughtBalloonManager.ShowBalloon(bd);
                        }
                        DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                        FinishSocial("Make Out", true);
                    }
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                FinishLinkedInteraction(IsMaster);
                EndCommodityUpdates(!Rejected);
                StandardExit(false, false);
                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, Target, "Make Out", !IsMaster, !Rejected, false, CommodityTypes.Undefined));
                if (!Rejected)
                {
                    Actor.SimDescription.SetFirstKiss(Target.SimDescription);
                    DoResume();
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

        public new class Definition : CuddleRelaxingMakeOut.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CuddleRelaxingMakeOutEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
