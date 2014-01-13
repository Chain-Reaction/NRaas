using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Scoring;
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

namespace NRaas.WoohooerSpace.Options.Risky
{
    public class RiskyBaseChanceScoringSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public RiskyBaseChanceScoringSetting()
        { }
        public RiskyBaseChanceScoringSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mRiskyBaseChanceScoringV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mRiskyBaseChanceScoringV2[SpeciesIndex] = value;

                ScoringLookup.UnloadCaches<RiskyBaseChanceScoring>();
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.UsingTraitScoring) return false;

            if (NRaas.Woohooer.Settings.mRiskyBabyMadeChanceV2[SpeciesIndex] <= 0) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "RiskyBaseChanceScoring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new RiskyBaseChanceScoringSetting(species);
        }
    }
}
