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
    public class SeasonOption : EnumSettingOption<Season,GameObject>, IDayOption
    {
        Season mSeason;

        HolidaySettings.Holiday mDay;

        public SeasonOption(Season season, HolidaySettings.Holiday day)
        {
            mSeason = season;
            mDay = day;
        }

        public override string GetTitlePrefix()
        {
            return "Holiday";
        }

        public override string GetLocalizedValue(Season value)
        {
            return Common.LocalizeEAString("Ui/Caption/Options:" + value);
        }

        protected override Season Value
        {
            get
            {
                return mDay.mSeason;
            }
            set
            {
                mDay.mSeason = value;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override Season Default
        {
            get { return mSeason; }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (base.Run(parameters) == OptionResult.Failure) return OptionResult.Failure;

            HolidayManagerEx.SetUpHolidayManager(true);
            return OptionResult.SuccessRetain;
        }
    }
}
