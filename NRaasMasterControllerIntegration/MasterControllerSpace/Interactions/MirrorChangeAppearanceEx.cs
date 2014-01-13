using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class MirrorChangeAppearanceEx : Mirror.ChangeAppearance, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<IMirror, Mirror.ChangeAppearance.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Mirror, Mirror.ChangeAppearance.Definition>(Singleton);
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
                            SpeedTrap.Sleep();
                        }

                        int careerOutfitIndex = Actor.SimDescription.CareerOutfitIndex;
                        if (Actor.CurrentOutfitCategory == OutfitCategories.Career)
                        {
                            ArrayList outfits = Actor.SimDescription.GetOutfits(Actor.CurrentOutfitCategory);
                            for (int i = 0; i < outfits.Count; i++)
                            {
                                if (Actor.CurrentOutfit.Key == (outfits[i] as SimOutfit).Key)
                                {
                                    Actor.SimDescription.CareerOutfitIndex = i;
                                    break;
                                }
                            }
                        }

                        new Sims.Mirror().Perform(new GameHitParameters<GameObject>(Actor, Actor, GameObjectHit.NoHit));

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            SpeedTrap.Sleep();
                        }

                        AnimateSim("NodAsApproval");
                        AnimateSim("LeaveMirror");

                        if (Actor.CurrentOutfitCategory == OutfitCategories.Career)
                        {
                            Actor.SimDescription.CareerOutfitIndex = careerOutfitIndex;
                        }

                        StandardExit();
                        
                        /*
                        Actor.RecreateOccupationOutfits();
                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                        */
                        return true;
                    }

                    StandardExit();
                }
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

        public new class Definition : Mirror.ChangeAppearance.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new MirrorChangeAppearanceEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IMirror target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
            
            public override bool Test(Sim a, IMirror target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.CurrentOutfitCategory == OutfitCategories.Singed) return false;

                    if (a.CurrentOutfitCategory == OutfitCategories.SkinnyDippingTowel) return false;

                    return Sims.CASBase.PublicAllow(a.SimDescription, ref greyedOutTooltipCallback);
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
