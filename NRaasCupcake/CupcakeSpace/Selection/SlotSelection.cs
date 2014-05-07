using NRaas;
using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CupcakeSpace.Helpers;
using NRaas.CupcakeSpace.Options.Displays;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Selection
{
    // I swear I will figure out why these won't sort from 0-26 without drama soon :P
    public class SlotSelection : CommonSelection<SlotSelection.Item>        
    {        
        public List<int> selectedItems = new List<int>();
        // better than looping again...
        private Dictionary<int, Slot> containmentSlots = new Dictionary<int,Slot>();
        public bool all = false;

        // should be using a custom type here but this one has quite a bit of what I need so
        public class Item : ValueSettingOption<int>
        {
            Dictionary<string, List<Quality>> slotSettings;
            public int itemSlot;

            public Item()
            { }
            public Item(string name, int slot, Dictionary<string, List<Quality>> settings)
                : base(slot, name, 0)
            {
                itemSlot = slot;
                slotSettings = settings;
            }            

            public string Auxillary
            {
                get
                {
                    if (slotSettings != null)
                    {
                        Common.StringBuilder slotSetup = new Common.StringBuilder();
                        if (slotSettings.Count > 1)
                        {
                            slotSetup.Append(Common.Localize("General:Random") + ": " + Common.NewLine);
                        }

                        int loop = 0;
                        foreach (KeyValuePair<string, List<Quality>> recipeSetting in slotSettings)
                        {
                            loop++;
                            if (Recipe.NameToRecipeHash.ContainsKey(recipeSetting.Key))
                            {
                                Recipe recipe = Recipe.NameToRecipeHash[recipeSetting.Key];
                                slotSetup.Append(recipe.GenericName + Common.NewLine);

                                if (loop == slotSettings.Count)
                                {
                                    if (recipeSetting.Value.Count > 1)
                                    {
                                        slotSetup.Append(Common.Localize("General:Random") + ": " + Common.NewLine);
                                    }

                                    foreach (Quality quality in recipeSetting.Value)
                                    {
                                        slotSetup.Append(quality.ToString() + Common.NewLine);
                                    }
                                }
                            }
                            else
                            {
                                Common.Notify("Missing recipe " + recipeSetting.Key);
                            }
                        }

                        return slotSetup.ToString();
                    }

                    return null;
                }
            }

            public override bool UsingCount
            {
                get { return false; }
            }           
        }

        private SlotSelection(string title, ICollection<Item> items, ObjectPickerDialogEx.CommonHeaderInfo<Item> auxillary)
            : base (title, null, items, auxillary)
        {
        }

        public class SlotComparer : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                return x.itemSlot.CompareTo(y.itemSlot);                
            }
        }

        public static SlotSelection Create(GameObject mTarget)
        {
            List<Item> options = new List<Item>();

            options.Add(new Item(Common.Localize("General:All"), 100, null));            

            DisplayHelper.DisplayTypes displayType;
            Dictionary<int, Slot> containmentSlots = DisplayHelper.GetEmptyOrFoodSlots(mTarget as CraftersConsignmentDisplay, out displayType);

            if (displayType == DisplayHelper.DisplayTypes.Chiller)
            {
                options.Add(new Item(Common.Localize("General:AllButTop"), 101, null));
            }

            foreach (KeyValuePair<int, Slot> displaySlots in containmentSlots)
            {
                options.Add(new Item(Common.Localize("General:Slot") + " " + displaySlots.Key, displaySlots.Key, Cupcake.Settings.GetDisplaySettingsForSlot(mTarget.ObjectId, displaySlots.Key)));
            }           
            
            options.Sort(new SlotComparer());

            SlotSelection selection = new SlotSelection(Common.Localize("SelectSlots:ListTitle"), options, new AuxillaryColumn());
            selection.containmentSlots = containmentSlots;
            return selection;
        }

        public OptionResult Perform()
        {
            SlotSelection.Results results = this.SelectMultiple();

            if (results.Count == 0)
            {
                SimpleMessageDialog.Show(Common.Localize("Selection:Error"), Common.Localize("Selection:NoChoices"));
                return OptionResult.Failure;
            }            

            foreach (Item item in results)
            {
                if (item.Value < 99)
                {
                    this.selectedItems.Add(item.Value);
                }
                else
                {
                    if (item.Value == 100)
                    {
                        // all
                        this.selectedItems.AddRange(this.containmentSlots.Keys);
                    }
                    else
                    {
                        // all but top
                        foreach (int key in this.containmentSlots.Keys)
                        {
                            if (key < 21)
                            {
                                this.selectedItems.Add(key);
                            }
                        }
                    }
                    this.all = true;
                }
            }

            return OptionResult.SuccessClose;
        }       

        public class AuxillaryColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public AuxillaryColumn()
                : base("NRaas.Cupcake.OptionList:TypeTitle", "NRaas.Cupcake.OptionList:TypeTooltip", 40)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                return new ObjectPicker.TextColumn(item.Auxillary);
            }
        }
    }
}
