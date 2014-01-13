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

namespace NRaas.MasterControllerSpace.Settings.CAS
{
    public class HairColorByOutfitSetting : BooleanSettingOption<GameObject>, ICASOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.MasterController.Settings.mHairColorByOutfit;
            }
            set
            {
                NRaas.MasterController.Settings.mHairColorByOutfit = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HairColorByOutfitSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
