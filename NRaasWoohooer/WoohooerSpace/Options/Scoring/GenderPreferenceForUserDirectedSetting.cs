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
    public class GenderPreferenceForUserDirectedSetting : BooleanSettingOption<GameObject>, IScoringOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mGenderPreferenceForUserDirectedV2;
            }
            set
            {
                NRaas.Woohooer.Settings.mGenderPreferenceForUserDirectedV2 = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return true;
        }

        public override string GetTitlePrefix()
        {
            return "GenderPreferenceForUserDirected";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
