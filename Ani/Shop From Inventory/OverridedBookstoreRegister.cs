using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.UI.Hud;
using Sims3.Gameplay.CAS;
using SellFromInventory;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.SimIFace.Enums;
using Sims3.Gameplay.Seasons;

namespace SellFromInventory
{
    #region Buy Food - serving plate
    /// <summary>
    /// Buy food - serving plate
    /// </summary>
    public class OverridedBuyFoodServingWithRegister : ShoppingRegister.BuyFoodWithRegister
    {
        // Fields
        private static int householdFunds;

        public static readonly BuyItemsDefinition BuyItemsSingleton = new BuyItemsDefinition();

        public override bool Run()
        {
            bool success = base.Run();

            //Pay lot owner
            WorldType type = GameUtils.GetCurrentWorldType();
            if (type != WorldType.Vacation)
            {
                bool processPurchase = success;

                //Don't process for the cashier
                if (!CommonMethods.IsSociable(base.Actor))
                    processPurchase = false;

                //Pay the lot owner
                if (processPurchase)
                {
                    int price = (int)((householdFunds - base.Actor.Household.FamilyFunds) * (new decimal(AddMenuItem.ReturnProfit()) / 100));
                    if (price <= 0)
                    {
                        price = 5;
                    }

                    CommonMethods.PayLotOwner(base.Actor, base.Target.LotCurrent, price);
                    householdFunds = 0;

                }
            }

            return success;

        }

        // Methods
        public override void PostAnimation()
        {
            householdFunds = base.Actor.Household.FamilyFunds;
            CommonMethods.ShowCustomShoppingDialog(base.Actor, this.mRegister, ItemDictionaryFoodServing());
        }

        public Dictionary<string, List<StoreItem>> ItemDictionaryFoodServing()
        {
            string food_serving = "food_serving";

            Dictionary<string, List<StoreItem>> servings = new Dictionary<string, List<StoreItem>>();
            servings.Add(food_serving, new List<StoreItem>());

            foreach (Recipe recipe in Recipe.Recipes)
            {
                string name = base.Actor.TraitManager.HasElement(TraitNames.Vegetarian) ? recipe.GenericVegetarianName : recipe.GenericName;
                List<WorldName> allowedWorlds = null;
                if (recipe.RequiredWorld != WorldName.Undefined)
                {
                    allowedWorlds = new List<WorldName>();
                    allowedWorlds.Add(recipe.RequiredWorld);
                }
                StoreItem item = null;

                float recipePrice = ((float)recipe.RegisterValue > 0 ? (float)recipe.RegisterValue : 10);

                if (!recipe.IsSnack)
                {
                    item = new StoreItem(name, recipePrice * 24, recipe, recipe.GetThumbnailKey(ThumbnailSize.Medium), "recipe_" + recipe.Key, new CreateObjectCallback(this.CreateServingCallback), new ProcessObjectCallback(ShoppingRabbitHole.ProcessObject), allowedWorlds);
                }
                else
                {
                    item = new StoreItem(name, recipePrice, recipe, recipe.GetThumbnailKey(ThumbnailSize.Medium), "recipe_" + recipe.Key, new CreateObjectCallback(this.CreateSnackCallback), new ProcessObjectCallback(ShoppingRabbitHole.ProcessObject), allowedWorlds);
                }

                //Skip cone ice-cream
                if (!servings[food_serving].Contains(item) && !CommonMethods.IsRecipeIceCream(recipe))
                    servings[food_serving].Add(item);
            }


            return servings;
        }

        public ObjectGuid CreateServingCallback(object customData, ref Simulator.ObjectInitParameters initData, Quality quality)
        {
            IFoodContainer container = (customData as Recipe).CreateFinishedFood(Recipe.MealQuantity.Group, Quality.Nice);
            if (container != null)
            {
                return container.ObjectId;
            }

            return ObjectGuid.InvalidObjectGuid;
        }

        public ObjectGuid CreateSnackCallback(object customData, ref Simulator.ObjectInitParameters initData, Quality quality)
        {
            Recipe recipe = (customData as Recipe);

            if (recipe != null)
            {
                IFoodContainer container = (customData as Recipe).CreateFinishedFood(Recipe.MealQuantity.Single, Quality.Outstanding);
                if (container != null)
                {
                    return container.ObjectId;
                }

            }
            return ObjectGuid.InvalidObjectGuid;
        }




