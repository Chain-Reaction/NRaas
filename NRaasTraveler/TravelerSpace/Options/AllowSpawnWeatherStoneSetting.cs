using NRaas.CommonSpace.Options;
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
    public class AllowSpawnWeatherStoneSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Traveler.Settings.mAllowSpawnWeatherStone;
            }
            set
            {
                Traveler.Settings.mAllowSpawnWeatherStone = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowSpawnWeatherStone";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP7)) return false;

            if (!GameUtils.IsInstalled(ProductVersion.EP8)) return false;

            return base.Allow(parameters);
        }
    }
}
