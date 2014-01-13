using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class ClothingPedestalPurchaseCustomOutfitEx : ClothingPedestal.PurchaseCustomOutfit, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ClothingPedestal, ClothingPedestal.PurchaseCustomOutfit.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ClothingPedestal, ClothingPedestal.PurchaseCustomOutfit.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
                {
                    bool flag = true;
                    if (!Target.RouteToPedestal(Actor, true))
                    {
                        return false;
                    }

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
                            SpeedTrap.Sleep(0x0);
                        }

                        StandardEntry();
                        BeginCommodityUpdates();
                        EnterStateMachine("ShoppingPedestal", "Enter", "x");
                        SetParameter("x:Age", Actor.SimDescription.Age);
                        AnimateSim("Change Item");

                        SimDescription simDesc = Actor.SimDescription;
                        if (Actor.FamilyFunds >= ClothingPedestal.kCostToPlanPurchaseOutfit)
                        {
                            simDesc = Actor.SimDescription;
                            if (simDesc == null)
                            {
                                throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                            }

                            if (GameUtils.IsInstalled(ProductVersion.EP2))
                            {
                                new Sims.Stylist(Sims.CASBase.EditType.None, Target.DisplayCategory).Perform(new GameHitParameters<GameObject>(Actor, Actor, GameObjectHit.NoHit));
                            }
                            else
                            {
                                new Sims.Dresser(Sims.CASBase.EditType.None, Target.DisplayCategory).Perform(new GameHitParameters<GameObject>(Actor, Actor, GameObjectHit.NoHit));
                            }

                            while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                            {
                                SpeedTrap.Sleep(0x0);
                            }

                            if (!CASChangeReporter.Instance.CasCancelled && !Actor.IsOnHomeLot(Target))
                            {
                                simDesc.Household.ModifyFamilyFunds(-ClothingPedestal.kCostToPlanPurchaseOutfit);
                            }
                        }

                        AnimateSim("Exit");
                        EndCommodityUpdates(true);
                        StandardExit();
                        if (Actor.CurrentOutfitIndex > simDesc.GetOutfitCount(Actor.CurrentOutfitCategory))
                        {
                            Actor.UpdateOutfitInfo();
                        }
                        Actor.RecreateOccupationOutfits();
                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                        EventTracker.SendEvent(EventTypeId.kBoughtOutfitFromPedestal, Actor, Target);
                        return flag;
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

        public new class Definition : ClothingPedestal.PurchaseCustomOutfit.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ClothingPedestalPurchaseCustomOutfitEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ClothingPedestal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
