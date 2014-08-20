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
    public class QualitySelection : CommonSelection<QualitySelection.Item>
    {
        public List<Quality> selectedItems = new List<Quality>();

        // should be using a custom type here but this one has quite a bit of what I need so
        public class Item : ValueSettingOption<Quality>
        {
            bool slotHasValue = false;
            string slotsHavingValue = null;

            public Item()
            { }
            public Item(string name, Quality quality, bool has)
                : base(quality, name, 0)
            {
                slotHasValue = has;
            }

            public Item(string name, Quality quality, string has)
                : base(quality, name, 0)
            {
                slotsHavingValue = has;
            }

            public string Auxillary
            {
                get
                {
                    if (slotHasValue)
                    {
                        return Cupcake.Localize("Value:Selected");
                    }

                    if (slotsHavingValue != null)
                    {
                        return slotsHavingValue;
                    }

                    return null;
                }
            }

            public override bool UsingCount
            {
                get { return false; }
            }
        }

        private QualitySelection(string title, ICollection<Item> items, ObjectPickerDialogEx.CommonHeaderInfo<Item> auxillary)
            : base(title, null, items, auxillary)
        {
        }

        public static QualitySelection Create(List<Quality> selectedValues)
        {
            List<Item> options = new List<Item>();

            foreach (Quality type in Enum.GetValues(typeof(Quality)))
            {                
                options.Add(new Item(type.ToString(), type, selectedValues.Contains(type)));
            }

            QualitySelection selection = new QualitySelection(Common.Localize("SelectQuality:ListTitle"), options, new AuxillaryColumn());

            return selection;
        }

        public static QualitySelection Create(Dictionary<Quality, string> selectedValues)
        {
            List<Item> options = new List<Item>();

            foreach (Quality type in Enum.GetValues(typeof(Quality)))
            {
                string selected = null;
                if(selectedValues.ContainsKey(type))
                {
                    selected = selectedValues[type];
                }

                options.Add(new Item(type.ToString(), type, selected));
            }

            QualitySelection selection = new QualitySelection(Common.Localize("SelectQuality:ListTitle"), options, new AuxillaryColumn());

            return selection;
        }

        public OptionResult Perform()
        {
            QualitySelection.Results results = this.SelectMultiple();

            if (results.Count == 0)
            {
                SimpleMessageDialog.Show(Common.Localize("Selection:Error"), Common.Localize("Selection:NoChoices"));
                return OptionResult.Failure;
            }

            foreach (Item item in results)
            {
                this.selectedItems.Add(item.Value);
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