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
    public abstract class IntegerRangeSettingOption<TTarget> : RangeSettingOption<int, TTarget>
        where TTarget : class, IGameObject
    {
        protected override string GetDisplayValue(int value, bool forPrompt)
        {
            if (forPrompt)
            {
                return EAText.GetNumberString(value);
            }
            else
            {
                return value.ToString();
            }
        }

        protected override Pair<int, int> Validate(int value1, int value2)
        {
            if (value1 > value2)
            {
                int exchange = value1;
                value1 = value2;
                value2 = exchange;
            }

            return base.Validate(value1, value2);
        }

        protected override int Convert(string value, bool prompt, out bool error)
        {
            int result = 0;
            if (!int.TryParse(value, out result))
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
