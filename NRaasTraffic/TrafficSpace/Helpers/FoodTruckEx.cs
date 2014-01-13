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
    public class FoodTruckEx
    {
        FoodTruck mTruck;

        public FoodTruckEx(FoodTruck truck)
        {
            mTruck = truck;
        }

        public static void RouteToNewLot(FoodTruck ths, Lot lot)
        {
            ths.mDestinationLot = lot;
            new FoodTruckEx(ths).RouteTruckToLot();
        }

        protected void RouteTruckToLot()
        {
            if (mTruck.mRouteToLot != null)
            {
                mTruck.mRouteToLot.Destroy();
            }

            mTruck.mRouteToLot = new OneShotFunction(new Sims3.Gameplay.Function(OneShotRouteToLot));
            Simulator.AddObject(mTruck.mRouteToLot);
        }

        protected void OneShotRouteToLot()
        {
            mTruck.RoutingComponent.ClearQueuedActions();

            bool flag = false;
            if (mTruck.mDestinationLot == null)
            {
                flag = true;
                List<Lot> randomList = new List<Lot>();
                Lot lotHome = null;
                Household activeHousehold = Household.ActiveHousehold;
                if (activeHousehold != null)
                {
                    lotHome = activeHousehold.LotHome;
                }

                Lot activeLot = LotManager.ActiveLot;
                foreach (Lot lot3 in LotManager.AllLots)
                {
                    if (((lot3.LotId != ulong.MaxValue) && (lot3 != activeLot)) && (lot3 != lotHome))
                    {
                        randomList.Add(lot3);
                    }
                }

                if (randomList.Count > 0x0)
                {
                    mTruck.mDestinationLot = RandomUtil.GetRandomObjectFromList<Lot>(randomList);
                }

                if (mTruck.mDestinationLot == null)
                {
                    mTruck.FadeOut(false, true);
                    mTruck.mRouteToLot = null;
                    return;
                }
            }

            ParkingSpace parent = mTruck.Parent as ParkingSpace;
            if (parent != null)
            {
                Matrix44 matrix = new Matrix44();
                mTruck.LotCurrent.StreetParking.GetParkingSpotForCar(mTruck, ref matrix);
                mTruck.PlaceAt(matrix.pos.V3, matrix.at.V3, null);
                mTruck.TruckOnRoad();
                parent.LotCurrent.StreetParking.FreeParkingSpotForCar(mTruck);
            }

            Matrix44 mat = new Matrix44();
            mTruck.mDestinationLot.StreetParking.GetParkingSpotForCar(mTruck, ref mat);
            Vector3 position = mTruck.Position;
            Route r = mTruck.CarRoutingComponent.CreateRoute();
            r.PlanToMatrixFromPoint(ref position, ref mat);
            if (!mTruck.DoRoute(r))
            {
                if (parent != null)
                {
                    parent.ParkVehicle(mTruck);
                }
                else
                {
                    flag = true;
                }
            }
            else if (parent != null)
            {
                parent.UnReserveSpot(mTruck);
            }

            mTruck.mDestinationLot.StreetParking.FreeParkingSpotForCar(mTruck);
            if (!flag)
            {
                ParkingSpace closestUnreservedParkingSpace = Vehicle.GetClosestUnreservedParkingSpace(mTruck.mDestinationLot.GetObjects<WideParkingSpace>(), mTruck);
                if (closestUnreservedParkingSpace != null)
                {
                    closestUnreservedParkingSpace.ParkVehicle(mTruck);
                    mTruck.TruckParked();
                }
                else if (Traffic.Settings.mRequireFoodParkingSpace)
                {
                    mTruck.FadeOut(false, true);
                }
                else
                {
                    mTruck.TruckParked();
                    mTruck.SetGeometryState("openWithLights");
                }
            }
            else if (Traffic.Settings.mRequireFoodParkingSpace)
            {
                mTruck.FadeOut(false, true);
            }
            else
            {
                mTruck.TruckParked();
                mTruck.SetGeometryState("openWithLights");                
            }

            AddMapTags(mTruck);

            mTruck.mDestinationLot = null;
            mTruck.mRouteToLot = null;
        }

        public static bool MoveToNewLot(FoodTruck ths)
        {
            /*
            if ((ths.RoutingComponent != null) && ths.RoutingComponent.IsRouting)
            {
                return true;
            }
            */
            if (ths.ReferenceList.Count > 0x0)
            {
                return false;
            }

            return (SimClock.ElapsedTime(TimeUnit.Hours, ths.mTimeAtCurrentLot) >= ths.FoodTruckTuning.HoursToStayOnLot);
        }

        public static void AddMapTags(FoodTruck truck)
        {
            if (Household.ActiveHousehold != null)
            {
                foreach (Sim sim in Household.ActiveHousehold.Sims)
                {
                    if (sim.IsHuman && sim.SimDescription.ChildOrAbove)
                    {
                        MapTagManager mapTagManager = sim.MapTagManager;
                        if (mapTagManager != null)
                        {
                            if (mapTagManager.HasTag(truck))
                            {
                                mapTagManager.RefreshTag(truck);
                            }
                            else
                            {
                                mapTagManager.AddTag(new FoodTruckMapTag(truck, sim));
                            }
                        }
                    }
                }
            }
        }

        private class FoodTruckMapTag : CarMapTag
        {
            protected FoodTruckMapTag()
            { }
            public FoodTruckMapTag(FoodTruck vehicle, Sim owner)
                : base(vehicle, owner)
            { }

            public override MapTagFilterType FilterType
            {
                get
                {
                    MapTagFilterType filterType = base.FilterType;
                    if (!CameraController.IsMapViewModeEnabled())
                    {
                        return filterType;
                    }
                    else if (ShouldShowActiveSimPlumbob)
                    {
                        return (filterType | ~MapTagFilterType.None);
                    }
                    else if (ContainsNonActiveHouseholdSims)
                    {
                        return (filterType | MapTagFilterType.HouseholdAndWork);
                    }
                    else if (PlumbBob.SelectedActor.SimDescription.TeenOrAbove)
                    {
                        return (filterType | ~MapTagFilterType.None);
                    }
                    return (filterType | MapTagFilterType.AllOnly);
                }
            }
        }
    }
}


