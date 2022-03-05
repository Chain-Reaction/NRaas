using Sims3.SimIFace;
using System;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.EventSystem;
using System.Collections.Generic;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Store.Objects;

namespace ani_GroceryShopping
{
    public class AddMenuItem
    {
        [Tunable]
        protected static bool ShoppailuNuuttis;

        //[Tunable]
        //public static string FruitRecipe;

        //[Tunable]
        //public static string CheeseRecipe;

        //[Tunable]
        //public static string VegetableRecipe;

        //[Tunable]
        //public static string VampireRecipe;

        [Tunable]
        public static string[] SnackRequirements = new string[0];

        //[Tunable]
        //protected static int Profit;

        //public static int ReturnProfit()
        //{
        //    return Profit;
        //}

        private static bool PreLoaded = false;

        private static ulong BuildBuyLotId;

        public static List<Type> ApplianceTypes = new List<Type>{ typeof(Fridge), typeof(Stove), typeof(Microwave), typeof(FoodProcessor), typeof(Grill) };

        // Methods
        static AddMenuItem()
        {
            LoadSaveManager.ObjectGroupsPreLoad += new ObjectGroupsPreLoadHandler(OnPreLoad);
            World.OnWorldLoadFinishedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            //World.OnLotAddedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            //World.OnObjectPlacedInLotEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
            LotManager.EnteringBuildBuyMode += () => BuildBuyLotId = LotManager.sActiveBuildBuyLot.LotId;
            LotManager.ExitingBuildBuyMode += new VoidEventHandler (OnExitBuildBuyMode);
        }

        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            foreach (Type type in ApplianceTypes)
            {
                AddInteractionsToObjects(type, null);
            }

            //Add the interaction when buying a new object 
            //EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(AddMenuItem.OnNewObject));
            }


        /*protected static ListenerAction OnNewObject(Event e)
            {
            GameObject o = e.TargetObject as GameObject;

            AddInteractions(o);
            
            return ListenerAction.Keep;
        }*/

        public static void OnExitBuildBuyMode()
        {
            Lot lot = LotManager.GetLot(BuildBuyLotId);
            if (lot != null)
            {
                foreach (Type type in ApplianceTypes)
                {
                    AddInteractionsToObjects(type, lot);
                }
            }
            }

        private static void OnPreLoad()
        {
            if (!PreLoaded)
            {
                PreLoaded = true;
                InjectTuning<Fridge, Fridge_Have.Definition, OverridedFridge_Have.Definition>();
                InjectTuning<Fridge, Fridge_Prepare.PrepareDefinition, OverridedFridge_Prepare.PrepareDefinition>();
                InjectTuning<Stove, Stove_Have.Definition, OverridedStove_Have.Definition>();
                InjectTuning<Microwave, Microwave_Have.Definition, OverridedMicrowave_Have.Definition>();
                InjectTuning<Grill, Grill_Have.Definition, OverridedGrill_Have.Definition>();
                InjectTuning<FoodProcessor, FoodProcessor.FoodProcessor_Have.Definition, OverridedFoodProcessor_Have.Definition>();
                if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                    InteractionTuning tuning = AutonomyTuning.GetTuning (typeof(MakeGourmetFoodForPet.Definition), "Sims3.Gameplay.Objects.Appliances.MakeGourmetFoodForPet+Definition", typeof(Fridge));
                    if (tuning != null)
                    {
                        tuning.LoadFieldDisallowAutonomous(true);
                    }
            }

                if (Recipe.NameToRecipeHash.ContainsKey("WOBakeBreadCountry"))
            {
                    ApplianceTypes.Add(typeof(WoodFireOven));
                    InjectTuning<WoodFireOven, WoodFireOven.WOBake.Definition, OverridedWOBakeDefinition>();
                }
                if (Recipe.NameToRecipeHash.ContainsKey("TGCookTeppanyakiSalmon"))
                {
                    ApplianceTypes.Add(typeof(TeppanyakiGrill));
                    InjectTuning<TeppanyakiGrill, TeppanyakiGrill.TGCook.Definition, OverridedTGCookDefinition>();
            }

                char[] separator = new char[]{ ':' };
                foreach (string current in SnackRequirements)
                {
                    string[] array = current.Split (separator, 2);
                    string key = array [0].Trim ();
                    Recipe recipe;
                    if (Recipe.NameToRecipeHash.TryGetValue(key, out recipe) && recipe.Ingredient1 == null)
            {
                        recipe.mNonPersistableData.mIngredient1 = recipe.InitIngredient(array[1].Trim());
                    }
                }
            }
        }

        public static void AddInteractionsToObjects(Type type, Lot lot)
        {
            bool onLoadup = lot == null;
            Array array = onLoadup ? Sims3.SimIFace.Queries.GetObjects(type) : Sims3.Gameplay.Queries.GetObjects(type, lot);
            if (array != null)
            {
                foreach (object obj in array)
        {
                    AddInteractions(obj as GameObject, !onLoadup);
                }
            }
        }

        public static void AddInteractions(GameObject obj, bool checkForDup)
        {
			if (obj != null && obj.Interactions != null)
            {
				if (obj is Fridge)
                {
                    ReplaceInteraction<Fridge_Have.Definition>(obj, OverridedFridge_Have.Singleton, checkForDup);
                    ReplaceInteraction<Fridge_Prepare.PrepareDefinition>(obj, OverridedFridge_Prepare.PrepareSingleton, checkForDup);
                }
				else if (obj is Microwave)
                {
                    ReplaceInteraction<Microwave_Have.Definition>(obj, OverridedMicrowave_Have.Singleton, checkForDup);
                }
				else if (obj is FoodProcessor)
                {
                    ReplaceInteraction<FoodProcessor.FoodProcessor_Have.Definition>(obj, OverridedFoodProcessor_Have.Singleton, checkForDup);
                }
				else if (obj is Stove)
                {
                    ReplaceInteraction<Stove_Have.Definition>(obj, OverridedStove_Have.Singleton, checkForDup);
                }
				else if (obj is Grill)
                {
                    ReplaceInteraction<Grill_Have.Definition>(obj, OverridedGrill_Have.Singleton, checkForDup);
                }
                else if (obj is WoodFireOven)
                {
                    ReplaceInteraction<WoodFireOven.WOBake.Definition>(obj, OverridedWOBakeDefinition.Singleton, checkForDup);
                }
                else if (obj is TeppanyakiGrill)
                {
                    ReplaceInteraction<TeppanyakiGrill.TGCook.Definition>(obj, OverridedTGCookDefinition.Singleton, checkForDup);
                }

            }
        }

        public static void ReplaceInteraction<DEF>(GameObject obj, InteractionDefinition newDef, bool checkForDup) where DEF : InteractionDefinition
        {
            obj.RemoveInteractionByType(typeof(DEF));
            obj.AddInteraction(newDef, checkForDup);
        }

        public static void InjectTuning<Target, OldType, NewType>() where Target : GameObject where OldType : InteractionDefinition where NewType : InteractionDefinition
        {
            InteractionTuning interactionTuning = AutonomyTuning.GetTuning (typeof(OldType), typeof(OldType).FullName, typeof(Target));
            if (interactionTuning != null) 
            {
                AutonomyTuning.AddTuning (typeof(NewType).FullName, typeof(Target).FullName, interactionTuning);
            }
        }
    }
}