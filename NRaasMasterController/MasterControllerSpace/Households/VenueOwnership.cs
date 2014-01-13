using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.RealEstate;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class VenueOwnership : HouseholdFromList, IHouseholdOption
    {
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

        public override string GetTitlePrefix()
        {
            return "Ownership";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot == null) return false;

            if (lot.IsPurchaseableVenue) return true;

            return lot.IsResidentialLot;
        }

        protected override int GetCount(Lot lot, Household lotHouse)
        {
            if (lot == null) return 0;

            int count = 0;

            foreach (Household house in Household.sHouseholdList)
            {
                if (house.RealEstateManager == null) continue;

                PropertyData data = house.RealEstateManager.FindProperty(lot);
                if (data == null) continue;

                count++;
            }

            return count;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (lot != null)
            {
                List<Item> allOptions = new List<Item>();

                foreach (Household house in Household.sHouseholdList)
                {
                    if (house.RealEstateManager == null) continue;

                    PropertyData data = house.RealEstateManager.FindProperty(lot);
                    if (data == null) continue;

                    allOptions.Add(new Item(house, lot, RealEstate.OwnerType.Full, data.TotalValue));
                }

                if (allOptions.Count == 0)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Failure"));
                    return OptionResult.Failure;
                }

                CommonSelection<Item>.Results choices = new CommonSelection<Item>(Name, allOptions).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

                foreach (Item item in choices)
                {
                    item.Perform();
                }
            }

            return OptionResult.SuccessClose;
        }

        public class Item : RealEstate.VenueItem
        {
            Household mHouse;

            public Item(Household house, Lot venue, RealEstate.OwnerType type, int totalValue)
                : base(venue, type, totalValue)
            {
                mHouse = house;
                mName = TownFamily.GetQualifiedName(house);
            }

            public bool Perform()
            {
                return Perform(mHouse);
            }
        }
    }
}
