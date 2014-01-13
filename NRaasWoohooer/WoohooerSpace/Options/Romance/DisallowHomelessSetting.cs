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

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class DisallowHomelessSetting : BooleanSettingOption<GameObject>, IRomanceOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mDisallowHomeless;
            }
            set
            {
                NRaas.Woohooer.Settings.mDisallowHomeless = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "DisallowHomeless";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
