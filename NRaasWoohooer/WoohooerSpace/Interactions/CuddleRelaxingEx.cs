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
using Sims3.Gameplay.Objects.Beds;
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
    public class CuddleRelaxingEx : CuddleRelaxing, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, CuddleRelaxing.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public override bool Run()
        {
            try
            {
                SetPriority(new InteractionPriority(InteractionPriorityLevel.Zero));
                Cancelled = false;
                if (!StartSync(IsMaster, true))
                {
                    return false;
                }

                // Custom
                mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, Conversation.ReactToSocialParams, SocialComponentEx.ReactToJealousEventMedium);

                CommonWoohoo.CheckForWitnessedCheating(Actor, Target, !Rejected);

                if (IsMaster)
                {
                    //StartSocialContext();
                    Actor.SocialComponent.StartSocializingWith(Target);

                    InitiateSocialUI(Actor, Target);
                    if (mCurrentStateMachine == null)
                    {
                        BedMultiPart container = Actor.Posture.Container as BedMultiPart;
                        EnterStateMachine("BedSocials", "FromRelax", "x", "y");
                        SetActor("bed", container);
                        SetActorsAndParameters();
                        mCurrentStateMachine.EnterState("y", "FromRelax");
                        mCurrentStateMachine.RequestState(false, "x", "Cuddle");
                        mCurrentStateMachine.RequestState(true, "y", "Cuddle");
                        FinishSocial("Cuddle", true);
                    }
                    else
                    {
                        mCurrentStateMachine.RequestState(null, "Cuddle");
                    }
                }
                StandardEntry(false);
                EnsureLinkedInteraction();
                BeginCommodityUpdates();
                Actor.BuffManager.RemoveElement(BuffNames.Lonely);
                bool succeeded = DoLoop();
                bool flag2 = true;
                if (Actor.HasExitReason(ExitReason.HigherPriorityNext))
                {
                    CopyExitReasonToLinkedInteraction();
                    flag2 = OnHigherPriorityNext();
                }
                if (flag2)
                {
                    ForceExitNested = true;
                }
                if (IsMaster && flag2)
                {
                    mCurrentStateMachine.RequestState(null, "ToRelax");
                }
                FinishLinkedInteraction(IsMaster);
                EndCommodityUpdates(succeeded);
                StandardExit(false, false);
                WaitForSyncComplete();
                return succeeded;
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

        public new class Definition : CuddleRelaxing.Definition
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CuddleRelaxingEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                /*
                if (!OccultImaginaryFriend.CanSimGetRomanticWithSim(a, target))
                {
                    return false;
                }
                */
                return true;
            }
        }
    }
}
