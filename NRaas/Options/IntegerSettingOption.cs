﻿using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.CommonSpace.Options
{
    public abstract class IntegerSettingOption<TTarget> : GenericSettingOption<int, TTarget>
        where TTarget : class, IGameObject
    {
        public override string DisplayValue
        {
            get { return EAText.GetNumberString(Value); }
        }

        protected virtual int Validate(int value)
        {
            return value;
        }

        public override void SetImportValue(string value)
        {
            int result = 0;
            if (int.TryParse(value, out result))
            {
                Value = result;
            }
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value.ToString());
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            int value;
            if (!int.TryParse(text, out value))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                return OptionResult.Failure;
            }

            Value = Validate(value);

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }
    }
}
