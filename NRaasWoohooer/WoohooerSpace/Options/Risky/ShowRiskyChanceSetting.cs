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
    public class ShowRiskyChanceSetting : BooleanSettingOption<GameObject>, IRiskyOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mShowRiskyChance;
            }
            set
            {
                NRaas.Woohooer.Settings.mShowRiskyChance = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)] <= 0) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "ShowRiskyChance";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
