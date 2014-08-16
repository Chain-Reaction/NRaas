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

namespace NRaas.WoohooerSpace.Options.KamaSimtra
{
    public class ShowRegisterInteractionSetting : BooleanSettingOption<GameObject>, IKamaSimtraOption
    {
        protected override bool Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mShowRegisterInteraction;
            }
            set
            {
                Skills.KamaSimtra.Settings.mShowRegisterInteraction = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShowRegisterInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
