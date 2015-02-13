

using Sims3.Gameplay.Objects.FoodObjects;
using System.Collections.Generic;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System.Text;
using Sims3.UI;
namespace ani_GroceryShopping
{
    public class CommonMethods
    {
        #region PrintMessage
        /// <summary>
        /// Print message on screen
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message)
        {
             StyledNotification.Show(new StyledNotification.Format(message, StyledNotification.NotificationStyle.kGameMessagePositive));
        }
        #endregion

        #region Recipe CanMakeFoodTestResult
        public static bool PrepareTestResultCheckAndGrayedOutPieMenuSet(Recipe recipe, Sim sim, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            List<Ingredient> lotIngredients = null;
            Lot lotHome = sim.LotHome;

            Recipe chosenRecipe = null;
            bool isSnack = recipe.IsSnack;

            bool lotHasCounter = Food.LotHasUsableCounter(sim.LotCurrent);
            bool lotHasStove = Food.LotHasUsableStove(sim.LotCurrent);
            bool lotHasMicrowave = Food.LotHasUsableMicrowave(sim.LotCurrent);
            bool lotHasGrill = Food.LotHasUsableGrill(sim.LotCurrent);

            if (lotHome.Household != null && lotHome.Household.SharedFridgeInventory != null && lotHome.Household.SharedFridgeInventory.Inventory != null)
            {
                lotIngredients = Recipe.GetCookableIngredients(lotHome.Household.SharedFridgeInventory.Inventory);
            }
            List<Ingredient> cookableIngredients = Recipe.GetCookableIngredients(sim.Inventory);

            //Find the correct recipes
            if ((!recipe.CookingProcessData.UsesAMicrowave || !sim.SimDescription.ChildOrBelow) && (!(recipe.Key == "VampireJuice") || sim.SimDescription.IsVampire) && recipe.DoesLotHaveRightTech(lotHasCounter, lotHasStove, lotHasMicrowave, lotHasGrill, Recipe.MealQuantity.Single) == Recipe.CanMakeFoodTestResult.Pass)
            {
                int cost = 0;
                chosenRecipe = AniRecipe.ReturnSnackIngredientRecipe(sim, recipe);

                if (chosenRecipe != null)
                    cost = AniRecipe.CalculateCost(chosenRecipe, cookableIngredients, lotIngredients, isSnack);
            }

            if (chosenRecipe == null)
                return false;
            if (chosenRecipe != null && AniRecipe.CalculateCost(chosenRecipe, cookableIngredients, lotIngredients, isSnack) == -2147483648)
            {
                greyedOutTooltipCallback = delegate
                {
                    return PrepareTestResultCheckAndGrayedOutPieMenuSet(sim, chosenRecipe, recipe.GenericName, isSnack);
                };

                return false;
            }

            return true;
        }
        #endregion

        #region return GrayedoutTooltipText
        public static string PrepareTestResultCheckAndGrayedOutPieMenuSet(Sim preparer, Recipe recipe, string dishName, bool isSnack)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Localization.LocalizeString("Gameplay/Objects/FoodObjects/Food:Requires", new object[]
            {
                dishName
            }));

            //If snack, only use one ingredient              
            if (isSnack)
            {
                if (recipe.Ingredient1 != null)
                {
                    sb.Append("\n");
                    sb.Append(recipe.Ingredient1.Name);
                }
            }
            else
            {
                foreach (IngredientData current in recipe.Ingredients.Keys)
                {
                    sb.Append("\n");
                    sb.Append(current.Name);
                    if (recipe.Ingredients[current] > 1)
                    {
                        sb.Append(" (");
                        sb.Append(recipe.Ingredients[current].ToString());
                        sb.Append(")");
                    }
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
