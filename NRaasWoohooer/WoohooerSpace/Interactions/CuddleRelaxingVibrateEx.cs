using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;

namespace NRaas.WoohooerSpace.Interactions
{
    public class CuddleRelaxingVibrateEx : CuddleRelaxingVibrate, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, CuddleRelaxingVibrate.Definition, Definition>(false);

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
                HeartShapedBed container = Actor.Posture.Container as HeartShapedBed;
                bool isVibrating = container.IsVibrating;
                if (IsMaster)
                {
                    ReturnInstance.EnsureMaster();
                    StartSocial("Vibrate Bed");
                    InitiateSocialUI(Actor, Target);
                    container.SetVibration(Actor, !isVibrating);
                    if (isVibrating)
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "Vibrate Stop");
                    }
                    else
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "Vibrate");
                    }
                    FinishSocial("Vibrate Bed", true);
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                DoResume();
                FinishLinkedInteraction(IsMaster);
                EndCommodityUpdates(!isVibrating);
                StandardExit(false, false);
                WaitForSyncComplete();

                if (isVibrating)
                {
                    if (RandomUtil.RandomChance(kSimGetsNegativeBuff))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Nauseous, Origin.FromVibratingBed);
                    }
                    else if (RandomUtil.RandomChance(kSimGetsPositiveBuff))
                    {
                        if (Actor.TraitManager.HasAnyElement(new TraitNames[] { TraitNames.Childish, TraitNames.Insane }))
                        {
                            Actor.BuffManager.AddElement(BuffNames.Amused, Origin.FromVibratingBed);
                        }
                        else if (Actor.TraitManager.HasAnyElement(new TraitNames[] { TraitNames.PartyAnimal, TraitNames.Flirty }) || Actor.BuffManager.HasElement(BuffNames.Blizzard))
                        {
                            Actor.BuffManager.AddElement(BuffNames.Excited, Origin.FromVibratingBed);
                        }
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

        public new class Definition : CuddleRelaxingVibrate.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CuddleRelaxingVibrateEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
