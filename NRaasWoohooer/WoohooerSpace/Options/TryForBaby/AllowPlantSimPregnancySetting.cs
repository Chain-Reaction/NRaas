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

namespace NRaas.WoohooerSpace.Options.TryForBaby
{
    public class AllowPlantSimPregnancySetting : BooleanSettingOption<GameObject>, ITryForBabyOption
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mAllowPlantSimPregnancy;
            }
            set
            {
                Woohooer.Settings.mAllowPlantSimPregnancy = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowPlantSimPregnancy";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
