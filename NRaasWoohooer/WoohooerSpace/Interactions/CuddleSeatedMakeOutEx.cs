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
    public class CuddleSeatedEx : CuddleSeated, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, CuddleSeated.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public override bool Run()
        {
            try
            {
                SetPriority(new InteractionPriority(InteractionPriorityLevel.Zero));
                Cancelled = false;
                if (!StartSync(ShouldBeMaster(), true))
                {
                    PushRelaxIfHottub();
                    return false;
                }
                if (ShouldBeMaster())
                {
                    //StartSocialContext();
                    Actor.SocialComponent.StartSocializingWith(Target);

                    InitiateSocialUI(Actor, Target);
                    if (mCurrentStateMachine == null)
                    {
                        EnterStateMachine("SeatingCuddle", "EnterSitting", "x", "y");
                        SetActorsAndParameters();
                        AnimateJoinSims("CuddleIdle");
                        FinishSocial("Cuddle", true);
                    }
                    else
                    {
                        string stateName = Actor.HasExitReason() ? "Cuddle" : "CuddleIdle";
                        mCurrentStateMachine.RequestState(null, stateName);
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
                if (ShouldBeMaster() && flag2)
                {
                    mCurrentStateMachine.RequestState(null, "ExitSitting");
                    FinishSocialContext();
                }
                FinishLinkedInteraction(ShouldBeMaster());
                EndCommodityUpdates(succeeded);
                if (flag2)
                {
                    PushRelaxIfHottub();
                }
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

        public new class Definition : CuddleSeated.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CuddleSeatedEx();
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
