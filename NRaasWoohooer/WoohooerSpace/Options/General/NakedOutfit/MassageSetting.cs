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
    public class MassageSetting : BooleanSettingOption<GameObject>, INakedOutfitOption
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mNakedOutfitMassage;
            }
            set
            {
                Woohooer.Settings.mNakedOutfitMassage = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "NakedOutfitMassage";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override string Name
        {
            get
            {
                return Common.Localize("Location:Massage");
            }
        }
    }
}
