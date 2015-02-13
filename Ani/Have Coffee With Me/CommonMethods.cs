using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Utilities;

namespace HaveCoffeeWithMe
{
    public class CommonMethods
    {
        #region Pay For Coffee
        public static void PayForCoffee(Sim sim, Lot lot)
        {
            //Pay for the coffee if we don't own the lot
            Household lotOwner = ReturnLotOwner(lot);

            if (lot.IsCommunityLot)
            {
                //If we don't own the lot
                int price = ReturnPrice();
                if (lotOwner != null && lotOwner != sim.Household)
                {
                    lotOwner.ModifyFamilyFunds(price);

                    //Can the customer pay, 
                    if (sim.Household.FamilyFunds >= price)
                    {
                        sim.Household.ModifyFamilyFunds(-price);
                    }
                    else
                    {
                        //Add to next bill
                        sim.Household.UnpaidBills += price;
                    }
                }
                else
                {
                    //if the lot has no owner
                    if (lotOwner == null)
                    {
                        sim.Household.ModifyFamilyFunds(-price);
                    }
                }
            }
        }
        #endregion

        #region ReturnOwner
        public static Household ReturnLotOwner(Lot lot)
        {
            Household lotOwner = null;
            if (lot != null)
            {
                //Check first is the lot a rabbit hole lot.
                List<RabbitHole> rList = new List<RabbitHole>(Sims3.Gameplay.Queries.GetObjects<RabbitHole>());
                RabbitHole rhOnThisLot = null;
                foreach (RabbitHole r in rList)
                {
                    if (r.LotCurrent == lot)
                    {
                        rhOnThisLot = r;
                        break;
                    }
                }
                //Is there a rabbit hole on this lot
                if (rhOnThisLot != null)
                {
                    List<Household> hList = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
                    Sims3.Gameplay.RealEstate.PropertyData pd = null;
                    if (hList != null && hList.Count > 0)
                    {
                        foreach (Household h in hList)
                        {
                            pd = h.RealEstateManager.FindProperty(rhOnThisLot);

                            if (pd != null && pd.Owner != null)
                            {
                                lotOwner = pd.Owner.OwningHousehold;
                                break;
                            }
                        }
                    }
                }

                //If the lot is not a RH lot, check for venues
                if (lotOwner == null)
                {
                    List<Household> hList = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
                    Sims3.Gameplay.RealEstate.PropertyData pd = null;
                    if (hList != null && hList.Count > 0)
                    {
                        foreach (Household h in hList)
                        {
                            pd = h.RealEstateManager.FindProperty(lot);

                            if (pd != null && pd.Owner != null)
                            {
                                lotOwner = pd.Owner.OwningHousehold;
                                break;
                            }
                        }
                    }
                }
            }
            return lotOwner;
        }
        #endregion

        #region Interaction index
        public static List<InteractionObjectPair> ReturnInteractionIndex(List<InteractionObjectPair> list, String s)
        {
            List<InteractionObjectPair> interactions = list.FindAll(
                delegate(InteractionObjectPair p)
                {
                    return p.ToString().Contains(s);
                });

            return interactions;
        }
        #endregion

        #region Return price
        public static int ReturnPrice()
        {
            int price = 0;
            XmlDbData xdb = XmlDbData.ReadData("CoffeeConfig");
            if ((xdb != null) && (xdb.Tables != null))
            {
                XmlDbTable table;

                if (xdb.Tables.TryGetValue("Coffee", out table) && (table != null))
                {
                    foreach (XmlDbRow row in table.Rows)
                    {
                        price = row.GetInt("price");
                    }
                }
            }
            return price;
        }
        #endregion
    }
}
