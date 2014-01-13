using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Resort;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class PlanResortUniformEx : Sim.PlanResortUniform, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            //Tunings.Inject<Sim, Sim.PlanResortUniform.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.PlanResortUniform.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (IntroTutorial.IsRunning && !IntroTutorial.AreYouExitingTutorial())
                {
                    return false;
                }
                if (GameStates.IsCurrentlySwitchingSubStates)
                {
                    return false;
                }
                Sim target = Target;
                if (target == null)
                {
                    throw new Exception("EditSimInCAS: Cannot edit a non-sim!");
                }

                if (Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }

                mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(Target, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                if (!mTookSemaphore)
                {
                    return false;
                }

                ResortExpenseDialog.SwitchToCAS();

                try
                {
                    new Sims.Stylist(Sims.CASBase.EditType.Uniform).Perform(new GameHitParameters<GameObject>(target, target, GameObjectHit.NoHit));

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep(0);
                    }

                    if (!CASChangeReporter.Instance.CasCancelled)
                    {
                        ResortWorker service = target.Service as ResortWorker;
                        if (service != null)
                        {
                            service.SetIsUsingCustomUniform(target.SimDescription.SimDescriptionId);
                        }
                        else
                        {
                            ResortMaintenance maintenance = target.Service as ResortMaintenance;
                            if (maintenance != null)
                            {
                                ServiceSituation situationOfType = target.GetSituationOfType<ServiceSituation>();
                                if (situationOfType != null)
                                {
                                    maintenance.SetWorkerOutfit(target.SimDescription, situationOfType.Lot.LotId, target.SimDescription.GetOutfit(OutfitCategories.Career, 0));
                                }
                            }
                        }
                    }
                    CASChangeReporter.Instance.ClearChanges();
                }
                finally
                {
                    ResortExpenseDialog.SwitchFromCAS();
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
            }

            return false;
        }

        public new class Definition : Sim.PlanResortUniform.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PlanResortUniformEx();
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
