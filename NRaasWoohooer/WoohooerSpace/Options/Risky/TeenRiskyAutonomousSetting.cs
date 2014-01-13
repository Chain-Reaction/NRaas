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

namespace NRaas.WoohooerSpace.Options.Risky
{
    public class TeenRiskyAutonomousSetting : BooleanSettingOption<GameObject>, IRiskyOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mTeenRiskyAutonomous;
            }
            set
            {
                NRaas.Woohooer.Settings.mTeenRiskyAutonomous = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.mAllowTeenWoohoo) return false;

            if (NRaas.Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)] <= 0) return false;

            if (!NRaas.Woohooer.Settings.mRiskyAutonomousV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)]) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "TeenRiskyAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
