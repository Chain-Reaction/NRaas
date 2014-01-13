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
    public abstract class ColumnFilterBaseSetting : ListedSettingOption<SkewerEx.VisibilityType, Sim>, IColumnsOption
    {
        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override string GetValuePrefix()
        {
            return "Type";
        }

        public override string ConvertToString(SkewerEx.VisibilityType value)
        {
            return value.ToString();
        }

        public override bool ConvertFromString(string value, out SkewerEx.VisibilityType newValue)
        {
            return ParserFunctions.TryParseEnum<SkewerEx.VisibilityType>(value, out newValue, SkewerEx.VisibilityType.ActiveHousehold);
        }

        protected override void PrivatePerform(IEnumerable<ListedSettingOption<SkewerEx.VisibilityType, Sim>.Item> results)
        {
            base.PrivatePerform(results);

            PortraitPanel.Settings.ResetUseLookup();

            SkewerEx.Instance.PopulateSkewers();
        }
    }
}
