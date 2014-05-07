using NRaas.CupcakeSpace;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Utilities;
using Sims3.Store.Objects;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CupcakeSpace.Helpers
{
    public class BakeryController
    {
        private static List<Recipe> goodies = new List<Recipe>();        
        private static List<Quality> qualites = new List<Quality>();

        private static void InitLists()
        {
            if (Cupcake.Settings.mRandomRestockSettings.Count == 0)
            {
                foreach (Recipe recipe in Recipe.Recipes)
                {
                    if (!recipe.SpecificNameKey.Contains("BSBake") && !recipe.SpecificNameKey.Contains("pie") && !recipe.SpecificNameKey.Contains("cookie") && !recipe.SpecificNameKey.Contains("brownie") && !recipe.SpecificNameKey.Contains("cake"))
                    {
                        continue;
                    }

                    if (recipe.Key != "BSBakeWeddingCake" && recipe.Key != "Wedding Cake Slice" && recipe.Key != "Pancakes" && recipe.Key != "WeddingCakeSliceDOT07" && recipe.SingleServingContainer != "PlateChild" && GameUtils.IsInstalled(recipe.CodeVersion))
                    {                        
                        goodies.Add(recipe);
                    }
                }
            }
           
            foreach (Quality quality in Enum.GetValues(typeof(Quality)))
            {
                if (quality != Quality.Any)
                {
                    qualites.Add(quality);
                }
            }
        }

        public static void UnlockRecipes()
        {
            BakersStation.BSFoodInfos.Clear();

            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSBread, BakersStation.BSFoodType.BSWhiteBread, "BSBakeWhiteBread", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSBread, BakersStation.BSFoodType.BSRyeBread, "BSBakeRyeBread", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSBread, BakersStation.BSFoodType.BSSourdough, "BSBakeSourdough", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSBread, BakersStation.BSFoodType.BSWheatBread, "BSBakeWheatBread", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSBread, BakersStation.BSFoodType.BSBaguette, "BSBakeBaguette", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCake, BakersStation.BSFoodType.BSWeddingCake, "BSBakeWeddingCake", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCake, BakersStation.BSFoodType.BSBirthdayCake, "BSBakeBirthdayCake", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCookies, BakersStation.BSFoodType.BSSugarCookie, "BSBakeSugarCookie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCookies, BakersStation.BSFoodType.BSOatmealRaisinCookie, "BSBakeOatmealRaisinCookie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCookies, BakersStation.BSFoodType.BSPeanutButterCookie, "BSBakePeanutButterCookie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCookies, BakersStation.BSFoodType.BSChocolateChipCookie, "BSBakeChocolateChipCookie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSAppleCobbler, "BSBakeAppleCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSApplePie, "BSBakeApplePie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSLifeFruitPie, "BSBakeLifeFruitPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSBannaCreamPie, "BSBakeBannaCreamPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCupcake, BakersStation.BSFoodType.BSCinnamonRoll, "BSBakeCinnamonRoll", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSBread, BakersStation.BSFoodType.BSDinnerRoll, "BSBakeDinnerRoll", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCupcake, BakersStation.BSFoodType.BSCupcakes, "BSBakeCupcakes", "Default", 0));

            // EP 1
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSPlumCobbler, "BSBakePlumCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSCherryCobbler, "BSBakeCherryCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSCherryPie, "BSBakeCherryPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSPomegranatePie, "BSBakePomegranatePie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSPomegranatePie, "BSBakePomegranatePie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSPlumPie, "BSBakePlumPie", "Default", 0));

            // EP 3 | 7 (EA didn't even unlock this for EP7, surprise
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSPlasmaCobbler, "BSBakePlasmaCobbler", "Default", 0));

            // Store Garden set
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSStrawberryCobbler, "BSBakeStrawberryCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSRaspberryCobbler, "BSBakeRaspberryCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSPearCobbler, "BSBakePearCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSStrawberryPie, "BSBakeStrawberryPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSRaspberryPie, "BSBakeRaspberryPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSBlueberryCobbler, "BSBakeBlueberryCobbler", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSRazelberryPie, "BSBakeRazelberryPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSBlueberryPie, "BSBakeBlueberryPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSPearPie, "BSBakePearPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSPeachPie, "BSBakePeachPie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSChocolatePie, "BSBakeChocolatePie", "Default", 0));
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSPecanPie, "BSBakePecanPie", "Default", 0));

            // Grandpa's grove store
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSPie, BakersStation.BSFoodType.BSOrangePie, "BSBakeOrangePie", "Default", 0));

            // Al Fresco Marketplace
            BakersStation.BSFoodInfos.Add(new BakersStation.BSFoodInfo(BakersStation.BSBaseFoodType.BSCobbler, BakersStation.BSFoodType.BSLemonCobbler, "BSBakeLemonCobbler", "Default", 0));
        }

        public static void RefillDisplays()
        {
            if (Cupcake.Settings.mAutoRestock)
            {
                Common.StringBuilder msg = new Common.StringBuilder();
                Common.StringBuilder displayMsg;
                foreach (CraftersConsignmentDisplay display in Sims3.Gameplay.Queries.GetObjects<CraftersConsignmentDisplay>())
                {                    
                    RestockDisplay(display, out displayMsg);
                    msg += displayMsg + Common.NewLine + Common.NewLine;
                }
                Common.DebugWriteLog(msg);
                msg = null;
                displayMsg = null;
                goodies.Clear();
                qualites.Clear();
            }
            else
            {
                Common.DebugNotify("Auto restock disabled.");
            }
        }

        public static void RestockDisplay(CraftersConsignmentDisplay display, out Common.StringBuilder debug)
        {
            debug = new Common.StringBuilder("Display: " + display.CatalogName + Common.NewLine + "ObjectID:" + display.ObjectId);
            if (qualites.Count == 0)
            {
                InitLists();
            }

            List<int> slotsToSkipForWeddingCakeSetupOnChiller = new List<int> { 23, 25 };
            List<int> slotsToSkipForWeddingCakeSetupOnRack = new List<int> { 0, 2, 4 };
            List<int> slotsForWeddingCakesChiller = new List<int> { 21, 22, 24 };
            List<int> slotsForWeddingCakesOnRack = new List<int> { 1, 3 };
            Recipe randomRestockRecipe = null;

            if (display.LotCurrent != null)
            {
                debug += Common.NewLine + "LotCurrent: " + display.LotCurrent.Name;
            }

            if (!display.InWorld)
            {
                debug += Common.NewLine + "Display not in world";
                return;
            }

            if (Cupcake.Settings.IsDisplayExempt(display.ObjectId))
            {
                debug += Common.NewLine + "Display has auto restock disabled";
                return;
            }

            bool random = false;
            if (!Cupcake.Settings.HasSettings(display.ObjectId))
            {
                debug += Common.NewLine + "Display has no user defined settings.";
                random = true;
            }

            if (!Cupcake.Settings.mAffectActive && random)
            {
                if (display.LotCurrent == null)
                {
                    debug += Common.NewLine + "LotCurrent null";
                    return;
                }

                if (display.LotCurrent.LotId == Household.ActiveHousehold.LotId)
                {
                    debug += Common.NewLine + "On active household lot";
                    return;
                }

                List<PropertyData> list = RealEstateManager.AllPropertiesFromAllHouseholds();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && display.LotCurrent.LotId == list[i].LotId && list[i].Owner != null && list[i].Owner.OwningHousehold == Household.ActiveHousehold)
                    {
                        debug += Common.NewLine + "On owned lot";
                        return;
                    }
                }
            }

            DisplayHelper.DisplayTypes displayType;
            Dictionary<int, Slot> slots = DisplayHelper.GetEmptyOrFoodSlots(display, out displayType);
            foreach (KeyValuePair<int, Slot> slot in slots)
            {
                debug += Common.NewLine + "Slot: " + slot.Key;
                if (displayType == DisplayHelper.DisplayTypes.Chiller)
                {
                    if (!Cupcake.Settings.SlotHasSettings(display.ObjectId, slot.Key))
                    {
                        if (slot.Key > 20 && !Cupcake.Settings.mStockWeddingCakes)
                        {
                            debug += Common.NewLine + "Wedding cakes disabled, skipping top shelf";
                            continue;
                        }

                        if (Cupcake.Settings.mStockWeddingCakes && slotsToSkipForWeddingCakeSetupOnChiller.Contains(slot.Key))
                        {
                            debug += Common.NewLine + "Skipping slots for presentable wedding cake setup";
                            continue;
                        }
                    }
                }

                if (displayType == DisplayHelper.DisplayTypes.Rack)
                {
                    if (Cupcake.Settings.mStockWeddingCakes && slotsToSkipForWeddingCakeSetupOnRack.Contains(slot.Key) && !Cupcake.Settings.SlotHasSettings(display.ObjectId, slot.Key))
                    {
                        debug += Common.NewLine + "Skipping slots for presentable wedding cake setup";
                        continue;
                    }
                }

                GameObject containedObject = display.GetContainedObject(slot.Value) as GameObject;
                if (containedObject == null)
                {
                    Dictionary<string, List<Quality>> settings = Cupcake.Settings.GetDisplaySettingsForSlot(display.ObjectId, slot.Key);

                    Recipe recipe = null;
                    IFoodContainer container = null;
                    Quality quality = Quality.Perfect;
                    if (random && !Cupcake.Settings.mDisableRandomAutoRestock && (!Cupcake.Settings.mOneRecipePerDisplayOnRandom || (Cupcake.Settings.mOneRecipePerDisplayOnRandom && randomRestockRecipe == null)))
                    {
                        debug += Common.NewLine + "Random";
                        if (Cupcake.Settings.mRandomRestockSettings.Count > 0)
                        {
                            if (Cupcake.Settings.Debugging)
                            {
                                debug += Common.NewLine + "Choices:";
                                foreach (KeyValuePair<string, List<Quality>> val in Cupcake.Settings.mRandomRestockSettings)
                                {
                                    debug += Common.NewLine + val.Key;
                                    foreach (Quality val2 in val.Value)
                                    {
                                        debug += Common.NewLine + val2.ToString();
                                    }
                                }
                            }

                            if (Recipe.NameToRecipeHash.Count > 0)
                            {
                                string pick = RandomUtil.GetRandomObjectFromList<string>(new List<string>(Cupcake.Settings.mRandomRestockSettings.Keys));
                                if (Recipe.NameToRecipeHash.ContainsKey(pick))
                                {
                                    recipe = Recipe.NameToRecipeHash[pick];

                                    debug += Common.NewLine + "Fetching random recipe...";
                                    debug += Common.NewLine + "Pick: " + recipe.Key;

                                    quality = RandomUtil.GetRandomObjectFromList<Quality>(Cupcake.Settings.mRandomRestockSettings[pick]);
                                    debug += Common.NewLine + "Fetching random quality...";
                                    debug += Common.NewLine + "Pick: " + quality.ToString();
                                }
                                else
                                {
                                    debug += Common.NewLine + "Failed to find defined recipe";
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (goodies.Count > 0)
                            {
                                recipe = RandomUtil.GetRandomObjectFromList<Recipe>(goodies);
                                debug += Common.NewLine + "Fetching random bakery recipe...";
                                debug += Common.NewLine + "Pick: " + recipe.SpecificNameKey;
                                debug += Common.NewLine + "Quality: Always Perfect";
                            }
                        }

                        randomRestockRecipe = recipe;
                    }

                    if (random && Cupcake.Settings.mOneRecipePerDisplayOnRandom && randomRestockRecipe != null)
                    {
                        debug += Common.NewLine + "OneRecipePerDisplayOnRandom = true" + Common.NewLine + "Last Recipe: " + randomRestockRecipe.GenericName;
                        recipe = randomRestockRecipe;
                    }

                    if (settings != null)
                    {                       
                        debug += Common.NewLine + "Reading user settings...";

                        if (Cupcake.Settings.Debugging)
                        {
                            debug += Common.NewLine + "Choices:";
                            foreach (KeyValuePair<string, List<Quality>> val in settings)
                            {
                                debug += Common.NewLine + val.Key;
                                foreach (Quality val2 in val.Value)
                                {
                                    debug += Common.NewLine + val2.ToString();
                                }
                            }
                        }

                        string pick = "";
                        if (settings.Count > 0)
                        {
                            List<string> tempList = new List<string>();
                            tempList.AddRange(settings.Keys);                            

                            pick = RandomUtil.GetRandomObjectFromList<string>(tempList);

                            if (Recipe.NameToRecipeHash.ContainsKey(pick))
                            {
                                recipe = Recipe.NameToRecipeHash[pick];
                                debug += Common.NewLine + "Fetching random recipe...";
                                debug += Common.NewLine + "Pick: " + recipe.Key;
                            }
                            else
                            {
                                debug += Common.NewLine + "Failed to find defined recipe: " + pick;
                                continue;
                            }
                        }
                        else
                        {
                            debug += Common.NewLine + "Settings for slot was 0 count.";
                            continue;
                        }

                        quality = RandomUtil.GetRandomObjectFromList<Quality>(Cupcake.Settings.mDisplayRestockSettings[display.ObjectId][slot.Key][pick]);
                        debug += Common.NewLine + "Fetching random quality...";
                        debug += Common.NewLine + "Pick: " + quality.ToString();
                    }

                    if (random && Cupcake.Settings.mStockWeddingCakes && !Cupcake.Settings.SlotHasSettings(display.ObjectId, slot.Key))
                    {
                        List<string> cakes = new List<string> { "BSBakeWeddingCake", "WeddingCakeSliceDOT07" };

                        if (GameUtils.IsInstalled(ProductVersion.EP4))
                        {
                            cakes.Add("Wedding Cake Slice");
                        }

                        if ((displayType == DisplayHelper.DisplayTypes.Chiller && slotsForWeddingCakesChiller.Contains(slot.Key)) || (displayType == DisplayHelper.DisplayTypes.Rack && slotsForWeddingCakesOnRack.Contains(slot.Key)))
                        {
                            debug += Common.NewLine + "Wedding cake slot";
                            recipe = null;
                            while (recipe == null)
                            {
                                string pick = RandomUtil.GetRandomObjectFromList<string>(cakes);
                                if (!Recipe.NameToRecipeHash.TryGetValue(pick, out recipe))
                                {
                                    // for folks with out of date mods
                                    if (pick == "BSBakeWeddingCake")
                                    {
                                        break;
                                    }
                                }                                
                            }
                        }                        
                    }                    

                    if (recipe != null)
                    {
                        if (quality == Quality.Any)
                        {
                            // EA standard apparently doesn't handle this correctly...
                            quality = RandomUtil.GetRandomObjectFromList<Quality>(qualites);
                        }

                        // Naturally EA didn't include group model definition for these 2 and it causes explosions
                        IGameObject cake = null;                        
                        if (recipe.Key == "WeddingCakeSliceDOT07")
                        {
                            debug += Common.NewLine + "Attempt at Monte Vista cake";
                            cake = GlobalFunctions.CreateObjectOutOfWorld("foodServeCakeWeddingDOT07", ProductVersion.BaseGame);
                            if (cake == null)
                            {
                                cake = GlobalFunctions.CreateObjectOutOfWorld("foodServeCakeWeddingDOT07", ~ProductVersion.Undefined);
                            }                            
                        }
                        else if (recipe.Key == "Wedding Cake Slice")
                        {
                            debug += Common.NewLine + "Attempt at Generations cake";
                            cake = GlobalFunctions.CreateObjectOutOfWorld("foodServeCakeWeddingTraditional", ProductVersion.EP4);                                                     
                        }
                        else
                        {
                            container = recipe.CreateFinishedFood(recipe.CanMakeGroupServing ? Recipe.MealQuantity.Group : Recipe.MealQuantity.Single, quality);
                        }

                        if (cake != null && !(cake is FailureObject))
                        {
                            debug += Common.NewLine + "Wedding cake success";
                            DisplayHelper.ParentToSlot(cake as GameObject, slot.Value, display);
                            cake.AddToWorld();

                            WeddingCake cake2 = cake as WeddingCake;
                            if (cake2 != null)
                            {
                                cake2.RemoveInteractionByType(WeddingCake.CutWeddingCake.Singleton);
                            }
                        }
                        else
                        {
                            debug += Common.NewLine + "Wedding cake fail";
                        }

                        if (container != null)
                        {
                            DisplayHelper.ParentToSlot(container as GameObject, slot.Value, display);
                            container.SetGeometryState(recipe.SingleServingContainer); // this is how EA sets it, don't ask                            
                            container.AddToWorld();

                            ServingContainer container2 = container as ServingContainer;
                            if (container2 != null)
                            {
                                int[] numArray = new int[] { 0, 0, 0, 0, 0, 0, 15, 30, 0x2d, 60, 0x4b, 100, 0x65 };
                                container2.CookingProcess.FoodPoints = numArray[(int)quality];
                                container2.CookingProcess.FoodState = FoodCookState.Cooked;
                                container2.FoodCookStateChanged(container2.CookingProcess.FoodState);

                                // EA fail
                                container2.RemoveInteractionByType(ServingContainerGroup.CallToMeal.Singleton);
                            }                            

                            // snack handling... needs a lot more
                            Snack container3 = container as Snack;
                            if (container3 != null)
                            {
                                GameObject container4 = container as GameObject;
                                if (container4 != null)
                                {
                                    container4.RemoveInteractionByType(CraftersConsignment.ChildObjectBrowseStub.Singleton);
                                    container4.RemoveInteractionByType(CraftersConsignment.ChildObjectPurchaseStub.Singleton);
                                    container4.AddInteraction(CraftersConsignment.ChildObjectBrowseStub.Singleton);
                                    container4.AddInteraction(CraftersConsignment.ChildObjectPurchaseStub.Singleton);

                                    container4.RemoveInteractionByType(Sims3.Gameplay.Objects.CookingObjects.Eat.Singleton);
                                    container4.RemoveInteractionByType(Snack_CleanUp.Singleton);
                                }

                                ISpoilable spoil = container as ISpoilable;
                                if (spoil != null)
                                {
                                    spoil.UpdateSpoilageTime(true, -1f);
                                }
                            }

                            debug += Common.NewLine + "Success: " + recipe.GenericName + Common.NewLine + quality.ToString();
                        }
                    }
                }
                else
                {
                    debug += Common.NewLine + "Slot contained object: " + containedObject.CatalogName;
                }
            }
            display.AddInteractionsToChildObjects();
        }
    }
}

