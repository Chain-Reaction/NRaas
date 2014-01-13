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
    public class AllowStrayPetAccessoriesSetting : BooleanSettingOption<GameObject>, IAccessoriesOption
    {
        protected override bool Value
        {
            get
            {
                return Dresser.Settings.mAllowStrayPetAccessories;
            }
            set
            {
                Dresser.Settings.mAllowStrayPetAccessories = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowStrayPetAccessories";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Dresser.Settings.mAllowPetAccessories) return false;

            return base.Allow(parameters);
        }
    }
}
