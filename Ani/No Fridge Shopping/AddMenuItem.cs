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
            World.OnWorldLoadFinishedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            World.OnLotAddedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            World.OnObjectPlacedInLotEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
        }

        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            //List<Fridge> fList = new List<Fridge>(Sims3.Gameplay.Queries.GetObjects<Fridge>());
			foreach (Fridge f in Sims3.Gameplay.Queries.GetObjects<Fridge>())
            {
                AddInteractions(f);
            }

            //List<Microwave> mList = new List<Microwave>(Sims3.Gameplay.Queries.GetObjects<Microwave>());
			foreach (Microwave m in Sims3.Gameplay.Queries.GetObjects<Microwave>())
            {
                AddInteractions(m);
            }

            //List<FoodProcessor> fpList = new List<FoodProcessor>(Sims3.Gameplay.Queries.GetObjects<FoodProcessor>());
			foreach (FoodProcessor fp in Sims3.Gameplay.Queries.GetObjects<FoodProcessor>())
            {
                AddInteractions(fp);
            }

            //List<Stove> sList = new List<Stove>(Sims3.Gameplay.Queries.GetObjects<Stove>());
			foreach (Stove s in Sims3.Gameplay.Queries.GetObjects<Stove>())
            {
                AddInteractions(s);
            }

            //List<Grill> gList = new List<Grill>(Sims3.Gameplay.Queries.GetObjects<Grill>());
			foreach (Grill g in Sims3.Gameplay.Queries.GetObjects<Grill>())
            {
                AddInteractions(g);
            }

            //Add the interaction when buying a new object 
            EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(AddMenuItem.OnNewObject));
        }


        protected static ListenerAction OnNewObject(Event e)
        {
            GameObject o = e.TargetObject as GameObject;

            //if (o != null)
                AddInteractions(o);
            
            return ListenerAction.Keep;
        }

        public static void AddInteractions(GameObject obj)
        {
            //Grill grill = obj as Grill;
            //Stove stove = obj as Stove;
            //Fridge fridge = obj as Fridge;
            //Microwave micro = obj as Microwave;            
            //FoodProcessor processor = obj as FoodProcessor;

            if (obj != null && obj.Interactions != null)
            {
				if (obj is Fridge) //(fridge != null)
                {
                    //InteractionObjectPair i2 = new InteractionObjectPair(OverridedFridge_Have.Singleton, obj);
                    //if (!obj.Interactions.Contains(i2))
                   // {
						obj.RemoveInteractionByType(typeof(Fridge_Have.Definition));
						obj.RemoveInteractionByType (typeof(Fridge_Prepare.PrepareDefinition));
                       // obj.Interactions.RemoveAt(0);
                        obj.AddInteraction(OverridedFridge_Have.Singleton, true);
                        obj.AddInteraction(OverridedFridge_Prepare.PrepareSingleton, true);
                        
                    //}
                }
				else if (obj is Microwave)//(micro != null)
                {
                    //InteractionObjectPair i2 = new InteractionObjectPair(OverridedMicrowave_Have.Singleton, obj);
                    //if (!obj.Interactions.Contains(i2))
                    //{
						obj.RemoveInteractionByType (typeof(Microwave_Have.Definition));
                        obj.AddInteraction(OverridedMicrowave_Have.Singleton, true);

                    //}
                }
				else if (obj is FoodProcessor)//(processor != null)
                {
                    //InteractionObjectPair i2 = new InteractionObjectPair(OverridedFoodProcessor_Have.Singleton, obj);
                    //if (!obj.Interactions.Contains(i2))
                    //{
						obj.RemoveInteractionByType (typeof(FoodProcessor.FoodProcessor_Have.Definition));
                        obj.AddInteraction(OverridedFoodProcessor_Have.Singleton, true);
                    //}
                }
				else if (obj is Stove)//(stove != null)
                {
                    //InteractionObjectPair i2 = new InteractionObjectPair(OverridedStove_Have.Singleton, obj);
                    //if (!obj.Interactions.Contains(i2))
                    //{
						obj.RemoveInteractionByType (typeof(Stove_Have.Definition));
                        obj.AddInteraction(OverridedStove_Have.Singleton, true);
                    //}
                }
				else if (obj is Grill)//(grill != null)
                {
                    //InteractionObjectPair i2 = new InteractionObjectPair(OverridedGrill_Have.Singleton, obj);
                    //if (!obj.Interactions.Contains(i2))
                    //{
						obj.RemoveInteractionByType (typeof(Grill_Have.Definition));
                        obj.AddInteraction(OverridedGrill_Have.Singleton, true);
                    //}
                }

            }
        }


    }
}
