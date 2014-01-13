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
    public class WoohooCooldownSetting : SpeciesIntegerSettingOption, ISpeciesOption
    {
        public WoohooCooldownSetting()
        { }
        public WoohooCooldownSetting(CASAgeGenderFlags species)
            : base(species)
        { }

        protected override int Value
        {
            get
            {
                return Woohooer.Settings.mWoohooCooldown[SpeciesIndex];
            }
            set
            {
                Woohooer.Settings.mWoohooCooldown[SpeciesIndex] = value;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return true;
        }

        public override string GetTitlePrefix()
        {
            return "WoohooCooldown";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new WoohooCooldownSetting(species);
        }
    }
}
