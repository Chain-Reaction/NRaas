using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class ServicePoolCleanup : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Task();
        }

        public class Task : Common.AlarmTask
        {
            public Task()
                : base(3, DaysOfTheWeek.All)
            { }

            protected override void OnPerform()
            {
                if (Register.Settings.mDisableServiceCleanup) return;

                Perform();
            }

            public static void Perform()
            {
                Common.StringBuilder msg = new Common.StringBuilder("ServicePoolCleanup");

                try
                {
                    GrimReaper grim = null;

                    for (int i = Services.AllServices.Count - 1; i >= 0; i--)
                    {
                        Service service = Services.AllServices[i];

                        if (service == null)
                        {
                            Services.AllServices.RemoveAt(i);

                            msg += Common.NewLine + "Empty Service Removed";
                            continue;
                        }

                        msg += Common.NewLine + "Service: " + service.ServiceType;

                        if (service is GrimReaper)
                        {
                            grim = service as GrimReaper;
                        }

                        List<SimDescription> pool = new List<SimDescription>();

                        for (int j = service.mPool.Count - 1; j >= 0; j--)
                        {
                            SimDescription sim = service.mPool[j];
                            if ((sim == null) || (!sim.IsValidDescription) || (sim.Household == null))
                            {
                                service.mPool.RemoveAt(j);

                                msg += Common.NewLine + "Bogus Service Removed " + service.GetType();
                            }
                            else
                            {
                                pool.Add(sim);
                            }
                        }

                        if (pool.Count == 0) continue;

                        int maxSims = 2;

                        if (service.Tuning != null)
                        {
                            maxSims = service.Tuning.kMaxNumNPCsInPool;
                            if (maxSims < 0)
                            {
                                maxSims = 0;
                            }
                        }

                        Dictionary<ulong, bool> assigned = new Dictionary<ulong, bool>();

                        msg += Common.NewLine + "Tuning: " + maxSims;
                        msg += Common.NewLine + "Count: " + pool.Count;

                        ResortWorker workerService = service as ResortWorker;
                        List<ObjectGuid> objsToUnReg = new List<ObjectGuid>();
                        if (workerService != null)
                        {
                            if (workerService.mWorkerInfo != null)
                            {
                                foreach (KeyValuePair<ObjectGuid, ResortWorker.WorkerInfo> info in workerService.mWorkerInfo)
                                {
                                    bool objValid = false;
                                    GameObject obj = GameObject.GetObject(info.Key);
                                    if (obj != null && obj.InWorld)
                                    {
                                        Lot lotCurrent = obj.LotCurrent;
                                        if (lotCurrent != null && lotCurrent.IsCommunityLotOfType(CommercialLotSubType.kEP10_Resort))
                                        {
                                            objValid = true;
                                            assigned[info.Value.CurrentSimDescriptionID] = true;
                                            assigned[info.Value.DesiredSimDescriptionID] = true;
                                        }
                                    }

                                    if (!objValid)
                                    {
                                        objsToUnReg.Add(info.Key);                                        
                                    }
                                }

                                foreach (ObjectGuid guid in objsToUnReg)
                                {
                                    workerService.UnregisterObject(guid);
                                }
                                objsToUnReg.Clear();
                            }
                        }

                        RandomUtil.RandomizeListOfObjects(pool);

                        for (int j = pool.Count - 1; j >= 0; j--)
                        {
                            if (workerService == null)
                            {
                                if (pool.Count <= maxSims) break;
                            }

                            SimDescription choice = pool[j];

                            if (service.IsSimAssignedTask(choice)) continue;

                            if (assigned.ContainsKey(choice.SimDescriptionId)) continue;

                            ServiceCleanup.AttemptServiceDisposal(choice, false, "Too Many " + service.ServiceType);

                            pool.RemoveAt(j);
                        }

                        List<SimDescription> serviceSims = new List<SimDescription>(service.Pool);
                        foreach (SimDescription serviceSim in serviceSims)
                        {
                            if (serviceSim == null) continue;

                            if (!serviceSim.IsValidDescription)
                            {
                                service.EndService(serviceSim);
                            }
                            else if (SimTypes.IsDead(serviceSim))
                            {
                                service.EndService(serviceSim);
                            }
                        }
                    }

                    if (grim != null)
                    {
                        if (grim.mPool.Count == 0)
                        {
                            SimDescription sim = grim.CreateNewNPCForPool(null);
                            if (sim != null)
                            {
                                grim.AddSimToPool(sim);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }
                finally
                {
                    Common.DebugNotify(msg);
                }
            }
        }
    }
}
