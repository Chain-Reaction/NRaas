using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles
{
    public class Range : IntegerRangeSettingOption<GameObject>, IProfileOption
    {
        Season mSeason;

        WeatherProfile mProfile;

        public Range(Season season, WeatherProfile data)
        {
            mSeason = season;
            mProfile = data;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "Range";
        }

        public override string DisplayValue
        {
            get
            {
                uint start = mProfile.GetActualStart(mSeason);
                uint end = mProfile.GetActualEnd(mSeason);

                if (start == end)
                {
                    return EAText.GetNumberString(start);
                }
                else
                {
                    return EAText.GetNumberString(start) + ":" + EAText.GetNumberString(end);
                }
            }
        }

        protected override Pair<int, int> Value
        {
            get
            {
                return new Pair<int, int>(mProfile.RelativeStart, mProfile.RelativeEnd);
            }
            set
            {
                mProfile.RelativeStart = value.First;
                mProfile.RelativeEnd = value.Second;

                Tempest.ReapplySettings();
            }
        }

        protected override Pair<int, int> Validate(int value1, int value2)
        {
            if (value1 == 0)
            {
                value1 = 1;
            }

            if (value2 == 0)
            {
                value2 = -1;
            }

            // Don't call base class it reorders the items
            return new Pair<int, int>(value1, value2);
        }
    }
}
