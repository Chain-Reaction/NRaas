using NRaas.CommonSpace.Options;
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

namespace NRaas.TempestSpace.Options.Holidays.Seasons.Days
{
    public class DayOption : IntegerSettingOption<GameObject>, IDayOption
    {
        Season mSeason;

        HolidaySettings.Holiday mDay;

        public DayOption(Season season, HolidaySettings.Holiday day)
        {
            mSeason = season;
            mDay = day;
        }

        public override string GetTitlePrefix()
        {
            return "Day";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override int Value
        {
            get
            {
                return mDay.RelativeDay;
            }
            set
            {
                mDay.RelativeDay = value;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (base.Run(parameters) == OptionResult.Failure) return OptionResult.Failure;

            if (mDay.RelativeDay == 0)
            {
                Tempest.Settings.GetHolidays(mSeason).Remove(mDay);

                HolidayManagerEx.SetUpHolidayManager(true);

                return OptionResult.SuccessLevelDown;
            }
            else
            {
                HolidayManagerEx.SetUpHolidayManager(true);

                return OptionResult.SuccessRetain;
            }
        }
    }
}
