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

namespace NRaas.PortraitPanelSpace.Options
{
    public class AddMultiple : OperationSettingOption<Sim>, IPanelOption
    {
        public override string GetTitlePrefix()
        {
            return "AddMultiple";
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            foreach (SimDescription sim in new SimSelection(Name, Household.EverySimDescription(), null).SelectMultiple())
            {
                PortraitPanel.Settings.AddSim(sim.CreatedSim);
            }

            PortraitPanel.Settings.AddSelectedSimsFilter();
            
            SkewerEx.Instance.PopulateSkewers();

            return OptionResult.SuccessRetain;
        }
    }
}
