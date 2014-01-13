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
    public class CyberWoohooChanceToClimaxEveryXMinutesSetting : IntegerSettingOption<GameObject>, ICyberWoohooOption
    {
        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mCyberWoohooChanceToClimaxEveryXMinutes;
            }
            set
            {
                Skills.KamaSimtra.Settings.mCyberWoohooChanceToClimaxEveryXMinutes = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CyberWoohooChanceToClimaxEveryXMinutes";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
