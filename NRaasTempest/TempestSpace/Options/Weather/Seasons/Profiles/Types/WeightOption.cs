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

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles.Types
{
    public class WeightOption : IntegerSettingOption<GameObject>, ITypeOption
    {
        WeatherData mData;

        public WeightOption(WeatherData data)
        {
            mData = data;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "Weight";
        }

        protected override int Value
        {
            get
            {
                return mData.mWeight;
            }
            set
            {
                mData.mWeight = value;

                Tempest.ReapplySettings();
            }
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                value = 0;
            }

            return base.Validate(value);
        }

        protected override string GetPrompt()
        {
            return Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { mData.Name });
        }
    }
}
