using NRaas.DreamerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public static class DreamerTuning
    {
        [Tunable]
        public static bool kNoTimeOut = false;
    }

    public class Dreamer : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static Dreamer()
        {
            Bootstrap();
        }

        public void OnWorldLoadFinished()
        {
            new AlarmTask(30f, TimeUnit.Minutes, OnTimer, 30f, TimeUnit.Minutes);

            foreach (EventTypeId id in Enum.GetValues(typeof(EventTypeId)))
            {
                EventTracker.AddListener(id, OnEvent); // Must be immediate
            }
        }

        protected static ListenerAction OnEvent(Event paramE)
        {
            try
            {
                if (!EventTracker.sCurrentlyUpdatingDreamsAndPromisesManagers)
                {
                    try
                    {
                        EventTracker.sCurrentlyUpdatingDreamsAndPromisesManagers = true;
                        while (DreamsAndPromisesManager.sNeedToUpdateList.Count > 0)
                        {
                            DreamsAndPromisesManager dnp = DreamsAndPromisesManager.sNeedToUpdateList[0];
                            try
                            {
                                DreamsAndPromisesManagerEx.Update(dnp);
                            }
                            catch (Exception e)
                            {
                                Common.Exception(dnp.Actor, e);
                            }

                            DreamsAndPromisesManager.sNeedToUpdateList.Remove(dnp);
                        }
                    }
                    finally
                    {
                        EventTracker.sCurrentlyUpdatingDreamsAndPromisesManagers = false;
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(paramE.Actor, paramE.TargetObject, e);
            }

            return ListenerAction.Keep;
        }

        protected static void OnTimer()
        {
            try
            {
                foreach (Sim sim in LotManager.Actors)
                {
                    try
                    {
                        DreamsAndPromisesManager dnp = sim.DreamsAndPromisesManager;
                        if (dnp == null) continue;

                        if ((!sim.IsSelectable) && (dnp.NeedsUpdate))
                        {
                            dnp.SetToUpdate(true, false);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTimer", e);
            }
        }
    }
}
