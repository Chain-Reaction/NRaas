using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Accessories.RandomAccessories
{
    public class MaxRandomFemaleAccessoriesSetting : IntegerSettingOption<GameObject>, IRandomAccessoriesOption
    {
        protected override int Value
        {
            get
            {
                return Dresser.Settings.mRandomAccessories.mMaxFemale;
            }
            set
            {
                Dresser.Settings.mRandomAccessories.mMaxFemale = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaxFemale";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
