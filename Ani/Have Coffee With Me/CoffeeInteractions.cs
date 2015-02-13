using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.ObjectComponents;
using Sims3.SimIFace;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.CookingObjects;

namespace HaveCoffeeWithMe
{
    class CoffeeInteractions : HotBeverageMachine
    {
        #region Call For Coffee
        public class CallForCoffee : ImmediateInteraction<Sim, BarTray>
        {
            // Fields

            public static readonly InteractionDefinition Singleton = new Definition();

            private const string sLocalizationKey = "HaveCoffeeWithMe:";

            // Methods

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString(sLocalizationKey + name, parameters);
            }

            public override bool Run()
            {
                List<Sim> selectedSims = new List<Sim>();
                List<object> selectedObjects = base.SelectedObjects;
                if (selectedObjects != null)
                {
                    foreach (object obj2 in selectedObjects)
                    {
                        selectedSims.Add(obj2 as Sim);
                    }
                }


                //Loop through the sims and add them the coffee drinking interaction
                foreach (Sim sim in selectedSims)
                {
                    InteractionInstance ii = DrinkCoffeeInteraction.DrinkCoffee.Singleton.CreateInstance(base.Target, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                    sim.InteractionQueue.Add(ii);
                }


                return true;

            }

            // Nested Types

            private sealed class Definition : ImmediateInteractionDefinition<Sim, BarTray, CoffeeInteractions.CallForCoffee>
            {

                // Methods
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 4;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in parameters.Target.LotCurrent.GetSims())
                    {
                        if (sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override string GetInteractionName(Sim a, BarTray target, InteractionObjectPair interaction)
                {

                    return CoffeeInteractions.CallForCoffee.LocalizeString("CallForCoffee", new object[0]);

                }

                public override bool Test(Sim a, BarTray target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a.SimDescription.TeenOrAbove;
                }

            }

        }
        #endregion

        #region OverridedServeHotBeverages
        public class OverridedServeCoffee : ServeHotBeverages
        {
            // Fields
            private new BarTray BarTray;
            private new HotBeverageMachine.Cup Cup;
            public new static readonly InteractionDefinition Singleton = new Definition();
           // private const string sLocalizationKey = "Gameplay/Objects/Appliances/HotBeverageMachine/ServeHotBeverages";

            //Fields from coffee machine
            //private static int kDrinkHoursBeforeWakeupTime = 12;


            // Methods
            private new void FillTray(StateMachineClient smc, IEvent evt)
            {
                int num = this.BarTray.CountEmptySlots();
                for (int i = 0; i < num; i++)
                {
                    HotBeverageMachine.Cup item = this.MakeCup();

                    this.BarTray.AddItem(item);
                    item.StartEffects();
                }
            }

            //private static string LocalizeString(string name, params object[] parameters)
            //{
            //    return Localization.LocalizeString("Gameplay/Objects/Appliances/HotBeverageMachine/ServeHotBeverages:" + name, parameters);
            //}

            private new HotBeverageMachine.Cup MakeCup()
            {
                HotBeverageMachine.Cup cup = GlobalFunctions.CreateObject("CoffeeCup", (base.Target.Parent ?? base.Target).Position, base.Target.Level, Vector3.UnitZ) as HotBeverageMachine.Cup;
                cup.Contents = new HotBeverageMachine.CustomDrinkRecipe();


                cup.RemoveAllInteractions();
                cup.AddInteraction(DrinkCoffee.Drink.Singleton);

                return cup;
            }

         
            public override bool Run()
            {
                Counter parent = base.Target.Parent as Counter;
                if (parent == null)
                {
                    return false;
                }
                if (!SurfaceUtil.RouteToObjectOnSurface(parent, base.Actor, base.Target))
                {
                    return false;
                }
                base.StandardEntry();

                this.Cup = this.MakeCup() as HotBeverageMachine.Cup;
                this.Cup.SetHiddenFlags(HiddenFlags.Model);

                this.BarTray = GlobalFunctions.CreateObject("barTray", (base.Target.Parent ?? base.Target).Position, base.Target.Level, Vector3.UnitZ) as BarTray;
                this.BarTray.SetHiddenFlags(HiddenFlags.Model);
                this.BarTray.AddInteraction(CallForCoffee.Singleton);

                base.Target.mLastCreatedTrayOfDrinksId = this.BarTray.ObjectId;
                HotBeverageMachine.EnterStateMachine(this);
                base.AddOneShotScriptEventHandler(100, new SacsEventHandler(this.FillTray));
                base.SetActor("coffeeCup", this.Cup);
                base.SetActor("barTray", this.BarTray);
                base.BeginCommodityUpdates();
                base.AnimateSim("Make Many Drinks");
                base.EndCommodityUpdates(true);
                base.StandardExit();
                CarrySystem.VerifyAnimationParent(this.BarTray, base.Actor);
                CarrySystem.EnterWhileHolding(base.Actor, this.BarTray);
                CarrySystem.PutDown(base.Actor, SurfaceType.Normal, true);
                //if ((!base.Autonomous || !base.Actor.HasBuffsToPreventSleepiness()) && CarrySystem.PickUp(base.Actor, this.Cup))
                //{
                //    this.Cup.PushDrinkAsContinuation(base.Actor);
                //}
                base.mCurrentStateMachine.Dispose();
                this.Cup.StartEffects();
                if (parent.IsCleanable)
                {
                    parent.Cleanable.DirtyInc(base.Actor);
                }
                return true;
            }

            // Nested Types
            private new sealed class Definition : InteractionDefinition<Sim, HotBeverageMachine, OverridedServeCoffee>
            {
                //Methods
                public override string GetInteractionName(Sim actor, HotBeverageMachine target, InteractionObjectPair iop)
                {
                    if (!actor.HasTrait(TraitNames.PartyAnimal) || !Party.IsInvolvedInAnyTypeOfParty(actor))
                    {
                        if (actor.HasTrait(unchecked((TraitNames)(-5175480303478029664L))))
                        {
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("EvilInteractionName", new object[0]);
                        }
                        if (actor.HasTrait(unchecked((TraitNames)(-5175480303478029536L))))
                        {
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("GoodInteractionName", new object[0]);
                        }
                        if (actor.HasTrait(unchecked((TraitNames)(-5175480303478029728L))))
                        {
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("ExtremeInteractionName", new object[0]);
                        }
                        return CoffeeInteractions.OverridedServeCoffee.LocalizeString("ServeCoffee", new object[0]);
                    }
                    switch ((((int)SimClock.HoursPassedOfDay) % 5))
                    {
                        case 0:
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("EpicInteractionName", new object[0]);

                        case 1:
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("SweetInteractionName", new object[0]);

                        case 2:
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("KickingInteractionName", new object[0]);

                        case 3:
                            return CoffeeInteractions.OverridedServeCoffee.LocalizeString("RockingInteractionName", new object[0]);
                    }
                    return CoffeeInteractions.OverridedServeCoffee.LocalizeString("AwesomeInteractionName", new object[0]);

                }

                public override bool Test(Sim a, HotBeverageMachine target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.Parent is Counter && (!isAutonomous || (a.HoursUntilWakeupTime >= (float)HotBeverageMachine.kDrinkHoursBeforeWakeupTime && Simulator.GetProxy(target.mLastCreatedTrayOfDrinksId) == null));
                }
            }
        }
        #endregion

        #region Overrided make one cup
        public sealed class OverridedMakeHotBeverage : Interaction<Sim, HotBeverageMachine>
        {
            // Fields
            public static readonly InteractionDefinition MenuSingleton = new MenuDefinition();
            public static readonly InteractionDefinition NormalSingleton = new Definition();
            private const string sLocalizationKey = "Gameplay/Objects/Appliances/HotBeverageMachine/MakeHotBeverage";

            // Methods
            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Appliances/HotBeverageMachine/MakeHotBeverage:" + name, parameters);
            }

            public override bool Run()
            {
                Counter parent = base.Target.Parent as Counter;
                if (parent == null)
                {
                    return false;
                }
                if (!SurfaceUtil.RouteToObjectOnSurface(parent, base.Actor, base.Target))
                {
                    return false;
                }
                base.StandardEntry();
                HotBeverageMachine.Cup actor = GlobalFunctions.CreateObject("CoffeeCup", base.Target.Parent.Position, base.Target.Level, Vector3.UnitZ) as HotBeverageMachine.Cup;
                try
                {
                    actor.AddToUseList(base.Actor);
                    actor.Contents = (base.InteractionDefinition as Definition).Drink;
                    actor.SetHiddenFlags(HiddenFlags.Model);
                    HotBeverageMachine.EnterStateMachine(this);
                    base.SetActor("coffeeCup", actor);
                    base.BeginCommodityUpdates();
                    base.AnimateSim("Make One Drink");
                    base.EndCommodityUpdates(true);
                    CarrySystem.VerifyAnimationParent(actor, base.Actor);
                }
                finally
                {
                    actor.RemoveFromUseList(base.Actor);
                }

                //pay for drink
                CommonMethods.PayForCoffee(base.Actor, base.Target.LotCurrent);

                CarrySystem.EnterWhileHolding(base.Actor, actor);
                actor.PushDrinkAsContinuation(base.Actor);
                if (parent.IsCleanable)
                {
                    parent.Cleanable.DirtyInc(base.Actor);
                }
                base.mCurrentStateMachine.Dispose();
                actor.StartEffects();
                base.StandardExit();
                return true;
            }

            // Nested Types
            private class Definition : InteractionDefinition<Sim, HotBeverageMachine, OverridedMakeHotBeverage>
            {
                // Fields
                public HotBeverageMachine.DrinkRecipe Drink;

                // Methods
                public override string GetInteractionName(Sim a, HotBeverageMachine target, InteractionObjectPair interaction)
                {
                    string str;
                    if (!a.HasTrait(unchecked((TraitNames)(-5175480303478028976L))) || !Party.IsInvolvedInAnyTypeOfParty(a))
                    {
                        if (a.HasTrait(unchecked((TraitNames)(-5175480303478029664L))))
                        {
                            str = "DrinkInteractionName_Evil";
                        }
                        else if (a.HasTrait(unchecked((TraitNames)(-5175480303478029536L))))
                        {
                            str = "DrinkInteractionName_Good";
                        }
                        else if (a.SimDescription.TraitManager.HasElement(TraitNames.Daredevil))
                        {
                            str = "DrinkInteractionName_Daredevil";
                        }
                        else
                        {
                            str = "DrinkInteractionName";
                        }
                    }
                    else
                    {
                        switch ((((int)SimClock.HoursPassedOfDay) % 5))
                        {
                            case 0:
                                str = "RagingInteractionName";
                                goto Label_00C3;

                            case 1:
                                str = "SweetInteractionName";
                                goto Label_00C3;

                            case 2:
                                str = "EpicInteractionName";
                                goto Label_00C3;

                            case 3:
                                str = "KickingInteractionName";
                                goto Label_00C3;
                        }
                        str = "AwesomeInteractionName";
                    }
                Label_00C3:
                    this.Drink = HotBeverageMachine.GetFavoriteDrink(a.SimDescription);
                    return HotBeverageMachine.LocalizeString(str, new object[] { this.Drink.Name });
                }

                public override bool Test(Sim a, HotBeverageMachine target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous && (a.HoursUntilWakeupTime < 12))
                    {
                        return false;
                    }
                    if (!(target.Parent is Counter))
                    {
                        return false;
                    }
                    if (isAutonomous && a.HasBuffsToPreventSleepiness())
                    {
                        return false;
                    }
                    return true;
                }
            }

