using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class EatHereEx : RestaurantInteraction<EatHereEx>, IGroupAllowedRabbitholeInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Restaurant, EatHere.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Restaurant>(Singleton);
        }

        public override string GetInteractionName()
        {
            if (ActiveStage != null)
            {
                return ActiveStage.Name;
            }
            return LocalizeString("EatAtRestaurant", new object[] { Food.GetCurrentMealTimeString() });
        }

        public override bool BeforeEnteringRabbitHole()
        {
            try
            {
                List<Stage> list = new List<Stage>();
                TimedStage item = new TimedStage(LocalizeString("EatAtRestaurant", new object[] { Food.GetCurrentMealTimeString() }), Target.RestaurantTuning.EatLength, false, true, true);
                list.Add(item);
                switch (Food.GetCurrentMealTime())
                {
                    case Recipe.MealTime.Lunch:
                    case Recipe.MealTime.Dinner:
                        {
                            TimedStage stage2 = new EatHere.EatDessertStage(LocalizeString("StayForDessert", new object[0x0]), Target.RestaurantTuning.DessertLength, false, true, true);
                            list.Add(stage2);
                            break;
                        }
                }

                Stages = list;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return base.BeforeEnteringRabbitHole();
        }

        public override void Cleanup()
        {
            Food.PostEat(Actor, Target, mIsSufficientlyFullForStuffed, true, mHasFatDelta);
            base.Cleanup();
        }

        public override bool Run()
        {
            try
            {
                Definition interactionDefinition = base.InteractionDefinition as Definition;
                if ((interactionDefinition != null) && (interactionDefinition.EatingType == Restaurant.EatType.Review))
                {
                    GroupingSituation situationOfType = base.Actor.GetSituationOfType<GroupingSituation>();
                    if ((situationOfType != null) && !situationOfType.ConfirmLeaveGroup(base.Actor))
                    {
                        return false;
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return base.Run();
        }

        public override bool InRabbitHole()
        {
            try
            {
                mMealTime = Food.GetCurrentMealTime();
                mStartHour = SimClock.HoursPassedOfDay;

                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    AddMotiveArrow(CommodityKind.Hunger, true);
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, Restaurant>.InsideLoopFunction(LoopCallback), null);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                if (succeeded)
                {
                    bool flag2 = ActiveStage is Sims3.Gameplay.Objects.RabbitHoles.EatHere.EatDessertStage;
                    int buffStrength = flag2 ? Target.DessertBuffStrength : Target.BuffStrength;
                    float buffDuration = flag2 ? Target.DessertBuffDuration : Target.BuffDuration;
                    EatHere.AddMealBuffAndSolveHunger(Actor, buffStrength, buffDuration);
                    if (Actor.CarryingChildPosture != null)
                    {
                        EatHere.AddMealBuffAndSolveHunger(Actor.CarryingChildPosture.Child, buffStrength, buffDuration);
                    }
                    EventTracker.SendEvent(EventTypeId.kAteAtRestaurant, Actor, Target);
                    int normalCost = (mMealTime == Recipe.MealTime.Dinner) ? Target.RestaurantTuning.DinnerCost : Target.RestaurantTuning.BreakfastLunchBrunchCost;
                    if (flag2)
                    {
                        normalCost += Target.RestaurantTuning.DessertCost;
                    }
                    if ((mMealTime == Recipe.MealTime.Dinner) && (mStartHour < Target.RestaurantTuning.EarlyBirdTime))
                    {
                        normalCost = (int)(normalCost * Target.RestaurantTuning.EarlyBirdDiscount);
                    }
                    if (mEatingType == Restaurant.EatType.Review)
                    {
                        OmniCareer.Career<Journalism>(Actor.Occupation).RabbitHolesReviewed.Add(new Journalism.ReviewedRabbitHole(Target, ShowVenue.ShowTypes.kNoShow));
                    }
                    else
                    {
                        int costForSim = Target.GetCostForSim(Actor, normalCost);
                        AddCostAndBuffsForOtherSims(normalCost, ref costForSim, true, buffStrength, buffDuration);
                        ChargeBill(costForSim, Sims3.Gameplay.Objects.RabbitHoles.EatHere.LocalizeString(Actor.IsFemale, "CantAffordMeal", new object[] { Actor }));
                    }
                    if (Actor.HasTrait(TraitNames.NaturalCook) && RandomUtil.RandomChance01(TraitTuning.NaturalCookTraitChanceToLearnRecipeAtRestaurant))
                    {
                        Recipe recipe = Recipe.RandomRecipeOfSkillLevelRange(0x0, Actor.SkillManager.GetSkillLevel(SkillNames.Cooking), Actor);
                        if ((recipe != null) && recipe.Learn(Actor))
                        {
                            Actor.ShowTNSIfSelectable(Sims3.Gameplay.Objects.RabbitHoles.EatHere.LocalizeString(Actor.IsFemale, "NaturalCookLearnedRecipe", new object[] { recipe, Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                        }
                    }
                }
                TraitFunctions.TraitKleptomaniacStealFromRestaurant(Actor, Target, false);
                Target.OnFinishedEating(Actor, mIsSufficientlyFullForStuffed, mHasFatDelta);
                mbDoneEating = true;
                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        // Nested Types
        public class Definition : RestaurantInteraction<EatHereEx>.RestaurantInteractionDefinition
        {
            public Definition()
            { }

            public Definition(Restaurant.EatType eatType, string menuText, string[] menuPath, int costPerSim, Restaurant target)
                : base(eatType, menuText, menuPath, costPerSim, "EatWith", target)
            { }

            public override string GetInteractionName(Sim actor, Restaurant target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(Sims3.Gameplay.Objects.RabbitHoles.EatHere.Singleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Restaurant target, List<InteractionObjectPair> results)
            {
                try
                {
                    int breakfastLunchBrunchCost = target.RestaurantTuning.BreakfastLunchBrunchCost;
                    if (Food.GetCurrentMealTime() == Recipe.MealTime.Dinner)
                    {
                        breakfastLunchBrunchCost = target.RestaurantTuning.DinnerCost;
                    }
                    int costForSim = target.GetCostForSim(actor, breakfastLunchBrunchCost);

                    Journalism job = OmniCareer.Career<Journalism>(actor.Occupation);
                    if ((job != null) && job.CanReviewRabbitHole(target))
                    {
                        Definition definition = new Definition(Restaurant.EatType.Review, EatHere.LocalizeString("ReviewRestaurant", new object[0x0]), new string[] { EatHere.LocalizeString("EatHere", new object[0x0]) }, 0x0, target);
                        results.Add(new InteractionObjectPair(definition, target));
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                }
            }

            public override bool Test(Sim a, Restaurant target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!(a.Occupation is OmniCareer))
                {
                    return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
