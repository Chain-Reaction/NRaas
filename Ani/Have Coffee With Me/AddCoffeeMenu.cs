using System;
using System.Collections.Generic;
using System.Text;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.DebugGameplay;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.UI;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.Appliances;

[assembly: Tunable]
namespace HaveCoffeeWithMe
{
    public class AddCoffeeMenu
    {
        [Tunable]
        protected static bool kahviNuuttis;

        static AddCoffeeMenu()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);             
        }

        public static void AddInteractionsForCoffeeMaker(GameObject obj)
        {
            InteractionObjectPair i = new InteractionObjectPair(CoffeeInteractions.OverridedServeCoffee.Singleton, obj);
            if (!obj.Interactions.Contains(i))
            {
                obj.AddInteraction(CoffeeInteractions.OverridedServeCoffee.Singleton);
                obj.AddInteraction(CoffeeInteractions.OverridedMakeHotBeverage.MenuSingleton);
                obj.AddInteraction(CoffeeInteractions.OverridedMakeHotBeverage.NormalSingleton);
            }
        }

        public static void AddInteractionForTray(GameObject obj)
        {
            List<InteractionObjectPair> list = CommonMethods.ReturnInteractionIndex(obj.Interactions, "CallForCoffee");
            if (list == null || (list != null && list.Count == 0))
            {
                obj.AddInteraction(CoffeeInteractions.CallForCoffee.Singleton);
            }
        }


        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            List<HotBeverageMachine> coffeeMachines = new List<HotBeverageMachine>(Sims3.Gameplay.Queries.GetObjects<HotBeverageMachine>());

            foreach (HotBeverageMachine machine in coffeeMachines)
            {
                AddInteractionsForCoffeeMaker(machine);
            }

            EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(AddCoffeeMenu.OnNewObject));
            EventTracker.AddListener(EventTypeId.kBoughtObjectAtRabbithole, new ProcessEventDelegate(AddCoffeeMenu.OnNewObject));

            //Add the interaction to all trays if it's not there already
            List<BarTray> coffeeTrays = new List<BarTray>(Sims3.Gameplay.Queries.GetObjects<BarTray>());
            foreach (BarTray bt in coffeeTrays)
            {
                foreach (Slot slot in bt.GetContainmentSlots())
                {
                    Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup cup = bt.GetContainedObject(slot) as Sims3.Gameplay.Objects.Appliances.HotBeverageMachine.Cup;
                    if (cup != null && !cup.FoodName.Equals("Juice") && !cup.FoodName.Equals("Nectar"))
                    {
                        //Add cup interactions 
                        cup.RemoveAllInteractions();
                        cup.AddInteraction(DrinkCoffee.Drink.Singleton);

                        AddInteractionForTray(bt);
                    }
                }
            }
        }

        protected static ListenerAction OnNewObject(Event e)
        {
            HotBeverageMachine targetObject = e.TargetObject as HotBeverageMachine;
            if (targetObject != null)
            {
                AddInteractionsForCoffeeMaker(targetObject);
            }
            return ListenerAction.Keep;
        }



    }
}
