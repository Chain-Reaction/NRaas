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
    public abstract class BooleanSettingOption<TTarget> : GenericSettingOption<bool, TTarget>
        where TTarget : class, IGameObject
    {
        public override string DisplayKey
        {
            get { return "Boolean"; }
        }

        public override void SetImportValue(string value)
        {
            bool newValue;
            if (!bool.TryParse(value, out newValue)) return;

            Value = newValue;
        }

        protected override OptionResult Run(GameHitParameters< TTarget> parameters)
        {
            string prompt = GetPrompt();
            if (prompt != null)
            {
                if (!AcceptCancelDialog.Show(prompt)) return OptionResult.Failure;
            }

            Value = !Value;

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }
    }
}
