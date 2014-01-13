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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits
{
    public class UseMasterControllerBlackListSetting : BooleanSettingOption<GameObject>, ICheckOutfitsOption
    {
        protected override bool Value
        {
            get
            {
                return Dresser.Settings.mUseMasterControllerBlackList;
            }
            set
            {
                Dresser.Settings.mUseMasterControllerBlackList = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "UseMasterControllerBlackList";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
