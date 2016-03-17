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
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.UI;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Store.Objects;

namespace ani_GroceryShopping
{
    public class OverridedWOBakeDefinition : WoodFireOven.WOBake.Definition
    {
        public static InteractionDefinition Singleton = new OverridedWOBakeDefinition();

        public OverridedWOBakeDefinition()
        {
        }

        public OverridedWOBakeDefinition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost) : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
        {
        }

        public override void AddInteractions(InteractionObjectPair iop, Sim actor, WoodFireOven target, List<InteractionObjectPair> results)
        {
            List<Ingredient> simIngredients = Recipe.GetCookableIngredients(actor.Inventory);
            List<Ingredient> lotIngredients = actor.Household != null && actor.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(actor.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
            List<Ingredient> list = null;

            Recipe.MealTime currentMealTime = Food.GetCurrentMealTime();
            string[] menuPath = new string[]
                {
                    WoodFireOven.LocalizeString("Serve") /*+ " " + Food.GetMealTimeString(currentMealTime)*/ + Localization.Ellipsis
                };
            foreach (WoodFireOven.WOFoodInfo info in WoodFireOven.WOFoodInfos)
            {
                Recipe current;
                if (Recipe.NameToRecipeHash.TryGetValue(info.mName, out current) && current.IsAvailableFor(currentMealTime))
                {
                    int cost = current.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count == 0 ? 0 : -1;
                    results.Add(new InteractionObjectPair(new OverridedWOBakeDefinition(current.GenericName, current, menuPath, target, Recipe.MealDestination.SurfaceAndCallToMeal, Recipe.MealQuantity.Group, Recipe.MealRepetition.MakeOne, false, cost), iop.Target));
                }
            }
        }

        public override bool SpecificTest(Sim actor, WoodFireOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (isAutonomous)
            {
                if (target.InUse)
                {
                    return false;
                }
                if (actor.IsSelectable)
                {
                    mWOFoodInfo = null;

                    List<Ingredient> simIngredients = Recipe.GetCookableIngredients(actor.Inventory);
                    List<Ingredient> lotIngredients = actor.Household != null && actor.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(actor.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
                    List<Ingredient> list = null;
                    Recipe.MealTime time = Food.GetCurrentMealTime();

                    int num = RandomUtil.GetInt(13);
                    int num2 = 0;
                    Cooking skill = actor.SkillManager.GetSkill<Cooking>(SkillNames.Cooking);
                    Recipe recipe;
                    WoodFireOven.WOFoodInfo wOFoodInfo;
                    while (true)
                    {
                        wOFoodInfo = WoodFireOven.WOFoodInfos[num];
                        if (Recipe.NameToRecipeHash.TryGetValue(wOFoodInfo.mName, out recipe) && (recipe.CookingSkillLevelRequired == 0 || (skill != null && skill.SkillLevel >= recipe.CookingSkillLevelRequired))
                            && recipe.IsAvailableFor(time) && recipe.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count == 0)
                        {
                            break;
                        }
                        if (++num2 >= 14)
                        {
                            return false;
                        }
                        if (++num >= 14)
                        {
                            num = 0;
                        }
                    }
                    target.mCurrentRecipe = recipe;
                    ChosenRecipe = recipe;
                    mWOFoodInfo = wOFoodInfo;
                    return true;
                }
            }
            return base.SpecificTest(actor, target, isAutonomous, ref greyedOutTooltipCallback)
                && (Cost == 0 || Food.PrepareTestResultCheckAndGrayedOutPieMenuSet(actor, ChosenRecipe, Recipe.CanMakeFoodTestResult.Fail_NeedIngredients, ref greyedOutTooltipCallback));
        }
    }

    public class OverridedTGCookDefinition : TeppanyakiGrill.TGCook.Definition
    {
        public static InteractionDefinition Singleton = new OverridedTGCookDefinition();

        public OverridedTGCookDefinition()
        {
        }

        public OverridedTGCookDefinition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost) : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
        {
        }

        public override void AddInteractions(InteractionObjectPair iop, Sim actor, TeppanyakiGrill target, List<InteractionObjectPair> results)
        {
            List<Ingredient> simIngredients = Recipe.GetCookableIngredients(actor.Inventory);
            List<Ingredient> lotIngredients = actor.Household != null && actor.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(actor.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
            List<Ingredient> list = null;

            Recipe.MealTime currentMealTime = Food.GetCurrentMealTime();
            string[] menuPath = new string[]
                {
                    TeppanyakiGrill.LocalizeString("Serve") /*+ " " + Food.GetMealTimeString(currentMealTime)*/ + Localization.Ellipsis
                };
            foreach (TeppanyakiGrill.TGFoodInfo info in TeppanyakiGrill.TGFoodInfos)
            {
                Recipe current;
                if (Recipe.NameToRecipeHash.TryGetValue(info.mName, out current) && current.IsAvailableFor(currentMealTime))
                {
                    int cost = current.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count == 0 ? 0 : -1;
                    results.Add(new InteractionObjectPair(new OverridedTGCookDefinition(current.GenericName, current, menuPath, target, Recipe.MealDestination.SurfaceAndCallToMeal, Recipe.MealQuantity.Group, Recipe.MealRepetition.MakeOne, false, cost), iop.Target));
                }
            }
        }

        public override bool SpecificTest(Sim actor, TeppanyakiGrill target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (isAutonomous)
            {
                if (target.InUse)
                {
                    return false;
                }
                if (actor.IsSelectable)
                {
                    mTGFoodInfo = null;
                    Cooking skill = actor.SkillManager.GetSkill<Cooking>(SkillNames.Cooking);

                    List<Ingredient> simIngredients = Recipe.GetCookableIngredients(actor.Inventory);
                    List<Ingredient> lotIngredients = actor.Household != null && actor.Household.SharedFridgeInventory != null ? Recipe.GetCookableIngredients(actor.Household.SharedFridgeInventory.Inventory) : new List<Ingredient>();
                    List<Ingredient> list = null;
                    Recipe.MealTime time = Food.GetCurrentMealTime();

                    int num = RandomUtil.GetInt(7);
                    int num2 = 0;
                    Recipe recipe;
                    TeppanyakiGrill.TGFoodInfo tGFoodInfo;
                    while (true)
                    {
                        tGFoodInfo = TeppanyakiGrill.TGFoodInfos[num];
                        if (Recipe.NameToRecipeHash.TryGetValue(tGFoodInfo.mName, out recipe) && (recipe.CookingSkillLevelRequired == 0 || (skill != null && skill.SkillLevel >= recipe.CookingSkillLevelRequired))
                            && recipe.IsAvailableFor(time) && recipe.BuildIngredientList(simIngredients, lotIngredients, ref list, ref list).Count == 0)
                        {
                            break;
                        }
                        if (++num2 >= 8)
                        {
                            return false;
                        }
                        if (++num >= 8)
                        {
                            num = 0;
                        }
                    }
                    target.mCurrentRecipe = recipe;
                    ChosenRecipe = recipe;
                    mTGFoodInfo = tGFoodInfo;
                    return true;
                }
            }
            return base.SpecificTest(actor, target, isAutonomous, ref greyedOutTooltipCallback)
                && (Cost == 0 || Food.PrepareTestResultCheckAndGrayedOutPieMenuSet(actor, ChosenRecipe, Recipe.CanMakeFoodTestResult.Fail_NeedIngredients, ref greyedOutTooltipCallback));
        }
    }
}
