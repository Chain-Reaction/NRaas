using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class PlanOutfitEx : Sim.PlanOutfit, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.PlanOutfit.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                mCurrentStateMachine = StateMachineClient.Acquire(Target, "Mirror", AnimationPriority.kAPDefault);
                mCurrentStateMachine.SetActor("x", Target);
                mCurrentStateMachine.EnterState("x", "StepInFrontOfMirror");
                AnimateSim("ChangeAppearance");

                new Sims.Dresser().Perform(new GameHitParameters<GameObject>(Target, Target, GameObjectHit.NoHit));

                while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                {
                    SpeedTrap.Sleep();
                }

                AnimateSim("NodAsApproval");
                AnimateSim("LeaveMirror");

                /*
                (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Target.ObjectId);
                */
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return false;
        }

        public new class Definition : Sim.PlanOutfit.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PlanOutfitEx();
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