            [DoesntRequireTuning]
            private class ItemDefinition : OverridedMakeHotBeverage.Definition
            {
                // Methods
                public override string GetInteractionName(Sim a, HotBeverageMachine target, InteractionObjectPair interaction)
                {
                    return base.Drink.Name;
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[] { HotBeverageMachine.LocalizeString("TrySomethingElse", new object[0]) };
                }

                public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    InteractionInstanceParameters parameters2 = new InteractionInstanceParameters(new InteractionObjectPair(OverridedMakeHotBeverage.NormalSingleton, parameters.Target), parameters.Actor, parameters.Priority, parameters.Autonomous, parameters.CancellableByPlayer, parameters.Hit);
                    return OverridedMakeHotBeverage.NormalSingleton.Test(ref parameters2, ref greyedOutTooltipCallback);
                }

                public override bool Test(Sim a, HotBeverageMachine target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (target.Parent is Counter);
                }
            }

            [DoesntRequireTuning]
            private class MenuDefinition : OverridedMakeHotBeverage.Definition
            {
                // Methods
                public override void AddInteractions(InteractionObjectPair iop, Sim actor, HotBeverageMachine target, List<InteractionObjectPair> results)
                {
                    Dictionary<string, OverridedMakeHotBeverage.ItemDefinition> dictionary = new Dictionary<string, OverridedMakeHotBeverage.ItemDefinition>();
                    string name = HotBeverageMachine.GetFavoriteDrink(actor.SimDescription).Name;
                    while (dictionary.Count < HotBeverageMachine.TryAnotherRecipeCount)
                    {
                        OverridedMakeHotBeverage.ItemDefinition definition = new OverridedMakeHotBeverage.ItemDefinition();
                        definition.Drink = new HotBeverageMachine.CustomDrinkRecipe();
                        if (definition.Drink.Name != name)
                        {
                            dictionary[definition.Drink.Name] = definition;
                        }
                    }
                    foreach (InteractionDefinition definition2 in dictionary.Values)
                    {
                        results.Add(new InteractionObjectPair(definition2, target));
                    }
                }

                public override string GetInteractionName(Sim a, HotBeverageMachine target, InteractionObjectPair interaction)
                {
                    return base.Drink.Name;
                }

                public override bool Test(Sim a, HotBeverageMachine target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return false;
                }
            }
        }





        #endregion

    }
}
