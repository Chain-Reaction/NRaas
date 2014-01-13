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

namespace NRaas.WoohooerSpace.Options.SkinnyDip
{
    public class EnforcePrivacySetting : BooleanSettingOption<GameObject>, ISkinnyDipOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mEnforceSkinnyDipPrivacy;
            }
            set
            {
                NRaas.Woohooer.Settings.mEnforceSkinnyDipPrivacy = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "EnforcePrivacy";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
