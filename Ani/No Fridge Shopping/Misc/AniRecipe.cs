using System.Collections.Generic;
using Sims3.Gameplay.Objects.FoodObjects;
using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Objects.Gardening;

namespace ani_GroceryShopping
{
    public class AniRecipe
    {
        public static Recipe ReturnSnackIngredientRecipe(Sim sim, Recipe recipe)
        {
            Recipe chosenRecipe = null;

            bool lotHasCounter = Food.LotHasUsableCounter(sim.LotCurrent);
            bool lotHasStove = Food.LotHasUsableStove(sim.LotCurrent);
            bool lotHasMicrowave = Food.LotHasUsableMicrowave(sim.LotCurrent);
            bool lotHasGrill = Food.LotHasUsableGrill(sim.LotCurrent);

            //Find the correct recipes
            Recipe fruitRecipe = Recipe.Recipes.Find(delegate(Recipe r) { return r.Key.Equals(AddMenuItem.FruitRecipe); });
            Recipe cheeseRecipe = Recipe.Recipes.Find(delegate(Recipe r) { return r.Key.Equals(AddMenuItem.CheeseRecipe); });
            Recipe vegetableRecipe = Recipe.Recipes.Find(delegate(Recipe r) { return r.Key.Equals(AddMenuItem.VegetableRecipe); });
            Recipe vampireRecipe = Recipe.Recipes.Find(delegate(Recipe r) { return r.Key.Equals(AddMenuItem.VampireRecipe); });

            if ((!recipe.CookingProcessData.UsesAMicrowave || !sim.SimDescription.ChildOrBelow) && (!(recipe.Key == "VampireJuice") || sim.SimDescription.IsVampire) && recipe.DoesLotHaveRightTech(lotHasCounter, lotHasStove, lotHasMicrowave, lotHasGrill, Recipe.MealQuantity.Single) == Recipe.CanMakeFoodTestResult.Pass)
            {
                if (vegetableRecipe != null && recipe.Key.Equals("CannedSoup"))
                {
                    chosenRecipe = vegetableRecipe;
                }
                else if (chosenRecipe == null && vampireRecipe != null && recipe.Key.Equals("VampireJuice"))
                {
                    chosenRecipe = vampireRecipe;
                }
                else if (chosenRecipe == null && fruitRecipe != null && !recipe.CookingProcessData.UsesAMicrowave)
                {
                    chosenRecipe = fruitRecipe;
                }
                else if (chosenRecipe == null && cheeseRecipe != null && recipe.CookingProcessData.UsesAMicrowave)
                {
                    chosenRecipe = cheeseRecipe;
                }
            }

            return chosenRecipe;
        }

        public static int CalculateCost(Recipe recipe, List<Ingredient> simIngredients, List<Ingredient> lotIngredients, bool isSnack)
        {
            List<Ingredient> list = null;
            List<Ingredient> list2 = null;
            List<IngredientData> list3 = null;
            return CalculateCost(recipe, simIngredients, lotIngredients, ref list, ref list2, out list3, isSnack);
        }

        public static int CalculateCost(Recipe recipe, List<Ingredient> simIngredients, List<Ingredient> lotIngredients, ref List<Ingredient> toRemoveFromSim, ref List<Ingredient> toRemoveFromFridge, out List<IngredientData> remainingIngredients, bool isSnack)
        {
            if (simIngredients == null || lotIngredients == null)
            {
                remainingIngredients = null;
                return 0;
            }
            remainingIngredients = BuildIngredientList(recipe, simIngredients, lotIngredients, ref toRemoveFromSim, ref toRemoveFromFridge, isSnack);
            int num = 0;
            foreach (IngredientData current in remainingIngredients)
            {
                if (!current.IsAbstract)
                {
                    if (!current.CanBuyFromStore)
                    {
                        return -2147483648;
                    }
                    num += current.Price;
                }
                else
                {
                    IngredientData cheapestIngredientOfAbstractType = IngredientData.GetCheapestIngredientOfAbstractType(current.Key, false);
                    if (cheapestIngredientOfAbstractType != null)
                    {
                        num += cheapestIngredientOfAbstractType.Price;
                    }
                }
            }
            //return num + (int)Math.Ceiling((double)((float)(num * Recipe.kFridgeRestockingPriceMarkupPercentage) / 100f));
            if (num > 0)
                num = -2147483648;

            return num;
        }

        private static List<IngredientData> BuildIngredientList(Recipe recipe, List<Ingredient> simIngredients, List<Ingredient> lotIngredients, ref List<Ingredient> toRemoveFromSim, ref List<Ingredient> toRemoveFromFridge, bool isSnack)
        {
            List<Ingredient> list = new List<Ingredient>(simIngredients);
            List<Ingredient> list2 = new List<Ingredient>(lotIngredients);
            List<IngredientData> list3 = new List<IngredientData>(recipe.IngredientsAll);

            //If snack, then only use one ingredient
            if (isSnack && recipe.Ingredient1 != null)
            {
                list3.Clear();
                list3.Add(recipe.Ingredient1);
            }

            for (int i = 0; i < list3.Count; i++)
            {
                IngredientData data = list3[i];
                if (RemoveIngredientFromList(list, data, ref toRemoveFromSim) || RemoveIngredientFromList(list2, data, ref toRemoveFromFridge))
                {
                    list3.RemoveAt(i);
                    i--;
                }
            }
            return list3;
        }

        private static bool RemoveIngredientFromList(List<Ingredient> list, IngredientData data, ref List<Ingredient> returnList)
        {
            foreach (Ingredient current in list)
            {
                if (data.IsIngredientMeOrAChildOfMe(current, null))
                {
                    list.Remove(current);
                    if (returnList != null)
                    {
                        returnList.Add(current);
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool UseUpIngredientsFrom(Recipe recipe, Sim sim, ref List<Ingredient> chosenIngredients, Recipe.MealQuantity quantity, bool isSnack)
        {
            int num = 1;
            for (int i = 0; i < num; i++)
            {
                if (!UseUpIngredientsForOneServing(recipe, sim, ref chosenIngredients, isSnack))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool UseUpIngredientsForOneServing(Recipe recipe, Sim sim, ref List<Ingredient> chosenIngredients, bool isSnack)
        {

            if (sim.Household == null || sim.Household.SharedFridgeInventory == null)
            {
                return false;
            }
            Inventory inventory = sim.Household.SharedFridgeInventory.Inventory;
            Inventory inventory2 = sim.Inventory;
            List<Ingredient> list = new List<Ingredient>();
            List<Ingredient> list2 = new List<Ingredient>();
            List<IngredientData> list3;

            //If recipe is a snack, repalce recipe. 
            if (recipe.IsSnack)
            {
                recipe = AniRecipe.ReturnSnackIngredientRecipe(sim, recipe);
                if (recipe == null)
                    return false;
            }

            int num = CalculateCost(recipe, Recipe.GetCookableIngredients(inventory2), Recipe.GetCookableIngredients(inventory), ref list2, ref list, out list3, isSnack);

            if (num == -2147483648)
            {
                return false;
            }

            if(!sim.IsNPC)            
            {
                foreach (Ingredient current in list2)
                {
                    inventory2.RemoveByForce(current);
                }
                foreach (Ingredient current2 in list)
                {
                    inventory.RemoveByForce(current2);
                }
                chosenIngredients.AddRange(list2);
                chosenIngredients.AddRange(list);


                if (num > 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
