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
    public class FoodTruckManagerEx
    {
        protected static bool IsValidLot(Lot lot, FoodTruckBase truck)
        {
            if (lot == null) return false;

            FoodTruckBase existing = TruckController.GetTruck(lot);
            if (existing != null)
            {
                if (existing != truck) return false;
            }

            if (lot.StreetParking == null) return false;

            if (lot.IsWorldLot) return false;

            if (Traffic.Settings.mRequireFoodParkingSpace)
            {
                List<FoodTruckBase> trucks = new List<FoodTruckBase>(lot.GetObjects<FoodTruckBase>());
                trucks.Remove(truck);

                if ((lot.CountObjects<WideParkingSpace>() == 0x0) || (trucks.Count > 0x0))
                {
                    return false;
                }
            }

            if (lot.IsCommunityLot)
            {
                float openHour = 0f;
                float closingHour = 0f;
                if ((Bartending.TryGetHoursOfOperation(lot, ref openHour, ref closingHour)) && (!SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, openHour, closingHour)))
                {
                    return false;
                }
            }
            else
            {
                if (lot.Household == null) return false;

                if (!Traffic.Settings.mAllowFoodTruckResidential) return false;

                if (lot.Household.IsActive)
                {
                    if (!Traffic.Settings.mAllowFoodTruckActiveLot) return false;
                }
            }

            return true;
        }

        public static void Update()
        {
            List<Lot> lotChoices = new List<Lot>();
            foreach (Lot lot in LotManager.AllLots)
            {
                if (!IsValidLot(lot, null)) continue;

                lotChoices.Add(lot);
            }

            List<FoodTruck> truckList = new List<FoodTruck>(Sims3.Gameplay.Queries.GetObjects<FoodTruck>());

            int trucksToSpawn = 0x0;
            int count = truckList.Count;
            if (count < Traffic.Settings.mMaxFoodTrucks)
            {
                if (lotChoices.Count > FoodTruckManager.kMinWideParkingSpaceRequired)
                {
                    trucksToSpawn = 0x1;
                }
                else if (lotChoices.Count < FoodTruckManager.kMinWideParkingSpaceRequired)
                {
                    trucksToSpawn = lotChoices.Count - FoodTruckManager.kMinWideParkingSpaceRequired;
                }
            }
            else if (count > Traffic.Settings.mMaxFoodTrucks)
            {
                trucksToSpawn = Traffic.Settings.mMaxFoodTrucks - count;
            }
            else if ((Traffic.Settings.mRequireFoodParkingSpace) && (lotChoices.Count < FoodTruckManager.kMinWideParkingSpaceRequired))
            {
                trucksToSpawn = lotChoices.Count - FoodTruckManager.kMinWideParkingSpaceRequired;
            }

            if ((trucksToSpawn > 0x0) && (lotChoices.Count > 0x0))
            {
                Lot randomObjectFromList = RandomUtil.GetRandomObjectFromList(lotChoices);
                lotChoices.Remove(randomObjectFromList);

                FoodTruck truck = FoodTruckManager.AddTruckIntoWorld(randomObjectFromList);
                if (truck != null)
                {
                    truckList.Add(truck);
                }
            }

            if (Common.kDebugging)
            {
                Common.DebugNotify("Food Truck Manager: " + truckList.Count + " " + trucksToSpawn + " " + lotChoices.Count);
            }

            if (truckList.Count > 0x0)
            {
                RandomUtil.RandomizeListOfObjects<FoodTruck>(truckList);
                foreach (FoodTruck truck in truckList)
                {
                    if ((trucksToSpawn < 0x0) || (lotChoices.Count == 0) || ((SimClock.IsNightTime()) && (!Traffic.Settings.mAllowFoodTruckAtNight)))
                    {
                        truck.FadeOut(false, true);
                        trucksToSpawn++;
                    }
                    else if (lotChoices.Count > 0x0)
                    {
                        bool moveToNewLot = FoodTruckEx.MoveToNewLot(truck);

                        if ((moveToNewLot) || (!IsValidLot(TruckController.GetLot(truck), truck)))
                        {
                            Lot item = RandomUtil.GetRandomObjectFromList(lotChoices);
                            lotChoices.Remove(item);

                            FoodTruckEx.RouteToNewLot(truck, item);

                            Common.DebugNotify("Food Truck: ", truck.ObjectId, item.ObjectId);

                            TruckController.AddTruck(truck);
                        }
                    }

                    FoodTruckEx.AddMapTags(truck);
                }
            }
        }
    }
}


