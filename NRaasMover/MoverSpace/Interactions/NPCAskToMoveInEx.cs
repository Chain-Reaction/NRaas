using NRaas.CommonSpace.Helpers;
using NRaas.MoverSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Interactions
{
    public class NPCAskToMoveInEx : NPCAskToMoveIn, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Sim, NPCAskToMoveIn.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, NPCAskToMoveIn.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                bool success = false;
                Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;

                if (BeginSocialInteraction(new SocialInteractionB.Definition(null, Localization.LocalizeString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:BeAskedToMoveIn", new object[0x0]), false), false, true))
                {
                    BeginCommodityUpdates();
                    mCurrentStateMachine = StateMachineClient.Acquire(Simulator.CurrentTask, "social_AskFor");
                    mCurrentStateMachine.SetActor("x", Actor);
                    mCurrentStateMachine.SetActor("y", Target);
                    mCurrentStateMachine.EnterState("x", "Enter");
                    mCurrentStateMachine.EnterState("y", "Enter");
                    if (AcceptCancelDialog.Show(Localization.LocalizeString("Gameplay/Moving:MoveInPrompt", new object[] { Target, Actor })))
                    {
                        mCurrentStateMachine.RequestState(false, "y", "Friendly");
                        mCurrentStateMachine.RequestState(true, "x", "Friendly");
                        if (!MovingSituation.MovingInProgress)
                        {
                            // Custom
                            using (GameplayMovingModelEx.ProtectFunds protect = new GameplayMovingModelEx.ProtectFunds(Target.Household))
                            {
                                MovingDialogEx.Show(new GameplayMovingModelEx(Target, Actor.Household));
                            }
                        }
                    }
                    else
                    {
                        mCurrentStateMachine.RequestState(false, "y", "Neutral");
                        mCurrentStateMachine.RequestState(true, "x", "Neutral");
                        Actor.SocialComponent.OnMoveInRequestRejected();
                    }

                    if (Actor.Household != Target.Household)
                    {
                        Actor.SocialComponent.OnMoveInRequestRejected();
                    }

                    success = true;
                    EndCommodityUpdates(true);
                }

                Actor.ClearSynchronizationData();
                return success;
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

        public new class Definition : NPCAskToMoveIn.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new NPCAskToMoveInEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
