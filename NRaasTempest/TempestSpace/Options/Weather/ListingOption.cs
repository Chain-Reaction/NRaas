using NRaas.CommonSpace.Options;
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

namespace NRaas.TempestSpace.Options.Weather
{
    public class ListingOption : AllSeasonListingOption<IWeatherOption>
    {
        public override string GetTitlePrefix()
        {
            return "WeatherRoot";
        }

        protected override IWeatherOption GetSeasonOption(Season season)
        {
            return new Seasons.ListingOption(season);
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP8)) return false;

            return base.Allow(parameters);
        }
    }
}
