using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.UI;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.ObjectComponents;

namespace ani_GroceryShopping
{
    #region OverridedFridge_Have
    public class OverridedFridge_Have : Fridge_Have //Interaction<Sim, Fridge>, ICookingInteraction, IPetWatchableFoodInteraction
    {
        public new class Definition : Fridge_Have.Definition //OverridedFoodMenuInteractionDefinition<Fridge, OverridedFridge_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            public override FoodMenuInteractionDefinition<Fridge, Fridge_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                if (cost > 0)
                {
                    cost = -2147483648;
                }
                return new Definition(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override void AddInteractions(InteractionObjectPair iop, Sim sim, Fridge fridge, List<InteractionObjectPair> results)
            {
                /*if (sim == null)
                {
                    return;
                }

                bool lotHasCounter = Food.LotHasUsableCounter(fridge.LotCurrent);
                bool lotHasStove = Food.LotHasUsableStove(fridge.LotCurrent);
                bool lotHasMicrowave = Food.LotHasUsableMicrowave(fridge.LotCurrent);
                bool lotHasGrill = Food.LotHasUsableGrill(fridge.LotCurrent);
                Lot lotCurrent = fridge.LotCurrent;
                if (lotCurrent == sim.LotHome || (lotCurrent.IsResidentialLot && sim.Household.IsGreetedOnLot(lotCurrent, ObjectGuid.InvalidObjectGuid)))
                {
                    base.AddFoodPrepInteractions(iop, sim, results, null);
                }
                Food.StringIndices stringIndex;
                if (sim.HasTrait(TraitNames.PartyAnimal) && Party.IsInvolvedInAnyTypeOfParty(sim))
                {
                    stringIndex = Food.StringIndices.PartySnack;
                }
                else
                {
                    if (sim.HasTrait(TraitNames.Evil))
                    {
                        stringIndex = Food.StringIndices.EvilSnack;
                    }
                    else
                    {
                        if (sim.HasTrait(TraitNames.EnvironmentallyConscious))
                        {
                            stringIndex = Food.StringIndices.OrganicSnack;
                        }
                        else
                        {
                            if (sim.HasTrait(TraitNames.Good))
                            {
                                stringIndex = Food.StringIndices.GoodSnack;
                            }
                            else
                            {
                                if (sim.TraitManager.HasElement(TraitNames.Mooch) && lotCurrent != sim.LotHome)
                                {
                                    stringIndex = Food.StringIndices.MoochSnack;
                                }
                                else
                                {
                                    stringIndex = Food.StringIndices.HaveSnack;
                                }
                            }
                        }
                    }
                }
                string[] menuPath = new string[]
				{
					Food.GetString(stringIndex) + Localization.Ellipsis
				};

                foreach (Recipe current in Recipe.Snacks)
                {
                    if ((!current.CookingProcessData.UsesAMicrowave || !sim.SimDescription.ChildOrBelow) && (!(current.Key == "VampireJuice") || sim.SimDescription.IsVampire) && current.DoesLotHaveRightTech(lotHasCounter, lotHasStove, lotHasMicrowave, lotHasGrill, Recipe.MealQuantity.Single) == Recipe.CanMakeFoodTestResult.Pass)
                    {
                        InteractionObjectPair item = new InteractionObjectPair(Create(current.GenericName, current, menuPath, null, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, 0), iop.Target);
                        results.Add(item);
                    }
                }*/
                base.AddInteractions(iop, sim, fridge, results);
                List<Ingredient> simIngredients = Recipe.GetCookableIngredients(sim.Inventory);
                List<Ingredient> lotIngredients = sim.Household != null && sim.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(sim.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
                List<Ingredient> list = null;
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    Fridge_Have.Definition definition = results[i].InteractionDefinition as Fridge_Have.Definition;
                    if (definition == null || definition.ChosenRecipe == null || !definition.ChosenRecipe.IsSnack)
                    {
                        return;
                    }
                    if (definition.ChosenRecipe.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count > 0)
                    {
                        definition.Cost = -2147483648;
                    }
                }
            }
            public override bool SpecificTest(Sim a, Fridge target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                /*if (target.FridgeInventory == null && target.LotCurrent != null && !target.LotCurrent.IsCommunityLot)
                {
                    return false;
                }
                if (isAutonomous && a.SimDescription.IsFrankenstein)
                {
                    return false;
                }
                if (ChosenRecipe != null && !ChosenRecipe.IsSnack)
                {
                    Recipe.CanMakeFoodTestResult result = Food.CanMake(ChosenRecipe, true, true, Recipe.MealTime.DO_NOT_CHECK, Repetition, target.LotCurrent, a, Quantity, Cost, ObjectClickedOn);
                    return Food.PrepareTestResultCheckAndGrayedOutPieMenuSet(a, ChosenRecipe, result, ref greyedOutTooltipCallback);
                }
                if (ChosenRecipe != null && ChosenRecipe.IsSnack)
                {
                    return CommonMethods.PrepareTestResultCheckAndGrayedOutPieMenuSet(ChosenRecipe, a, ref greyedOutTooltipCallback);
                }
                return !target.InUse;*/
                return base.SpecificTest(a, target, isAutonomous, ref greyedOutTooltipCallback) && (ChosenRecipe != null || a.IsNPC || ChooseRecipeRandomly(target.LotCurrent, a, Quantity, true) != null);
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                return CommonMethods.CreateInstance<OverridedFridge_Have>(ref parameters);
            }
        }

