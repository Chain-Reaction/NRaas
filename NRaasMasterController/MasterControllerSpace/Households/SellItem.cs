using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fireplaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Insect;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class SellItem : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "SellItem";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (me == null) return false;

            if (me.SharedFamilyInventory == null) return false;

            if (me.SharedFamilyInventory.Inventory == null) return false;

            return true;
        }

        protected override OptionResult Run(Lot lot, Household house)
        {
            throw new NotImplementedException();
        }

        protected override OptionResult RunAll(List<LotHouseItem> houses)
        {
            string subTitle = null;

            List<Inventory> inventories = new List<Inventory>();

            foreach (LotHouseItem item in houses)
            {
                if (item.mHouse == null) continue;

                subTitle = item.mHouse.Name;

                inventories.Add(item.mHouse.SharedFamilyInventory.Inventory);
            }

            if (Sims.Basic.SellItem.Perform(Name, subTitle, inventories))
            {
                return OptionResult.SuccessClose;
            }
            else
            {
                return OptionResult.Failure;
            }
        }
    }
}
