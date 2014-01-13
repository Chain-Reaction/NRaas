using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class SimEx
    {
        public static bool IsPointInLotSafelyRoutable(Sim ths, Lot lot, Vector3 pos)
        {
            if ((lot == null) || (lot.Corners == null))
            {
                return false;
            }

            if (pos.IsSimilarTo(Vector3.Zero) || pos.IsSimilarTo(Vector3.OutOfWorld))
            {
                return false;
            }

            LotLocation invalid = LotLocation.Invalid;
            ulong lotLocation = World.GetLotLocation(pos, ref invalid);
            if (!lot.IsWorldLot && (lotLocation != lot.LotId))
            {
                return false;
            }

            TerrainType terrainType = World.GetTerrainType(pos);
            switch (terrainType)
            {
                case TerrainType.WorldSea:
                case TerrainType.WorldPond:
                case TerrainType.LotPool:
                case TerrainType.LotPond:
                    return false;
            }

            if (!lot.IsWorldLot)
            {
                // Custom
                Route route = SimRoutingComponentEx.CreateRouteAsAdult(ths.SimRoutingComponent);
                route.SetOption(Route.RouteOption.EnableWaterPlanning, ths.IsHuman);
                float num2 = 1f;
                if (lot.IsHouseboatLot())
                {
                    num2++;
                }

                for (int i = 0x0; i < 0x4; i++)
                {
                    RadialRangeDestination destination = new RadialRangeDestination();
                    destination.mCenterPoint = lot.Corners[i];
                    destination.mfMinRadius = 0f;
                    destination.mfMaxRadius = num2;
                    route.AddDestination(destination);
                    destination = new RadialRangeDestination();
                    int num4 = (i + 0x1) % 0x4;
                    destination.mCenterPoint = (Vector3)((lot.Corners[i] + lot.Corners[num4]) * 0.5f);
                    destination.mfMinRadius = 0f;
                    destination.mfMaxRadius = num2;
                    route.AddDestination(destination);
                }

                route.PlanFromPoint(pos);
                if (route.PlanResult.mType != RoutePlanResultType.Succeeded)
                {
                    return false;
                }
            }
            return true;
        }

        public static Vehicle GetOwnedAndUsableVehicle(Sim ths, Lot lot)
        {
            return GetOwnedAndUsableVehicle(ths, lot, false, true, false, true);
        }
        public static Vehicle GetOwnedAndUsableVehicle(Sim ths, Lot lot, bool bAddPending, bool bAllowBikes, bool bIgnoreCanDrive, bool allowUFO)
        {
            if (ths.IsPet)
            {
                return null;
            }

            Vehicle vehicleForCurrentInteraction = ths.GetVehicleForCurrentInteraction();
            if (vehicleForCurrentInteraction != null)
            {
                return vehicleForCurrentInteraction;
            }

            /*
            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                ActiveFireFighter occupation = ths.Occupation as ActiveFireFighter;
                if (occupation != null)
                {
                    Car vehicle = occupation.GetVehicle(bAddPending);
                    if (vehicle != null)
                    {
                        return vehicle;
                    }
                }
            }
            */

            Vehicle reservedVehicle = ths.GetReservedVehicle();
            if ((reservedVehicle != null) && (bAllowBikes || (!(reservedVehicle is Bicycle) && !(reservedVehicle is MagicBroom))))
            {
                IOwnableVehicle vehicle3 = reservedVehicle as IOwnableVehicle;
                if (vehicle3 == null)
                {
                    return reservedVehicle;
                }

                if (ths.CanUseVehicleRightNow(vehicle3, true, true))
                {
                    if (bAddPending)
                    {
                        vehicle3.PendingUse = true;
                    }
                    return reservedVehicle;
                }
            }

            /*
            if (ths.OccupationAsPerformanceCareer != null)
            {
                Car car2 = ths.OccupationAsPerformanceCareer.GetCareerCar();
                if (car2 != null)
                {
                    return car2;
                }
            }
            else if (ths.CareerManager.OccupationAsCareer != null)
            {
                FortuneTellerCareer career = ths.CareerManager.Occupation as FortuneTellerCareer;
                if (((career != null) && (career.CurLevelBranchName == "ScamArtist")) && (career.Level >= FortuneTellerCareer.kLevelNeededForLimoRide))
                {
                    Car car3 = GlobalFunctions.CreateObjectOutOfWorld("CarLimo") as Car;
                    car3.DestroyOnRelease = true;
                    return car3;
                }
            }
            */

            IOwnableVehicle preferredVehicle = ths.GetPreferredVehicle();
            if (ths.CanUseVehicleRightNow(preferredVehicle, true, bIgnoreCanDrive) && (bAllowBikes || (!(reservedVehicle is Bicycle) && !(reservedVehicle is MagicBroom))))
            {
                if (bAddPending)
                {
                    preferredVehicle.PendingUse = true;
                }
                return (preferredVehicle as Vehicle);
            }

            if (bAllowBikes && ths.HasTrait(TraitNames.Rebellious))
            {
                IOwnableVehicle motorcycleIfAvailable = ths.GetMotorcycleIfAvailable(bAddPending, bIgnoreCanDrive);
                if (motorcycleIfAvailable != null)
                {
                    return (motorcycleIfAvailable as Vehicle);
                }
            }

            if (bAllowBikes && ths.HasTrait(TraitNames.EnvironmentallyConscious))
            {
                IOwnableVehicle bikeIfAvailable = ths.GetBikeIfAvailable(bAddPending, bIgnoreCanDrive);
                if (bikeIfAvailable != null)
                {
                    return (bikeIfAvailable as Vehicle);
                }
            }

            // Custom
            List<IOwnableVehicle> vehicles = Inventories.QuickDuoFind<IOwnableVehicle, Vehicle>(ths.Inventory);
            IOwnableVehicle vehicle = ths.GetBroomOrMostExpensiveUsableVehicle(vehicles, bAllowBikes, bAddPending, bIgnoreCanDrive, allowUFO, false);
            if (vehicle != null)
            {
                return (vehicle as Vehicle);
            }

            vehicles.Clear();
            vehicle = ths.GetMostExpensiveUsableParkedVehicle(lot, bAllowBikes, bAddPending, bIgnoreCanDrive, allowUFO);
            if (vehicle != null)
            {
                return (vehicle as Vehicle);
            }

            if (lot != ths.LotHome)
            {
                vehicle = ths.GetMostExpensiveUsableParkedVehicle(ths.LotHome, bAllowBikes, bAddPending, bIgnoreCanDrive, allowUFO);
                if (vehicle != null)
                {
                    return (vehicle as Vehicle);
                }
            }
            /*
            Car careerCar = ths.GetCareerCar();
            if (careerCar != null)
            {
                IOwnableVehicle vehicle8 = careerCar as IOwnableVehicle;
                if (bAddPending && (vehicle8 != null))
                {
                    vehicle8.PendingUse = true;
                }
                return careerCar;
            }
            */
            return null;
        }
    }
}
