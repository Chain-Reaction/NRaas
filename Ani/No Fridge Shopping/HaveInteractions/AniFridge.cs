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
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.ObjectComponents;

namespace ani_GroceryShopping
{
    #region OverridedFridge_Have
    public class OverridedFridge_Have : Interaction<Sim, Fridge>, ICookingInteraction, IPetWatchableFoodInteraction
    {
        public class Definition : OverridedFoodMenuInteractionDefinition<Fridge, OverridedFridge_Have>
        {
            public Definition()
            {
            }
            public Definition(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
                : base(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost)
            {
            }
            protected override OverridedFoodMenuInteractionDefinition<Fridge, OverridedFridge_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                return new OverridedFridge_Have.Definition(menuText, recipe, menuPath, objectClickedOn, destination, quantity, repetition, bWasHaveSomething, cost);
            }
            public override void AddInteractions(InteractionObjectPair iop, Sim sim, Fridge fridge, List<InteractionObjectPair> results)
            {
                if (sim == null)
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
                        InteractionObjectPair item = new InteractionObjectPair(this.Create(current.GenericName, current, menuPath, null, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, 0), iop.Target);
                        results.Add(item);
                    }
                }
            }
            protected override bool SpecificTest(Sim a, Fridge target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.FridgeInventory == null && target.LotCurrent != null && !target.LotCurrent.IsCommunityLot)
                {
                    return false;
                }
                if (isAutonomous && a.SimDescription.IsFrankenstein)
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

