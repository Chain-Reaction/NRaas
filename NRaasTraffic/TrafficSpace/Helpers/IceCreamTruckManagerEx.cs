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
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.TrafficSpace.Helpers
{
    public class IceCreamTruckManagerEx
    {
        private static void AddTruckIntoWorld(IceCreamTruckManager ths, Lot lot)
        {
            IceCreamTruck car = GlobalFunctions.CreateObjectOutOfWorld("carTruckIceCream", ProductVersion.EP5, "Sims3.Gameplay.Objects.Vehicles.IceCreamTruck", null) as IceCreamTruck;
            if (car != null)
            {
                Vector3[] vectorArray;
                Quaternion[] quaternionArray;

                /*
                Lot lot = null;
                foreach (Lot lot2 in LotManager.AllLots)
                {
                    if (!lot2.IsWorldLot)
                    {
                        lot = lot2;
                        break;
                    }
                }
                */

                Matrix44 mat = new Matrix44();
                car.StreetParkingBeingUsed = lot.StreetParking;
                car.StreetParkingBeingUsed.GetParkingSpotForCar(car, ref mat);
                if (World.FindPlaceOnRoadOffScreen(car.Proxy, mat.pos.V3, FindPlaceOnRoadOption.Road, 150f, out vectorArray, out quaternionArray))
                {
                    car.PlaceAt(vectorArray[0x0], mat.at.V3, null);
                    car.BeginRoute(lot);
                    car.AddMapTags();
                    ths.mIceCreamTrucks.Add(car);
                }
                else
                {
                    car.Destroy();
                }
            }
        }

        public static void Update(IceCreamTruckManager ths)
        {
            bool winter = SeasonsManager.Enabled && (SeasonsManager.CurrentSeason == Season.Winter);
            int[] numArray = (SeasonsManager.Enabled && (SeasonsManager.CurrentSeason == Season.Summer)) ? IceCreamTruckManager.kHoursOfOperationSummer : IceCreamTruckManager.kHoursOfOperation;

            if ((winter || !SimClock.IsTimeBetweenTimes((float)numArray[0], (float)numArray[1])) || ((SimClock.IsNightTime()) && (!Traffic.Settings.mAllowIceCreamAtNight)))
            {
                foreach (IceCreamTruck truck in new List<IceCreamTruck>(ths.mIceCreamTrucks))
                {
                    truck.EndService();
                }

                ths.mIceCreamTrucks.Clear();
            }
            else
            {
                List<IceCreamTruckManager.LotData> data = null;
                if (data == null)
                {
                    data = new List<IceCreamTruckManager.LotData>();
                    PopulateLotList(data);
                }

                if ((data.Count > 0) && (ths.mIceCreamTrucks.Count < Traffic.Settings.mMaxIceCreamTrucks))
                {
                    IceCreamTruckManager.LotData randomObjectFromList = RandomUtil.GetRandomObjectFromList(data);
                    data.Remove(randomObjectFromList);

                    AddTruckIntoWorld(ths, randomObjectFromList.mLot);
                }
                else if ((ths.mIceCreamTrucks.Count > 0) && ((data.Count == 0) || (ths.mIceCreamTrucks.Count > Traffic.Settings.mMaxIceCreamTrucks)))
                {
                    IceCreamTruck truck = RandomUtil.GetRandomObjectFromList(ths.mIceCreamTrucks);

                    ths.mIceCreamTrucks.Remove(truck);

                    truck.FadeOut(false, true);
                }

                //Common.DebugNotify("Ice Cream Trucks " + ths.mIceCreamTrucks.Count);

                foreach (IceCreamTruck truck in ths.mIceCreamTrucks)
                {
                    if ((truck.MoveToNewLot()) || (!IsValidLot(TruckController.GetLot(truck), truck)))
                    {
                        if (data.Count == 0) continue;

                        ths.PopulateLotListDistanceAndScore(truck, data);
                        ths.RouteToNextBestSpot(truck, data);

                        for (int i = 0; i < data.Count; i++)
                        {
                            if (data[i].mLot == truck.mDestinationLot)
                            {
                                data.RemoveAt(i);
                                break;
                            }
                        }

                        Common.DebugNotify("Ice Cream Truck", truck.ObjectId, truck.mDestinationLot.ObjectId);

                        TruckController.AddTruck(truck);
                    }
                }
            }
        }

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

            if (!lot.IsCommunityLot)
            {
                if (lot.Household == null) return false;

                if (!Traffic.Settings.mAllowIceCreamResidential) return false;

                if (lot.Household.IsActive)
                {
                    if (!Traffic.Settings.mAllowIceCreamActiveLot) return false;
                }
            }

            return true;
       }

        protected static void PopulateLotList(List<IceCreamTruckManager.LotData> data)
        {
            foreach (Lot lot in LotManager.AllLots)
            {
                if (!IsValidLot(lot, null)) continue;

                IceCreamTruckManager.LotData item = new IceCreamTruckManager.LotData(lot);

                item.mDistanceReduction = 0f;

                float openHour = 0f;
                float closingHour = 0f;
                if (lot.LotType == LotType.Commercial)
                {
                    if ((Bartending.TryGetHoursOfOperation(lot, ref openHour, ref closingHour)) && (SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, openHour, closingHour)))
                    {
                        if (lot.CountObjects<ISchoolRabbitHole>() > 0x0)
                        {
                            item.mDistanceReduction += IceCreamTruckManager.kSchoolLotDistanceReduction;
                        }
                    }
                }

                if (lot.Household != null)
                {
                    item.mDistanceReduction += lot.Household.GetNumberOfKids() * IceCreamTruckManager.kKidsLotDistanceReduction;
                    if (lot.Household.IsActive)
                    {
                        item.mDistanceReduction += IceCreamTruckManager.kSelectableSimLotDistanceReduction;
                    }
                }

                switch (lot.CommercialLotSubType)
                {
                    case CommercialLotSubType.kSmallPark:
                    case CommercialLotSubType.kBigPark:
                    case CommercialLotSubType.kEP5_DogPark:
                    case CommercialLotSubType.kEP5_CatJungle:
                    case CommercialLotSubType.kEP11_RecreationPark:
                        item.mDistanceReduction += IceCreamTruckManager.kParkLotDistanceReduction;
                        break;
                }

                data.Add(item);
            }
        }
    }
}


