using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.UI;

namespace Alcohol
{
    public class CommonMethods
    {
        #region PrintMessage
        /// <summary>
        /// Print message on screen
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message)
        {
            StyledNotification.Show(new StyledNotification.Format(message, StyledNotification.NotificationStyle.kGameMessagePositive));
        }
        #endregion

        #region Localization
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Alcohol:" + name, parameters);
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

        #region Return lot owner
        public static Household ReturnLotOwner(Lot lot)
        {
            Household lotOwner = null;

            if (lot != null)
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

            return lotOwner;
        }
        #endregion

        #region Check Availability

        public static bool IsSociable(Sim sim)
        {
            bool sociable = true;
            if (sim.CareerManager.Occupation != null)
            {
                if (sim.CareerManager.Occupation.IsAtWork || ((sim != null) && Occupation.SimIsWorkingAtJobOnLot(sim, sim.LotCurrent)))
                {
                    sociable = false;
                }
                else if (sim.CareerManager.Occupation.ShouldBeAtWork())
                {
                    sociable = false;
                }                
            }
            return sociable;
        }

 


        #endregion
    }
}
