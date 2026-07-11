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

namespace NRaas.WoohooerSpace.Options.TryForBaby
{
    public enum PregnancyChoice
    {
        Initiator = 0,
        Target,
        InitiatorThenTarget,
        Either = InitiatorThenTarget, // for compatibility with legacy tuning
        Random,
        TargetThenInitiator,
    }

    public class PregnancyChoiceSetting : EnumSettingOption<PregnancyChoice, GameObject>, ITryForBabyOption
    {
        protected override PregnancyChoice Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mTryForBabyPregnancyChoice;
            }
            set
            {
                NRaas.Woohooer.Settings.mTryForBabyPregnancyChoice = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "PregnancyChoice";
        }

        public override PregnancyChoice Default
        {
            get { return WoohooerTuning.kTryForBabyPregnancyChoice; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
