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

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class ChangeLearnedDrinks : SimFromList, IAdvancedOption
    {
        IEnumerable<Item> mChoices = null;

        public override string GetTitlePrefix()
        {
            return "ChangeLearnedDrinks";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
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

            Bartending tending = me.SkillManager.GetSkill<Bartending>(SkillNames.Bartending);
            return (tending != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Bartending tending = me.SkillManager.GetSkill<Bartending>(SkillNames.Bartending);

            if (tending == null) return false;

            if (!ApplyAll)
            {
                List<Item> allOptions = new List<Item>();

                allOptions.Add(null);

                foreach (KeyValuePair<string,Bartending.DrinkData> data in Bartending.sDrinks)
                {
                    if (!data.Value.IsLearnable) continue;

                    int count = 0;
                    if (!tending.mNumberOfDataDrinksMade.TryGetValue(data.Key, out count))
                    {
                        count = 0;
                    }

                    allOptions.Add(new Item(data.Value, data.Key, count));
                }

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectMultiple();
                if ((selection == null) || (selection.Count == 0)) return false;

                CommonSelection<Item>.HandleAllOrNothing(selection);

                mChoices = selection;
            }

            foreach (Item item in mChoices)
            {
                if (item == null) continue;

                if (tending.mNumberOfDataDrinksMade.ContainsKey(item.Value))
                {
                    if (!item.IsSet) continue;

                    tending.mNumberOfDataDrinksMade.Remove(item.Value);
                }
                else
                {
                    if (item.IsSet) continue;

                    tending.mNumberOfDataDrinksMade.Add(item.Value, 1);

                    tending.mDataDrinkLearnChances.Remove(item.Value);
                }
            }

            return true;
        }

        public class Item : ValueSettingOption<string>
        {
            public Item()
            { }
            public Item(Bartending.DrinkData drink, string key, int count)
                : base(key, drink.GetLocalizedName(), count, drink.GetThumbnailKey())
            { }
        }
    }
}
