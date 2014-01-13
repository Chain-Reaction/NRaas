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
    public abstract class FloatSettingOption<TTarget> : GenericSettingOption<float, TTarget>
        where TTarget : class, IGameObject
    {
        public FloatSettingOption()
        { }
        public FloatSettingOption(string name)
            : base(name)
        { }

        public override string DisplayValue
        {
            get { return EAText.GetNumberString(Value); }
        }

        protected virtual float Validate(float value)
        {
            return value;
        }

        public override void SetImportValue(string value)
        {
            float result;
            if (!float.TryParse(value, out result)) return;

            Value = result;
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value.ToString());
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            float value;
            if (!float.TryParse(text, out value))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                return OptionResult.Failure;
            }

            Value = Validate (value);

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }
    }
}
