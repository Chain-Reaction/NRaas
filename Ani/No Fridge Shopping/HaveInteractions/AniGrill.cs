
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Actors;
using System.Collections.Generic;
using Sims3.SimIFace;
using Sims3.Gameplay;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Autonomy;
namespace ani_GroceryShopping
{
    internal class OverridedGrill_Have : Interaction<Sim, Grill>
    {
        private sealed class Definition : OverridedFoodMenuInteractionDefinition<Grill, OverridedGrill_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            protected override OverridedFoodMenuInteractionDefinition<Grill, OverridedGrill_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                return new OverridedGrill_Have.Definition(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override void AddInteractions(InteractionObjectPair iop, Sim sim, Grill stove, List<InteractionObjectPair> results)
            {
                base.AddFoodPrepInteractions(iop, sim, results, iop.Target as GameObject);
            }
            protected override bool SpecificTest(Sim a, Grill target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.Posture is SwimmingInPool)
                {
                    return false;
                }
                if (!target.CommonMakeTest(a))
                {
                    return false;
                }
                if (this.ChosenRecipe == null)
                {
                    return a.IsNPC && target.LotCurrent.IsCommunityLot;
                }
                Recipe.CanMakeFoodTestResult result = Food.CanMake(this.ChosenRecipe, true, !target.LotCurrent.IsCommunityLot, Recipe.MealTime.DO_NOT_CHECK, this.Repetition, target.LotCurrent, a, this.Quantity, this.Cost, this.ObjectClickedOn);
                return Food.PrepareTestResultCheckAndGrayedOutPieMenuSet(a, this.ChosenRecipe, result, ref greyedOutTooltipCallback);

            }
        }
        private Recipe ChosenRecipe;
        public static readonly InteractionDefinition Singleton = new OverridedGrill_Have.Definition();
        private bool StartCookingProcessWithFoodTray(string menuText, string[] menuPath, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition)
        {
            List<Ingredient> ingredientsUsed = new List<Ingredient>();
            if (!this.Actor.IsNPC && !AniRecipe.UseUpIngredientsFrom(this.ChosenRecipe, this.Actor, ref ingredientsUsed, quantity, false))
            {
                return false;
            }
            this.Actor.SkillManager.AddElement(SkillNames.Cooking);
            FoodTray foodTray = (FoodTray)GlobalFunctions.CreateObject("FoodTray", Vector3.OutOfWorld, 0, Vector3.UnitZ);
            foodTray.CookingProcess = new CookingProcess(this.ChosenRecipe, ingredientsUsed, this.Target, this.Target.LotCurrent, destination, quantity, repetition, menuText, menuPath, foodTray, this.Actor);
            foodTray.CreateFoodProp(Slots.Hash("Slot_0"), foodTray.CookingProcess.Recipe.ModelsAndMaterials.GrillModel_FoodTray);
            CookingProcess.MoveToNextStep(foodTray, this.Actor);
            CookingProcess.MoveToNextStep(foodTray, this.Actor);
            CookingProcess.MoveToNextStep(foodTray, this.Actor);
            StateMachineClient stateMachineClient = StateMachineClient.Acquire(this.Actor.Proxy.ObjectId, "Grill");
            if (stateMachineClient == null)
            {
                return false;
            }
            stateMachineClient.AddInterest<TraitNames>(TraitNames.Clumsy);
            stateMachineClient.SetActor("x", this.Actor);
            stateMachineClient.SetActor("FoodTray", foodTray);
            stateMachineClient.EnterState("x", "Enter - Hands Empty");
            stateMachineClient.RequestState("x", "Take Out Food Tray");
            stateMachineClient.RequestState("x", "Exit - Holding Food Tray");
            if (this.Actor.HasExitReason(ExitReason.Canceled))
            {
                CarrySystem.EnterWhileHolding(this.Actor, foodTray);
                Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
                return false;
            }
            CarrySystem.EnterWhileHolding(this.Actor, foodTray);
            InteractionInstance instance =  FoodTray_Prepare.Singleton.CreateInstance(foodTray, this.Actor, this.Actor.InheritedPriority(), base.Autonomous, true);
            return this.Actor.InteractionQueue.PushAsContinuation(instance, true);
        }
        public override void Init(ref InteractionInstanceParameters parameters)
        {
            base.Init(ref parameters);
            OverridedGrill_Have.Definition definition = base.InteractionDefinition as OverridedGrill_Have.Definition;
            if (definition.ChosenRecipe == null)
            {
                this.ChosenRecipe = RandomUtil.GetRandomObjectFromList<Recipe>(new List<Recipe>
				{
					Recipe.NameToRecipeHash["HotDog"], 
					Recipe.NameToRecipeHash["Hamburger"], 
					Recipe.NameToRecipeHash["Cheesesteak"], 
					Recipe.NameToRecipeHash["GrilledSalmon"], 
					Recipe.NameToRecipeHash["TriTipSteak"]
				});
                return;
            }
            this.ChosenRecipe = definition.ChosenRecipe;
        }
        public override bool OnLoadFixup()
        {
            return this.ChosenRecipe != null && this.ChosenRecipe.OnLoadFixup() && base.OnLoadFixup();
        }
        public override bool Run()
        {
            OverridedGrill_Have.Definition definition = base.InteractionDefinition as OverridedGrill_Have.Definition;
            if (this.Target.LotCurrent.IsCommunityLot)
            {
                return this.StartCookingProcessWithFoodTray(definition.MenuText, definition.MenuPath, definition.Destination, Recipe.MealQuantity.Group, definition.Repetition);
            }
            return Fridge.ForcePushFridgeHave(this.Actor, this.Target, this.ChosenRecipe, definition.MenuText, definition.MenuPath, definition.ObjectClickedOn, definition.Destination, Recipe.MealQuantity.Group, definition.Repetition, false, definition.Cost);
        }
    }
}
