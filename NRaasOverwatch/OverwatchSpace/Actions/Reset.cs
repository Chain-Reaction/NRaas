using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Actions
{
    public class Reset : OptionItem, IActionOption
    {
        public Reset()
        { }

        public override string GetTitlePrefix()
        {
            return "Reset";
        }

        protected override OptionResult Run(GameHitParameters< GameObject> parameters)
        {
            Overwatch.Log("Reset");

            if (AcceptCancelDialog.Show(Common.Localize("Reset:Prompt")))
            {
                Overwatch.ResetSettings();

                SimpleMessageDialog.Show(Name, Common.Localize("Reset:Complete"));
            }

            return OptionResult.SuccessRetain;
        }
    }
}
