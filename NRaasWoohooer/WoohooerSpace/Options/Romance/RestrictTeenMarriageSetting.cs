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

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class RestrictTeenMarriageSetting : BooleanSettingOption<GameObject>, IRomanceOption
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mRestrictTeenMarriage;
            }
            set
            {
                Woohooer.Settings.mRestrictTeenMarriage = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "RestrictTeenMarriage";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
