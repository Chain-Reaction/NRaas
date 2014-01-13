using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
    public class CreateAnOutfitEx : Dresser.CreateAnOutfit, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Dresser, Dresser.CreateAnOutfit.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Dresser, Dresser.CreateAnOutfit.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
                {
                    if (!Target.RouteAndOpenDrawer(this, Actor))
                    {
                        return false;
                    }
                    if (Responder.Instance.OptionsModel.SaveGameInProgress)
                    {
                        BeginCommodityUpdates();
                        bool succeeded = Target.CloseDrawerAndExit(this, Actor);
                        EndCommodityUpdates(succeeded);
                        return succeeded;
                    }

                    mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(Actor, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (!mTookSemaphore)
                    {
                        StandardExit();
                        return false;
                    }

                    if (Actor.IsSelectable)
                    {
                        while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                        {
                            SpeedTrap.Sleep();
                        }

                        BeginCommodityUpdates();

                        new Sims.Dresser().Perform(new GameHitParameters<GameObject>(Actor, Actor, GameObjectHit.NoHit));

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            SpeedTrap.Sleep();
                        }

                        bool flag2 = Target.CloseDrawerAndExit(this, Actor);
                        Actor.InteractionQueue.CancelAllInteractionsByType(Dresser.ChangeClothes.Singleton);

                        EndCommodityUpdates(flag2);

                        /*
                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                         */
                        return flag2;
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

        public new class Definition : Dresser.CreateAnOutfit.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CreateAnOutfitEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Dresser target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Dresser target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if ((a.CurrentOutfitCategory == OutfitCategories.SkinnyDippingTowel) && (a.BuffManager.HasElement(BuffNames.EmbarrassedClothesHidden)))
                    {
                        greyedOutTooltipCallback = Dresser.ChangeClothes.ClothesHiddenCallback;
                        return false;
                    }

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
