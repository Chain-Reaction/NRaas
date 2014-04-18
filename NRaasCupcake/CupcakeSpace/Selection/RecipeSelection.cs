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
    public class RecipeSelection : CommonSelection<RecipeSelection.Item>
    {
        public List<Recipe> selectedItems = new List<Recipe>();        

        // should be using a custom type here but this one has quite a bit of what I need so
        public class Item : ValueSettingOption<Recipe>
        {
            bool slotHasValue = false;
            string slotsHavingValue = null;

            public Item()
            { }
            public Item(string name, Recipe recipe)
                : base (recipe, name, 0)
            {
                SetThumbnail(recipe.GetThumbnailKey(ThumbnailSize.Large));
            }
            public Item(string name, Recipe recipe, bool has)
                : base(recipe, name, 0)
            {
                SetThumbnail(recipe.GetThumbnailKey(ThumbnailSize.Large));
                slotHasValue = has;
            }
            public Item(string name, Recipe recipe, string has)
                : base(recipe, name, 0)
            {
                SetThumbnail(recipe.GetThumbnailKey(ThumbnailSize.Large));
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

        private RecipeSelection(string title, ICollection<Item> items, ObjectPickerDialogEx.CommonHeaderInfo<Item> auxillary)
            : base(title, null, items, auxillary)
        {
        }

        public static RecipeSelection Create(List<string> selectedValues)
        {
            List<Item> options = new List<Item>();

            foreach (Recipe recipe in Recipe.Recipes)
            {
                options.Add(new Item(recipe.GenericName, recipe, selectedValues.Contains(recipe.Key)));                
            }

            RecipeSelection selection = new RecipeSelection(Common.Localize("SelectRecipes:ListTitle"), options, new AuxillaryColumn());

            return selection;
        }

        public static RecipeSelection Create(Dictionary<string, string> selectedValues)
        {
            List<Item> options = new List<Item>();

            foreach (Recipe recipe in Recipe.Recipes)
            {
                string selected = null;
                if (selectedValues.ContainsKey(recipe.SpecificNameKey))
                {
                    selected = selectedValues[recipe.SpecificNameKey];
                }

                options.Add(new Item(recipe.GenericName + (Cupcake.Settings.Debugging ? " (" + recipe.Key + ")" : ""), recipe, selected));                
            }

            RecipeSelection selection = new RecipeSelection(Common.Localize("SelectRecipes:ListTitle"), options, new AuxillaryColumn());

            return selection;
        }

        public static RecipeSelection CreateWithFilter(List<string> filter)
        {
            List<Item> options = new List<Item>();

            foreach (Recipe recipe in Recipe.Recipes)
            {
                if (filter.Contains(recipe.Key))
                {
                    options.Add(new Item(recipe.GenericName, recipe));
                }
            }

            RecipeSelection selection = new RecipeSelection(Common.Localize("SelectRecipes:ListTitle"), options, new AuxillaryColumn());
            
            return selection;
        }

        public OptionResult Perform()
        {
            RecipeSelection.Results results = this.SelectMultiple();

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

        // can merge this into a base class in the future...
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