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
    public class AutonomousOffLotTryForBabySetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public AutonomousOffLotTryForBabySetting()
        { }
        public AutonomousOffLotTryForBabySetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mAllowOffLotTryForBabyAutonomous[SpeciesIndex];
            }
            set
            {
                Woohooer.Settings.mAllowOffLotTryForBabyAutonomous[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowOffLotAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Woohooer.Settings.mTryForBabyAutonomousV2[SpeciesIndex]) return false;

            return base.Allow(parameters);
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new AutonomousOffLotTryForBabySetting(species);
        }
    }
}
