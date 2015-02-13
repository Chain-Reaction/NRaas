
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
        private sealed class Definition : OverridedFoodMenuInteractionDefinition<Microwave, OverridedMicrowave_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            protected override OverridedFoodMenuInteractionDefinition<Microwave, OverridedMicrowave_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                return new OverridedMicrowave_Have.Definition(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override void AddInteractions(InteractionObjectPair iop, Sim sim, Microwave microwave, List<InteractionObjectPair> results)
            {
                base.AddFoodPrepInteractions(iop, sim, results, iop.Target as GameObject);
                string[] menuPath = new string[]
				{
					Food.GetString(Food.StringIndices.HaveSnack) + Localization.Ellipsis
				};
                foreach (Recipe current in Recipe.Snacks)
                {
                    if (current.CookingProcessData.UsesAMicrowave)
                    {
                        InteractionObjectPair item = new InteractionObjectPair(this.Create(current.GenericName, current, menuPath, null, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, 0), iop.Target);
                        results.Add(item);
                    }
                }
            }
            protected override bool SpecificTest(Sim a, Microwave target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.MakeSomethingStandardTests())
                {
                    return false;
                }
                if (this.ChosenRecipe != null && !this.ChosenRecipe.IsSnack)
                {
                    Recipe.CanMakeFoodTestResult result = Food.CanMake(this.ChosenRecipe, true, true, Recipe.MealTime.DO_NOT_CHECK, this.Repetition, target.LotCurrent, a, this.Quantity, this.Cost, this.ObjectClickedOn);
                    return Food.PrepareTestResultCheckAndGrayedOutPieMenuSet(a, this.ChosenRecipe, result, ref greyedOutTooltipCallback);
                }

                if (this.ChosenRecipe != null && this.ChosenRecipe.IsSnack)
                {
                    return CommonMethods.PrepareTestResultCheckAndGrayedOutPieMenuSet(this.ChosenRecipe, a, ref greyedOutTooltipCallback);
                }
                return true;
            }
        }
        public static readonly InteractionDefinition Singleton = new OverridedMicrowave_Have.Definition();
        public override bool Run()
        {
            OverridedMicrowave_Have.Definition definition = base.InteractionDefinition as OverridedMicrowave_Have.Definition;
            return Fridge.ForcePushFridgeHave(this.Actor, this.Target, definition.ChosenRecipe, definition.MenuText, definition.MenuPath, definition.ObjectClickedOn, definition.Destination, definition.Quantity, definition.Repetition, false, definition.Cost);
        }
    }
}