        public new static InteractionDefinition Singleton = new Definition();

        public override void Initialize(ref InteractionInstanceParameters parameters)
        {
            Fridge_Have.Definition definition = parameters.InteractionDefinition as Fridge_Have.Definition;
            if (definition.ChosenRecipe != null || Actor.IsNPC)
            {
                base.Initialize(ref parameters);
                return;
            }
            Lot lotCurrent = Actor.LotCurrent;
            if (lotCurrent.GetSims(new Predicate<Sim>(ShouldCookFor)).Count > 1)
            {
                Quantity = Recipe.MealQuantity.Group;
            }
            ChosenRecipe = ChooseRecipeRandomly(lotCurrent, Actor, Quantity, false);
            if (ChosenRecipe != null)
            {
                if (!ChosenRecipe.CanMakeSingleServing)
                {
                    Quantity = Recipe.MealQuantity.Group;
                }
                if (!ChosenRecipe.CanMakeGroupServing)
                {
                    Quantity = Recipe.MealQuantity.Single;
                }
                ObjectClickedOn = Target;
            }
        }

        public override bool Run()
        {
            try
            {
                if (CheckForCancelAndCleanup())
                {
                    return false;
                }
                if (!Target.RouteToOpen(this, true))
                {
                    return false;
                }
                if (Target.InUse)
                {
                    Actor.AddExitReason(ExitReason.RouteFailed);
                    return false;
                }
                mImpassableRegion.AddMember(Actor);
                mImpassableRegion.AddMember(Target);
                mImpassableRegion.UpdateFootprint();
                base.StandardEntry();
                if (Actor.SimDescription.TeenOrAbove && !ChosenRecipe.IsSnack)
                {
                    Actor.SkillManager.AddElement(SkillNames.Cooking);
                }
                bool flag = true;
                List<Ingredient> ingredientsUsed = new List<Ingredient>();

                //if (AniRecipe.UseUpIngredientsFrom(ChosenRecipe, Actor, ref ingredientsUsed, Quantity, ChosenRecipe.IsSnack) || Actor.IsNPC)
                if (AniRecipe.UseUpIngredientsFrom(ChosenRecipe, Actor, ref ingredientsUsed))
                {
                    //If the food is a snack, remove ingredient 
                    /*if (ChosenRecipe.IsSnack && !Actor.IsNPC)
                    {
                        Recipe snack = AniRecipe.ReturnSnackIngredientRecipe(Actor, ChosenRecipe);
                        if (snack != null)
                        {
                            //Create new temp ingredient list  
                            foreach (var item in ingredientsUsed)
                            {
                                item.Destroy();
                            }
                            ingredientsUsed.Clear();
                        }
                    }*/

                    //CommonMethods.PrintMessage("Snack: " + ChosenRecipe.IsSnack + " / " + ingredientsUsed.Count.ToString());

                    Fridge.EnterStateMachine(this);
                    IRemovableFromFridgeAsInitialRecipeStep removableFromFridgeAsInitialRecipeStep = GlobalFunctions.CreateObjectOutOfWorld(ChosenRecipe.ObjectToCreateInFridge, ChosenRecipe.ModelCodeVersion) as IRemovableFromFridgeAsInitialRecipeStep;
                    GameObject gameObject = removableFromFridgeAsInitialRecipeStep as GameObject;
                    gameObject.AddToUseList(Actor);
                    try
                    {
                        Target.PutOnFridgeShelf(gameObject);
                        mThingToPrepareOrEat = (removableFromFridgeAsInitialRecipeStep as IPartOfCookingProcess);
                        mThingToPrepareOrEat.CookingProcess = new CookingProcess(ChosenRecipe, ingredientsUsed, ObjectClickedOn, Target.LotCurrent, Destination, Quantity, Repetition, MenuText, MenuPath, removableFromFridgeAsInitialRecipeStep as IPartOfCookingProcess, Actor);
                        removableFromFridgeAsInitialRecipeStep.InitializeForRecipe(ChosenRecipe, false);
                        CookingProcess.MoveToNextStep(removableFromFridgeAsInitialRecipeStep as IPartOfCookingProcess, Actor);
                        base.SetActor(removableFromFridgeAsInitialRecipeStep.ActorNameForFridge, gameObject);
                        if (mbWasHaveSomething)
                        {
                            base.AnimateSim("Ponder");
                        }
                        base.AnimateSim("Remove - " + removableFromFridgeAsInitialRecipeStep.ActorNameForFridge);
                        TriggerWatchCookingReactionBroadcaster();
                    }
                    catch (Exception ex)
                    {
                        if (ex == null)
                        {
                        }
                        gameObject.Destroy();
                        throw;
                    }
                    CarrySystem.EnterWhileHolding(Actor, removableFromFridgeAsInitialRecipeStep, false);
                    if (CheckForCancelAndCleanup())
                    {
                        return false;
                    }
                    /*if (Actor.HasTrait(TraitNames.NaturalCook))
                    {
                        TraitTipsManager.ShowTraitTip(13271263770231522448uL, Actor, TraitTipsManager.TraitTipCounterIndex.NaturalCook, TraitTipsManager.kNaturalCookCountOfMealsCooked);
                    }
                    if (Actor.HasTrait(TraitNames.Vegetarian))
                    {
                        TraitTipsManager.ShowTraitTip(13271263770231522928uL, Actor, TraitTipsManager.TraitTipCounterIndex.Vegetarian, TraitTipsManager.kVegetarianCountOfMealsCooked);
                    }*/
                    PushNextInteraction(removableFromFridgeAsInitialRecipeStep, gameObject);
                    base.AnimateSim("Exit - Standing");
                }
                else
                {
                    flag = false;
                }

                base.StandardExit();
                if (flag)
                {
                    ActiveTopic.AddToSim(Actor, "Has Made Food");
                }
                return flag;
            }
            catch (Exception ex)
            {
                CommonMethods.PrintMessage("Fridge: " + ex.Message);
                return false;
            }
        }

