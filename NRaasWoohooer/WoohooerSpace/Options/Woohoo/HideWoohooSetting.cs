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

namespace NRaas.WoohooerSpace.Options.Woohoo
{
    public class HideWoohooSetting : BooleanSettingOption<GameObject>, IWoohooOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mHideWoohoo;
            }
            set
            {
                NRaas.Woohooer.Settings.mHideWoohoo = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.Woohooer.Settings.ReplaceWithRisky) return false;

            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            return "HideWoohoo";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
