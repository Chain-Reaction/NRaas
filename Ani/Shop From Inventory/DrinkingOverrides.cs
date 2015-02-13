using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.ActorSystems;
using Sims3.UI;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace.Enums;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.Seating;

namespace SellFromInventory
{
    public class DrinkHeldCup : EatHeldFood.Definition
    {
        public static DrinkHeldCup Singleton = new DrinkHeldCup();

        public override string GetInteractionName(Sim a, IEdible target, InteractionObjectPair interaction)
        {

            return "Drinking coffee";
        }
        public override bool Test(Sim a, IEdible target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return true;
        }
    }

    public static class CupOverrides
    {
        public static bool PushDrinkAsContinuation(Sim sim, ConcessionsStand.ConcessionsBeverage g)
        {
            GameObject chair = FindClosestChair(sim);

            Posture posture = new Glass.CarryingGlassPosture(sim, g);
            if (sim.Posture is Sim.StandingPosture)
            {
                sim.Posture = posture;
            }
            else
            {
                posture.PreviousPosture = sim.Posture.PreviousPosture;
                sim.Posture.PreviousPosture = posture;
            }
             
          
            //if (chair != null)
            //{
            //    foreach (InteractionObjectPair item in chair.Interactions)
            //    {
            //        //Get the sit interaction
            //        if (item.ToString().ToLower().Contains(".sit+definition"))
            //        {
            //            g.mTotalSips = AddMenuItem.ReturnTotalSipsSitting();
            //            g.mAvailableSipsLeft = g.mTotalSips;
            //            sim.InteractionQueue.PushAsContinuation(Sims3.Gameplay.InteractionsShared.Sit.Singleton, chair, true, new InteractionPriority(InteractionPriorityLevel.UserDirected), true);
            //            sim.InteractionQueue.PushAsContinuation(Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup.DrinkHeldCup.Singleton, g, true, new InteractionPriority(InteractionPriorityLevel.UserDirected), true);
            //        }
            //    }
            //}
            //else
            {
                try
                {
                    g.mTotalSips = AddMenuItem.ReturnTotalSipsStanding();
                    g.mAvailableSipsLeft = g.mTotalSips;
                    sim.InteractionQueue.PushAsContinuation(Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup.DrinkHeldCup.Singleton, g, true, new InteractionPriority(InteractionPriorityLevel.UserDirected), true);

                }
                catch (Exception ex)
                {
                    StyledNotification.Show(new StyledNotification.Format(ex.Message, StyledNotification.NotificationStyle.kGameMessageNegative));
                }
               // sim.InteractionQueue.PushAsContinuation(Glass.Drink.Singleton, g, true);
            }


            return true;
        }

        private static GameObject FindClosestChair(Sim sim)
        {
            GameObject chair = null;

            List<GameObject> objects = sim.LotCurrent.GetObjectsInRoom<GameObject>(sim.RoomId);

            foreach (var item in objects)
            {
                if (item is ISittable)
                {
                   // StyledNotification.Show(new StyledNotification.Format(item.CatalogName + " " + ((ISittable)item).BeingUsed, StyledNotification.NotificationStyle.kGameMessagePositive));

                    //Is the chair free
                    if (!((ISittable)item).BeingUsed)
                    {
                        chair = item;
                        break;
                    }

                }
            }

            return chair;
        }


        public static EatingPosture GetPostureParam(Sim sim)
        {
            IEdible target = (IEdible)RandomUtil.GetRandomObjectFromList(Recipe.Recipes);

            if (sim.Posture.Satisfies(CommodityKind.Standing, target))
            {
                return EatingPosture.standing;
            }
            if (sim.Posture.Satisfies(CommodityKind.Sitting, target))
            {
                SitData sitData = null;
                SittingPosture sittingPosture = sim.Posture as SittingPosture;
                if (sittingPosture != null)
                {
                    sitData = sittingPosture.Part.Target;
                }
                else
                {
                    ISittable sittable = SittingHelpers.CastToSittable(sim.Posture.Container as GameObject);
                    if (sittable != null)
                    {
                        PartData partSimIsIn = sittable.PartComponent.GetPartSimIsIn(sim);
                        sitData = (partSimIsIn as SitData);
                    }
                }
                if (sitData != null)
                {
                    switch (sitData.SitStyle)
                    {
                        case SitStyle.Dining:
                            {
                                if (target.Parent != sim)
                                {
                                    return EatingPosture.diningIn;
                                }
                                return EatingPosture.diningOut;
                            }
                        case SitStyle.Living:
                            {
                                if (target is HotBeverageMachine.Cup)
                                {
                                    return EatingPosture.living;
                                }
                                if (target.Parent != sim)
                                {
                                    return EatingPosture.diningIn;
                                }
                                return EatingPosture.diningOut;
                            }
                        case SitStyle.Stool:
                            {
                                if (target.Parent != sim)
                                {
                                    return EatingPosture.barstoolIn;
                                }
                                return EatingPosture.barstoolOut;
                            }
                    }
                }
                return EatingPosture.diningOut;
            }
            return EatingPosture.standing;
        }
    }


}
