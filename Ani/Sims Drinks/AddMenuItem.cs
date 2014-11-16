using Sims3.SimIFace;
using Sims3.Gameplay.Objects;
using Sims3.UI;
using Sims3.Gameplay.Abstracts;
using System.Collections;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Autonomy;
using System.Collections.Generic;
using System;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ActorSystems;
using System.Text;

[assembly: Tunable]
namespace Alcohol
{
    public class AddMenuItem
    {
        [Tunable]
        protected static bool Nuuttis;
        [Tunable]
        protected static int DrinkPrice;
        [Tunable]
        protected static int BarFoodPrice;
        [Tunable]
        protected static bool ShowNotification;
        [Tunable]
        protected static bool DrunkFromNectar;


        public static bool ReturnShowNotification()
        {
            return ShowNotification;
        }

        // Methods
        static AddMenuItem()
        {
          //  LoadSaveManager.ObjectGroupsPreLoad += new ObjectGroupsPreLoadHandler(LoadBuffData);
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(AddMenuItem.OnWorldLoadFinishedHandler);
            
            
        }

        public static void LoadBuffData()
        {
            BuffHelper.Load("Ani_Drunk_Buffs");
          //  UIManager.NewHotInstallStoreBuffData = (UIManager.NewHotInstallStoreBuffCallback)Delegate.Combine(UIManager.NewHotInstallStoreBuffData, new UIManager.NewHotInstallStoreBuffCallback(Common.AddBuffs));
        }
      
        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            //Load the buffs
            LoadBuffData();

            EventTracker.AddListener(EventTypeId.kHadADrink, new ProcessEventDelegate(AddMenuItem.HadADrink));
            EventTracker.AddListener(EventTypeId.kJuiceKegPourJuice, new ProcessEventDelegate(AddMenuItem.HadADrink));
            EventTracker.AddListener(EventTypeId.kPlayedJuicePong, new ProcessEventDelegate(AddMenuItem.HadADrink));
            EventTracker.AddListener(EventTypeId.kJuiceKegPerformKegStand, new ProcessEventDelegate(AddMenuItem.HadADrink));  
            EventTracker.AddListener(EventTypeId.kOrderedFood, new ProcessEventDelegate(AddMenuItem.OrderedBarFood));

            if (DrunkFromNectar)
                EventTracker.AddListener(EventTypeId.kDrankNectar, new ProcessEventDelegate(AddMenuItem.HadNectar));
        }

        protected static ListenerAction HadADrink(Event e)
        {           
            //Push the drunk interaction on the Sim
            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                DrunkInteractions.PayAndConsequencs(sim, DrinkPrice, true, false);
            }
            
            return ListenerAction.Keep;
        }

        protected static ListenerAction HadNectar(Event e)
        {
            //Push the drunk interaction on the Sim
            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                DrunkInteractions.PayAndConsequencs(sim, DrinkPrice, true, true);
            }

            return ListenerAction.Keep;
        }


        protected static ListenerAction OrderedBarFood(Event e)
        {
            //Push the drunk interaction on the Sim
            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                DrunkInteractions.PayAndConsequencs(sim, BarFoodPrice, false, false);
            }
            return ListenerAction.Keep;
        }
    }
}
