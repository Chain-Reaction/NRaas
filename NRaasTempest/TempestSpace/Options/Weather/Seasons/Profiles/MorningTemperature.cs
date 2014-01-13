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
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles
{
    public class MorningTemperature : TemperatureOption
    {
        public MorningTemperature(WeatherProfile profile)
            : base(profile)
        { }

        public override string GetTitlePrefix()
        {
            return "MorningTemperature";
        }

        protected override Pair<float, float> Value
        {
            get
            {
                return mProfile.mMorningTemp;
            }
            set
            {
                mProfile.mMorningTemp = value;

                Tempest.ReapplySettings();
            }
        }
    }
}
