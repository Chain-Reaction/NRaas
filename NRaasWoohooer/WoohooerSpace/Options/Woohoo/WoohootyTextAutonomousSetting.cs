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
    public class WoohootyTextAutonomousSetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public WoohootyTextAutonomousSetting()
        { }
        public WoohootyTextAutonomousSetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override bool Value
        {
            get
            {
                if (SpeciesIndex == 0)
                {
                    return NRaas.Woohooer.Settings.mWoohootyTextAutonomous[SpeciesIndex];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                NRaas.Woohooer.Settings.mWoohootyTextAutonomous[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "WoohootyTextAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (mSpecies != CASAgeGenderFlags.Human) return false;

            return base.Allow(parameters);
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new WoohootyTextAutonomousSetting(species);
        }
    }
}