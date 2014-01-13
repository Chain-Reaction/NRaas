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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits
{
    public class FullAdultRerollSetting : BooleanSettingOption<GameObject>, ICheckOutfitsOption
    {
        protected override bool Value
        {
            get
            {
                return Dresser.Settings.mFullAdultReroll;
            }
            set
            {
                Dresser.Settings.mFullAdultReroll = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "FullAdultReroll";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Dresser.Settings.mCheckOutfits) return false;

            return base.Allow(parameters);
        }
    }
}
