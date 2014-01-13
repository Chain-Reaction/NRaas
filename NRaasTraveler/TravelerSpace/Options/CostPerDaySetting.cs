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
    public class CostPerDaySetting : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override int Value
        {
            get
            {
                return Traveler.Settings.mCostPerDay;
            }
            set
            {
                Traveler.Settings.mCostPerDay = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CostPerDay";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            return base.Validate(value);
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (GameStates.IsOnVacation) return false;

            return base.Allow(parameters);
        }
    }
}