        public static Recipe ChooseRecipeRandomly(Lot lotToCookOn, Sim sim, Recipe.MealQuantity quantity, bool forTesting)
        {
            if (lotToCookOn == null)
            {
                return null;
            }
            List<Ingredient> simIngredients = Recipe.GetCookableIngredients(sim.Inventory);
            List<Ingredient> lotIngredients = sim.Household != null && sim.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(sim.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
            if (GameUtils.IsInstalled(ProductVersion.EP3) && !sim.SimDescription.IsVampire)
            {
                Predicate<Ingredient> vampireFruit = (i => i.Key == "VampireFruit");
                simIngredients.RemoveAll(vampireFruit);
                lotIngredients.RemoveAll(vampireFruit);
            }
            if (simIngredients.Count + lotIngredients.Count == 0)
            {
                return null;
            }

            List<Ingredient> list = null;
            Predicate<Recipe> p = (r => r.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count == 0);

            Recipe recipe = null;
            if (sim.SimDescription.IsVampire)
            {
                recipe = Recipe.NameToRecipeHash[GameUtils.IsInstalled(ProductVersion.EP7) ? "VampireJuiceEP7" : "VampireJuice"];
            }
            else if (sim.SimDescription.IsZombie)
            {
                recipe = Recipe.NameToRecipeHash["BrainFreeze"];
            }
            if (recipe != null)
            {
                return p(recipe) ? recipe : null;
            }

            bool lotHasCounter = Food.LotHasUsableCounter(lotToCookOn);
            bool lotHasMicrowave = Food.LotHasUsableMicrowave(lotToCookOn);
            //For testing purposes quicker to test snacks first since they only require 1 ingredient.
            if (forTesting)
            {
                recipe = ChooseRandomSnack(sim, lotHasCounter, lotHasMicrowave, p, true);
                if (recipe != null)
                {
                    return recipe;
                }
            }
            if (!lotToCookOn.IsCommunityLot)
            {
                List<Recipe> availableRecipes = new List<Recipe>();
                bool lotHasStove = Food.LotHasUsableStove(lotToCookOn);
                bool lotHasGrill = Food.LotHasUsableGrill(lotToCookOn);
                Recipe.MealTime mealTime = Food.GetCurrentMealTime();

                bool flag = sim.HasTrait(TraitNames.Vegetarian);
                Cooking cooking = sim.SkillManager.GetSkill<Cooking>(SkillNames.Cooking);
                if (cooking != null && cooking.IsHiddenSkill() && cooking.SkillLevel >= 1)
                {
                    foreach (string current in cooking.KnownRecipes)
                    {
                        Recipe r;
                        if (Recipe.NameToRecipeHash.TryGetValue(current, out r) && !r.IsPetFood && (!flag || r.IsVegetarian) && Food.CanMake(r, false, true, mealTime, Recipe.MealRepetition.MakeOne, lotToCookOn, sim, Recipe.MealQuantity.DoNotCheck, lotHasCounter, lotHasStove, lotHasMicrowave, lotHasGrill, 0) == Recipe.CanMakeFoodTestResult.Pass
                            && p(r))
                        {
                            if (forTesting)
                            {
                                return r;
                            }
                            availableRecipes.Add(r);
                        }
                    }
                }
                else if (sim.SimDescription.TeenOrAbove && quantity == Recipe.MealQuantity.Group)
                {
                    foreach (Recipe current2 in Recipe.IntroRecipes)
                    {
                        if ((!flag || current2.IsVegetarian) && Food.CanMake(current2, false, true, mealTime, Recipe.MealRepetition.MakeOne, lotToCookOn, sim, Recipe.MealQuantity.DoNotCheck, lotHasCounter, lotHasStove, lotHasMicrowave, lotHasGrill, 0) == Recipe.CanMakeFoodTestResult.Pass
                            && p(current2))
                        {
                            if (forTesting)
                            {
                                return current2;
                            }
                            availableRecipes.Add(current2);
                        }
                    }
                }
                if (availableRecipes.Count > 0)
                {
                    if (RandomUtil.RandomChance(Food.kChanceOfChoosingFavoriteFood))
                    {
                        FavoriteFoodType favoriteFood = sim.SimDescription.FavoriteFood;
                        if (favoriteFood != FavoriteFoodType.None)
                        {
                            foreach (Recipe current3 in availableRecipes)
                            {
                                if (current3.Favorite == favoriteFood)
                                {
                                    return current3;
                                }
                            }
                        }
                    }
                    if (quantity == Recipe.MealQuantity.Group)
                    {
                        p = (r => r.CanMakeGroupServing);
                    }
                    else
                    {
                        p = (r => r.CanMakeSingleServing);
                    }
                    List<Recipe> limitedRecipes = availableRecipes.FindAll(p);
                    return RandomUtil.GetRandomObjectFromList<Recipe>(limitedRecipes.Count > 0 ? limitedRecipes : availableRecipes);
                }
            }
            if (!forTesting)
            {
                return ChooseRandomSnack(sim, lotHasCounter, lotHasMicrowave, p, false);
            }
            return null;
        }

        public static Recipe ChooseRandomSnack(Sim sim, bool lotHasCounter, bool lotHasMicrowave, Predicate<Recipe> condition, bool forTesting)
        {
            List<Recipe> availableSnacks = new List<Recipe>();
            foreach (Recipe current in Recipe.Snacks)
            {
                string key;
                if ((key = current.Key) == null || key == "VampireJuice" || key == "VampireJuiceEP7" || key == "PineappleJuice" || key == "VirginPineappleJuice" || key == "CoconutJuice" || key == "VirginCoconutJuice")
                {
                    continue;
                }
                if ((!current.AdultOnly || (!sim.SimDescription.TeenOrBelow && !sim.SimDescription.IsPregnant)) && !Fridge.sExcludeList.Contains(key) && (!current.CookingProcessData.UsesAMicrowave || !sim.SimDescription.ChildOrBelow) 
                    && current.DoesLotHaveRightTech(lotHasCounter, false, lotHasMicrowave, false, Recipe.MealQuantity.Single) == Recipe.CanMakeFoodTestResult.Pass && condition(current))
                {
                    if (forTesting)
                    {
                        return current;
                    }
                    availableSnacks.Add(current);
                }
            }
            if (availableSnacks.Count == 0)
            {
                return null;
            }
            return RandomUtil.GetRandomObjectFromList<Recipe>(availableSnacks);
        }
    }

