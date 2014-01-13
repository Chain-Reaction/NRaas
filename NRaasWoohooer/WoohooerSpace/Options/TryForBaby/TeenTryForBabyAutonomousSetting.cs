using NRaas.CommonSpace.Options;
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

namespace NRaas.WoohooerSpace.Options.TryForBaby
{
    public class TeenTryForBabyAutonomousSetting : BooleanSettingOption<GameObject>, ITryForBabyOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mTeenTryForBabyAutonomous;
            }
            set
            {
                NRaas.Woohooer.Settings.mTeenTryForBabyAutonomous = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.mAllowTeenWoohoo) return false;

            if (NRaas.Woohooer.Settings.mTryForBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)] <= 0) return false;

            if (!NRaas.Woohooer.Settings.mTryForBabyAutonomousV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)]) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "TeenTryForBabyAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
