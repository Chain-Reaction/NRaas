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

namespace NRaas.MasterControllerSpace.Settings.Visibility.Object
{
    public class VisibilityRadiusPurgeSetting : VisibilitySettingOption, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "RadiusPurge";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.MasterController.Settings.mMenuVisibleRadiusPurge;
            }
            set
            {
                NRaas.MasterController.Settings.mMenuVisibleRadiusPurge = value;
            }
        }
    }
}
