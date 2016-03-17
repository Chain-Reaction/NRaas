
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System.Collections.Generic;
namespace ani_GroceryShopping
{
    internal class OverridedMicrowave_Have : Interaction<Sim, Microwave>
    {
        public sealed class Definition : Microwave_Have.Definition //OverridedFoodMenuInteractionDefinition<Microwave, OverridedMicrowave_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            public override FoodMenuInteractionDefinition<Microwave, Microwave_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                if (cost > 0)
                {
                    cost = -2147483648;
                }
                return new Definition (menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim sim, Microwave microwave, List<InteractionObjectPair> results)
            {
                base.AddFoodPrepInteractions(iop, sim, results, iop.Target as GameObject);

                string[] menuPath = new string[]
                    {
                        Food.GetString(Food.StringIndices.HaveSnack) + Localization.Ellipsis
                    };
                List<Ingredient> simIngredients = Recipe.GetCookableIngredients(sim.Inventory);
                List<Ingredient> lotIngredients = sim.Household != null && sim.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(sim.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
                List<Ingredient> list = null;
                foreach (Recipe current in Recipe.Snacks)
                {
                    if (current.CookingProcessData.UsesAMicrowave)
                    {
                        int cost = current.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count > 0 ? -2147483648 : 0;
                        results.Add(new InteractionObjectPair(Create(current.GenericName, current, menuPath, null, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, cost), iop.Target));
                    }
                }
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                return CommonMethods.CreateInstance<OverridedMicrowave_Have>(ref parameters);
            }
        }
        public static readonly InteractionDefinition Singleton = new OverridedMicrowave_Have.Definition();
        public override bool Run()
        {
            Microwave_Have.Definition definition = base.InteractionDefinition as Microwave_Have.Definition;
            if (definition.ChosenRecipe.RecipeClassName == "MicrowaveOnlyMeal")
            {
                if (!AniRecipe.HasRequiredIngredients(definition.ChosenRecipe, Actor))
                {
                    return false;
                }
                return Microwave.CreateRecipeObjectAndPutInMicrowave(Actor, Target, definition.ChosenRecipe, null, null, Target, definition.Destination, definition.Quantity, definition.Repetition, false, 0);
            }
            return AniRecipe.ForcePushFridgeHave(Actor, Target, definition.ChosenRecipe, definition.Destination, definition.Quantity, definition.Repetition);
        }
    }
}
