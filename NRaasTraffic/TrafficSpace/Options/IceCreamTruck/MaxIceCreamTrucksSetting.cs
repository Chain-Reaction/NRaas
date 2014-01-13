using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.TrafficSpace.Options.IceCreamTruck
{
    public class MaxIceCreamTrucksSetting : IntegerSettingOption<GameObject>, IIceCreamTruckOption
    {
        protected override int Value
        {
            get
            {
                return Traffic.Settings.mMaxIceCreamTrucks;
            }
            set
            {
                Traffic.Settings.mMaxIceCreamTrucks = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaxIceCreamTrucks";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}


