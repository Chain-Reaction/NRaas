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
    public class PhotoBoothSetting : BooleanSettingOption<GameObject>, IValidLocationOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mAutonomousPhotoBooth;
            }
            set
            {
                NRaas.Woohooer.Settings.mAutonomousPhotoBooth = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AutonomousPhotoBooth";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override string Name
        {
            get
            {
                return Common.Localize("Location:PhotoBooth");
            }
        }
    }
}
