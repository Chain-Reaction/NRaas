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
    public class TreatAsVacationSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Traveler.Settings.mTreatAsVacation;
            }
            set
            {
                Traveler.Settings.mTreatAsVacation = value;

                WorldData.SetVacationWorld(false, true);
            }
        }

        public override string GetTitlePrefix()
        {
            return "TreatAsVacation";
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
