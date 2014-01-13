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
    public abstract class RangeSettingOption<TType, TTarget> : GenericSettingOption<Pair<TType, TType>, TTarget>
        where TTarget : class, IGameObject
    {
        public override string DisplayValue
        {
            get { return GetDisplayValue(false); }
        }

        protected virtual string GetDisplayValue(bool forPrompt)
        {
            return GetDisplayValue(Value.First, forPrompt) + ":" + GetDisplayValue(Value.Second, forPrompt);
        }

        protected virtual Pair<TType, TType> Validate(TType value1, TType value2)
        {
            return new Pair<TType, TType>(value1, value2);
        }

        protected abstract string GetDisplayValue(TType value, bool forPrompt);

        protected abstract TType Convert(string value, bool prompt, out bool error);

        public override void SetImportValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(':');
                if (values.Length == 2)
                {
                    bool error;
                    Value = new Pair<TType, TType>(Convert(values[0], false, out error), Convert(values[1], false, out error));
                    return;
                }
            }

            Value = new Pair<TType, TType>(default(TType), default(TType));
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), GetDisplayValue(true));
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            string[] values = text.Split(':');
            if (values.Length != 2)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Range:Error"));
                return OptionResult.Failure;
            }

            bool error;
            TType value1 = Convert(values[0], true, out error);
            if (error) return OptionResult.Failure;

            TType value2 = Convert(values[1], true, out error);
            if (error) return OptionResult.Failure;

            Value = Validate(value1, value2);

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }
    }
}
