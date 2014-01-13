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

namespace NRaas.WoohooerSpace.Options.General
{
    public class MaximumHouseholdSizeForAutonomousSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public MaximumHouseholdSizeForAutonomousSetting()
        { }
        public MaximumHouseholdSizeForAutonomousSetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mMaximumHouseholdSizeForAutonomousV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mMaximumHouseholdSizeForAutonomousV2[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaximumHouseholdSizeForAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new MaximumHouseholdSizeForAutonomousSetting(species);
        }
    }
}
