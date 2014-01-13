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
    public class MinIntensityDuration : FloatSettingOption<GameObject>, ITypeOption
    {
        PercipitationData mData;

        public MinIntensityDuration(PercipitationData data)
        {
            mData = data;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "MinIntensityDuration";
        }

        protected override float Value
        {
            get
            {
                return mData.mMinIntensityDuration;
            }
            set
            {
                mData.mMinIntensityDuration = value;

                Tempest.ReapplySettings();
            }
        }

        protected override float Validate(float value)
        {
            if (value < 0)
            {
                value = 0;
            }

            return base.Validate(value);
        }
    }
}
