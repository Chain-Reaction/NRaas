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
using Sims3.Gameplay.Objects.Alchemy;
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
    public class ChangeAlchemyRecipes : SimFromList, IBooksOption
    {
        IEnumerable<Item> mChoices = null;

        public class Item : ValueSettingOption<AlchemyRecipe>
        {
            public Item()
            { }
            public Item(AlchemyRecipe recipe, int count)
                : base(recipe, recipe.Name, count)
            { }

            public override string DisplayKey
            {
                get { return "Knows"; }
            }
        }

        public override string GetTitlePrefix()
        {
            return "ChangeAlchemyRecipes";
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

            SpellcraftSkill skill = me.SkillManager.GetSkill<SpellcraftSkill>(SkillNames.Spellcraft);
            return (skill != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            SpellcraftSkill skill = me.SkillManager.GetSkill<SpellcraftSkill>(SkillNames.Spellcraft);

            if (skill == null)
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

                foreach (AlchemyRecipe recipe in AlchemyRecipe.AlchemyRecipes)
                {
                    if (skill.SkillLevel < recipe.SpellcraftSkillLevelRequired) continue;

                    int count = 0;
                    if (skill.KnownAlchemyRecipes.Contains(recipe))
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

                AlchemyRecipe recipe = item.Value;

                if (skill.KnownAlchemyRecipes.Contains(recipe))
                {
                    if (!item.IsSet) continue;

                    skill.KnownAlchemyRecipes.Remove(recipe);
                }
                else
                {
                    if (item.IsSet) continue;

                    skill.LearnARecipe(recipe);
                }
            }

            return true;
        }
    }
}
