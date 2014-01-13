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

namespace NRaas.TempestSpace.Options.Holidays.Seasons
{
    public class ListingOption : SeasonListingOption<ISeasonOption>, IHolidayOption
    {
        public ListingOption(Season season)
            : base(season)
        { }

        public override List<ISeasonOption> GetOptions()
        {
            List<ISeasonOption> results = new List<ISeasonOption>();

            results.Add(new CurrentDay(mSeason));
            results.Add(new AddDay(mSeason));

            HolidaySettings settings = Tempest.Settings.GetHolidays(mSeason);

            foreach (HolidaySettings.Holiday day in settings.Days)
            {
                results.Add(new Days.ListingOption(mSeason, day));
            }

            return results;
        }
    }
}
