using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Holidays.Seasons
{
    public class AllowSchool : BooleanSettingOption<GameObject>, ISeasonOption
    {
        HolidaySettings mSettings;

        public AllowSchool(HolidaySettings settings)
        {
            mSettings = settings;
        }

        protected override bool Value
        {
            get
            {
                return !mSettings.mNoSchool;
            }
            set
            {
                mSettings.mNoSchool = !value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowSchool";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
