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
    public class HayStackSetting : BooleanSettingOption<GameObject>, INakedOutfitOption
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mNakedOutfitHayStack;
            }
            set
            {
                Woohooer.Settings.mNakedOutfitHayStack = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "NakedOutfitHayStack";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override string Name
        {
            get
            {
                return Common.Localize("Location:HayStack");
            }
        }
    }
}