        // Nested Types
        public class BuyItemsDefinition : InteractionDefinition<Sim, Sim, OverridedBuyFoodServingWithRegister>
        {
            // Methods

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return CommonMethods.LocalizeString("BuyFoodServing", new object[] { });
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return ShoppingRegister.TestIfSocialValid(a, target);
            }
        }
    }

    /// <summary>
    /// Bookstore
    /// </summary>
    public class OverridedFoodRegisterServing : FoodStoreRegister
    {
        public new class Buy : Interaction<Sim, FoodStoreRegister>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                Sim simInRole = base.Target.CurrentRole.SimInRole;
                InteractionInstance instance = OverridedBuyFoodServingWithRegister.BuyItemsSingleton.CreateInstance(simInRole, base.Actor, base.Actor.InheritedPriority(), base.Autonomous, base.CancellableByPlayer);
                base.Actor.InteractionQueue.PushAsContinuation(instance, true);

                return true;
            }

            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, FoodStoreRegister, OverridedFoodRegisterServing.Buy>
            {
                public override string GetInteractionName(Sim actor, FoodStoreRegister target, InteractionObjectPair iop)
                {
                    return CommonMethods.LocalizeString("BuyFoodServing", new object[] { });
                }

                // Methods
                public override bool Test(Sim a, FoodStoreRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.InteractionTestAvailability();
                }
            }
        }
    }
    #endregion

    #region Buy Food - Single plate

    #region Single plate
    /// <summary>
    /// Buy food - single plate
    /// </summary>
    public class OverridedBuyFoodWithRegister : ShoppingRegister.BuyFoodWithRegister
    {
        // Fields
        private static int householdFunds;

        public static readonly BuyItemsDefinition BuyItemsSingleton = new BuyItemsDefinition();

        public override bool Run()
        {
            bool success = base.Run();
            WorldType type = GameUtils.GetCurrentWorldType();

            bool processPurchase = success;

            //Don't process for the cashier
            if (!CommonMethods.IsSociable(base.Actor))
                processPurchase = false;

            //Pay the lot owner
            if (processPurchase && success)
            {
                int price = (int)((householdFunds - base.Actor.Household.FamilyFunds) * (new decimal(AddMenuItem.ReturnProfit()) / 100));
                if (price <= 0)
                {
                    price = 5;
                }
                //No lot owners abroad
                if (type != WorldType.Vacation)
                {
                    CommonMethods.PayLotOwner(base.Actor, base.Target.LotCurrent, price);
                }
                householdFunds = 0;

                //if this happened through "Call for meal"
                //Removing the price from the active buying Sim needs to be done manually
                if (base.Autonomous)
                {
                    base.Actor.ModifyFunds(-price);
                }

            }

            return success;

        }

        // Methods
        public override void PostAnimation()
        {
            householdFunds = base.Actor.Household.FamilyFunds;
            base.mRegister.BuyAndEatFood(base.Actor, base.Autonomous);
        }

        // Nested Types
        public class BuyItemsDefinition : InteractionDefinition<Sim, Sim, OverridedBuyFoodWithRegister>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return ShoppingRegister.TestIfSocialValid(a, target);
            }
        }
    }
    #endregion

    #region Buy Concession stand drink
    /// <summary>
    /// Buy Coffee
    /// </summary>
    public class BuyConcessionStandDrink : ShoppingRegister.BuyFoodWithRegister
    {
        // Fields
        private static int householdFunds;

        public static readonly BuyItemsDefinition BuyCoffeeSingleton = new BuyItemsDefinition();

        public override bool Run()
        {
            bool success = base.Run();
            WorldType type = GameUtils.GetCurrentWorldType();

            bool processPurchase = success;

            //Don't process for the cashier
            if (!CommonMethods.IsSociable(base.Actor))
                processPurchase = false;

            //Pay the lot owner
            if (processPurchase && success && base.Actor.IsHoldingAnything())
            {
                int price = AddMenuItem.ReturnCoffeePrice();
                //No lot owners abroad
                if (type != WorldType.Vacation)
                {
                    CommonMethods.PayLotOwner(base.Actor, base.Target.LotCurrent, price);
                }
                householdFunds = 0;

                //if this happened through "Call for meal"
                //Removing the price from the active buying Sim needs to be done manually
                base.Actor.ModifyFunds(-price);
            }

            return success;

        }

        // Methods
        public override void PostAnimation()
        {
            householdFunds = base.Actor.Household.FamilyFunds;
            BuyAndEatConcessionsFood(base.Actor, base.Autonomous);
        }

        public bool BuyAndEatConcessionsFood(Sim sim, bool autonomous)
        {
            bool result = false;
            ConcessionsStand.BaseFoodData foodData = ShowBuyConcessionsFoodDialog(autonomous);
            
            if (foodData != null)
            {
                CommonMethods.ShowMessage("Food data is not null");
                result = CreateFoodAndPushConsumeOnSim(foodData, sim);

            }
            return result;
        }

        public static BaseFoodStand.BaseFoodStandBeverage CreateBeverage(bool isHotBeverage, Sim sim)
        {
            string instanceName = isHotBeverage ? "beverageCupHot" : "beverageCupCold";
            return GlobalFunctions.CreateObject(instanceName, ProductVersion.EP8, sim.Position, 1, Vector3.UnitZ, null, null) as ConcessionsStand.ConcessionsBeverage;
        }

        public static bool CreateFoodAndPushConsumeOnSim(ConcessionsStand.BaseFoodData baseFoodData, Sim sim)
        {
            CommonMethods.ShowMessage(baseFoodData.mFoodType.ToString() + " " + baseFoodData);
            bool result = true;
            switch (baseFoodData.mFoodType)
            {

                case ConcessionsStand.FoodType.HotBeverage:
                case ConcessionsStand.FoodType.ColdBeverage:
                    {

                        BaseFoodStand.BaseFoodStandBeverage baseFoodStandBeverage = CreateBeverage(baseFoodData.FoodType == BaseFoodStand.FoodType.HotBeverage, sim);
                        if (baseFoodStandBeverage != null)
                        {
                            BaseFoodStand.FoodType foodType = baseFoodData.FoodType;
                            baseFoodStandBeverage.IsColdResortDrink = (foodType == BaseFoodStand.FoodType.ColdBeverage);
                            float tempChangePerSip = (foodType == BaseFoodStand.FoodType.HotBeverage) ? ConcessionsStand.kTempChangePerSipHotDrink : ConcessionsStand.kTempChangePerSipColdDrink;
                            baseFoodStandBeverage.InitData(baseFoodData.DrinkNameLocKey, baseFoodData.BevFoodUnits, baseFoodData.BuffToAdd, tempChangePerSip);
                            baseFoodData = null;
                            baseFoodStandBeverage.SetOpacity(0f, 0f);
                            if (sim.ParentToRightHand(baseFoodStandBeverage))
                            {
                                CarrySystem.EnterWhileHolding(sim, baseFoodStandBeverage);
                                baseFoodStandBeverage.FadeIn(true);
                                sim.Wander(ConcessionsStand.kMinMaxWanderBeforeDrink[0], ConcessionsStand.kMinMaxWanderBeforeDrink[1], false, RouteDistancePreference.NoPreference, false);
                                result = baseFoodStandBeverage.PushDrinkAsContinuation(sim);
                            }
                            else
                            {
                                baseFoodStandBeverage.Destroy();
                                result = false;
                            }
                        }
                        break;

                        //string instanceName = (selectedFood.mFoodType == ConcessionsStand.FoodType.HotBeverage) ? "beverageCupHot" : "beverageCupCold";
                        //ConcessionsStand.ConcessionsBeverage concessionsBeverage = GlobalFunctions.CreateObject(instanceName, ProductVersion.EP8, sim.Position, 1, Vector3.UnitZ, null, null) as ConcessionsStand.ConcessionsBeverage;
                        //if (concessionsBeverage != null)
                        //{
                        //    float tempChangePerSip = (selectedFood.mFoodType == BaseFoodStand.FoodType.HotBeverage) ? ConcessionsStand.kTempChangePerSipHotDrink : ConcessionsStand.kTempChangePerSipColdDrink;
                        //    concessionsBeverage.InitData(selectedFood.DrinkNameLocKey, selectedFood.BevFoodUnits, selectedFood.BuffToAdd, tempChangePerSip);
                        //  //  concessionsBeverage.InitData(selectedFood.mFoodType, selectedFood.mDrinkNameLocKey, selectedFood.mBevFoodUnits, selectedFood.mBuffToAdd);
                        //    concessionsBeverage.SetOpacity(0f, 0f);
                        //    if (sim.ParentToRightHand(concessionsBeverage))
                        //    {
                        //        CarrySystem.EnterWhileHolding(sim, concessionsBeverage);
                        //        concessionsBeverage.FadeIn(true);
                        //        sim.Wander(ConcessionsStand.kMinMaxWanderBeforeDrink[0], ConcessionsStand.kMinMaxWanderBeforeDrink[1], false, RouteDistancePreference.NoPreference, false);
                        //        result = concessionsBeverage.PushDrinkAsContinuation(sim);
                        //    }
                        //    else
                        //    {
                        //        concessionsBeverage.Destroy();
                        //        result = false;
                        //    }
                        //}
                        //break;
                    }
            }
            return result;
        }

        public void BuildThumbKeysIfNeeded()
        {
            if (ConcessionsStand.sHotBevThumbKey == ThumbnailKey.kInvalidThumbnailKey)
            {
                ConcessionsStand.sHotBevThumbKey = new ThumbnailKey(UIUtils.GetModelResourceKey("beverageCupHot", ProductVersion.EP8), ThumbnailSize.Medium);
            }
            if (ConcessionsStand.sColdBevThumbKey == ThumbnailKey.kInvalidThumbnailKey)
            {
                ConcessionsStand.sColdBevThumbKey = new ThumbnailKey(UIUtils.GetModelResourceKey("beverageCupCold", ProductVersion.EP8), ThumbnailSize.Medium);
            }
        }

        public List<ushort> GetAvailibleFoodGuids(BaseFoodStand.FoodType foodType, Sim sim)
        {
            List<ushort> list = new List<ushort>();
            Season season = SeasonsManager.Enabled ? SeasonsManager.CurrentSeason : Season.Summer;
            using (Dictionary<ushort, BaseFoodStand.BaseFoodData>.ValueCollection.Enumerator enumerator = ConcessionsStand.SeasonalFoodData.sSeasonalFoodData.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ConcessionsStand.SeasonalFoodData seasonalFoodData = (ConcessionsStand.SeasonalFoodData)enumerator.Current;
                    if ((foodType == BaseFoodStand.FoodType.None || seasonalFoodData.FoodType == foodType) && (byte)(seasonalFoodData.AvailableSeasons & season) > 0)
                    {
                        list.Add(seasonalFoodData.Guid);
                    }
                }
            }
            return list;
        }

        public ThumbnailKey GetThumbnailKeyForDrink(bool isHotBeverage)
        {
            BuildThumbKeysIfNeeded();

            if (!isHotBeverage)
            {
                return ConcessionsStand.sColdBevThumbKey;
            }
            return ConcessionsStand.sHotBevThumbKey;
        }

        public BaseFoodStand.BaseFoodData ShowBuyConcessionsFoodDialog(bool autonomous)
        {
            List<ushort> availibleFoodGuids = GetAvailibleFoodGuids(BaseFoodStand.FoodType.None, base.Actor);

            ThumbnailKey thumbnail = ThumbnailKey.kInvalidThumbnailKey;
            string text = string.Empty;
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            List<BaseFoodStand.BaseFoodData> foods = new List<BaseFoodStand.BaseFoodData>();
            foreach (ushort current in availibleFoodGuids)
            {

                BaseFoodStand.BaseFoodData baseFoodData = null;
                if (ConcessionsStand.SeasonalFoodData.sSeasonalFoodData.TryGetValue(current, out baseFoodData))
                {
                    List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                    bool addToList = false;
                    switch (baseFoodData.FoodType)
                    {

                        case ConcessionsStand.FoodType.HotBeverage:
                            {
                                thumbnail = GetThumbnailKeyForDrink(baseFoodData.FoodType == BaseFoodStand.FoodType.HotBeverage);
                                text = Localization.LocalizeString(ConcessionsStand.sBaseBeverageLocKey + baseFoodData.DrinkNameLocKey, new object[0]);
                                addToList = true;
                                foods.Add(baseFoodData);
                                break;
                            }
                    }
                    if (addToList)
                    {
                        list2.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, text));
                        int num = AddMenuItem.ReturnCoffeePrice();

                        list2.Add(new ObjectPicker.MoneyColumn(num));
                        ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(current, list2);
                        list.Add(item);
                    }
                }
            }
            BaseFoodStand.BaseFoodData foodData = null;

            if (!autonomous)
            {
                List<ObjectPicker.HeaderInfo> list3 = new List<ObjectPicker.HeaderInfo>();
                List<ObjectPicker.TabInfo> list4 = new List<ObjectPicker.TabInfo>();
                list3.Add(new ObjectPicker.HeaderInfo(ShoppingRegister.sLocalizationKey + ":BuyConcessionsFoodColumnName", ShoppingRegister.sLocalizationKey + ":BuyConcessionsFoodColumnTooltip", 200));
                list3.Add(new ObjectPicker.HeaderInfo("Ui/Caption/Shopping/Cart:Price", "Ui/Tooltip/Shopping/Cart:Price"));
                list4.Add(new ObjectPicker.TabInfo("coupon", ShoppingRegister.LocalizeString("AvailableConcessionsFoods", new object[0]), list));
                List<ObjectPicker.RowInfo> list5 = SimplePurchaseDialog.Show(ShoppingRegister.LocalizeString("BuyConcessionsFoodTitle", new object[0]), base.Actor.Household.FamilyFunds, list4, list3, true);
                if (list5 == null || list5.Count != 1)
                {
                    return null;
                }
                foodData = list5[0].Item as  BaseFoodStand.BaseFoodData;

                CommonMethods.ShowMessage(list5.Count.ToString());
                CommonMethods.ShowMessage(list5[0].Item.ToString());

                if (foodData == null)
                    CommonMethods.ShowMessage("foodData null");
            }
            else
            {
                foodData = RandomUtil.GetRandomObjectFromList<BaseFoodStand.BaseFoodData>(foods);
            }
            return foodData;

        }


        // Nested Types
        public class BuyItemsDefinition : InteractionDefinition<Sim, Sim, BuyConcessionStandDrink>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return ShoppingRegister.TestIfSocialValid(a, target);
            }
        }
    }
    #endregion

    #region Buy Coffee
    /// <summary>
    /// Buy Coffee
    /// </summary>
    public class BuyCoffee : ShoppingRegister.BuyFoodWithRegister
    {
        // Fields
        private static int householdFunds;

        public static readonly BuyItemsDefinition BuyCoffeeSingleton = new BuyItemsDefinition();

        public override bool Run()
        {
            bool success = base.Run();
            WorldType type = GameUtils.GetCurrentWorldType();

            bool processPurchase = success;

            //Don't process for the cashier
            if (!CommonMethods.IsSociable(base.Actor))
                processPurchase = false;

            //Pay the lot owner
            if (processPurchase && success && base.Actor.IsHoldingAnything())
            {
                int price = AddMenuItem.ReturnCoffeePrice();
                //No lot owners abroad
                if (type != WorldType.Vacation)
                {
                    CommonMethods.PayLotOwner(base.Actor, base.Target.LotCurrent, price);
                }
                householdFunds = 0;

                //if this happened through "Call for meal"
                //Removing the price from the active buying Sim needs to be done manually
                base.Actor.ModifyFunds(-price);
            }

            return success;

        }

        // Methods
        public override void PostAnimation()
        {
            householdFunds = base.Actor.Household.FamilyFunds;
            BuyAndEatConcessionsFood(base.Actor, base.Autonomous);
        }

        public bool BuyAndEatConcessionsFood(Sim sim, bool autonomous)
        {
            bool result = true;

            //Find the counter the cash register is on
            bool registerFound = false;
            int cupLevel = 1;
            Vector3 cupPosition = sim.Position;
            if (base.Target.SimDescription.HasActiveRole && base.Target.SimDescription.AssignedRole != null && base.Target.SimDescription.AssignedRole.RoleGivingObject != null)
            {
                if (base.Target.SimDescription.AssignedRole.RoleGivingObject.Parent != null)
                    registerFound = true;
            }

            if (registerFound)
            {
                cupLevel = base.Target.SimDescription.AssignedRole.RoleGivingObject.Level + 100;
                cupPosition = base.Target.SimDescription.AssignedRole.RoleGivingObject.Position;
            }

            HotBeverageMachine.Cup cup = GlobalFunctions.CreateObject("CoffeeCup", cupPosition, cupLevel, Vector3.UnitZ) as HotBeverageMachine.Cup;

            if (cup != null)
            {
                cup.Contents = new HotBeverageMachine.CustomDrinkRecipe();
                if (sim.ParentToRightHand(cup))
                {
                    CarrySystem.EnterWhileHolding(sim, cup);
                    cup.PushDrinkAsContinuation(sim);
                    cup.StartEffects();
                    result = true;
                }
                else
                {
                    cup.Destroy();
                    result = false;
                }
            }

            //HotBeverageMachine.Cup cup = GlobalFunctions.CreateObject("CoffeeCup", cupPosition, cupLevel, Vector3.UnitZ) as HotBeverageMachine.Cup;
            //cup.Contents = new HotBeverageMachine.CustomDrinkRecipe();
            //CarrySystem.PickUp(sim, cup);
            ////   CarrySystem.EnterWhileHolding(sim, cup);
            //cup.PushDrinkAsContinuation(sim);


            return result;
        }

        // Nested Types
        public class BuyItemsDefinition : InteractionDefinition<Sim, Sim, BuyCoffee>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return ShoppingRegister.TestIfSocialValid(a, target);
            }
        }
    }
    #endregion
    /// <summary>
    /// 
    /// </summary>
    public class OverridedFoodRegister : FoodStoreRegister
    {
        #region Call For Meal

        public class CallForMeal : ImmediateInteraction<Sim, FoodStoreRegister>
        {
            // Fields

            public static readonly InteractionDefinition Singleton = new Definition();

            private const string sLocalizationKey = "Gameplay/Objects/CookingObjects/ServingContainerGroup/CallToMeal:";

            // Methods

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString(sLocalizationKey + name, parameters);
            }

            public override bool Run()
            {
                List<Sim> selectedSims = new List<Sim>();
                List<object> selectedObjects = base.SelectedObjects;
                if (selectedObjects != null)
                {
                    foreach (object obj2 in selectedObjects)
                    {
                        selectedSims.Add(obj2 as Sim);
                    }
                }

                //Loop through the sims and add them the eating interaction
                foreach (Sim sim in selectedSims)
                {
                    InteractionInstance ii = OverridedFoodRegister.Buy.Singleton.CreateInstance(base.Target, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), true, true);
                    sim.InteractionQueue.AddNext(ii);
                }

                return true;

            }

            // Nested Types

            private sealed class Definition : ImmediateInteractionDefinition<Sim, FoodStoreRegister, CallForMeal>
            {

                // Methods
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 4;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in parameters.Target.LotCurrent.GetSims())
                    {

                        if (sim.SimDescription.Child || sim.SimDescription.Teen || CommonMethods.IsSociable(sim))
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override string GetInteractionName(Sim a, FoodStoreRegister target, InteractionObjectPair interaction)
                {

                    return CallForMeal.LocalizeString("CallToMeal", new object[0]);

                }

                public override bool Test(Sim a, FoodStoreRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a.SimDescription.ChildOrAbove;
                }

            }

        }
        #endregion

        #region Buy Single plate
        public new class Buy : Interaction<Sim, FoodStoreRegister>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Register/ShoppingRegister:" + name, parameters);
            }


            // Methods
            public override bool Run()
            {
                Sim simInRole = base.Target.CurrentRole.SimInRole;
                InteractionInstance instance = OverridedBuyFoodWithRegister.BuyItemsSingleton.CreateInstance(simInRole, base.Actor, base.Actor.InheritedPriority(), base.Autonomous, base.CancellableByPlayer);
                base.Actor.InteractionQueue.PushAsContinuation(instance, true);

                return true;
            }

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, FoodStoreRegister, OverridedFoodRegister.Buy>
            {
                public override string GetInteractionName(Sim actor, FoodStoreRegister target, InteractionObjectPair iop)
                {
                    return LocalizeString("BuyFoodTitle", new object[0]);
                }

                // Methods
                public override bool Test(Sim a, FoodStoreRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.InteractionTestAvailability();
                }
            }
        }
        #endregion

        #region Call Coffee

        public class CallForCoffee : ImmediateInteraction<Sim, FoodStoreRegister>
        {
            // Fields

            public static readonly InteractionDefinition Singleton = new Definition();

            private const string sLocalizationKey = "Gameplay/Objects/CookingObjects/ServingContainerGroup/CallToMeal:";

            // Methods

            public static string LocalizeString(string name, params object[] parameters)
            {
                return CommonMethods.LocalizeString(name, parameters);
            }

            public override bool Run()
            {
                List<Sim> selectedSims = new List<Sim>();
                List<object> selectedObjects = base.SelectedObjects;
                if (selectedObjects != null)
                {
                    foreach (object obj2 in selectedObjects)
                    {
                        selectedSims.Add(obj2 as Sim);
                    }
                }

                //Loop through the sims and add them the eating interaction
                foreach (Sim sim in selectedSims)
                {
                    InteractionInstance ii = BuyCoffee.BuyCoffeeSingleton.CreateInstance(base.Target.CurrentRole.SimInRole, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                    sim.InteractionQueue.AddNext(ii);
                    //  sim.InteractionQueue.Add(ii);
                }

                return true;

            }

            // Nested Types

            public sealed class Definition : ImmediateInteractionDefinition<Sim, FoodStoreRegister, CallForCoffee>
            {

                // Methods
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 4;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in parameters.Target.LotCurrent.GetSims())
                    {

                        if (sim.SimDescription.TeenOrAbove || CommonMethods.IsSociable(sim))
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override string GetInteractionName(Sim a, FoodStoreRegister target, InteractionObjectPair interaction)
                {

                    return CallForCoffee.LocalizeString("CallForCoffee", new object[0]);

                }

                public override bool Test(Sim a, FoodStoreRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a.SimDescription.ChildOrAbove;
                }

            }

        }
        #endregion

        #region Concession Drink

        public class BuyConcessionDrink : Interaction<Sim, FoodStoreRegister>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return CommonMethods.LocalizeString(name, parameters);
            }


            // Methods
            public override bool Run()
            {
                Sim simInRole = base.Target.CurrentRole.SimInRole;
                InteractionInstance ii = BuyConcessionStandDrink.BuyCoffeeSingleton.CreateInstance(base.Target.CurrentRole.SimInRole, base.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), base.Autonomous, true);
                base.Actor.InteractionQueue.PushAsContinuation(ii, true);

                return true;
            }

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, FoodStoreRegister, OverridedFoodRegister.BuyConcessionDrink>
            {
                public override string GetInteractionName(Sim actor, FoodStoreRegister target, InteractionObjectPair iop)
                {
                    return LocalizeString("BuyTakeAway", new object[0]);
                }

                // Methods
                public override bool Test(Sim a, FoodStoreRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;// target.InteractionTestAvailability();
                }
            }
        }
        #endregion

        #region Coffee

        public class BuyCupOffCoffee : Interaction<Sim, FoodStoreRegister>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return CommonMethods.LocalizeString(name, parameters);
            }


            // Methods
            public override bool Run()
            {
                InteractionInstance ii = BuyCoffee.BuyCoffeeSingleton.CreateInstance(base.Target.CurrentRole.SimInRole, base.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                base.Actor.InteractionQueue.Add(ii);

                return true;
            }

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, FoodStoreRegister, OverridedFoodRegister.BuyCupOffCoffee>
            {
                public override string GetInteractionName(Sim actor, FoodStoreRegister target, InteractionObjectPair iop)
                {
                    return LocalizeString("SingleCupOfCoffee", new object[0]);
                }

                // Methods
                public override bool Test(Sim a, FoodStoreRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.InteractionTestAvailability();
                }
            }
        }
        #endregion


    }
    #endregion

    #region Buy Food - Concessions Stand
    /// <summary>
    /// Buy food - single plate
    /// </summary>
    public class OverridedBuyFoodFromConcessionsStand : ConcessionsStand.Buy_FoodFromConcessionsStand
    {
        // Fields
        private static int householdFunds;

        public static InteractionDefinitionConcessionStand SingletonConcessionsStand = new InteractionDefinitionConcessionStand();

        public override bool Run()
        {
            StyledNotification.Show(new StyledNotification.Format("RUN", StyledNotification.NotificationStyle.kGameMessageNegative));

            bool success = base.Run();

            WorldType type = GameUtils.GetCurrentWorldType();

            bool processPurchase = success;

            //Don't process for the cashier
            if (!CommonMethods.IsSociable(base.Actor))
                processPurchase = false;

            StyledNotification.Show(new StyledNotification.Format("After run", StyledNotification.NotificationStyle.kGameMessageNegative));

            //Pay the lot owner
            if (processPurchase && success)
            {
                int price = (int)((householdFunds - base.Actor.Household.FamilyFunds) * (new decimal(AddMenuItem.ReturnProfit()) / 100));
                if (price <= 0)
                {
                    price = 5;
                }
                StyledNotification.Show(new StyledNotification.Format(base.Actor.Name, StyledNotification.NotificationStyle.kGameMessageNegative));

                //No lot owners abroad
                if (type != WorldType.Vacation)
                {
                    CommonMethods.PayLotOwner(base.Actor, base.Target.LotCurrent, price);
                }
                householdFunds = 0;

                //if this happened through "Call for meal"
                //Removing the price from the active buying Sim needs to be done manually
                if (base.Autonomous)
                {
                    base.Actor.ModifyFunds(-price);
                }

            }

            return success;

        }
        // Nested Types
        public class InteractionDefinitionConcessionStand : InteractionDefinition<Sim, Sim, OverridedBuyFoodFromConcessionsStand>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;// ConcessionsStand.TestIfSocialValid(a, target);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>

    #endregion

    #region Buy Items
    /// <summary>
    /// 
    /// </summary>
    public class OverridedBuyItemsWithRegister : ShoppingRegister.BuyWithRegister
    {
        // Fields
        public static readonly BuyItemsDefinition BuyItemsSingleton = new BuyItemsDefinition();

        public override bool Run()
        {
            bool success = false;
            Dictionary<uint, List<ObjectGuid>> inventoryItemStack = null;

            if (base.Actor.Household.IsActive)
            {
                inventoryItemStack = new Dictionary<uint, List<ObjectGuid>>();
                foreach (IInventoryItemStack item in base.Actor.InventoryComp.InventoryUIModel.InventoryItems)
                {
                    inventoryItemStack.Add(item.StackId, item.StackObjects);
                }
            }

            success = base.Run();

            if (base.Actor.Household.IsActive)
            {
                CommonMethods.CalculateBroughtItems(inventoryItemStack, base.Actor, base.Target);
            }

            return success;

        }

        // Methods
        public override void PostAnimation()
        {
            if (base.mRegister.GetType() == typeof(BookStoreRegister))
            {
                CommonMethods.ShowCustomShoppingDialog(base.Actor, this.mRegister, ItemDictionaryBooks());
            }
            else
            {
                base.mRegister.ShowShoppingDialog(base.Actor);
            }
        }

        public Dictionary<string, List<StoreItem>> ItemDictionaryBooks()
        {
            Dictionary<string, List<StoreItem>> books = Bookstore.sItemDictionary;

            //Add written books into the shopping list
            List<Sim> sList = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
            BookWrittenData data3 = null;
            foreach (Sim s in sList)
            {
                if (s.SimDescription != null && s.SimDescription.ChildOrAbove && s.SkillManager != null)
                {
                    Writing element = s.SkillManager.GetElement(SkillNames.Writing) as Writing;

                    if ((element != null) && (element.WrittenBookDataList.Count > 0))
                    {
                        BookGeneralStoreItem item = null;
                        foreach (WrittenBookData data in element.WrittenBookDataList.Values)
                        {
                            //Check has the book already been added
                            object o = books["General"].Find(delegate(StoreItem i) { return i.Name.Equals(data.Title); });

                            if (o == null)
                            {
                                data3 = new BookWrittenData(data, false);
                                data3.Author = s.SimDescription.FullName;

                                ThumbnailKey thumb = new ThumbnailKey(new ResourceKey((ulong)ResourceUtils.XorFoldHashString32("book_standard"), 0x1661233, 1), ThumbnailSize.Medium, ResourceUtils.HashString32("default"), ResourceUtils.HashString32(data.MaterialState));

                                item = new BookGeneralStoreItem(data.Title, (float)data.Value, data3, thumb, ("BookGeneral_" + data.Title.Replace(" ", string.Empty)), new CreateObjectCallback(CreateWrittenBookCallback), new ProcessObjectCallback(ProcessWrittenBookCallback), null, new List<WorldType> { WorldType.Undefined, WorldType.Base, WorldType.Downtown }, data3.Author, data.Title, data.NumPages, BookData.GetGenreLocalizedString(data.Genre));

                                if (books.ContainsKey("General"))
                                {
                                    books["General"].Add(item);
                                    books["All"].Add(item);
                                }
                            }

                        }
                    }
                }
            }
            return books;
        }

        public static void ProcessWrittenBookCallback(object customData, IGameObject book)
        {
            BookWrittenData bookWrittenData = customData as BookWrittenData;
            if (bookWrittenData == null)
            {
                return;
            }
            try
            {
                BookWritten.ProcessCallback(bookWrittenData, book as BookWritten);
            }
            catch (Exception ex)
            {
                StyledNotification.Show(new StyledNotification.Format("ProcessWrittenBookCallback: " + ex.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
            }
        }

        public static ObjectGuid CreateWrittenBookCallback(object customData, ref Simulator.ObjectInitParameters initData, Quality quality)
        {
            BookWrittenData bookWrittenData = customData as BookWrittenData;
            if (bookWrittenData == null)
            {
                return ObjectGuid.InvalidObjectGuid;
            }
            ObjectGuid result;
            try
            {
                BookWritten bookWritten = GlobalFunctions.CreateObjectOutOfWorld("BookWritten") as BookWritten;
                if (bookWritten == null)
                {
                    result = ObjectGuid.InvalidObjectGuid;
                }
                else
                {
                    bookWritten = BookWritten.CreateOutOfWorld(bookWrittenData);
                    result = bookWritten.ObjectId;
                }
            }
            catch (Exception ex)
            {
                StyledNotification.Show(new StyledNotification.Format("CreateWrittenBookCallback: " + ex.Message, StyledNotification.NotificationStyle.kGameMessagePositive));

                result = ObjectGuid.InvalidObjectGuid;
            }
            return result;
        }

        // Nested Types
        public class BuyItemsDefinition : InteractionDefinition<Sim, Sim, OverridedBuyItemsWithRegister>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return ShoppingRegister.TestIfSocialValid(a, target);
            }
        }
    }
    /// <summary>
    /// Other Items
    /// </summary>
    public class OverridedRegister : BookStoreRegister
    {
        public new class Buy : Interaction<Sim, ShoppingRegister>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                Sim simInRole = base.Target.CurrentRole.SimInRole;
                InteractionInstance instance = OverridedBuyItemsWithRegister.BuyItemsSingleton.CreateInstance(simInRole, base.Actor, base.Actor.InheritedPriority(), base.Autonomous, base.CancellableByPlayer);
                base.Actor.InteractionQueue.PushAsContinuation(instance, true);

                return true;
            }

            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, ShoppingRegister, OverridedRegister.Buy>
            {
                // Methods
                public override bool Test(Sim a, ShoppingRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.InteractionTestAvailability();
                }
            }
        }
    }




    #endregion


}
