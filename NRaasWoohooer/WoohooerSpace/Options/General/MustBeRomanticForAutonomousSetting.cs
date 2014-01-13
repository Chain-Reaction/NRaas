using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.General
{
    public class MustBeRomanticForAutonomousSetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        private static int sBedUseVisitorPrivlageThreshold = int.MinValue;

        public MustBeRomanticForAutonomousSetting()
        { }
        public MustBeRomanticForAutonomousSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mMustBeRomanticForAutonomousV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mMustBeRomanticForAutonomousV2[SpeciesIndex] = value;

                AdjustVisitorPrivilege(!value);
            }
        }

        public static void AdjustVisitorPrivilege(bool value)
        {
            if (sBedUseVisitorPrivlageThreshold == int.MinValue)
            {
                sBedUseVisitorPrivlageThreshold = Sims3.Gameplay.Objects.Beds.Bed.kBedUseVisitorPrivlageThreshold;
            }

            if (value)
            {
                Sims3.Gameplay.Objects.Beds.Bed.kBedUseVisitorPrivlageThreshold = 1;
            }
            else
            {
                Sims3.Gameplay.Objects.Beds.Bed.kBedUseVisitorPrivlageThreshold = sBedUseVisitorPrivlageThreshold;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MustBeRomanticForAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new MustBeRomanticForAutonomousSetting(species);
        }
    }
}
