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

namespace NRaas.TrafficSpace.Options.FoodTruck
{
    public class MaxFoodTrucksSetting : IntegerSettingOption<GameObject>, IFoodTruckOption
    {
        protected override int Value
        {
            get
            {
                return Traffic.Settings.mMaxFoodTrucks;
            }
            set
            {
                Traffic.Settings.mMaxFoodTrucks = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaxFoodTrucks";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}


