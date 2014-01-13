using NRaas.CommonSpace.Options;
using NRaas.CareerSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Options.Homemaker
{
    public class HomemakerDiscountLevelSetting : IntegerSettingOption<GameObject>, IHomemakerOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mHomemakerLevelDiscount;
            }
            set
            {
                NRaas.Careers.Settings.mHomemakerLevelDiscount = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HomemakerDiscountLevel";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!HomemakerBooter.HasValue) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