    #endregion

    #region OverridedFridge_Prepare
    public class OverridedFridge_Prepare : OverridedFridge_Have
    {
        public sealed class PrepareDefinition : OverridedFridge_Have.Definition
        {
            public PrepareDefinition()
            {
            }
            public PrepareDefinition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }

            public override bool Test(Sim a, Fridge target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return Party.IsHostAtAParty(a) && (!isAutonomous || target.LotCurrent.CountObjects<IPreparedFood>() <= 0u) && base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
            public override FoodMenuInteractionDefinition<Fridge, Fridge_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                menuPath = new string[]
				{
					Localization.LocalizeString("Gameplay/Objects/FoodObjects:PrepareMenuPath", new object[0])
				};
                if (cost > 0)
                {
                    cost = -2147483648;
                }
                return new OverridedFridge_Prepare.PrepareDefinition(menuText, recipe, menuPath, objectClickedOn, Recipe.MealDestination.SurfaceOnly, Recipe.MealQuantity.Group, repetition, bWasHaveSomething, cost);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim sim, Fridge fridge, List<InteractionObjectPair> results)
            {
                if (sim != null && Party.IsHostAtAParty(sim))
                {
                    Lot lotCurrent = fridge.LotCurrent;
                    if (lotCurrent == sim.LotHome || (lotCurrent.IsResidentialLot && sim.Household.IsGreetedOnLot(lotCurrent, ObjectGuid.InvalidObjectGuid)))
                    {
                        base.AddFoodPrepInteractions(iop, sim, results, null);
                    }
                }
            }
        }
        public static readonly InteractionDefinition PrepareSingleton = new OverridedFridge_Prepare.PrepareDefinition();
        /*public override bool Run()
        {
            return base.Run();
        }*/
    }
    #endregion

}
