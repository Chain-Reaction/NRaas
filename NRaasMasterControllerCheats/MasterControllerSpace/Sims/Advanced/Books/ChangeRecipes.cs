using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Books
{
    public class ChangeRecipes : SimFromList, IBooksOption
    {
        IEnumerable<Item> mChoices = null;

        public class Item : ValueSettingOption<Recipe>
        {
            public Item()
            { }
            public Item(Recipe recipe, int count)
                : base(recipe, recipe.GenericName, count)
            { }

            public override string DisplayKey
            {
                get { return "Knows"; }
            }
        }

        public override string GetTitlePrefix()
        {
            return "ChangeRecipes";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override void Reset()
        {
            mChoices = null;

            base.Reset();
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.SkillManager == null) return false;

            Cooking cooking = me.SkillManager.GetSkill<Cooking>(SkillNames.Cooking);
            return (cooking != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Cooking cooking = me.SkillManager.GetSkill<Cooking>(SkillNames.Cooking);

            if (cooking == null)
            {
                if (singleSelection)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix () + ":Prompt", me.IsFemale, new object[] { me }));
                }
                return false;
            }

            if (!ApplyAll)
            {
                List<Item> allOptions = new List<Item>();

                allOptions.Add(null);

                foreach (Recipe recipe in Recipe.Recipes)
                {
                    if (recipe.IsSnack) continue;

                    if (!recipe.Learnable) continue;

                    if (cooking.SkillLevel < recipe.CookingSkillLevelRequired) continue;

                    int count = 0;
                    if (cooking.KnownRecipes.Contains(recipe.Key))
                    {
                        count = 1;
                    }

                    allOptions.Add(new Item(recipe, count));
                }

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectMultiple();
                if ((selection == null) || (selection.Count == 0)) return false;

                CommonSelection<Item>.HandleAllOrNothing(selection);

                mChoices = selection;
            }

            foreach (Item item in mChoices)
            {
                if (item == null) continue;

                Recipe recipe = item.Value;

                if (cooking.KnownRecipes.Contains(recipe.Key))
                {
                    if (!item.IsSet) continue;

                    cooking.KnownRecipes.Remove(recipe.Key);
                }
                else
                {
                    if (item.IsSet) continue;

                    recipe.Learn (null, cooking);
                }
            }

            return true;
        }
    }
}
