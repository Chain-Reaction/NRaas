using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims;
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
    public class ClothingSortOrderSetting : EnumSettingOption<CASBase.SortOrder, GameObject>, ICASOption
    {
        protected override CASBase.SortOrder Value
        {
            get
            {
                return NRaas.MasterController.Settings.mClothingSortOrder;
            }
            set
            {
                NRaas.MasterController.Settings.mClothingSortOrder = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ClothingSortOrderSetting";
        }

        public override CASBase.SortOrder Default
        {
            get { return CASBase.SortOrder.EAStandard; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
