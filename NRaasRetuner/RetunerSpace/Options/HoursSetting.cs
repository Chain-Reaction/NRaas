using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options
{
    public class HoursSetting : FloatRangeSettingOption<GameObject>, ITuningOption
    {
        public override string GetTitlePrefix()
        {
            return "Hours";
        }

        protected bool IsDefault
        {
            get
            {
                return ((Value.First == -1) && (Value.Second == 25));
            }
        }

        public override string DisplayValue
        {
            get
            {
                if (IsDefault)
                {
                    return Common.Localize(GetTitlePrefix() + ":Default");
                }
                else
                {
                    return base.DisplayValue;
                }
            }
        }

        protected override Pair<float, float> Value
        {
            get
            {
                return new Pair<float,float>(Retuner.SeasonSettings.Key.mHours.x, Retuner.SeasonSettings.Key.mHours.y);
            }
            set
            {
                Retuner.SeasonSettings.Key.mHours.x = value.First;
                Retuner.SeasonSettings.Key.mHours.y = value.Second;

                Retuner.ApplySettings();

                Retuner.StartAlarms();
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (IsDefault)
            {
                SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":DefaultPrompt"));
                return OptionResult.Failure;
            }

            return base.Run(parameters);
        }

        protected override Pair<float, float> Validate(float value1, float value2)
        {
            if (value1 < 0)
            {
                value1 = 0;
            }
            else if (value1 > 24)
            {
                value1 = 24;
            }

            if (value2 < 0)
            {
                value2 = 0;
            }
            else if (value2 > 24)
            {
                value2 = 24;
            }

            return base.Validate(value1, value2);
        }
    }
}
