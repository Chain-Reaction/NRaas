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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Hair.RandomBodyHair
{
    public class MaxRandomFemaleBodyHairSetting : IntegerSettingOption<GameObject>, IRandomBodyHairOption
    {
        protected override int Value
        {
            get
            {
                return Dresser.Settings.mRandomBodyHair.mMaxFemale;
            }
            set
            {
                Dresser.Settings.mRandomBodyHair.mMaxFemale = value;
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
