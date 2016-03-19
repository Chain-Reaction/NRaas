
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Abstracts;
using System.Collections.Generic;

namespace ani_GroceryShopping
{
    public class OverridedFoodProcessor_Have : Interaction<Sim, FoodProcessor>
    {
        public sealed class Definition : FoodProcessor.FoodProcessor_Have.Definition //OverridedFoodMenuInteractionDefinition<FoodProcessor, OverridedFoodProcessor_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            public override FoodMenuInteractionDefinition<FoodProcessor, FoodProcessor.FoodProcessor_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                if (cost > 0)
                {
                    cost = -2147483648;
                }
                return new Definition(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                return CommonMethods.CreateInstance<OverridedFoodProcessor_Have>(ref parameters);
            }
        }
        public static readonly InteractionDefinition Singleton = new OverridedFoodProcessor_Have.Definition();
        public override bool Run()
        {
            FoodProcessor.FoodProcessor_Have.Definition definition = base.InteractionDefinition as FoodProcessor.FoodProcessor_Have.Definition;
            return AniRecipe.ForcePushFridgeHave(Actor, Target, definition.ChosenRecipe, definition.Destination, definition.Quantity, definition.Repetition);
        }
    }
}
