using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class SortLotInventory : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "SortInventory";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Allow(Lot lot, Household house)
        {
            if (!base.Allow(lot, house)) return false;

            if (lot is WorldLot) return false;

            return (lot != null);
        }

        protected override OptionResult Run(Lot lot, Household house)
        {
            NRaas.MasterControllerSpace.Helpers.SortInventory.Item item = NRaas.MasterControllerSpace.Helpers.SortInventory.GetSortType(Name);
            if (item == null) return OptionResult.Failure;

            foreach (GameObject obj in lot.GetObjects<GameObject>())
            {
                NRaas.MasterControllerSpace.Helpers.SortInventory.Perform(obj.Inventory, item);
            }

            return OptionResult.SuccessClose;
        }
    }
}
