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
    public enum MyLoveBuffLevel
    {
        Default = 0,
        Partner,
        Spouse,
        AnyRomantic
    }

    public class MyLoveBuffSetting : EnumSettingOption<MyLoveBuffLevel,GameObject>, IRomanceOption
    {
        protected override MyLoveBuffLevel Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mMyLoveBuffLevel;
            }
            set
            {
                NRaas.Woohooer.Settings.mMyLoveBuffLevel = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MyLoveBuff";
        }

        public override MyLoveBuffLevel Default
        {
            get { return WoohooerTuning.kMyLoveBuffLevel; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
