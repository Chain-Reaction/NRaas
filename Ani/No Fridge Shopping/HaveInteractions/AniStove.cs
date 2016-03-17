
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using System.Collections.Generic;
using Sims3.SimIFace;
namespace ani_GroceryShopping
{
    internal class OverridedStove_Have : Interaction<Sim, Stove>
    {
        public sealed class Definition : Stove_Have.Definition //OverridedFoodMenuInteractionDefinition<Stove, OverridedStove_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            public override FoodMenuInteractionDefinition<Stove, Stove_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                if (cost > 0)
                {
                    cost = -2147483648;
                }
                return new Definition (menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                return CommonMethods.CreateInstance<OverridedStove_Have>(ref parameters);
            }
        }
        public static readonly InteractionDefinition Singleton = new OverridedStove_Have.Definition();
        public override bool Run()
        {
            Stove_Have.Definition definition = base.InteractionDefinition as Stove_Have.Definition;
            TraitFunctions.CheckForNeuroticAnxiety(Actor, TraitFunctions.NeuroticTraitAnxietyType.Stove);
            return AniRecipe.ForcePushFridgeHave(Actor, Target, definition.ChosenRecipe, definition.Destination, definition.Quantity, definition.Repetition);
        }
    }
}
