using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.TryForBaby
{
    public class TestAllConditionsForUserDirectedSetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public TestAllConditionsForUserDirectedSetting()
        { }
        public TestAllConditionsForUserDirectedSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mTestAllConditionsForUserDirected[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mTestAllConditionsForUserDirected[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "TestAllConditionsForUserDirected";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new TestAllConditionsForUserDirectedSetting(species);
        }
    }
}
