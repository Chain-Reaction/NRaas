using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Settings
{
    public class ResetSettings : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "ResetSettings";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt"))) return OptionResult.Failure;

            Dresser.ResetSettings();
            return OptionResult.SuccessClose;
        }
    }
}
