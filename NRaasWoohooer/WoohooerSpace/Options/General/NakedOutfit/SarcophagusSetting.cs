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

namespace NRaas.WoohooerSpace.Options.General.NakedOutfit
{
    public class SarcophagusSetting : BooleanSettingOption<GameObject>, INakedOutfitOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mNakedOutfitSarcophagus;
            }
            set
            {
                NRaas.Woohooer.Settings.mNakedOutfitSarcophagus = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "NakedOutfitSarcophagus";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
        
        public override string Name
        {
            get
            {
                return Common.Localize("Location:Sarcophagus");
            }
        }
    }
}
