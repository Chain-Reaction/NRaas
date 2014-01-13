using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupServicePools : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupServicePools");

            Dictionary<ulong, SimDescription> allSims = SimListing.GetResidents(false);

            for(int j=Services.AllServices.Count-1; j>=0; j--)
            {
                Service service = Services.AllServices[j];
                if (service == null)
                {
                    Services.AllServices.RemoveAt(j);

                    Overwatch.Log("Empty Service Removed");
                }
                else if (service.Pool == null)
                {
                    service.mPool = new List<SimDescription>();

                    Overwatch.Log("Missing Pool Added " + service.GetType());
                }
                else
                {
                    for (int i=service.mPool.Count-1; i>=0; i--)
                    {
                        SimDescription sim = service.mPool[i];
                        if ((sim == null) || (!sim.IsValidDescription) || (sim.Household == null))
                        {
                            service.mPool.RemoveAt(i);
                        
                            Overwatch.Log("Bogus Service Removed " + service.GetType());
                        }
                        /* Not valid to do
                        else if (sim.Service != service)
                        {
                            sim.Service = service;

                            Overwatch.Log("Bogus Service Reset " + sim.FullName);
                        }
                        */
                    }
                }

                ResortWorker resortWorker = service as ResortWorker;
                if (resortWorker != null)
                {
                    if (resortWorker.mWorkerInfo != null)
                    {
                        List<ObjectGuid> remove = new List<ObjectGuid>();

                        foreach (KeyValuePair<ObjectGuid, ResortWorker.WorkerInfo> info in resortWorker.mWorkerInfo)
                        {
                            if (!allSims.ContainsKey(info.Value.CurrentSimDescriptionID))
                            {
                                remove.Add(info.Key);

                                Overwatch.Log("Bogus Worker Removed " + service.GetType());
                            }
                        }

                        foreach (ObjectGuid rem in remove)
                        {
                            resortWorker.mWorkerInfo[rem] = new ResortWorker.WorkerInfo();
                        }
                    }
                }
            }
        }
    }
}
