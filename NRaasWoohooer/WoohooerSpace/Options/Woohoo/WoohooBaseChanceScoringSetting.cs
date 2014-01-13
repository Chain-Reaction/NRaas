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

namespace NRaas.WoohooerSpace.Options.Woohoo
{
    public class WoohooBaseChanceScoringSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public WoohooBaseChanceScoringSetting()
        { }
        public WoohooBaseChanceScoringSetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mWoohooBaseChanceScoringV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mWoohooBaseChanceScoringV2[SpeciesIndex] = value;

                ScoringLookup.UnloadCaches<WoohooBaseChanceScoring>();
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.UsingTraitScoring) return false;

            if (NRaas.Woohooer.Settings.mHideWoohoo) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "WoohooBaseChanceScoring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new WoohooBaseChanceScoringSetting(species);
        }
    }
}
