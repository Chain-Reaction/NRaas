using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Weather.Seasons
{
    public class ListingOption : SeasonListingOption<ISeasonOption>, IWeatherOption
    {
        public ListingOption(Season season)
            : base(season)
        { }

        public override List<ISeasonOption> GetOptions()
        {
            List<ISeasonOption> results = new List<ISeasonOption>();

            WeatherSettings settings = Tempest.Settings.GetWeather(mSeason);

            results.Add(new AddProfile(mSeason));

            foreach (WeatherProfile profile in settings.Profiles)
            {
                results.Add(new Profiles.ListingOption(mSeason, profile));
            }

            return results;
        }
    }
}
