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
    public class UseMoodInScoringSetting : BooleanSettingOption<GameObject>, IScoringOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mUseMoodInScoring;
            }
            set
            {
                NRaas.Woohooer.Settings.mUseMoodInScoring = value;

                ScoringLookup.UnloadCaches(true);
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return NRaas.Woohooer.Settings.UsingTraitScoring;
        }

        public override string GetTitlePrefix()
        {
            return "UseMoodInScoring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
