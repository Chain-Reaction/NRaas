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
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.Settings
{
    public class AddSetting : OperationSettingOption<GameObject>, ISettingsOption
    {
        public override string GetTitlePrefix()
        {
            return "Add";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Item> choices = new List<Item>();

            choices.Add(new Item(new GeneralKey(new Vector2(0, 24))));
            choices.Add(new Item(new SeasonKey(Season.Spring, new Vector2(0, 24))));
            choices.Add(new Item(new SeasonKey(Season.Summer, new Vector2(0, 24))));
            choices.Add(new Item(new SeasonKey(Season.Fall, new Vector2(0, 24))));
            choices.Add(new Item(new SeasonKey(Season.Winter, new Vector2(0, 24))));

            Item selection = new CommonSelection<Item>(Name, choices).SelectSingle();
            if (selection == null) return OptionResult.Failure;

            Retuner.Settings.Add(selection.Value, false);
            return OptionResult.SuccessRetain;
        }

        public class Item : ValueSettingOption<SettingsKey>
        {
            public Item(SettingsKey key)
                : base(key, key.LocalizedName, 0)
            { }
        }
    }
}
