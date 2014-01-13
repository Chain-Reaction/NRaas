using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class JealousyLevelSetting : EnumSettingOption<JealousyLevel, GameObject>, IRomanceOption
    {
        protected override JealousyLevel Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mRomanceJealousyLevel;
            }
            set
            {
                NRaas.Woohooer.Settings.mRomanceJealousyLevel = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "JealousyLevel";
        }

        public override JealousyLevel Default
        {
            get { return WoohooerTuning.kRomanceJealousyLevel; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
