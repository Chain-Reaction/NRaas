using NRaas.CommonSpace.Options;
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

namespace NRaas.WoohooerSpace.Options.Risky
{
    public class PregnancyChoiceSetting : EnumSettingOption<TryForBaby.PregnancyChoice, GameObject>, IRiskyOption
    {
        protected override TryForBaby.PregnancyChoice Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mRiskyPregnancyChoice;
            }
            set
            {
                NRaas.Woohooer.Settings.mRiskyPregnancyChoice = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "PregnancyChoice";
        }

        public override TryForBaby.PregnancyChoice Default
        {
            get { return WoohooerTuning.kRiskyPregnancyChoice; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
