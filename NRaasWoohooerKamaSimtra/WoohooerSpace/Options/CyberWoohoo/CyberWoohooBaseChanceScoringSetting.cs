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

namespace NRaas.WoohooerSpace.Options.CyberWoohoo
{
    public class CyberWoohooBaseChanceScoringSetting : IntegerSettingOption<GameObject>, ICyberWoohooOption
    {
        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mCyberWoohooBaseChanceScoring;
            }
            set
            {
                Skills.KamaSimtra.Settings.mCyberWoohooBaseChanceScoring = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CyberWoohooBaseChanceScoring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
