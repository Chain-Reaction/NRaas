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

namespace NRaas.WoohooerSpace.Options.Woohoo.ValidLocation
{
    public class HotAirBalloonSetting : BooleanSettingOption<GameObject>, IValidLocationOption
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mAutonomousHotAirBalloon;
            }
            set
            {
                Woohooer.Settings.mAutonomousHotAirBalloon = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AutonomousHotAirBalloon";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override string Name
        {
            get
            {
                return Common.Localize("Location:HotAirBalloon");
            }
        }
    }
}
