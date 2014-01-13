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
    public class CyberWoohooChanceToClimaxSetting : IntegerSettingOption<GameObject>, ICyberWoohooOption
    {
        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mCyberWoohooChanceToClimax;
            }
            set
            {
                Skills.KamaSimtra.Settings.mCyberWoohooChanceToClimax = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CyberWoohooChanceToClimax";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
