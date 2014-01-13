using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.TrafficSpace.Helpers
{
    public class TruckController
    {
        static Dictionary<ulong, FoodTruckBase> sLotTrucks = new Dictionary<ulong, FoodTruckBase>();

        static Dictionary<FoodTruckBase, ulong> sTruckLots = new Dictionary<FoodTruckBase, ulong>();

        public static Lot GetLot(FoodTruckBase truck)
        {
            ulong lotId;
            if (!sTruckLots.TryGetValue(truck, out lotId)) return null;

            return LotManager.GetLot(lotId);
        }

        public static FoodTruckBase GetTruck(Lot lot)
        {
            FoodTruckBase truck;
            if (!sLotTrucks.TryGetValue(lot.LotId, out truck)) return null;

            if (!truck.HasBeenDestroyed)
            {
                return truck;
            }
            else
            {
                sTruckLots.Remove(truck);

                sLotTrucks.Remove(lot.LotId);
                return null;
            }
        }

        public static void AddTruck(FoodTruckBase truck)
        {
            sTruckLots.Remove(truck);

            if (truck.mDestinationLot != null)
            {
                sLotTrucks.Remove(truck.mDestinationLot.LotId);
                sLotTrucks.Add(truck.mDestinationLot.LotId, truck);

                sTruckLots.Add(truck, truck.mDestinationLot.LotId);
            }
        }
    }
}


