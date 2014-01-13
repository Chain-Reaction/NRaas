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

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles
{
    public class Rename : StringSettingOption<GameObject>, IProfileOption
    {
        WeatherProfile mProfile;

        public Rename(WeatherProfile profile)
        {
            mProfile = profile;
        }

        protected override string Value
        {
            get
            {
                return mProfile.Name;
            }
            set
            {
                mProfile.Name = value;
            }
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "RenameProfile";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
