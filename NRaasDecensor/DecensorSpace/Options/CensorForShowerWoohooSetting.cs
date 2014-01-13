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

namespace NRaas.DecensorSpace.Options
{
    public class CensorForShowerWoohooSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Decensor.Settings.mCensorForShowerWoohoo;
            }
            set
            {
                Decensor.Settings.mCensorForShowerWoohoo = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CensorForShowerWoohoo";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
