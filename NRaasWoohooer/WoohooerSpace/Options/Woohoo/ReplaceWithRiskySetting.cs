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

namespace NRaas.WoohooerSpace.Options.Woohoo
{
    public class ReplaceWithRiskySetting : BooleanSettingOption<GameObject>, IWoohooOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.ReplaceWithRisky;
            }
            set
            {
                NRaas.Woohooer.Settings.ReplaceWithRisky = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)] <= 0) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "ReplaceWithRisky";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
