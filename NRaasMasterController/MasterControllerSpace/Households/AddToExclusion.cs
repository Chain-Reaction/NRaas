using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class AddToExclusion : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "AddToExclusion";
        }

        protected override Lot GetLot(SimDescription sim)
        {
            return sim.LotHome;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot is WorldLot) return false;

            if (lot == null) return false;

            return (!MasterController.Settings.IsExcludedLot(lot));
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            MasterController.Settings.AddExcludedLot(lot);

            return OptionResult.SuccessClose;
        }
    }
}
