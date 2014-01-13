using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
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

namespace NRaas.TravelerSpace.Options
{
    public class StoreVehiclesSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Traveler.Settings.mStoreVehicles;
            }
            set
            {
                Traveler.Settings.mStoreVehicles = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "StoreVehicles";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameStates.IsOnVacation) return false;

            if (GameStates.TravelHousehold != Household.ActiveHousehold) return false;

            return base.Allow(parameters);
        }
    }
}
