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
    public class RevertOnFailureSetting : BooleanSettingOption<Sim>, IColumnsOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.PortraitPanel.Settings.mRevertToSingleListOnFilterFail;
            }
            set
            {
                NRaas.PortraitPanel.Settings.mRevertToSingleListOnFilterFail = value;

                SkewerEx.Instance.PopulateSkewers();
            }
        }

        public override string GetTitlePrefix()
        {
            return "RevertToSingleListOnFilterFail";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
