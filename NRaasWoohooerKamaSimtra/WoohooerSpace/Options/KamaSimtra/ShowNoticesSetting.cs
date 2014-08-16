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
    public class ShowNoticesSetting : BooleanSettingOption<GameObject>, IKamaSimtraOption
    {
        protected override bool Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mShowNotices;
            }
            set
            {
                Skills.KamaSimtra.Settings.mShowNotices = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShowNotices";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
