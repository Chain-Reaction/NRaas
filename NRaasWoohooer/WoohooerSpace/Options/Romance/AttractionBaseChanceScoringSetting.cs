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

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class AttractionBaseChanceScoringSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public AttractionBaseChanceScoringSetting()
        { }
        public AttractionBaseChanceScoringSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override int Value
        {
            get
            {
                return Woohooer.Settings.mAttractionBaseChanceScoringV3[SpeciesIndex];
            }
            set
            {
                Woohooer.Settings.mAttractionBaseChanceScoringV3[SpeciesIndex] = value;

                Woohooer.AdjustAttractionTuning();

                ScoringLookup.UnloadCaches<AttractionBaseChanceScoring>();
            }
        }

        public override string GetTitlePrefix()
        {
            return "AttractionBaseChanceScoring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new AttractionBaseChanceScoringSetting(species);
        }
    }
}
