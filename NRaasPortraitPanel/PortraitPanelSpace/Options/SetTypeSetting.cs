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

namespace NRaas.PortraitPanelSpace.Options
{
    public class SetTypeSetting : ListedSettingOption<SkewerEx.VisibilityType, Sim>, IPanelOption
    {
        public override string GetTitlePrefix()
        {
            return "SetType";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override string GetValuePrefix()
        {
            return "Type";
        }

        protected override bool Allow(SkewerEx.VisibilityType value)
        {
            switch (value)
            {
                case SkewerEx.VisibilityType.OnlyIdle:
                case SkewerEx.VisibilityType.OnlySelectable:
                case SkewerEx.VisibilityType.SpeciesHumans:
                case SkewerEx.VisibilityType.SpeciesHorses:
                case SkewerEx.VisibilityType.SpeciesCats:
                case SkewerEx.VisibilityType.SpeciesDogs:
                case SkewerEx.VisibilityType.SpeciesOther:
                case SkewerEx.VisibilityType.Relatives:
                case SkewerEx.VisibilityType.InactiveAnimals:
                case SkewerEx.VisibilityType.InactiveHumans:
                    return false;
            }

            return base.Allow(value);
        }

        protected override Proxy GetList()
        {
            return new ListProxy(PortraitPanel.Settings.mSetTypeV3);
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

            PortraitPanel.Settings.ResetSetTypeLookup();

            PortraitPanel.InitializeWatchers();

            SkewerEx.Instance.PopulateSkewers();
        }
    }
}
