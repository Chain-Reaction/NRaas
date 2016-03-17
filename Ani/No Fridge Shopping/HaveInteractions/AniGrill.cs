
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
    internal class OverridedGrill_Have : Grill_Have //Interaction<Sim, Grill>
    {
        private new sealed class Definition : Grill_Have.Definition //OverridedFoodMenuInteractionDefinition<Grill, OverridedGrill_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            public override FoodMenuInteractionDefinition<Grill, Grill_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                if (cost > 0)
                {
                    cost = -2147483648;
                }
                return new Definition(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                return CommonMethods.CreateInstance<OverridedGrill_Have>(ref parameters);
            }
        }
        //private Recipe ChosenRecipe;
        public new static readonly InteractionDefinition Singleton = new OverridedGrill_Have.Definition();
        /*private new bool StartCookingProcessWithFoodTray(string menuText, string[] menuPath, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition)
        {
            List<Ingredient> ingredientsUsed = new List<Ingredient>();
            if (!Actor.IsNPC && !AniRecipe.UseUpIngredientsFrom(ChosenRecipe, Actor, ref ingredientsUsed, quantity, false))
            {
                return false;
            }
            Actor.SkillManager.AddElement(SkillNames.Cooking);
            FoodTray foodTray = (FoodTray)GlobalFunctions.CreateObject("FoodTray", Vector3.OutOfWorld, 0, Vector3.UnitZ);
            foodTray.CookingProcess = new CookingProcess(ChosenRecipe, ingredientsUsed, Target, Target.LotCurrent, destination, quantity, repetition, menuText, menuPath, foodTray, Actor);
            foodTray.CreateFoodProp(Slots.Hash("Slot_0"), foodTray.CookingProcess.Recipe.ModelsAndMaterials.GrillModel_FoodTray);
            CookingProcess.MoveToNextStep(foodTray, Actor);
            CookingProcess.MoveToNextStep(foodTray, Actor);
            CookingProcess.MoveToNextStep(foodTray, Actor);
            StateMachineClient stateMachineClient = StateMachineClient.Acquire(Actor.Proxy.ObjectId, "Grill");
            if (stateMachineClient == null)
            {
                return false;
            }
            stateMachineClient.AddInterest<TraitNames>(TraitNames.Clumsy);
            stateMachineClient.SetActor("x", Actor);
            stateMachineClient.SetActor("FoodTray", foodTray);
            stateMachineClient.EnterState("x", "Enter - Hands Empty");
            stateMachineClient.RequestState("x", "Take Out Food Tray");
            stateMachineClient.RequestState("x", "Exit - Holding Food Tray");
            if (Actor.HasExitReason(ExitReason.Canceled))
            {
                CarrySystem.EnterWhileHolding(Actor, foodTray);
                Food.PutHeldObjectDownOnCounterTableOrFloor(Actor, SurfaceType.Normal);
                return false;
            }
            CarrySystem.EnterWhileHolding(Actor, foodTray);
            InteractionInstance instance =  FoodTray_Prepare.Singleton.CreateInstance(foodTray, Actor, Actor.InheritedPriority(), base.Autonomous, true);
            return Actor.InteractionQueue.PushAsContinuation(instance, true);
        }
        public override void Init(ref InteractionInstanceParameters parameters)
        {
            base.Init(ref parameters);
            OverridedGrill_Have.Definition definition = base.InteractionDefinition as OverridedGrill_Have.Definition;
            if (definition.ChosenRecipe == null)
            {
                ChosenRecipe = RandomUtil.GetRandomObjectFromList<Recipe>(new List<Recipe>
				{
					Recipe.NameToRecipeHash["HotDog"], 
					Recipe.NameToRecipeHash["Hamburger"], 
					Recipe.NameToRecipeHash["Cheesesteak"], 
					Recipe.NameToRecipeHash["GrilledSalmon"], 
					Recipe.NameToRecipeHash["TriTipSteak"]
				});
                return;
            }
            ChosenRecipe = definition.ChosenRecipe;
        }
        public override bool OnLoadFixup()
        {
            return ChosenRecipe != null && ChosenRecipe.OnLoadFixup() && base.OnLoadFixup();
        }*/
        public override bool Run()
        {
            Grill_Have.Definition definition = base.InteractionDefinition as Grill_Have.Definition;
            if (Target.LotCurrent.IsCommunityLot)
            {
                if (!AniRecipe.HasRequiredIngredients(ChosenRecipe, Actor))
                {
                    return false;
                }
                return StartCookingProcessWithFoodTray(null, null, definition.Destination, Recipe.MealQuantity.Group, definition.Repetition);
            }
            return AniRecipe.ForcePushFridgeHave(Actor, Target, ChosenRecipe, definition.Destination, Recipe.MealQuantity.Group, definition.Repetition);
        }
    }
}
