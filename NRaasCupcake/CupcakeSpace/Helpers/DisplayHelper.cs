using NRaas.CupcakeSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Store.Objects;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CupcakeSpace.Helpers
{
    public class DisplayHelper
    {
        public enum DisplayTypes
        {
            Chiller,
            Rack
        }

        public static DisplayTypes GetDisplayType(CraftersConsignmentDisplay display)
        {
            Slot[] displaySlots = display.GetContainmentSlots();

            DisplayTypes displayType;
            // Yay to brappl for helping me nail this one
            int length = displaySlots.Length - 1;
            if (displaySlots[length].ToString() != "TransformBone")
            {
                if (displaySlots.Length == 26)
                {
                    displayType = DisplayTypes.Chiller;
                }
                else
                {
                    displayType = DisplayTypes.Rack;
                }
            }
            else
            {
                if (displaySlots.Length == 27)
                {
                    displayType = DisplayTypes.Chiller;
                }
                else
                {
                    displayType = DisplayTypes.Rack;
                }
            }

            return displayType;
        }

        public static Dictionary<int, Slot> GetEmptyOrFoodSlots(CraftersConsignmentDisplay display, out DisplayTypes displayType)
        {
            Dictionary<int, Slot> slots = new Dictionary<int, Slot>();
            
            Slot[] displaySlots = display.GetContainmentSlots();
            for (int i = 0; i < displaySlots.Length; i++)
            {
                if (i > 25 && displaySlots[i].ToString() == "TransformBone")
                {
                    break;
                }

                GameObject containedObject = display.GetContainedObject(displaySlots[i]) as GameObject;

                if (containedObject is FailureObject)
                {
                    try
                    {
                        containedObject.Destroy();
                    }
                    catch { }
                }

                if (containedObject is IFoodContainer || containedObject is ServingContainer || containedObject == null)
                {
                    slots.Add(i, displaySlots[i]);                    
                }
            }

            displayType = GetDisplayType(display);            

            return slots;
        }

        public static string ExtractRecipeKeyFromSlot(CraftersConsignmentDisplay display, int num, Slot slot, out Quality quality)
        {
            GameObject containedObject = display.GetContainedObject(slot) as GameObject;
            if (containedObject != null)
            {
                if (containedObject is IFoodContainer) //|| containedObject is ServingContainer)
                {
                    ServingContainer container = containedObject as ServingContainer;
                    if (container != null)
                    {
                        CookingProcess containerConfig = container.CookingProcess;
                        if (containerConfig != null)
                        {
                            quality = containerConfig.Quality;
                            return containerConfig.RecipeKey;
                        }
                        else
                        {
                            Common.Notify("Unable to find recipe for food in slot " + num + ". The catalog name was " + containedObject.CatalogName);
                        }
                    }
                    else
                    {
                        Common.Notify("Encountered food that was unidentifyable in slot " + num + ". The catalog name was " + containedObject.CatalogName);
                    }
                }
            }

            quality = Quality.Any;
            return null;
        }

        public static void ParentToSlot(GameObject obj, Slot slot, GameObject parent)
        {
            if (obj != null && parent != null)
            {
                obj.SetPosition(parent.GetPositionOfSlot(slot));
                obj.SetForward(parent.GetForwardOfSlot(slot));                
                obj.ParentToSlot(parent, slot);                
            }
        }

        public static List<ObjectGuid> GetObjectsICanBuyInDisplay(Sim actor, CraftersConsignment display)
        {
            List<ObjectGuid> list = new List<ObjectGuid>();
            if (!display.Charred)
            {
                Slot[] containmentSlots = display.GetContainmentSlots();
                for (int i = 0; i < containmentSlots.Length; i++)
                {
                    GameObject containedObject = display.GetContainedObject(containmentSlots[i]) as GameObject;
                    if (TestIfObjectCanBeBoughtByActor(containedObject, actor))
                    {
                        list.Add(containedObject.ObjectId);
                    }
                }
            }
            return list;
        }

        public static bool TestIfObjectCanBeBoughtByActor(IGameObject obj, Sim actor)
        {
            /*ServingContainer container = obj as ServingContainer;
            if (container != null)
            {                
                return container.AmountLeft == AmountLeftState.Full;
            }
 
            Snack snackContainer = obj as Snack;            
            return ((snackContainer != null) && (snackContainer.HasFoodLeft()));*/
			PreparedFood food = obj as PreparedFood;
			return food != null && food.HasFoodLeft ();
        }

        public static int ComputeFinalPriceOnObject(ObjectGuid targetGuid)
        {
            return ComputeFinalPriceOnObject(targetGuid, false);
        }

        public static int ComputeFinalPriceOnObject(ObjectGuid targetGuid, bool singleServingOnly)
        {
            /*int finalPrice = 0;
            int basePrice = 0;
            GameObject obj2 = GlobalFunctions.ConvertGuidToObject<GameObject>(targetGuid);
            BasePriceFinalPriceDiff(obj2, singleServingOnly, out finalPrice, out basePrice);
            return finalPrice;*/
            return ComputeFinalPriceOnObject(GlobalFunctions.ConvertGuidToObject<GameObject>(targetGuid), singleServingOnly);
        }

        public static int ComputeFinalPriceOnObject(GameObject obj, bool singleServingOnly)
        {
            int finalPrice;
            int basePrice;
            BasePriceFinalPriceDiff(obj, singleServingOnly, out finalPrice, out basePrice);
            return finalPrice;
        }

        public static int BasePriceFinalPriceDiff(GameObject obj, bool singleServingOnly, out int FinalPrice, out int BasePrice)
        {
            CraftersConsignment display = obj.Parent as CraftersConsignment;

            if (display == null)
            {
                BasePrice = 0;
                FinalPrice = 0;
                return 0;
            }

            BasePrice = 0;
            if (obj != null)
            {
                ServingContainer container = obj as ServingContainer;
                if (container != null)
                {
                    float kSingleServingBasePrice = 0f;
                    if (singleServingOnly || container is ISingleServingContainer)
                    {
                        kSingleServingBasePrice = CraftersConsignment.kSingleServingBasePrice;
                    }
                    else
                    {
                        kSingleServingBasePrice = CraftersConsignment.kGroupServingBasePrice;
                    }
                    kSingleServingBasePrice *= CraftersConsignment.kFoodQualityMuliplier[QualityHelper.GetQualityIndex(container.GetQuality())];
                    BasePrice = (int)kSingleServingBasePrice;
                }

                Snack snackContainer = obj as Snack;
                if (snackContainer != null)
                {
                    float kSingleServingBasePrice = CraftersConsignment.kSingleServingBasePrice;
                    kSingleServingBasePrice *= CraftersConsignment.kFoodQualityMuliplier[QualityHelper.GetQualityIndex(Quality.Perfect)];
                    BasePrice = (int)kSingleServingBasePrice;
                }
            }
            float num2 = CraftersConsignment.ConvertMarkupToPercent(display.mMarkup);
            float num3 = ((float)BasePrice) * num2;
            num3 -= num3 * display.mSaleDiscount;
            FinalPrice = (int)num3;
            return (BasePrice - FinalPrice);
        }
    }
}
