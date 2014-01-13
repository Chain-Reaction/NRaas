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
    public class ForcePetMateSetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public ForcePetMateSetting()
        { }
        public ForcePetMateSetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mForcePetMate[SpeciesIndex];
            }
            set
            {
                Woohooer.Settings.mForcePetMate[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ForcePetMate";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters< GameObject> parameters)
        {
            if (Species == CASAgeGenderFlags.Human) return false;

            return base.Allow(parameters);
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new ForcePetMateSetting(species);
        }
    }
}
