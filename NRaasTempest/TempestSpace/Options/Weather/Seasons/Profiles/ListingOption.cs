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

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles
{
    public class ListingOption : InteractionOptionList<IProfileOption,GameObject>, ISeasonOption
    {
        Season mSeason;

        WeatherProfile mProfile;

        public ListingOption(Season season, WeatherProfile profile)
            : base(profile.Name)
        {
            mSeason = season;
            mProfile = profile;
        }

        public override string DisplayValue
        {
            get
            {
                uint start = mProfile.GetActualStart(mSeason);
                uint end = mProfile.GetActualEnd(mSeason);

                string result = null;

                if (start == end)
                {
                    result = EAText.GetNumberString(start);
                }
                else
                {
                    result = EAText.GetNumberString(start) + ":" + EAText.GetNumberString(end);
                }

                if (!mProfile.mEnabled)
                {
                    result = "(" + result + ")";
                }

                return result;
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IProfileOption> GetOptions()
        {
            List<IProfileOption> results = new List<IProfileOption>();

            results.Add(new CurrentDay(mSeason));
            results.Add(new Range(mSeason, mProfile));
            results.Add(new Enabled(mProfile));
            results.Add(new Rename(mProfile));
            results.Add(new Remove(mSeason, mProfile));
            results.Add(new MorningTemperature(mProfile));
            results.Add(new NoonTemperature(mProfile));
            results.Add(new EveningTemperature(mProfile));
            results.Add(new NightTemperature(mProfile));

            foreach (WeatherData data in mProfile.Data)
            {
                results.Add(new Types.ListingOption(data));
            }

            return results;
        }
    }
}
