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
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Options
{
    public class RemoveMultiple : OperationSettingOption<Sim>, IPanelOption
    {
        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            return (PortraitPanel.Settings.mSimsV2.Count > 0);
        }

        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(PortraitPanel.Settings.mSimsV2.Count);
            }
        }

        public override string GetTitlePrefix()
        {
            return "RemoveMultiple";
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            foreach (SimDescription sim in new SimSelection(Name, PortraitPanel.Settings.SelectedSims, null).SelectMultiple())
            {
                PortraitPanel.Settings.RemoveSim(sim);
            }

            SkewerEx.Instance.PopulateSkewers();

            return OptionResult.SuccessRetain;
        }
    }
}
