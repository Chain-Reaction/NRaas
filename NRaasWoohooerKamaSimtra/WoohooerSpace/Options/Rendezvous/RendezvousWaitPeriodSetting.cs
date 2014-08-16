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

namespace NRaas.WoohooerSpace.Options.Rendezvous
{
    public class RendezvousWaitPeriodSetting : IntegerSettingOption<GameObject>, IRendezvousOption
    {
        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mRendezvousWaitPeriod;
            }
            set
            {
                Skills.KamaSimtra.Settings.mRendezvousWaitPeriod = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "RendezvousWaitPeriod";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
