using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
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
    public class PlanResortUniformObjectEx : ResortStaffedObjectComponent.PlanResortUniform, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            //Tunings.Inject<IGameObject, ResortStaffedObjectComponent.PlanResortUniform.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, ResortStaffedObjectComponent.PlanResortUniform.Definition>(Singleton);
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

                if (Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }

                ResortStaffedObjectComponent resortStaffedObjectComponent = Target.ResortStaffedObjectComponent;
                if (Target.ResortStaffedObjectComponent == null)
                {
                    return false;
                }

                ResortWorker service = resortStaffedObjectComponent.GetService();
                SimDescription assignedWorker = service.GetAssignedWorker(Target);
                if (assignedWorker == null)
                {
                    Sim worker = service.GetWorker(Target);
                    if (worker != null)
                    {
                        assignedWorker = worker.SimDescription;
                    }
                }

                if (assignedWorker == null) return false;

                mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore();
                if (!mTookSemaphore)
                {
                    return false;
                }

                ResortExpenseDialog.SwitchToCAS();

                try
                {
                    new Sims.Stylist(Sims.CASBase.EditType.Uniform).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(assignedWorker), GameObjectHit.NoHit));

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep(0);
                    }

                    if (!CASChangeReporter.Instance.CasCancelled)
                    {
                        service = assignedWorker.Service as ResortWorker;
                        if (service != null)
                        {
                            service.SetIsUsingCustomUniform(assignedWorker.SimDescriptionId);
                        }
                        else
                        {
                            ResortMaintenance maintenance = assignedWorker.Service as ResortMaintenance;
                            if (maintenance != null)
                            {
                                maintenance.SetWorkerOutfit(assignedWorker, Target.LotCurrent.LotId, assignedWorker.GetOutfit(OutfitCategories.Career, 0));
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

        public new class Definition : ResortStaffedObjectComponent.PlanResortUniform.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PlanResortUniformObjectEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IGameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
