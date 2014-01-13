using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Traffic : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static TrafficSettings sSettings;

        static Traffic()
        {
            Bootstrap();
        }

        public static TrafficSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new TrafficSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;
        }

        [Persistable]
        public class TrafficSettings
        {
            [Tunable, TunableComment("The delta change from the EA defined traffic values")]
            public static int kTrafficDelta = 0;

            [Tunable, TunableComment("Whether to run the ice cream truck manager while on vacation")]
            public static bool kEnableIceCreamTruckVacation = false;

            [Tunable, TunableComment("The number of ice cream trucks to spawn in town")]
            public static int kMaxIceCreamTrucks = IceCreamTruckManager.kMaxIceCreamTrucksToSpawn;

            [Tunable, TunableComment("Whether to allow the ice cream truck to visit the active lot")]
            public static bool kAllowIceCreamActiveLot = true;

            [Tunable, TunableComment("Whether to allow the ice cream truck to visit residential lots")]
            public static bool kAllowIceCreamResidential = true;

            [Tunable, TunableComment("Whether to allow the ice cream truck at night")]
            public static bool kAllowIceCreamAtNight = false;

            [Tunable, TunableComment("Whether to require parking spaces for food trucks")]
            public static bool kRequireFoodParkingSpace = false;

            [Tunable, TunableComment("The number of food trucks to spawn in town")]
            public static int kMaxFoodTrucks = FoodTruckManager.kMaxFoodTrucksToSpawn;

            [Tunable, TunableComment("Whether to allow the use of cars during routing")]
            public static bool kAllowRoutingVehicles = true;

            [Tunable, TunableComment("Whether to allow the use of performance career limos")]
            public static bool kAllowLimos = true;

            [Tunable, TunableComment("Whether to always use taxis rather than temporary routing vehicles")]
            public static bool kAlwaysUseTaxis = true;

            [Tunable, TunableComment("Whether to allow the food truck to visit the active lot")]
            public static bool kAllowFoodTruckActiveLot = false;

            [Tunable, TunableComment("Whether to allow the food truck to visit residential lots")]
            public static bool kAllowFoodTruckResidential = false;

            [Tunable, TunableComment("Whether to allow the food truck at night")]
            public static bool kAllowFoodTruckAtNight = false;

            public int mTrafficDelta = kTrafficDelta;

            public bool mEnableIceCreamTruckVacation = kEnableIceCreamTruckVacation;

            public int mMaxIceCreamTrucks = kMaxIceCreamTrucks;

            public bool mAllowIceCreamActiveLot = kAllowIceCreamActiveLot;

            public bool mAllowIceCreamResidential = kAllowIceCreamResidential;

            public bool mAllowIceCreamAtNight = kAllowIceCreamAtNight;

            public bool mRequireFoodParkingSpace = kRequireFoodParkingSpace;

            public int mMaxFoodTrucks = kMaxFoodTrucks;

            public bool mAllowRoutingVehicles = kAllowRoutingVehicles;

            public bool mAllowLimos = kAllowLimos;

            public bool mAlwaysUseTaxis = kAlwaysUseTaxis;

            public bool mAllowFoodTruckActiveLot = kAllowFoodTruckActiveLot;

            public bool mAllowFoodTruckResidential = kAllowFoodTruckResidential;

            public bool mAllowFoodTruckAtNight = kAllowFoodTruckAtNight;

            protected bool mDebugging = false;

            public bool Debugging
            {
                get
                {
                    return mDebugging;
                }
                set
                {
                    mDebugging = value;

                    Common.kDebugging = value;
                }
            }
        }
    }
}
