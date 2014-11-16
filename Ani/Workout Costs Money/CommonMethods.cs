using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.UI;
using Sims3.Gameplay.Utilities;

namespace NoFreeWorkout
{
    public class CommonMethods
    {
        #region Pay For Workout
        public static void PayForWorkOut(Sim sim, Lot lot, int fee)
        {
            if (lot.IsCommunityLot)
            {
                //Pay if we don't own the lot
                Household lotOwner = ReturnLotOwner(lot);
                
                //If we don't own the lot
                if (lotOwner != null && lotOwner != sim.Household)
                {
                    lotOwner.ModifyFamilyFunds(fee);
                                      
                    //pay if we have the money, if not add to next bill
                    if (sim.FamilyFunds >= fee)
                    {
                        sim.Household.ModifyFamilyFunds(-fee);
                    }
                    else
                    {
                        sim.Household.UnpaidBills += fee;
                    }                  
                }
                else
                {
                    //if the lot has no owner, or we don't own the lot
                    if (lotOwner == null || (lotOwner != null && lotOwner != sim.Household))
                    {
                        sim.Household.ModifyFamilyFunds(-fee);
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

        #region workout Enum
        public enum WorkOut
        {
            Swim = 1,
            WorkOut = 2
        }
        #endregion

        #region Return Fee
        public static int ReturnFee(WorkOut w)
        {
            int price = 0;
            XmlDbData xdb = XmlDbData.ReadData("WorkOutConfig");
            if ((xdb != null) && (xdb.Tables != null))
            {
                XmlDbTable table;

                if (xdb.Tables.TryGetValue("WorkOut", out table) && (table != null))
                {
                    foreach (XmlDbRow row in table.Rows)
                    {
                        switch (w)
                        {
                            case WorkOut.Swim:
                                price = row.GetInt("swimfee");
                                break;
                            case WorkOut.WorkOut:
                                price = row.GetInt("workoutfee");
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
            }
            return price;
        }
        #endregion
    }
}
