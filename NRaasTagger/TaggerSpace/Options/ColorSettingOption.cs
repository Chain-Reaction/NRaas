using NRaas.CommonSpace.Options;
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

namespace NRaas.TaggerSpace.Options
{
    public abstract class ColorSettingOption<TTarget> : GenericSettingOption<uint, TTarget>
        where TTarget : class, IGameObject
    {
        public override string DisplayValue
        {
            get { return Value.ToString("X").Remove(0, 2); }
        }

        protected virtual uint Validate(uint value)
        {
            return value;
        }

        public override void SetImportValue(string value)
        {
            Value = uint.Parse(value, System.Globalization.NumberStyles.HexNumber);
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value.ToString("X"));
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            uint value = 0;
            try
            {
                value = uint.Parse("FF" + text, System.Globalization.NumberStyles.HexNumber);
            }
            catch
            { }

            if (value == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Hex:Error"));
                return OptionResult.Failure;
            }

            Value = Validate(value);

            Tagger.InitTags(false);

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }
    }
}