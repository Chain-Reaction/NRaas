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
    public class NearRelationWoohooSetting : SpeciesBooleanSettingOption, ISpeciesOption
    {
        public NearRelationWoohooSetting()
        { }
        public NearRelationWoohooSetting(CASAgeGenderFlags species)
            : base (species)
        { }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mAllowNearRelationWoohooV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mAllowNearRelationWoohooV2[SpeciesIndex] = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.Woohooer.Settings.mAllowNearRelationRomanceV2[SpeciesIndex]) return false;

            return true;
        }

        public override string GetTitlePrefix()
        {
            return "AllowNearRelationWoohoo";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new NearRelationWoohooSetting(species);
        }
    }
}
