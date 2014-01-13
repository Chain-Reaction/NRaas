using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class RoutingHelper : Common.IPreLoad, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        static float sDistanceToDestinationSoSimWillDrive;
        static float sDistanceToDestinationSoSimWillDriveInDowntownWorld;
        static float sDistanceToDestinationSoSimWillDriveInChina;
        static float sDistanceToDestinationSoSimWillDriveInEgypt;
        static float sDistanceToDestinationSoSimWillDriveInFrance;

        static int sPerformanceArtistCareerLevelForALimoRide;
        static int sFortuneTellerCareerLevelForALimoRide;

        static float sCarNpcTaxiChance;

        public void OnPreLoad()
        {
            sDistanceToDestinationSoSimWillDrive = Car.kDistanceToDestinationSoSimWillDrive;
            sDistanceToDestinationSoSimWillDriveInDowntownWorld = Car.kDistanceToDestinationSoSimWillDriveInDowntownWorld;
            sDistanceToDestinationSoSimWillDriveInChina = Car.kDistanceToDestinationSoSimWillDriveInChina;
            sDistanceToDestinationSoSimWillDriveInEgypt = Car.kDistanceToDestinationSoSimWillDriveInEgypt;
            sDistanceToDestinationSoSimWillDriveInFrance = Car.kDistanceToDestinationSoSimWillDriveInFrance;

            sPerformanceArtistCareerLevelForALimoRide = PerformanceArtistCareer.kLevelForALimoRide;
            sFortuneTellerCareerLevelForALimoRide = FortuneTellerCareer.kLevelNeededForLimoRide;

            sCarNpcTaxiChance = CarNpc.TaxiChance;
        }

        public void OnWorldLoadFinished()
        {
            AdjustMinDistance(Traffic.Settings.mAllowRoutingVehicles);
            AdjustLimos(Traffic.Settings.mAllowLimos);
            AdjustUseTaxis(Traffic.Settings.mAlwaysUseTaxis);
        }

        public void OnWorldQuit()
        {
            AdjustMinDistance(true);
            AdjustLimos(true);
            AdjustUseTaxis(true);
        }

        public static void AdjustUseTaxis(bool value)
        {
            if (value)
            {
                CarNpc.kTaxiChance = 2;
            }
            else
            {
                CarNpc.kTaxiChance = sCarNpcTaxiChance;
            }
        }

        public static void AdjustLimos(bool value)
        {
            if (value)
            {
                PerformanceArtistCareer.kLevelForALimoRide = sPerformanceArtistCareerLevelForALimoRide;
                FortuneTellerCareer.kLevelNeededForLimoRide = sFortuneTellerCareerLevelForALimoRide;
            }
            else
            {
                PerformanceArtistCareer.kLevelForALimoRide = 11;
                FortuneTellerCareer.kLevelNeededForLimoRide = 11;
            }
        }

        public static void AdjustMinDistance(bool value)
        {
            if (value)
            {
                Car.kDistanceToDestinationSoSimWillDrive = sDistanceToDestinationSoSimWillDrive;
                Car.kDistanceToDestinationSoSimWillDriveInDowntownWorld = sDistanceToDestinationSoSimWillDriveInDowntownWorld;
                Car.kDistanceToDestinationSoSimWillDriveInChina = sDistanceToDestinationSoSimWillDriveInChina;
                Car.kDistanceToDestinationSoSimWillDriveInEgypt = sDistanceToDestinationSoSimWillDriveInEgypt;
                Car.kDistanceToDestinationSoSimWillDriveInFrance = sDistanceToDestinationSoSimWillDriveInFrance;
            }
            else
            {
                Car.kDistanceToDestinationSoSimWillDrive = float.MaxValue;
                Car.kDistanceToDestinationSoSimWillDriveInDowntownWorld = float.MaxValue;
                Car.kDistanceToDestinationSoSimWillDriveInChina = float.MaxValue;
                Car.kDistanceToDestinationSoSimWillDriveInEgypt = float.MaxValue;
                Car.kDistanceToDestinationSoSimWillDriveInFrance = float.MaxValue;
            }

            Route.SetMinimumDistanceForCarTravel(Car.GetWorldDrivingMinimumDistance());
        }
    }
}


