using Sims3.SimIFace;
using System;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.EventSystem;
using System.Collections.Generic;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Appliances;

namespace ani_GroceryShopping
{
    public class AddMenuItem
    {
        [Tunable]
        protected static bool ShoppailuNuuttis;

        [Tunable]
        public static string FruitRecipe;

        [Tunable]
        public static string CheeseRecipe;

        [Tunable]
        public static string VegetableRecipe;

        [Tunable]
        public static string VampireRecipe;

        //[Tunable]
        //protected static int Profit;

        //public static int ReturnProfit()
        //{
        //    return Profit;
        //}

        // Methods
        static AddMenuItem()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            World.sOnLotAddedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            World.OnObjectPlacedInLotEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
        }

        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            List<Fridge> fList = new List<Fridge>(Sims3.Gameplay.Queries.GetObjects<Fridge>());
            foreach (Fridge f in fList)
            {
                AddInteractions(f);
            }

            List<Microwave> mList = new List<Microwave>(Sims3.Gameplay.Queries.GetObjects<Microwave>());
            foreach (Microwave m in mList)
            {
                AddInteractions(m);
            }

            List<FoodProcessor> fpList = new List<FoodProcessor>(Sims3.Gameplay.Queries.GetObjects<FoodProcessor>());
            foreach (FoodProcessor fp in fpList)
            {
                AddInteractions(fp);
            }

            List<Stove> sList = new List<Stove>(Sims3.Gameplay.Queries.GetObjects<Stove>());
            foreach (Stove s in sList)
            {
                AddInteractions(s);
            }

            List<Grill> gList = new List<Grill>(Sims3.Gameplay.Queries.GetObjects<Grill>());
            foreach (Grill g in gList)
            {
                AddInteractions(g);
            }

            //Add the interaction when buying a new object 
            EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(AddMenuItem.OnNewObject));
        }


        protected static ListenerAction OnNewObject(Event e)
        {
            GameObject o = e.TargetObject as GameObject;

            if (o != null)
                AddInteractions(o);
            
            return ListenerAction.Keep;
        }

        public static void AddInteractions(GameObject obj)
        {
            Grill grill = obj as Grill;
            Stove stove = obj as Stove;
            Fridge fridge = obj as Fridge;
            Microwave micro = obj as Microwave;            
            FoodProcessor processor = obj as FoodProcessor;

            if (obj != null && obj.Interactions != null)
            {
                if (fridge != null)
                {
                    InteractionObjectPair i2 = new InteractionObjectPair(OverridedFridge_Have.Singleton, obj);
                    if (!obj.Interactions.Contains(i2))
                    {
                        obj.RemoveInteractionByType(Fridge_Have.Singleton);
                       // obj.Interactions.RemoveAt(0);
                        obj.AddInteraction(OverridedFridge_Have.Singleton);
                        obj.AddInteraction(OverridedFridge_Prepare.PrepareSingleton);
                        
                    }
                }else if(micro != null)
                {
                    InteractionObjectPair i2 = new InteractionObjectPair(OverridedMicrowave_Have.Singleton, obj);
                    if (!obj.Interactions.Contains(i2))
                    {
                        obj.AddInteraction(OverridedMicrowave_Have.Singleton);

                    }
                }
                else if (processor != null)
                {
                    InteractionObjectPair i2 = new InteractionObjectPair(OverridedFoodProcessor_Have.Singleton, obj);
                    if (!obj.Interactions.Contains(i2))
                    {
                        obj.AddInteraction(OverridedFoodProcessor_Have.Singleton);
                    }
                }
                else if (stove != null)
                {
                    InteractionObjectPair i2 = new InteractionObjectPair(OverridedStove_Have.Singleton, obj);
                    if (!obj.Interactions.Contains(i2))
                    {
                        obj.AddInteraction(OverridedStove_Have.Singleton);
                    }
                }
                else if (grill != null)
                {
                    InteractionObjectPair i2 = new InteractionObjectPair(OverridedGrill_Have.Singleton, obj);
                    if (!obj.Interactions.Contains(i2))
                    {
                        obj.AddInteraction(OverridedGrill_Have.Singleton);
                    }
                }

            }
        }


    }
}
