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
    public abstract class StringSettingOption<TTarget> : GenericSettingOption<string, TTarget>
        where TTarget : class, IGameObject
    {
        public override string DisplayValue
        {
            get { return Value; }
        }

        protected virtual string Validate(string value)
        {
            return value;
        }

        public override void SetImportValue(string value)
        {
            Value = value;
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value.ToString(), 256, StringInputDialog.Validation.None);
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            Value = Validate(text);

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }
    }
}
