using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
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
    public class RealEstate : HouseholdFromList, IHouseholdOption
    {
        public enum OwnerType
        {
            NotOwned,
            Partial,
            Full,
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        public override string GetTitlePrefix()
        {
            return "RealEstate";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (me == null) return false;

            if (me.RealEstateManager == null) return false;

            return true;
        }

        protected static bool IsAlreadyOwned(RabbitHole hole, Household me)
        {
            foreach (Household house in Household.sHouseholdList)
            {
                if (house == me) continue;

                if (house.RealEstateManager == null) continue;

                PropertyData data = house.RealEstateManager.FindProperty(hole);
                if (data != null)
                {
                    return data.IsFullOwner;
                }
            }

            return false;
        }
        protected static bool IsAlreadyOwned(Lot lot, Household me)
        {
            foreach (Household house in Household.sHouseholdList)
            {
                if (house == me) continue;

                if (house.RealEstateManager == null) continue;

                PropertyData data = house.RealEstateManager.FindProperty(lot);
                if (data != null)
                {
                    return data.IsFullOwner;
                }
            }

            return false;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (me != null)
            {
                List<ItemBase> allOptions = new List<ItemBase>();

                foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    if (!hole.RabbitHoleTuning.kCanInvestHere) continue;

                    if (IsAlreadyOwned(hole, me)) continue;

                    OwnerType type = OwnerType.NotOwned;

                    PropertyData data = me.RealEstateManager.FindProperty(hole);
                    if (data != null)
                    {
                        if (data.IsFullOwner)
                        {
                            type = OwnerType.Full;
                        }
                        else
                        {
                            type = OwnerType.Partial;
                        }
                    }

                    allOptions.Add(new RabbitHoleItem(hole, type));
                }

                foreach (Lot venue in LotManager.AllLots)
                {
                    if (IsAlreadyOwned(venue, me)) continue;

                    OwnerType type = OwnerType.NotOwned;

                    int totalValue = 0;

                    PropertyData data = me.RealEstateManager.FindProperty(venue);
                    if (data != null)
                    {
                        type = OwnerType.Full;

                        totalValue = data.TotalValue;
                    }
                    else
                    {
                        if (!venue.IsPurchaseableVenue) continue;
                    }

                    allOptions.Add(new VenueItem(venue, type, totalValue));
                }

                CommonSelection<ItemBase>.Results choices = new CommonSelection<ItemBase>(Name, allOptions).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

                List<ItemBase> purchase = new List<ItemBase>();

                foreach (ItemBase item in choices)
                {
                    if (item.mType != OwnerType.Full)
                    {
                        purchase.Add(item);
                    }
                    else
                    {
                        item.Perform(me);
                    }
                }

                foreach (ItemBase item in purchase)
                {
                    if (!item.Perform(me))
                    {
                        SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Failure", false, new object[] { me.Name }));
                        return OptionResult.Failure;
                    }
                }
            }

            return OptionResult.SuccessClose;
        }

        public abstract class ItemBase : CommonOptionItem
        {
            public readonly OwnerType mType;

            public ItemBase(string name, OwnerType type)
                : base(name)
            {
                mType = type;
            }

            public override string Name
            {
                get
                {
                    return base.Name + " (" + EAText.GetMoneyString(Cost) + ")";
                }
            }

            public override string DisplayValue
            {
                get
                {
                    return Common.Localize("RealEstate:" + mType);
                }
            }

            public abstract bool Perform(Household house);

            public abstract int Cost
            {
                get;
            }
        }

        public class RabbitHoleItem : ItemBase
        {
            RabbitHole mHole;

            public RabbitHoleItem(RabbitHole hole, OwnerType type)
                : base(hole.CatalogName, type)
            {
                mHole = hole;
            }

            public override int Cost
            {
                get 
                {
                    if (mType == OwnerType.NotOwned)
                    {
                        return mHole.RabbitHoleTuning.kInvestCost;
                    }
                    else if (mType == OwnerType.Partial)
                    {
                        return mHole.RabbitHoleTuning.kBuyoutCost;
                    }
                    else
                    {
                        return -mHole.RabbitHoleTuning.kBuyoutCost;
                    }
                }
            }

            public override bool Perform(Household house)
            {
                int purchase = 0;
                if (mType == OwnerType.NotOwned)
                {
                    purchase = mHole.RabbitHoleTuning.kInvestCost;
                }
                else
                {
                    if (mType == OwnerType.Partial)
                    {
                        if (TwoButtonDialog.Show(
                            Common.Localize("RealEstate:BuyPrompt", false, new object[] { house.Name, mHole.CatalogName }),
                            Common.Localize("RealEstate:Full"),
                            Common.Localize("RealEstate:Sell")
                        ))
                        {
                            purchase = mHole.RabbitHoleTuning.kBuyoutCost;
                        }
                    }
                }

                if (purchase > 0)
                {
                    if (purchase > house.FamilyFunds)
                    {
                        return false;
                    }

                    house.RealEstateManager.PurchaseorUpgradeRabbitHole(mHole);
                    return true;
                }
                else
                {
                    house.RealEstateManager.SellProperty(house.RealEstateManager.FindProperty(mHole), false);
                    return true;
                }
            }
        }

        public class VenueItem : ItemBase
        {
            Lot mVenue;

            int mTotalValue;

            public VenueItem(Lot venue, OwnerType type, int totalValue)
                : base(venue.Name, type)
            {
                mVenue = venue;
                mTotalValue = totalValue;
            }

            public override int Cost
            {
                get 
                {
                    if (mType == OwnerType.NotOwned)
                    {
                        return RealEstateData.GetVenuePurchaseCost(mVenue);
                    }
                    else
                    {
                        if (mVenue.IsResidentialLot)
                        {
                            return -mTotalValue;
                        }
                        else
                        {
                            return -RealEstateData.GetVenuePurchaseCost(mVenue);
                        }
                    }
                }
            }

            public override bool Perform(Household house)
            {
                if (mType == OwnerType.NotOwned)
                {
                    if (mVenue.IsResidentialLot)
                    {
                        return false;
                    }
                    else if (RealEstateData.GetVenuePurchaseCost(mVenue) > house.FamilyFunds)
                    {
                        return false;
                    }

                    house.RealEstateManager.PurchaseVenue(mVenue);
                    return true;
                }
                else
                {
                    house.RealEstateManager.SellProperty(house.RealEstateManager.FindProperty(mVenue), false);
                    return true;
                }
            }
        }
    }
}
