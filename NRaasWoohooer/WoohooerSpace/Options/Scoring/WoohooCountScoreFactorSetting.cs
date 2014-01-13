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
    public class WoohooCountScoreFactorSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public WoohooCountScoreFactorSetting()
        { }
        public WoohooCountScoreFactorSetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override int Value
        {
            get
            {
                return Woohooer.Settings.mWoohooCountScoreFactor[SpeciesIndex];
            }
            set
            {
                Woohooer.Settings.mWoohooCountScoreFactor[SpeciesIndex] = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Woohooer.Settings.UsingTraitScoring) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "WoohooCountScoreFactor";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new WoohooCountScoreFactorSetting(species);
        }
    }
}
