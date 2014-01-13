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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Scoring
{
    public class PrivacyBaseChanceScoringSetting : IntegerSettingOption<GameObject>, IScoringOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mPrivacyBaseChanceScoring;
            }
            set
            {
                NRaas.Woohooer.Settings.mPrivacyBaseChanceScoring = value;

                ScoringLookup.UnloadCaches<PrivacyBaseChanceScoring>();
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.UsingTraitScoring) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "PrivacyBaseChanceScoring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
