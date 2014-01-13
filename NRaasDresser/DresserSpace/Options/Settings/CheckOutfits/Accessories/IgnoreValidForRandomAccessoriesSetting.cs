using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Accessories
{
    public class IgnoreValidForRandomAccessoriesSetting : BooleanSettingOption<GameObject>, IAccessoriesOption
    {
        protected override bool Value
        {
            get
            {
                return Dresser.Settings.mIgnoreValidForRandomAccessories;
            }
            set
            {
                Dresser.Settings.mIgnoreValidForRandomAccessories = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "IgnoreValidForRandom";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
