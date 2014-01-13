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
    public class CustomizeCollarAndCoatsEx : Sim.CustomizeCollarAndCoats, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.CustomizeCollarAndCoats.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.CustomizeCollarAndCoats.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (Actor.IsHuman)
                {
                    InteractionInstance entry = Singleton.CreateInstance(Target, Target, GetPriority(), false, true);
                    return Target.InteractionQueue.Add(entry);
                }
                if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
                {
                    if (Responder.Instance.OptionsModel.SaveGameInProgress)
                    {
                        return false;
                    }
                    mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(Actor, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (!mTookSemaphore)
                    {
                        return false;
                    }
                    if (Actor.IsSelectable)
                    {
                        while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                        {
                            SpeedTrap.Sleep();
                        }

                        BeginCommodityUpdates();
                        SimDescription simDescription = Actor.SimDescription;
                        if (simDescription == null)
                        {
                            throw new Exception("CustomizeCollarAndCoats: Sim doesn't have a description!");
                        }
                        CASChangeReporter.Instance.ClearChanges();

                        new Sims.Dresser().Perform(new GameHitParameters<GameObject>(Target, Target, GameObjectHit.NoHit));

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            SpeedTrap.Sleep();
                        }

                        EndCommodityUpdates(true);

                        /*
                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                         */
                        return true;
                    }
                }
                return false;
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

        public new class Definition : Sim.CustomizeCollarAndCoats.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CustomizeCollarAndCoatsEx();
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
