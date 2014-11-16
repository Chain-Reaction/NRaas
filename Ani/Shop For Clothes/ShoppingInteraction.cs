using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations.Mimics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace.CAS;
using System.Collections.Generic;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay;
using Sims3.UI.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Tutorial;
using System;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ThoughtBalloons;

namespace ani_ShopForClothes
{
    public class ShoppingInteraction
    {
        // Nested Types
        public class ShopForClothes : Interaction<Sim, SculptureFloorClothingRack2x1>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            bool mTookSemaphore;

            private static float kInteractionTimeLength = 10f;

            public override void ConfigureInteraction()
            {
                TimedStage stage = new TimedStage("CheckSelfOutInMirror", kInteractionTimeLength, false, true, false);
                base.Stages = new List<Stage>(new Stage[] { stage });
            }

            public virtual bool RouteAndAnimate(InteractionInstance interaction, Sim Actor)
            {
                if (!base.Actor.RouteToPointRadialRange(base.Target.Position, 0.5f, 2f))
                {
                    return false;
                }
                interaction.StandardEntry();
                if (Actor.SimDescription.ChildOrAbove)
                {
                    //Think about styling
                    ThoughtBalloonManager.BalloonData balloonData = ThoughtBalloonManager.GetBalloonData("Makeover", base.Actor);
                    balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                    base.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                }
                return true;
            }

            public override void Cleanup()
            {
                if (this.mTookSemaphore)
                {
                    GameStates.ReleaseInteractionStateChangeSemaphore();
                }
                base.Cleanup();
            }

            // Methods
            public override bool Run()
            {
                if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
                {
                    if (!RouteAndAnimate(this, base.Actor))
                    {
                        return false;
                    }

                    if (Sims3.Gameplay.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
                    {
                        base.BeginCommodityUpdates();
                        base.EndCommodityUpdates(true);
                        return true;
                    }
                    this.mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(base.Actor, ~(ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (!this.mTookSemaphore)
                    {
                        base.StandardExit();
                        return false;
                    }
                    //  if (base.Actor.IsSelectable)
                    {
                        while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                        {
                            Simulator.Sleep(0);
                        }
                        base.BeginCommodityUpdates();
                        SimDescription simDescription = base.Actor.SimDescription;
                        if (simDescription == null)
                        {
                            throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                        }
                        CASChangeReporter.Instance.ClearChanges();
                        GameStates.TransitionToCASDresserMode();
                        CASLogic singleton = CASLogic.GetSingleton();
                        bool flag2 = false;

                        try
                        {
                            singleton.LoadSim(simDescription, base.Actor.CurrentOutfitCategory, 0);
                            while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                            {
                                Simulator.Sleep(0);
                            }
                            CASChangeReporter.Instance.SendChangedEvents(base.Actor);
                            CASChangeReporter.Instance.ClearChanges();
                            EventTracker.SendEvent(EventTypeId.kPlannedOutfit, base.Actor, base.Target);
                            flag2 = true;
                            base.Actor.InteractionQueue.CancelAllInteractionsByType(Dresser.ChangeClothes.Singleton);
                        }
                        finally
                        {

                        }
                        base.EndCommodityUpdates(flag2);
                        if (base.Actor.CurrentOutfitIndex > simDescription.GetOutfitCount(base.Actor.CurrentOutfitCategory))
                        {                            
                            base.Actor.UpdateOutfitInfo();
                        }
                        base.Actor.RecreateOccupationOutfits();
                        (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).NotifySimChanged(base.Actor.ObjectId);

                        //Animate View object, and handle the purchases
                        this.AcquireStateMachine("viewobjectinteraction");
                        this.SetActor("x", Actor);
                        this.EnterSim("Enter");
                        this.AnimateSim("1");
                        
                        logic_ClosingDown(this.Actor);

                        return flag2;
                    }

                }

                return false;
            }



            private static void logic_ClosingDown(Sim _sim)
            {
                if (_sim.SimDescription != null)
                {
                    string title = string.Empty;// CommonMethods.LocalizeString("ShoppingDialogTitle", new object[0]);
                    string description = CommonMethods.LocalizeString("ShoppingDialogTitle", new object[0]);
                    string price = StringInputDialog.Show(title, description, string.Empty, true);

                    int parsePrise = 0;
                    int.TryParse(price, out parsePrise);
                    CommonMethods.HandlePayments(parsePrise, _sim.SimDescription);
                }
            }

            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, IGameObject, ShoppingInteraction.ShopForClothes>
            {
                // Methods
                public override string GetInteractionName(Sim a, IGameObject target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("ShopForClothes", new object[0]);
                }

                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (a.CurrentOutfitCategory != OutfitCategories.Career)
                    {
                        return a.SimDescription.IsUsingRegularOutfits;
                    }
                    greyedOutTooltipCallback = delegate
                    {
                        return CommonMethods.LocalizeString("ChangeOutOfCareerOutfit", new object[0]);
                    };
                    return false;
                }
            }
        }

        public class SuggestOutfit : ImmediateInteraction<Sim, SculptureFloorClothingRack2x1>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                Sim selectedObject = base.GetSelectedObject() as Sim;
                CommonMethods.PerformeInteraction(selectedObject, base.Target, ShoppingInteraction.ShopForClothes.Singleton);
                return true;
            }

            // Nested Types
            private sealed class Definition : ImmediateInteractionDefinition<Sim, IGameObject, ShoppingInteraction.SuggestOutfit>
            {
                // Methods
                public override string GetInteractionName(Sim a, IGameObject target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SuggestOutfit", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    GameObject obj = parameters.Target as GameObject;
                    List<Sim> sims = new List<Sim>();
                    foreach (Sim sim2 in obj.LotCurrent.GetSims())
                    {
                        if (sim2 != actor)
                        {
                            sims.Add(sim2);
                        }
                    }
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
    }


}
