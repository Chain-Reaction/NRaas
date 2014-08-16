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
    public class ShowRendezvousIntereactionSetting : BooleanSettingOption<GameObject>, IRendezvousOption
    {
        protected override bool Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mShowRendezvousInteraction;
            }
            set
            {
                Skills.KamaSimtra.Settings.mShowRendezvousInteraction = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShowRendezvousIntereaction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
