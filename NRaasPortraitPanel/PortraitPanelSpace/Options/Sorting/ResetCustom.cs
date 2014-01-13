using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Dialogs;
using NRaas.PortraitPanelSpace.Dialogs;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Options.Sorting
{
    public class ResetCustom : OperationSettingOption<Sim>, ISortingOption
    {
        public override string GetTitlePrefix()
        {
            return "ResetCustom";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (PortraitPanel.Settings.mSimsV2.Count == 0) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix () + ":Prompt"))) return OptionResult.Failure;

            PortraitPanel.Settings.ResetSimSort();

            SkewerEx.Instance.PopulateSkewers();

            return OptionResult.SuccessRetain;
        }
    }
}
