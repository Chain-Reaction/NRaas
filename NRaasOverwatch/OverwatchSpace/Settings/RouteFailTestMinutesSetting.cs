using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using NRaas.OverwatchSpace.Loadup;
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

namespace NRaas.OverwatchSpace.Settings
{
    public class RouteFailTestMinutesSetting : IntegerOption
    {
        public override string GetTitlePrefix()
        {
            return "RouteFailTestMinutesSetting";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override int Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mRouteFailTestMinutesV2;
            }
            set
            {
                NRaas.Overwatch.Settings.mRouteFailTestMinutesV2 = value;
            }
        }
    }
}
