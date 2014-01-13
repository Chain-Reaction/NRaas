using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.CAS
{
    public class BodySliderMultipleSetting : IntegerSettingOption<GameObject>, ICASOption
    {
        public override string GetTitlePrefix()
        {
            return "BodySliderMultipleSetting";
        }

        protected override int Value
        {
            get
            {
                return NRaas.MasterController.Settings.mBodySliderMultiple;
            }
            set
            {
                NRaas.MasterController.Settings.mBodySliderMultiple = value;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
