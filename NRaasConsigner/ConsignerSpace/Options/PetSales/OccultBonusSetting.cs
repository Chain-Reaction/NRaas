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

namespace NRaas.ConsignerSpace.Options.PetSales
{
    public class OccultBonusSetting : IntegerSettingOption<GameObject>, IPetSalesOption
    {
        protected override int Value
        {
            get
            {
                return Consigner.Settings.mOccultBonus;
            }
            set
            {
                Consigner.Settings.mOccultBonus = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "OccultBonus";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