                return !target.InUse;
            }
            static Definition()
            {
                Maid.AddInappropriateAction(new Maid.ActionFinder(OverridedFridge_Have.Definition.TryFindSnack), false);
            }
            private static InteractionObjectPair TryFindSnack(MaidSituation situation)
            {
                Fridge[] objects = Sims3.Gameplay.Queries.GetObjects<Fridge>(situation.Lot);
                if (objects == null || objects.Length == 0)
                {
                    return null;
                }
                List<InteractionObjectPair> list = new List<InteractionObjectPair>();
                Fridge randomObjectFromList = RandomUtil.GetRandomObjectFromList<Fridge>(objects);
                if (randomObjectFromList != null)
                {
                    bool lotHasCounter = Food.LotHasUsableCounter(randomObjectFromList.LotCurrent);
                    bool lotHasStove = Food.LotHasUsableStove(randomObjectFromList.LotCurrent);
                    bool lotHasMicrowave = Food.LotHasUsableMicrowave(randomObjectFromList.LotCurrent);
                    bool lotHasGrill = Food.LotHasUsableGrill(randomObjectFromList.LotCurrent);
                    foreach (Recipe current in Recipe.Snacks)
                    {
                        if ((!current.CookingProcessData.UsesAMicrowave || !situation.Worker.SimDescription.ChildOrBelow) && current.DoesLotHaveRightTech(lotHasCounter, lotHasStove, lotHasMicrowave, lotHasGrill, Recipe.MealQuantity.Single) == Recipe.CanMakeFoodTestResult.Pass)
                        {
                            InteractionObjectPair item = new InteractionObjectPair((OverridedFridge_Have.Singleton as OverridedFridge_Have.Definition).Create(current.GenericName, current, null, null, Recipe.MealDestination.SurfaceOrEat, Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, false, 0), randomObjectFromList);
                            list.Add(item);
                        }
                    }
                }
                if (list.Count == 0)
                {
                    return null;
                }
                return RandomUtil.GetRandomObjectFromList<InteractionObjectPair>(list);
            }
        }
        private const string sLocalizationKey = "Gameplay/Objects/Appliances/Fridge_Have";
        public Recipe ChosenRecipe;
        public GameObject ObjectClickedOn;
        public Recipe.MealDestination Destination;
        public Recipe.MealQuantity Quantity = Recipe.MealQuantity.Single;
        public Recipe.MealRepetition Repetition;
        public string MenuText;
        public string[] MenuPath;
        protected ImpassableRegion mImpassableRegion = new ImpassableRegion();
        protected bool mbWasHaveSomething;
        private IPartOfCookingProcess mThingToPrepareOrEat;
        public static readonly InteractionDefinition Singleton = new OverridedFridge_Have.Definition();
        public bool IsPreparingGroupServing
        {
            get
            {
                return this.Quantity == Recipe.MealQuantity.Group;
            }
        }
        public bool Watchable
        {
            get
            {
                PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
                return petWatchableCookingHelper != null && petWatchableCookingHelper.Watchable;
            }
        }
        public bool AllowsThrowScrap
        {
            get
            {
                return false;
            }
        }
        public bool IsCooking
        {
            get
            {
                return true;
            }
        }
        public int NumWatchingPets
        {
            get
            {
                PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
                if (petWatchableCookingHelper != null)
                {
                    return petWatchableCookingHelper.NumWatchingPets;
                }
                return 0;
            }
        }
        private static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Gameplay/Objects/Appliances/Fridge_Have:" + name, parameters);
        }
        private bool ShouldCookFor(Sim sim)
        {
            return sim.Household == this.Actor.Household || sim.IsGreetedOnLot(this.Actor.LotHome);
        }
        protected virtual void Initialize(ref InteractionInstanceParameters parameters)
        {
            OverridedFridge_Have.Definition definition = parameters.InteractionDefinition as OverridedFridge_Have.Definition;
            this.MenuText = definition.MenuText;
            this.MenuPath = definition.MenuPath;
            this.mbWasHaveSomething = definition.WasHaveSomething;
            this.Destination = definition.Destination;
            this.Quantity = definition.Quantity;
            if (definition.ChosenRecipe == null)
            {
                if (base.Autonomous && this.Actor.SimDescription.IsVampire)
                {
                    this.ChosenRecipe = Recipe.NameToRecipeHash["VampireJuice"];
                }
                else
                {
                    Lot lotCurrent = parameters.Actor.LotCurrent;
                    if (lotCurrent != null && lotCurrent.GetSims(new Predicate<Sim>(this.ShouldCookFor)).Count > 1)
                    {
                        this.Quantity = Recipe.MealQuantity.Group;
                    }
                    this.ChosenRecipe = Food.ChooseRecipeRandomly(parameters.Target.LotCurrent, parameters.Actor as Sim, parameters.Autonomous, null, this.Quantity);
                    if (!this.ChosenRecipe.CanMakeGroupServing && this.Quantity == Recipe.MealQuantity.Group)
                    {
                        this.Quantity = Recipe.MealQuantity.Single;
                    }
                }
                this.ObjectClickedOn = (parameters.Target as GameObject);
                return;
            }
            this.ChosenRecipe = definition.ChosenRecipe;
            this.ObjectClickedOn = definition.ObjectClickedOn;
            this.Repetition = definition.Repetition;
        }
        public override void Init(ref InteractionInstanceParameters parameters)
        {
            base.Init(ref parameters);
            this.Initialize(ref parameters);
        }
        public override bool OnLoadFixup()
        {
            bool flag = base.OnLoadFixup();
            if (flag && this.ChosenRecipe != null)
            {
                flag = this.ChosenRecipe.OnLoadFixup();
            }
            return flag;
        }
        public override string GetInteractionName()
        {
            return OverridedFridge_Have.LocalizeString("HaveInteractionName", new object[]
			{
				this.Actor.HasTrait(TraitNames.Vegetarian) ? this.ChosenRecipe.GenericVegetarianName : this.ChosenRecipe.GenericName
			});
        }
        public override void CleanupAfterExitReason()
        {
            if (this.Actor.IsHoldingAnything())
            {
                if (this.Actor.CarryStateMachine == null)
                {
                    CarrySystem.EnterWhileHolding(this.Actor, this.Actor.GetObjectInRightHand() as ICarryable);
                }
                Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
            }
            base.CleanupAfterExitReason();
        }
        public override void Cleanup()
        {
            this.mImpassableRegion.Cleanup();
            this.mImpassableRegion = null;
            base.Cleanup();
        }
        public override bool Run()
        {
            try
            {
                if (this.CheckForCancelAndCleanup())
                {
                    return false;
                }
                if (!this.Target.RouteToOpen(this, true))
                {
                    return false;
                }
                if (this.Target.InUse)
                {
                    this.Actor.AddExitReason(ExitReason.RouteFailed);
                    return false;
                }
                this.mImpassableRegion.AddMember(this.Actor);
                this.mImpassableRegion.AddMember(this.Target);
                this.mImpassableRegion.UpdateFootprint();
                base.StandardEntry();
                if (this.Actor.SimDescription.TeenOrAbove && !this.ChosenRecipe.IsSnack)
                {
                    this.Actor.SkillManager.AddElement(SkillNames.Cooking);
                }
                bool flag = true;
                List<Ingredient> ingredientsUsed = new List<Ingredient>();

                if (AniRecipe.UseUpIngredientsFrom(this.ChosenRecipe, this.Actor, ref ingredientsUsed, this.Quantity, this.ChosenRecipe.IsSnack) || this.Actor.IsNPC)
                {
                    //If the food is a snack, remove ingredient 
                    if (this.ChosenRecipe.IsSnack && !this.Actor.IsNPC)
                    {
                        Recipe snack = AniRecipe.ReturnSnackIngredientRecipe(this.Actor, this.ChosenRecipe);
                        if (snack != null)
                        {
                            //Create new temp ingredient list  
                            foreach (var item in ingredientsUsed)
                            {
                                item.Destroy();
                            }
                            ingredientsUsed.Clear();
                        }
                    }

                    //CommonMethods.PrintMessage("Snack: " + this.ChosenRecipe.IsSnack + " / " + ingredientsUsed.Count.ToString());

                    Fridge.EnterStateMachine(this);
                    IRemovableFromFridgeAsInitialRecipeStep removableFromFridgeAsInitialRecipeStep = GlobalFunctions.CreateObjectOutOfWorld(this.ChosenRecipe.ObjectToCreateInFridge, this.ChosenRecipe.ModelCodeVersion) as IRemovableFromFridgeAsInitialRecipeStep;
                    GameObject gameObject = removableFromFridgeAsInitialRecipeStep as GameObject;
                    gameObject.AddToUseList(this.Actor);
                    try
                    {
                        this.Target.PutOnFridgeShelf(gameObject);
                        this.mThingToPrepareOrEat = (removableFromFridgeAsInitialRecipeStep as IPartOfCookingProcess);
                        this.mThingToPrepareOrEat.CookingProcess = new CookingProcess(this.ChosenRecipe, ingredientsUsed, this.ObjectClickedOn, this.Target.LotCurrent, this.Destination, this.Quantity, this.Repetition, this.MenuText, this.MenuPath, removableFromFridgeAsInitialRecipeStep as IPartOfCookingProcess, this.Actor);
                        removableFromFridgeAsInitialRecipeStep.InitializeForRecipe(this.ChosenRecipe, false);
                        CookingProcess.MoveToNextStep(removableFromFridgeAsInitialRecipeStep as IPartOfCookingProcess, this.Actor);
                        base.SetActor(removableFromFridgeAsInitialRecipeStep.ActorNameForFridge, gameObject);
                        if (this.mbWasHaveSomething)
                        {
                            base.AnimateSim("Ponder");
                        }
                        base.AnimateSim("Remove - " + removableFromFridgeAsInitialRecipeStep.ActorNameForFridge);
                        this.TriggerWatchCookingReactionBroadcaster();
                    }
                    catch (Exception ex)
                    {
                        if (ex == null)
                        {
                        }
                        gameObject.Destroy();
                        throw;
                    }
                    CarrySystem.EnterWhileHolding(this.Actor, removableFromFridgeAsInitialRecipeStep, false);
                    if (this.CheckForCancelAndCleanup())
                    {
                        return false;
                    }
                    if (this.Actor.HasTrait(TraitNames.NaturalCook))
                    {
                        TraitTipsManager.ShowTraitTip(13271263770231522448uL, this.Actor, TraitTipsManager.TraitTipCounterIndex.NaturalCook, TraitTipsManager.kNaturalCookCountOfMealsCooked);
                    }
                    if (this.Actor.HasTrait(TraitNames.Vegetarian))
                    {
                        TraitTipsManager.ShowTraitTip(13271263770231522928uL, this.Actor, TraitTipsManager.TraitTipCounterIndex.Vegetarian, TraitTipsManager.kVegetarianCountOfMealsCooked);
                    }
                    this.PushNextInteraction(removableFromFridgeAsInitialRecipeStep, gameObject);
                    base.AnimateSim("Exit - Standing");
                }
                else
                {
                    flag = false;
                }

                base.StandardExit();
                if (flag)
                {
                    ActiveTopic.AddToSim(this.Actor, "Has Made Food");
                }
                return flag;
            }
            catch (Exception ex)
            {
                CommonMethods.PrintMessage("Fridge: " + ex.Message);
                return false;
            }
           
        }
        private PetWatchableFoodInteractionHelper GetPetWatchableCookingHelper()
        {
            if (this.mThingToPrepareOrEat != null && this.mThingToPrepareOrEat.CookingProcess != null)
            {
                return this.mThingToPrepareOrEat.CookingProcess.PetWatchableCookingHelper;
            }
            return null;
        }
        public void RegisterPetWatching(Sim pet)
        {
            PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
            if (petWatchableCookingHelper != null)
            {
                petWatchableCookingHelper.RegisterPetWatching(pet);
            }
        }
        public void UnregisterPetWatching(Sim pet)
        {
            PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
            if (petWatchableCookingHelper != null)
            {
                petWatchableCookingHelper.UnregisterPetWatching(pet);
            }
        }
        protected void TriggerWatchCookingReactionBroadcaster()
        {
            PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
            if (petWatchableCookingHelper != null)
            {
                petWatchableCookingHelper.TriggerReactionBroadcaster(this.Actor);
            }
        }
        public void RegisterUserDirectedThrowScrap(Sim pet)
        {
            PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
            if (petWatchableCookingHelper != null)
            {
                petWatchableCookingHelper.RegisterUserDirectedThrowScrap(pet);
            }
        }
        public List<Sim> GetWatchingPets()
        {
            PetWatchableFoodInteractionHelper petWatchableCookingHelper = this.GetPetWatchableCookingHelper();
            if (petWatchableCookingHelper != null)
            {
                return petWatchableCookingHelper.GetWatchingPets();
            }
            return new List<Sim>();
        }
        protected virtual void PushNextInteraction(IRemovableFromFridgeAsInitialRecipeStep thingToEatOrPrepare, GameObject thingToEatOrPrepareAsScriptObject)
        {
            this.Actor.InteractionQueue.PushAsContinuation(thingToEatOrPrepare.FollowupInteraction, thingToEatOrPrepareAsScriptObject, true);
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
            protected override OverridedFoodMenuInteractionDefinition<Fridge, OverridedFridge_Have> Create(string menuText, Recipe recipe, string[] menuPath, GameObject objectClickedOn, Recipe.MealDestination destination, Recipe.MealQuantity quantity, Recipe.MealRepetition repetition, bool bWasHaveSomething, int cost)
            {
                menuPath = new string[]
				{
					Localization.LocalizeString("Gameplay/Objects/FoodObjects:PrepareMenuPath", new object[0])
				};
                return new OverridedFridge_Prepare.PrepareDefinition(menuText, recipe, menuPath, objectClickedOn, Recipe.MealDestination.SurfaceOnly, Recipe.MealQuantity.Group, repetition, bWasHaveSomething, cost);
            }
        }
        public static readonly InteractionDefinition PrepareSingleton = new OverridedFridge_Prepare.PrepareDefinition();
        public override bool Run()
        {
            return base.Run();
        }
    }
    #endregion

}
