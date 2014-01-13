using NRaas.CommonSpace.Options;
using NRaas.PortraitPanelSpace.Dialogs;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Options
{
    public class RemoveSim : OperationSettingOption<Sim>, ISimOption
    {
        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            return PortraitPanel.Settings.HasSim(parameters.mTarget);
        }

        public override string GetTitlePrefix()
        {
            return "RemoveSim";
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            PortraitPanel.Settings.RemoveSim(parameters.mTarget.SimDescription);

            SkewerEx.Instance.PopulateSkewers();

            return OptionResult.SuccessRetain;
        }
    }
}
