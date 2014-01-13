using NRaas.CommonSpace.Helpers;
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
    public class CuddleSeatedMakeOutEx : CuddleSeatedMakeOut, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, CuddleSeatedMakeOut.Definition, Definition>(false);

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

                StandardEntry(false);
                BeginCommodityUpdates();
                if (IsMaster)
                {
                    ReturnInstance.EnsureMaster();
                    StartSocial("Make Out");
                    InitiateSocialUI(Actor, Target);
                    (LinkedInteractionInstance as NestedCuddleInteraction).Rejected = Rejected;

                    //Custom
                    mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventHigh);
                    //SocialComponent.SendCheatingEvents(Actor, Target, !Rejected);

                    IHasSeatingGroup container = Actor.Posture.Container as IHasSeatingGroup;
                    Seat seat = container.SeatingGroup[Actor];
                    ReturnInstance.mCurrentStateMachine.SetParameter("IsHottub", seat is IHotTubSeat);
                    if (Rejected)
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "MakeOutReject");
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "ExitSitting");
                        FinishSocial("Make Out", true);
                        FinishSocialContext();
                    }
                    else
                    {
                        ReturnInstance.mCurrentStateMachine.RequestState(null, "MakeOut");
                        StartStages();
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
                EventTracker.SendEvent(EventTypeId.kMadeOut, Actor, Target);
                if (Rejected)
                {
                    InvokeDoResumeOnCleanup = false;
                }
                else
                {
                    Actor.SimDescription.SetFirstKiss(Target.SimDescription);
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

        public new class Definition : CuddleSeatedMakeOut.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CuddleSeatedMakeOutEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
