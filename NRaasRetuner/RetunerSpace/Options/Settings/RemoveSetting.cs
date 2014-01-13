using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.Settings
{
    public class RemoveSetting : OperationSettingOption<GameObject>, ISettingsOption
    {
        public override string GetTitlePrefix()
        {
            return "Remove";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Item> choices = new List<Item>();

            foreach (SettingsKey key in Retuner.Settings.Keys)
            {
                if (key.IsDefault) continue;

                choices.Add(new Item(key));
            }

            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, choices).SelectMultiple();
            if ((selection == null) || (selection.Count == 0)) return OptionResult.Failure;

            foreach (Item item in selection)
            {
                Retuner.Settings.Remove(item.Value);
            }

            return OptionResult.SuccessClose;
        }

        public class Item : ValueSettingOption<SettingsKey>
        {
            public Item(SettingsKey key)
                : base(key, key.LocalizedName, 0)
            { }
        }
    }
}
