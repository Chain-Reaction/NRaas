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

namespace NRaas.TempestSpace.Options.Holidays.Seasons.Days
{
    public class ListingOption : InteractionOptionList<IDayOption, GameObject>, ISeasonOption
    {
        Season mSeason;

        HolidaySettings.Holiday mDay;

        public ListingOption(Season season, HolidaySettings.Holiday day)
            : base(day.DisplayValue(season))
        {
            mSeason = season;
            mDay = day;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IDayOption> GetOptions()
        {
            List<IDayOption> results = new List<IDayOption>();

            results.Add(new SeasonOption(mSeason, mDay));
            results.Add(new DayOption(mSeason, mDay));

            return results;
        }
    }
}
