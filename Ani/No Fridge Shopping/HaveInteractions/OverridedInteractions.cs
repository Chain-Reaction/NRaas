using System;
using System.Collections.Generic;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.UI;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Objects.Gardening;

namespace ani_GroceryShopping
{
    #region OverridedFoodMenuInteraction
    public abstract class OverridedFoodMenuInteractionDefinition<TTarget, TInteraction> : InteractionDefinition<Sim, TTarget, TInteraction>, IRequiresPostLoad
        where TTarget : class, IGameObject
        where TInteraction : InteractionInstance, new()
    {
        public Recipe ChosenRecipe;
        public string MenuText = string.Empty;
        public string[] MenuPath;
        public GameObject ObjectClickedOn;
        public Recipe.MealDestination Destination;
        public Recipe.MealQuantity Quantity = Recipe.MealQuantity.Single;
        public Recipe.MealRepetition Repetition;
        public bool WasHaveSomething;
        public int Cost;
        public OverridedFoodMenuInteractionDefinition()
        {
        }
        public OverridedFoodMenuInteractionDefinition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
        {
            this.MenuText = menuText;
            this.ChosenRecipe = recipe;
            this.MenuPath = menuPath;
            this.Destination = destination;
            this.Quantity = quantity;
            this.Repetition = repetition;
            this.WasHaveSomething = bWasHaveSomething;
            this.ObjectClickedOn = objectClickedOn;
            this.Cost = cost;
        }
        protected abstract OverridedFoodMenuInteractionDefinition<TTarget, TInteraction> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost);
        public override string[] GetPath(bool isFemale)
        {
            return this.MenuPath;
        }
        public override string GetInteractionName(Sim sim, TTarget target, InteractionObjectPair interaction)
        {
            string text = "";
            if (this.Cost > 0)
            {
                text += "Cost";
            }
            if (sim.HasTrait(TraitNames.EnvironmentallyConscious))
            {
                text += "Organic";
            }
            if (string.IsNullOrEmpty(text))
            {
                return this.MenuText;
            }
            return Localization.LocalizeString("Gameplay/Objects/CookingObjects/Food:RecipeWith" + text, new object[]
			{
				this.MenuText, 
				this.Cost
			});
        }
        protected abstract bool SpecificTest(Sim actor, TTarget target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback);
        public override bool Test(Sim actor, TTarget target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (isAutonomous)
            {
                if (actor.LotCurrent != null && actor.LotCurrent.IsAnotherSimPreparingFood(actor))
                {
                    return false;
                }
                if (!actor.BuffManager.HasElement(BuffNames.VeryHungry) && !actor.BuffManager.HasElement(BuffNames.Starving))
                {
                    Lot lotHome = actor.LotHome;
                    if (lotHome != null && actor.LotCurrent == lotHome)
                    {
                        Butler instance = Butler.Instance;
                        if (instance != null)
                        {
                            Sim simActiveOnLot = instance.GetSimActiveOnLot(lotHome);
                            if (simActiveOnLot != null)
                            {
                                return simActiveOnLot.IsSleeping;
                            }
                        }
                    }
                }
            }
            return this.SpecificTest(actor, target, isAutonomous, ref greyedOutTooltipCallback);
        }
        public override void AddInteractions(InteractionObjectPair iop, Sim sim, TTarget target, List<InteractionObjectPair> results)
        {
            this.AddFoodPrepInteractions(iop, sim, results, iop.Target as GameObject);
        }
        public bool OnLoadFixup()
        {
            return this.ChosenRecipe != null && this.ChosenRecipe.OnLoadFixup();
        }
        protected void AddFoodPrepInteractions(InteractionObjectPair iop, Sim sim, List<InteractionObjectPair> results, GameObject objectClickedOn)
        {
            if (sim.Household == null)
            {
                return;
            }
            Lot lotHome = sim.LotHome;
            if (lotHome == null)
            {
                return;
            }
            Type type = (objectClickedOn != null) ? objectClickedOn.GetType() : null;
            bool flag = Reflection.IsTypeAssignableFrom(typeof(Stove), type);
            bool flag2 = Reflection.IsTypeAssignableFrom(typeof(Microwave), type);
            bool flag3 = Reflection.IsTypeAssignableFrom(typeof(Grill), type);
            bool flag4 = Reflection.IsTypeAssignableFrom(typeof(FoodProcessor), type);
            List<Recipe> availableRecipes = Food.GetAvailableRecipes(iop.Target.LotCurrent, sim, false, false, false);
            Recipe.MealTime mealTime = flag3 ? Recipe.MealTime.DO_NOT_CHECK : Food.GetCurrentMealTime();
            string currentMealTimeString = Food.GetCurrentMealTimeString();
            string[] menuPath = new string[]
			{
				Localization.LocalizeString("Gameplay/Objects/CookingObjects/Food:Have", new object[]
				{
					currentMealTimeString
				})
			};
            string[] menuPath2 = new string[]
			{
				Localization.LocalizeString("Gameplay/Objects/CookingObjects/Food:Serve", new object[]
				{
					currentMealTimeString
				})
			};
            string[] menuPath3 = new string[]
			{
				Localization.LocalizeString("Gameplay/Objects/CookingObjects/Food:Serve", new object[]
				{
					Food.GetString(Food.StringIndices.Dessert)
				})
			};
            string[] menuPath4 = new string[]
			{
				Localization.LocalizeString("Gameplay/Objects/CookingObjects/Food:PetFood", new object[0])
			};
            List<Ingredient> lotIngredients = null;
            if (lotHome.Household != null && lotHome.Household.SharedFridgeInventory != null && lotHome.Household.SharedFridgeInventory.Inventory != null)
            {
                lotIngredients = Recipe.GetCookableIngredients(lotHome.Household.SharedFridgeInventory.Inventory);
            }
            List<Ingredient> cookableIngredients = Recipe.GetCookableIngredients(sim.Inventory);
            foreach (Recipe current in availableRecipes)
            {
                if (type == null || (current.CookingProcessData.UsesAStove && flag) || (current.CookingProcessData.UsesAMicrowave && flag2) || (current.CookingProcessData.UsesAGrill && flag3) || (current.AllowFoodProcessor && flag4))
                {
                    int cost = 0;

                    //If sim is not in the active household, skip the check
                    if (sim.IsNPC)
                    {
                        cost = current.CalculateCost(cookableIngredients, lotIngredients);
                    }
                    else
                    {
                        cost = AniRecipe.CalculateCost(current, cookableIngredients, lotIngredients, false);
                    }

                    string menuText = sim.HasTrait(TraitNames.Vegetarian) ? current.GenericVegetarianName : current.GenericName;
                    if (current.IsDessert)
                    {
                        if (current.CanMakeGroupServing)
                        {
                            InteractionObjectPair item = new InteractionObjectPair(this.Create(menuText, current, menuPath3, objectClickedOn, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Group, Recipe.MealRepetition.MakeOne, false, cost), iop.Target);
                            results.Add(item);
                        }
                    }
                    else
                    {
                        if (current.IsPetFood)
                        {

                            InteractionObjectPair item = new InteractionObjectPair(this.Create(menuText, current, menuPath4, objectClickedOn, Recipe.MealDestination.SurfaceOnly, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, cost), iop.Target);
                            results.Add(item);
                        }
                        else
                        {
                            if (current.IsAvailableFor(mealTime))
                            {
                                if (current.CanMakeSingleServing && !flag3 && (!sim.HasTrait(TraitNames.Vegetarian) || current.IsVegetarian || current.HasVegetarianAlternative))
                                {
                                    InteractionObjectPair item = new InteractionObjectPair(this.Create(menuText, current, menuPath, objectClickedOn, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, cost), iop.Target);
                                    results.Add(item);
                                }
                                if (current.CanMakeGroupServing)
                                {
                                    InteractionObjectPair item = new InteractionObjectPair(this.Create(menuText, current, menuPath2, objectClickedOn, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Group, Recipe.MealRepetition.MakeOne, false, cost), iop.Target);
                                    results.Add(item);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion    

    public static class OverridedInteractions
    {
        #region OverridedUseUpIngredients

        public static bool UseUpIngredientsForOneServing(Recipe recipe, Sim sim, ref List<Ingredient> chosenIngredients)
        {
            if (sim.Household == null || sim.Household.SharedFridgeInventory == null)
            {
                return false;
            }

            //if Recipe is snack, replace it with a snack recipe
            bool isSnack = recipe.IsSnack;
            /*if (recipe.IsSnack)
            {
                recipe = AniRecipe.ReturnSnackIngredientRecipe(sim, recipe);

                if (recipe == null)
                    return false;
            }*/

            Inventory inventory = sim.Household.SharedFridgeInventory.Inventory;
            Inventory inventory2 = sim.Inventory;
            List<Ingredient> list = new List<Ingredient>();
            List<Ingredient> list2 = new List<Ingredient>();
            List<IngredientData> list3;
            int num = AniRecipe.CalculateCost(recipe, Recipe.GetCookableIngredients(inventory2), Recipe.GetCookableIngredients(inventory), ref list2, ref list, out list3, isSnack);
            if (num == -2147483648)
            {
                return false;
            }            
            return true;
        }
        #endregion
    }
       
}
