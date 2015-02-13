using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Actors;
using Sims3.UI;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping;

[assembly: Tunable]

namespace SellFromInventory
{
    public class AddMenuItem
    {
        [Tunable]
        protected static bool Nuuttis;
        [Tunable]
        protected static int CustomerPrecentage;
        [Tunable]
        protected static bool ShowNotifications;
        [Tunable]
        protected static int Profit;
        [Tunable]
        protected static int ProfitForIngredients;
        [Tunable]
        protected static int ProfitForNoneGrownGoods;
        [Tunable]
        protected static bool ShowBuyFromInventory;
        [Tunable]
        protected static bool ShowBuyGroceries;
        [Tunable]
        protected static int foodPriceBad;
        [Tunable]
        protected static int foodPriceNice;
        [Tunable]
        protected static int foodPriceGreat;
        [Tunable]
        protected static int foodPricePerfect;
        [Tunable]
        protected static int foodPriceDefault;
        [Tunable]
        protected static bool SellIngredientsInGeneralRegister;
        [Tunable]
        protected static bool SellIngredientsInNectarRegister;
        [Tunable]
        protected static bool SellIngredientsInConsignRegister;
        [Tunable]
        protected static int CoffeePrice;
        [Tunable]
        protected static int TotalSipsSitting;
        [Tunable]
        protected static int minSipsSitting;
        [Tunable]
        protected static int maxSipsSitting;
        [Tunable]
        protected static int TotalSipsStanding;
        [Tunable]
        protected static int minSipsStanding;
        [Tunable]
        protected static int maxSipsStanding;      


        public static bool ReturnShowNotifications()
        {
            return ShowNotifications;
        }
               
        public static int ReturnCustomerPrecentage()
        {
            return CustomerPrecentage;
        }

        public static int ReturnProfit()
        {
            return Profit;
        }

        public static int ReturnFoodPriceBad()
        {
            return foodPriceBad;
        }

        public static int ReturnFoodPriceNice()
        {
            return foodPriceNice;
        }

        public static int ReturnFoodPriceGreat()
        {
            return foodPriceGreat;
        }

        public static int ReturnFoodPricePerfect()
        {
            return foodPricePerfect;
        }

        public static int ReturnFoodPriceDefault()
        {
            return foodPriceDefault;
        }

        public static bool ReturnSellIngInConsignRegister()
        {
            return SellIngredientsInConsignRegister;
        }

        public static bool ReturnSellIngInGeneralRegister()
        {
            return SellIngredientsInGeneralRegister;
        }

        public static bool ReturnSellIngInNectarRegister()
        {
            return SellIngredientsInNectarRegister;
        }

        public static int ReturnProfitForIngredients()
        {
            return ProfitForIngredients;
        }

        public static int ReturnProfitForNoneGrownGoods()
        {
            return ProfitForNoneGrownGoods;
        }
                

         public static int ReturnCoffeePrice()
        {
            return CoffeePrice;
        }

         public static int ReturnTotalSipsSitting()
         {
             return TotalSipsSitting;
         }

         public static int ReturnTotalSipsStanding()
         {
             return TotalSipsStanding;
         }


        // Methods
        static AddMenuItem()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(AddMenuItem.OnWorldLoadFinishedHandler);
            World.sOnLotAddedEventHandler += new EventHandler(AddMenuItem.OnWorldLoadFinishedHandler);
            World.OnObjectPlacedInLotEventHandler += new EventHandler(AddMenuItem.OnWorldLoadFinishedHandler);

        }

        public static void AddInteractions(GameObject obj)
        {
            if (obj != null && obj.Interactions != null)
            {
                //Show the interactions only when at home
                WorldType type = GameUtils.GetCurrentWorldType();
                if (type != WorldType.Vacation)
                {
                   // InteractionObjectPair i = new InteractionObjectPair(RegisterInteraction.ShopFromInventory.Singleton, obj);
                    InteractionObjectPair i = new InteractionObjectPair(ShoppingRegister.Buy.Singleton, obj);
                    if (obj.Interactions.Contains(i))
                    {
                        //StyledNotification.Show(new StyledNotification.Format(obj.GetType().ToString(), StyledNotification.NotificationStyle.kGameMessageNegative));
                        //Replace the original buyfood interaction
                        if (obj.GetType() == typeof(FoodStoreRegister))
                        {
                            obj.RemoveInteractionByType(ShoppingRegister.Buy.Singleton);
                            obj.RemoveInteractionByType(ShoppingRegister.Buy_Food.Singleton);

                            obj.AddInteraction(OverridedFoodRegister.Buy.Singleton);
                            obj.AddInteraction(OverridedFoodRegister.CallForMeal.Singleton);
                            obj.AddInteraction(OverridedFoodRegisterServing.Buy.Singleton);

                            obj.AddInteraction(OverridedFoodRegister.CallForCoffee.Singleton);
                            obj.AddInteraction(OverridedFoodRegister.BuyConcessionDrink.Singleton);
                            obj.AddInteraction(OverridedFoodRegister.BuyCupOffCoffee.Singleton);

                            if (ShowBuyGroceries)
                            {
                                obj.AddInteraction(OverridedRegister.Buy.Singleton);
                            }
                        }
                        else if (obj.GetType() == typeof(ConcessionsStand))
                        {
                            //obj.RemoveInteractionByType(ConcessionsStand.Buy_FoodFromConcessionsStand.Singleton);
                            //obj.AddInteraction(OverridedBuyFoodFromConcessionsStand.SingletonConcessionsStand);
                        }
                        else
                        {
                            //Replace the original buy interaction      
                            obj.RemoveInteractionByType(ShoppingRegister.Buy.Singleton);
                            obj.AddInteraction(OverridedRegister.Buy.Singleton);
                        }

                        if (ShowBuyFromInventory)
                        {
                            obj.AddInteraction(RegisterInteraction.ShopFromInventory.Singleton);
                        }
                    }
                }
                else
                {
                    //Call to meal should also be seen abroad.
                    if (obj.GetType() == typeof(FoodStoreRegister))
                    {
                        InteractionObjectPair i2 = new InteractionObjectPair(OverridedFoodRegister.CallForMeal.Singleton, obj);
                        if (!obj.Interactions.Contains(i2))
                            obj.AddInteraction(OverridedFoodRegister.CallForMeal.Singleton);
                    }
                }
            }
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            List<ShoppingRegister> cList = new List<ShoppingRegister>(Sims3.Gameplay.Queries.GetObjects<ShoppingRegister>());
            foreach (ShoppingRegister r in cList)
            {
                AddInteractions(r);
            }

            //Add the interaction when buying a new object 
            EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(AddMenuItem.OnNewObject));


        }

        protected static ListenerAction OnNewObject(Event e)
        {
            ShoppingRegister targetObject = e.TargetObject as ShoppingRegister;
            if (targetObject != null)
            {
                AddInteractions(targetObject);
            }

            // OnBroughtObject(e);
            return ListenerAction.Keep;
        }
    }
}
