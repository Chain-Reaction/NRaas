using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.TryForBaby
{
    public class ChanceOfQuadsSetting : FloatSettingOption<GameObject>, ISpeciesOption
    {
        public ChanceOfQuadsSetting()
        { }

        protected override float Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mChanceOfQuads;
            }
            set
            {
                NRaas.Woohooer.Settings.mChanceOfQuads = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ChanceOfQuads";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public CASAgeGenderFlags Species
        {
            get { return CASAgeGenderFlags.Human; }
        }

        public ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new ChanceOfQuadsSetting();
        }
    }
}
