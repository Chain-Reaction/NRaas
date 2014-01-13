using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class RentalHelper : Common.IPreLoad, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        static Dictionary<Household, Lot> sHouseToLot = new Dictionary<Household, Lot>();

        public void OnPreLoad()
        {
            ManagerLot.OnGetLotCost += GetRentalLotCost;
            
            ManagerMoney.OnAllowRent += OnAllowRent;

            MoveInLotScenario.OnPresetLotHome += SetLotHome;
        }

        public void OnWorldLoadFinished()
        {
            // Fire against all moves, regardless of the event's name
            new Common.DelayedEventListener(EventTypeId.kLotChosenForActiveHousehold, OnLotChanged);

            sHouseToLot.Clear();
            foreach (Household house in Household.sHouseholdList)
            {
                sHouseToLot.Add(house, house.LotHome);
            }
        }

        public void OnWorldQuit()
        {
            sHouseToLot.Clear();
        }

        protected static void OnLotChanged(Event e)
        {
            Lot lot = e.TargetObject as Lot;
            if (lot == null) return;

            if (lot.Household == null) return;

            if (GameUtils.IsOnVacation()) return;

            int totalAdjustment = 0;

            Lot oldLot;
            if (sHouseToLot.TryGetValue(lot.Household, out oldLot))
            {
                // Property already adjusted
                if (lot == oldLot) return;

                ReimburseMoveOutRental(StoryProgression.Main.Money, oldLot, lot.Household, ref totalAdjustment);
            }

            ReimburseMoveInRental(StoryProgression.Main.Money, lot, ref totalAdjustment);

            if ((lot.Household == Household.ActiveHousehold) && (totalAdjustment != 0))
            {
                SimDescription head = SimTypes.HeadOfFamily(lot.Household);

                if (totalAdjustment > 0)
                {
                    Common.Notify(head, Common.Localize("Rental:AdjustmentPos", false, new object[] { lot.Household.Name, totalAdjustment }));
                }
                else
                {
                    Common.Notify(head, Common.Localize("Rental:AdjustmentNeg", false, new object[] { lot.Household.Name, -totalAdjustment }));
                }
            }

            SetLotHome(lot, lot.Household);
        }

        public static void SetLotHome(Lot lot, Household house)
        {
            sHouseToLot[house] = lot;
        }

        protected static bool OnAllowRent(Common.IStatGenerator stats, SimData settings, Managers.Manager.AllowCheck check)
        {
            return settings.GetValue<RentableOption, bool>();
        }

        public static int GetRentalLotCost(Lot lot)
        {
            if ((Common.IsAwayFromHomeworld()) || (StoryProgression.Main.Money.GetDeedOwner(lot) == null))
            {
                return lot.Cost;
            }
            else 
            {
                return lot.Cost - ManagerLot.GetUnfurnishedLotCost(lot, 0);
            }
        }

        public static int GetPurchaseLotCost(Lot lot)
        {
            if (Common.IsAwayFromHomeworld())
            {
                return lot.Cost;
            }
            else
            {
                return ManagerLot.GetUnfurnishedLotCost(lot, int.MaxValue);
            }
        }

        public static bool IsRentable(StoryProgressionObject manager, Lot lot)
        {
            if (lot.ResidentialLotSubType == ResidentialLotSubType.kEP10_PrivateLot) return false;

            if (!manager.GetValue<RentableOption, bool>(lot)) return false;

            if (lot.Household != null)
            {
                SimDescription sim = SimTypes.HeadOfFamily(lot.Household);
                if (sim != null)
                {
                    if (!manager.GetValue<RentableOption, bool>(sim)) return false;
                }
            }

            if (UnchartedIslandMarker.IsUnchartedIsland(lot)) return false;

            return true;
        }

        public static List<Lot> GetPurchaseableLots()
        {
            if (Common.IsAwayFromHomeworld())
            {
                return RealEstateManager.GetPurchaseableLots();
            }
            else
            {
                return GetPurchaseableLots(StoryProgression.Main.Money, Household.ActiveHousehold);
            }
        }
        public static List<Lot> GetPurchaseableLots(StoryProgressionObject manager, Household buyer)
        {
            Dictionary<ulong, bool> owned = new Dictionary<ulong, bool>();

            foreach (PropertyData data in RealEstateManager.AllPropertiesFromAllHouseholds())
            {
                owned[data.LotId] = true;
            }

            List<Lot> list = new List<Lot>();
            foreach (Lot lot in LotManager.AllLots)
            {
                if (lot.IsWorldLot) continue;

                if (lot.Household == buyer) continue;

                if (lot.LotType != LotType.Residential) continue;

                if (!IsRentable(manager, lot)) continue;

                if (owned.ContainsKey(lot.LotId)) continue;

                if (buyer.FamilyFunds < GetPurchaseLotCost(lot)) continue;

                list.Add(lot);
            }

            return list;
        }

        public static void PurchaseRentalLot(ManagerMoney manager, Sim buyer, Lot lot)
        {
            int cost = GetPurchaseLotCost(lot);

            Household buyerHouse = buyer.SimDescription.Household;

            int lotCost = lot.Cost;

            int loan = lotCost - buyerHouse.FamilyFunds;
            if (loan > 0)
            {
                manager.AdjustFunds(buyerHouse, "Loan", loan);
            }

            try
            {
                if (TravelUtil.CompleteLocationHomePurchase(buyer, lot))
                {
                    int difference = lotCost - cost;

                    manager.AdjustFunds(buyerHouse, "Expense", difference);

                    manager.AdjustAccounting(buyerHouse, "Expense", cost);
                    manager.AdjustAccounting(buyerHouse, "PropertyBought", -cost);

                    if (Common.IsAwayFromHomeworld()) return;
                }
            }
            finally
            {
                if (loan > 0)
                {
                    manager.AdjustFunds(buyerHouse, "Loan", -loan);
                }
            }

            PropertyData data = buyer.Household.RealEstateManager.FindProperty(lot);
            if (data == null)
            {
                throw new NullReferenceException("data");
            }

            data.mStoredValue = cost;

            manager.Lots.ClearWorth(lot);

            Household resident = lot.Household;
            if (resident == null) return;

            manager.AdjustFunds(resident, "PropertySold", cost);
        }

        public static void SellRentalLot(ManagerMoney manager, Sim seller, Lot lot)
        {
            PropertyData data = seller.Household.RealEstateManager.FindProperty(lot);
            if (data == null) return;

            int cost = data.TotalValue;

            // There is a prompt in SellHome()
            if (SimTypes.IsSelectable(seller))
            {
                data.SellHome(seller);
                if (seller.Household.RealEstateManager.FindProperty(lot) != null) return;
            }
            else
            {
                data.Owner.SellProperty(data, false);
                if (seller.Household != null)
                {
                    foreach (Sim sim in seller.Household.AllActors)
                    {
                        sim.ResetMapTagManager();
                    }
                }
            }

            manager.Lots.ClearWorth(lot);

            Household sellerHouse = seller.SimDescription.Household;

            manager.AdjustAccounting(sellerHouse, "Income", -cost);
            manager.AdjustAccounting(sellerHouse, "PropertySold", cost);

            if (Common.IsAwayFromHomeworld()) return;

            Household resident = lot.Household;
            if (resident == null) return;

            // Family living on lot buys the lot
            manager.AdjustFunds(resident, "PropertyBought", -cost);
        }

        protected static void ReimburseMoveInRental(ManagerMoney manager, Lot lot, ref int totalAdjustment)
        {
            if (lot == null) return;

            switch (lot.ResidentialLotSubType)
            {
                case ResidentialLotSubType.kEP1_PlayerOwnable:
                case ResidentialLotSubType.kEP10_PrivateLot:
                    return;
            }

            Household owner = manager.GetDeedOwner(lot);
            if (owner == null) return;

            Household resident = lot.Household;
            if (resident == null) return;

            int adjustment = ManagerLot.GetUnfurnishedLotCost(lot, 0);

            totalAdjustment += adjustment;

            manager.AdjustFunds(resident, "RentalAdjustment", adjustment);
        }

        protected static void ReimburseMoveOutRental(ManagerMoney manager, Lot lot, Household resident, ref int totalAdjustment)
        {
            if (lot == null) return;

            switch (lot.ResidentialLotSubType)
            {
                case ResidentialLotSubType.kEP1_PlayerOwnable:
                case ResidentialLotSubType.kEP10_PrivateLot:
                    return;
            }

            if (resident == null) return;

            Household owner = manager.GetDeedOwner(lot);
            if (owner == null) return;

            int adjustment = ManagerLot.GetUnfurnishedLotCost(lot, 0);

            totalAdjustment -= adjustment;

            manager.AdjustFunds(resident, "RentalAdjustment", -adjustment);
        }
    }
}
