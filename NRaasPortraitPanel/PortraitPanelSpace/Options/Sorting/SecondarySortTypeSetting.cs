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

namespace NRaas.PortraitPanelSpace.Options.Sorting
{
    public class SecondarySortTypeSetting : EnumSettingOption<SkewerEx.SortType, Sim>, ISortingOption
    {
        protected override SkewerEx.SortType Value
        {
            get
            {
                return NRaas.PortraitPanel.Settings.mSecondSortType;
            }
            set
            {
                NRaas.PortraitPanel.Settings.mSecondSortType = value;

                SkewerEx.Instance.PopulateSkewersNoRebuild();
            }
        }

        public override string GetTitlePrefix()
        {
            return "SecondarySortType";
        }

        public override SkewerEx.SortType Default
        {
            get { return PersistedSettings.kSecondSortType; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override string GetValuePrefix()
        {
            return "Type";
        }
    }
}
