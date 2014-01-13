using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class HomeOpener : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        protected static Household sLastHouse = null;

        static HomeOpener()
        {
            Bootstrap();
        }

        public void OnWorldLoadFinished()
        {
            foreach (Household house in Household.sHouseholdList)
            {
                CloseHouse(house);
            }

            OpenHomes();

            new Common.DelayedEventListener(EventTypeId.kEventSimSelected, OnSimSelected);
        }

        protected static void OnSimSelected(Event e)
        {
            OpenHomes();
        }

        protected static void CloseHouse(Household house)
        {
            if (house != null)
            {
                if (house == Household.ActiveHousehold) return;

                Dictionary<Lot, bool> lots = new Dictionary<Lot, bool>();
                foreach (Sim sim in Households.AllSims(house))
                {
                    if (sim.LotCurrent == null) continue;

                    if (!lots.ContainsKey(sim.LotCurrent))
                    {
                        lots.Add(sim.LotCurrent, true);
                    }
                }

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lots.ContainsKey(lot)) continue;

                    if (lot.IsResidentialLot)
                    {
                        house.RemoveGreetedLotForHousehold(lot, ObjectGuid.InvalidObjectGuid);
                    }
                }
            }
        }

        protected static void OpenHomes()
        {
            if ((sLastHouse == null) || (sLastHouse != Household.ActiveHousehold))
            {
                CloseHouse(sLastHouse);

                sLastHouse = Household.ActiveHousehold;
            }

            if (sLastHouse != null)
            {
                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.IsResidentialLot)
                    {
                        sLastHouse.AddGreetedLotToHousehold(lot, ObjectGuid.InvalidObjectGuid);
                    }
                }
            }
        }
    }
}
