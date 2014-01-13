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

namespace NRaas.TempestSpace.Options
{
    public abstract class TemperatureBaseOption : FloatRangeSettingOption<GameObject>
    {
        public TemperatureBaseOption()
        { }

        protected override string GetPrompt()
        {
            if (Sims3.UI.Responder.Instance.OptionsModel.IsCelcius)
            {
                return Common.Localize("Temperature:PromptC");
            }
            else
            {
                return Common.Localize("Temperature:PromptF");
            }
        }

        protected override string GetDisplayValue(float value, bool forPrompt)
        {
            if (Sims3.UI.Responder.Instance.OptionsModel.IsCelcius)
            {
                value = SeasonsManager.TemperatureManager.TempAsCelcius(value);
            }

            return base.GetDisplayValue(value, forPrompt);
        }

        protected override Pair<float, float> Validate(float value1, float value2)
        {
            if (Sims3.UI.Responder.Instance.OptionsModel.IsCelcius)
            {
                // Back to Fahrenheit
                value1 = (value1 / 0.5555556f) + 32;
                value2 = (value2 / 0.5555556f) + 32;
            }

            /*
            if (value1 < 0)
            {
                value1 = 0;
            }

            if (value2 < 0)
            {
                value2 = 0;
            }
            */
            return base.Validate(value1, value2);
        }
    }
}
