using NRaas.CommonSpace.Options;
using NRaas.PortraitPanelSpace.Dialogs;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Options.General
{
    public class ShowSimMenuSetting : BooleanSettingOption<Sim>, IGeneralOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.PortraitPanel.Settings.mShowSimMenu;
            }
            set
            {
                NRaas.PortraitPanel.Settings.mShowSimMenu = value;

                // Restart the Cycle Portraits button
                SkewerEx.Instance.PopulateSkewers();
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShowSimMenu";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
