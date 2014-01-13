using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class ChangeScaleColorEx : Mirror.ChangeScaleColor, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<IMirror, Mirror.ChangeScaleColor.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<IMirror, Mirror.ChangeScaleColor.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (Target.Route(this))
                {
                    StandardEntry();
                    Mirror.EnterStateMachine(this);
                    AnimateSim("ChangeAppearance");
                    StartStages();
                    if (Responder.Instance.OptionsModel.SaveGameInProgress)
                    {
                        StandardExit();
                        return false;
                    }

                    mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(Actor, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (!mTookSemaphore)
                    {
                        StandardExit();
                        return false;
                    }

                    if (!Actor.IsSelectable)
                    {
                        StandardExit();
                        return false;
                    }

                    if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
                    {
                        while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                        {
                            SpeedTrap.Sleep(0x0);
                        }

                        int careerOutfitIndex = Actor.SimDescription.CareerOutfitIndex;
                        if (Actor.CurrentOutfitCategory == OutfitCategories.Career)
                        {
                            ArrayList outfits = Actor.SimDescription.GetOutfits(Actor.CurrentOutfitCategory);
                            for (int i = 0x0; i < outfits.Count; i++)
                            {
                                if (Actor.CurrentOutfit.Key == (outfits[i] as SimOutfit).Key)
                                {
                                    Actor.SimDescription.CareerOutfitIndex = i;
                                    break;
                                }
                            }
                        }

                        CASChangeReporter.Instance.ClearChanges();

                        new Sims.Basic.ChangeScaleColor().Perform(new GameHitParameters<GameObject>(Actor, Actor, GameObjectHit.NoHit));

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            SpeedTrap.Sleep();
                        }

                        CASChangeReporter.Instance.SendChangedEvents(Actor);
                        CASChangeReporter.Instance.ClearChanges();
                        AnimateSim("NodAsApproval");
                        AnimateSim("LeaveMirror");

                        if (Actor.CurrentOutfitCategory == OutfitCategories.Career)
                        {
                            Actor.SimDescription.CareerOutfitIndex = careerOutfitIndex;
                        }

                        StandardExit();
                        Actor.RecreateOccupationOutfits();
                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                        return true;
                    }
                    StandardExit();
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

        public new class Definition : Mirror.ChangeScaleColor.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ChangeScaleColorEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IMirror target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, IMirror target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!a.SimDescription.IsMermaid) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
