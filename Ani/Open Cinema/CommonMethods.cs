using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Sims3.UI;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.RabbitHoles;
using System.Collections.Generic;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.UI.Hud;
using Sims3.Gameplay.EventSystem;

namespace OpenCinema
{
    public class CommonMethods
    {
        #region Pay For Movie
        public static void PayForMovie(Sim sim, Lot lot, int fee)
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

                    //If we are playing the lot owner, and somebody watches a movie, inform about the money earned
                    //if (lotOwner.IsActive)
                    //{
                    //    StyledNotification.Show(new StyledNotification.Format(sim.Name + " Watched a movie ", StyledNotification.NotificationStyle.kGameMessagePositive));
                    //}
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
                List<Theatre> rList = new List<Theatre>(Sims3.Gameplay.Queries.GetObjects<Theatre>());
                Theatre rhOnThisLot = null;
                foreach (Theatre r in rList)
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

        #region Fulfil Want

        public static void FulfillWant(Sim sim, Theatre theater)
        {
            EventTracker.SendEvent(new AttendedShowEvent(Sims3.Gameplay.Abstracts.ShowVenue.ShowTypes.kMovie, sim));
            EventTracker.SendEvent(EventTypeId.kReceivedRabbitHoleTreatment, sim, theater);
        }
        #endregion
    }
}
