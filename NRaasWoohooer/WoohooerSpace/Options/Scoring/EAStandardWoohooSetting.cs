using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Scoring
{
    public class EAStandardWoohooSetting : BooleanSettingOption<GameObject>, IScoringOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mEAStandardWoohoo;
            }
            set
            {
                NRaas.Woohooer.Settings.mEAStandardWoohoo = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "EAStandardWoohoo";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
