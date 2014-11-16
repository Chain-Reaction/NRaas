using Sims3.UI;
using System.Collections.Generic;
using System;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.CAS;
using System.Collections;
using Sims3.SimIFace;
using System.Text;
using Sims3.UI.CAS;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Objects.Decorations.Mimics;
namespace ani_ShopForClothes
{
    public class CommonMethods
    {      

        // Methods             
        public static void HandlePayments(int price, SimDescription sim)
        {           
           
            //Return lot owner
            Household household = null;
            if (((sim.CreatedSim != null) && (sim.CreatedSim.LotCurrent != null)) && sim.CreatedSim.LotCurrent.IsCommunityLot)
            {
                household = ReturnLotOwner(sim.CreatedSim.LotCurrent);
            }
         
            if ((household == null) || ((household != null) && (sim.Household != household)))
            {
                if (sim.Household.FamilyFunds >= price)
                {
                    sim.Household.ModifyFamilyFunds(-price);
                }
                else if (price > 0)
                {
                    Household household1 = sim.Household;
                    household1.UnpaidBills += price;
                    StyledNotification.Show(new StyledNotification.Format(LocalizeString("CantAffordToPay", new object[] { sim.FullName }), StyledNotification.NotificationStyle.kGameMessagePositive));
                }
            }
            //Pay Lot owner
            if ((household != null) && (sim.Household != household))
            {
                household.ModifyFamilyFunds(price);
                StyledNotification.Show(new StyledNotification.Format(LocalizeString("LotOwnerEarned", new object[] { household.Name, price }), StyledNotification.NotificationStyle.kGameMessagePositive));
            }
          //  StyledNotification.Show(new StyledNotification.Format(LocalizeString("ItemsPurchased", new object[] { sim.FullName, itemCount.ToString(), num * itemCount }), StyledNotification.NotificationStyle.kGameMessagePositive));
        }

        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("ShopForClothes:" + name, parameters);
        }
                

        public static void PerformeInteraction(Sim sim, SculptureFloorClothingRack2x1 target, InteractionDefinition definition)
        {
            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
            InteractionInstance entry = definition.CreateInstance(target, sim, priority, false, true);
            sim.InteractionQueue.Add(entry);
        }       

        public static Household ReturnLotOwner(Lot lot)
        {
            Household owningHousehold = null;
            if (lot != null)
            {
                List<RabbitHole> list = new List<RabbitHole>(Sims3.Gameplay.Queries.GetObjects<RabbitHole>());
                RabbitHole rabbitHole = null;
                foreach (RabbitHole hole2 in list)
                {
                    if (hole2.LotCurrent == lot)
                    {
                        rabbitHole = hole2;
                        break;
                    }
                }
                if (rabbitHole != null)
                {
                    List<Household> list2 = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
                    PropertyData data = null;
                    if ((list2 != null) && (list2.Count > 0))
                    {
                        foreach (Household household2 in list2)
                        {
                            data = household2.RealEstateManager.FindProperty(rabbitHole);
                            if ((data != null) && (data.Owner != null))
                            {
                                owningHousehold = data.Owner.OwningHousehold;
                                break;
                            }
                        }
                    }
                }
                if (owningHousehold == null)
                {
                    List<Household> list3 = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
                    PropertyData data2 = null;
                    if ((list3 == null) || (list3.Count <= 0))
                    {
                        return owningHousehold;
                    }
                    foreach (Household household3 in list3)
                    {
                        data2 = household3.RealEstateManager.FindProperty(lot);
                        if ((data2 != null) && (data2.Owner != null))
                        {
                            return data2.Owner.OwningHousehold;
                        }
                    }
                }
            }
            return owningHousehold;
        }
       
    }


}
