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
    public class TransitionTime : FloatRangeSettingOption<GameObject>, ITypeOption
    {
        PercipitationData mData;

        public TransitionTime(PercipitationData data)
        {
            mData = data;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "TransitionTime";
        }

        protected override Pair<float, float> Value
        {
            get
            {
                return mData.mTransitionTime;
            }
            set
            {
                mData.mTransitionTime = value;

                Tempest.ReapplySettings();
            }
        }

        protected override Pair<float, float> Validate(float value1, float value2)
        {
            if (value1 < 0)
            {
                value1 = 0;
            }

            if (value2 < 0)
            {
                value2 = 0;
            }

            return base.Validate(value1, value2);
        }
    }
}
