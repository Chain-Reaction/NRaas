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

namespace NRaas.PortraitPanelSpace.Options.Columns
{
    public class RevertOnTooManySetting : BooleanSettingOption<Sim>, IColumnsOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.PortraitPanel.Settings.mRevertToSingleListOnTooMany;
            }
            set
            {
                NRaas.PortraitPanel.Settings.mRevertToSingleListOnTooMany = value;

                SkewerEx.Instance.PopulateSkewers();
            }
        }

        public override string GetTitlePrefix()
        {
            return "RevertToSingleListOnTooMany";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
