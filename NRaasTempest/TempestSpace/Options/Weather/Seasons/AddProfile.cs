using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Weather.Seasons
{
    public class AddProfile : OperationSettingOption<GameObject>, ISeasonOption
    {
        Season mSeason;

        public AddProfile(Season season)
        {
            mSeason = season;
        }

        public override string GetTitlePrefix()
        {
            return "Add";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Dictionary<string,bool> lookup = new Dictionary<string,bool>();

            List<Item> profiles = new List<Item>();

            foreach(WeatherSettings settings in Tempest.Settings.Weather)
            {
                foreach(WeatherProfile profile in settings.Profiles)
                {
                    string name = Common.LocalizeEAString("Ui/Caption/Options:" + settings.Season) + ": " + profile.Name;
                    if (lookup.ContainsKey(name)) continue;
                    lookup.Add(name, true);

                    profiles.Add(new Item (name, profile));
                }
            }

            Item choice = new CommonSelection<Item>(Name, profiles).SelectSingle();
            if (choice == null) return OptionResult.Failure;

            Tempest.Settings.GetWeather(mSeason).AddProfile(choice.Value);

            Tempest.ReapplySettings();
            return OptionResult.SuccessRetain;
        }

        public class Item : ValueSettingOption<WeatherProfile>
        {
            public Item(string name, WeatherProfile profile)
                : base(profile, name, 0)
            { }
        }
    }
}
