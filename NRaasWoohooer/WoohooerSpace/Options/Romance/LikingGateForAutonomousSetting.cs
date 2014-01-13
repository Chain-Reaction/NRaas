using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class LikingGatingForAutonomousSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public LikingGatingForAutonomousSetting()
        { }
        public LikingGatingForAutonomousSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mLikingGatingForAutonomousRomance[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mLikingGatingForAutonomousRomance[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "LikingGatingForAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new SpeciesListingOption(Species); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new LikingGatingForAutonomousSetting(species);
        }
    }
}
