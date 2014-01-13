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
    public class AutonomousMaleMaleTryForBabySetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public AutonomousMaleMaleTryForBabySetting()
        { }
        public AutonomousMaleMaleTryForBabySetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mAutonomousMaleMaleTryForBabyV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mAutonomousMaleMaleTryForBabyV2[SpeciesIndex] = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Woohooer.Settings.mTestAllConditionsForUserDirected[SpeciesIndex])
            {
                if (!NRaas.Woohooer.Settings.mTryForBabyAutonomousV2[SpeciesIndex]) return false;

                if (NRaas.Woohooer.Settings.mTryForBabyMadeChanceV2[SpeciesIndex] <= 0) return false;

                if (!NRaas.Woohooer.Settings.mAllowSameSexTryForBabyV2[SpeciesIndex]) return false;
            }

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "AutonomousMaleMaleTryForBaby";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new AutonomousMaleMaleTryForBabySetting(species);
        }
    }
}
