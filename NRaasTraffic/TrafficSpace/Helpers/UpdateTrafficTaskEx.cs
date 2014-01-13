using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
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
    public class UpdateTrafficTaskEx : RepeatingTask
    {
        protected override bool OnPerform()
        {
            string msg = "OnPerform" + Common.NewLine;

            try
            {
                if ((TrafficManager.Singleton == null) || (LotManager.sLots == null))
                {
                    return true;
                }

                Simulator.DestroyObject(TrafficManager.Singleton.mUpdateTrafficTask);
                TrafficManager.Singleton.mUpdateTrafficTask = ObjectGuid.InvalidObjectGuid;

                while (!TrafficManager.Enabled)
                {
                    FoodTruckManagerEx.Update();
                    SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(RandomUtil.GetFloat(TrafficManager.kTrafficCheckTime[0x0], TrafficManager.kTrafficCheckTime[0x1]), TimeUnit.Hours));
                }

                msg += "A";

                List<Lot> list = new List<Lot>();
                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.StreetParking == null) continue;

                    if (lot.IsWorldLot) continue;

                    list.Add(lot);
                }

                if (TrafficManager.GeneratedTrafficTuning.sLotTuning == null) return true;

                msg += "B";

                PairedListDictionary<Type, int> dictionary = new PairedListDictionary<Type, int>();
                foreach (Type type in TrafficManager.GeneratedTrafficTuning.sLotTuning.Keys)
                {
                    if (!TrafficManager.Enabled)
                    {
                        break;
                    }

                    int[] carRange = TrafficManager.GeneratedTrafficTuning.GetCarRange(type);
                    if (carRange == null) continue;

                    if (Math.Max(carRange[0x0], carRange[0x1]) > 0x0)
                    {
                        msg += "C";

                        int numTaxis = RandomUtil.GetInt(carRange[0x0], carRange[0x1]);
                        foreach (Lot lot in list)
                        {
                            if (!TrafficManager.Enabled)
                            {
                                break;
                            }

                            int num2 = (int)Sims3.SimIFace.Queries.CountObjects(type, lot.LotId);
                            if (num2 > 0x0)
                            {
                                msg += "D";

                                PairedListDictionary<Type, int> dictionary2;
                                Type type2;
                                int num3 = 0x0;
                                if (TrafficManager.sGeneratedCarCount != null)
                                {
                                    TrafficManager.sGeneratedCarCount.TryGetValue(type.ToString(), out num3);
                                }

                                msg += "E";

                                if (!dictionary.ContainsKey(type))
                                {
                                    dictionary.Add(type, 0x0);
                                }

                                msg += "F";

                                (dictionary2 = dictionary)[type2 = type] = dictionary2[type2] + num2;
                                if (num3 < (numTaxis * dictionary[type]))
                                {
                                    msg += "G";

                                    CarNpcWithNoDriver car = GlobalFunctions.CreateObjectOutOfWorld("CarTaxiLightweight", ProductVersion.EP3) as CarNpcWithNoDriver;
                                    if (car != null)
                                    {
                                        Matrix44 mat = new Matrix44();
                                        if (lot.StreetParking.GetParkingSpotForCar(car, ref mat))
                                        {
                                            msg += "H";

                                            Vector3[] vectorArray;
                                            Quaternion[] quaternionArray;
                                            if (World.FindPlaceOnRoadOffScreen(car.Proxy, mat.pos.V3, FindPlaceOnRoadOption.Road, 100f, out vectorArray, out quaternionArray))
                                            {
                                                msg += "I";

                                                car.GeneratingClassName = type.ToString();
                                                car.PlaceAt(vectorArray[0x0], mat.at.V3, null);
                                                car.DriveAroundFor(RandomUtil.GetFloat(TrafficManager.kCarDriveAroundTimeRange[0x0], TrafficManager.kCarDriveAroundTimeRange[0x1]), TimeUnit.Hours);
                                            }
                                            else
                                            {
                                                car.Destroy();
                                            }

                                            msg += "J";

                                            lot.StreetParking.FreeParkingSpotForCar(car);
                                            SpeedTrap.Sleep((uint)RandomUtil.GetInt(TrafficManager.kAfterSpawnWaitTime[0x0], TrafficManager.kAfterSpawnWaitTime[0x1]));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                msg += "K";

                // Custom
                FoodTruckManagerEx.Update();

                msg += "L";

                uint tickCount = (uint)SimClock.ConvertToTicks(RandomUtil.GetFloat(TrafficManager.kTrafficCheckTime[0x0], TrafficManager.kTrafficCheckTime[0x1]), TimeUnit.Hours);
                SpeedTrap.Sleep(tickCount);
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }

            return true;
        }

        public static void ApplyTrafficDelta()
        {
            TrafficManager.GeneratedTrafficTuning.sLotTuning = null;
            TrafficManager.GeneratedTrafficTuning.Parse();

            if (TrafficManager.GeneratedTrafficTuning.sLotTuning != null)
            {
                foreach (List<TrafficManager.GeneratedTrafficTuning> tunings in TrafficManager.GeneratedTrafficTuning.sLotTuning.Values)
                {
                    foreach (TrafficManager.GeneratedTrafficTuning tuning in tunings)
                    {
                        AlterValue(ref tuning.mMinCarCount);
                        AlterValue(ref tuning.mMaxCarCount);
                    }
                }
            }
        }

        protected static void AlterValue(ref int value)
        {
            value += Traffic.Settings.mTrafficDelta;

            if (value < 0)
            {
                value = 0;
            }
        }

        public class Loader : Common.IPreLoad, Common.IDelayedWorldLoadFinished
        {
            public void OnPreLoad()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP3)) return;

                UpdateTrafficTaskEx.Create<UpdateTrafficTaskEx>();
            }

            public void OnDelayedWorldLoadFinished()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP3)) return;

                if (TrafficManager.Singleton == null)
                {
                    TrafficManager.Singleton = new TrafficManager();
                }

                ApplyTrafficDelta();
            }
        }
    }
}


