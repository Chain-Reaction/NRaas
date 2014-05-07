using NRaas.CupcakeSpace.Helpers;
using Sims3.Gameplay.Abstracts;
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
                if (containedObject is IFoodContainer || containedObject is ServingContainer)
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
    }
}
