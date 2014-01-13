using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class FloatRangeSettingOption<TTarget> : RangeSettingOption<float, TTarget>
        where TTarget : class, IGameObject
    {
        protected override string GetDisplayValue(float value, bool forPrompt)
        {
            return EAText.GetNumberString(value);
        }

        protected override Pair<float, float> Validate(float value1, float value2)
        {
            if (value1 > value2)
            {
                float exchange = value1;
                value1 = value2;
                value2 = exchange;
            }

            return base.Validate(value1, value2);
        }

        protected override float Convert(string value, bool prompt, out bool error)
        {
            float result = 0;
            if (!float.TryParse(value, out result))
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                }

                error = true;
                return 0;
            }

            error = false;
            return result;
        }
    }
}
