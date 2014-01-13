using NRaas.CommonSpace.Options;
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
    public class TraitScoredPrivacySetting : BooleanSettingOption<GameObject>, IScoringOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mTraitScoredPrivacy;
            }
            set
            {
                NRaas.Woohooer.Settings.mTraitScoredPrivacy = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.UsingTraitScoring) return false;

            if (NRaas.Woohooer.Settings.mEnforcePrivacy) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "TraitScoredPrivacy";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
