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
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Woohoo
{
    public class WoohooRechargeRateSetting : IntegerSettingOption<GameObject>, IWoohooOption
    {
        public WoohooRechargeRateSetting()
        { }

        protected override int Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mWoohooRechargeRate;
            }
            set
            {
                NRaas.Woohooer.Settings.mWoohooRechargeRate = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.UsingTraitScoring) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "WoohooRechargeRate";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
