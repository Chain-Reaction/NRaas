using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.TryForBaby
{
    public class TryForBabyUserMaximumSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public TryForBabyUserMaximumSetting()
        { }
        public TryForBabyUserMaximumSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mTryForBabyUserMaximum[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mTryForBabyUserMaximum[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "TryForBabyUserMaximum";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new TryForBabyUserMaximumSetting(species);
        }
    }
}
