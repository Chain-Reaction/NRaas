using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Woohoo
{
    public class WoohooAutonomousSetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public WoohooAutonomousSetting()
        { }
        public WoohooAutonomousSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mWoohooAutonomousV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mWoohooAutonomousV2[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "WoohooAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new WoohooAutonomousSetting(species);
        }
    }
}
