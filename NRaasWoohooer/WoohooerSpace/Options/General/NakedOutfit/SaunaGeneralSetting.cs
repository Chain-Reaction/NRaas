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
    public class SaunaGeneralSetting : BooleanSettingOption<GameObject>, INakedOutfitOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mNakedOutfitSaunaGeneral;
            }
            set
            {
                NRaas.Woohooer.Settings.mNakedOutfitSaunaGeneral = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "NakedOutfitSaunaGeneral";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
