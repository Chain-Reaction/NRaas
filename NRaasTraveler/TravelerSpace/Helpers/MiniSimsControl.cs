using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    /*
    public class MiniSimsControl : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSimInstantiated, OnCreated);
            new Common.DelayedEventListener(EventTypeId.kEventSimSelected, OnSelected);

            Common.FunctionTask.Perform(OnCleanup);
        }

        public static void OnCreated(Event e)
        {
            Sim sim = e.Actor as Sim;

            if (sim != null)
            {
                SimDescription desc = sim.SimDescription;
                if (desc != null)
                {
                    if (!GameUtils.IsWorldInstalled(desc.mHomeWorld))
                    {
                        desc.mHomeWorld = WorldName.UserCreated;

                        // Don't use Find() here, it check World
                        MiniSimDescription miniSim;
                        if (MiniSimDescription.sMiniSims.TryGetValue(desc.SimDescriptionId, out miniSim))
                        {
                            miniSim.mHomeWorld = WorldName.UserCreated;
                        }
                    }
                }
            }
        }

        public static void OnSelected(Event e)
        {
            UpdateRelations.Perform(e.Actor as Sim);
        }

        public class UpdateRelations : Common.FunctionTask
        {
            Sim mSim;

            protected UpdateRelations(Sim sim)
            {
                mSim = sim;
            }

            public static void Perform(Sim sim)
            {
                new UpdateRelations(sim).AddToSimulator();
            }

            protected override void OnPerform()
            {
                MiniSims.UpdateRelationsThumbnails(mSim);
            }
        }

        protected static void Update(MiniSimDescription miniSim)
        {
            if (miniSim == null) return;

            while ((AgingManager.Singleton == null) || (AlarmManager.Global == null))
            {
                SpeedTrap.Sleep();
            }

            if (MiniSims.UpdateThumbnailKey(miniSim) == MiniSims.Results.Success)
            {
                //Common.DebugNotify("Updated: " + miniSim.FullName);

                SpeedTrap.Sleep();
            }
        }

        public static void OnCleanup()
        {
            Dictionary<ulong,MiniSimDescription> original = MiniSimDescription.sMiniSims;

            if (MiniSimDescription.sMiniSims != null)
            {
                // Corrects for an error in MiniSimDescription:Find(), where the system is unable to retrieve custom WorldName sims
                foreach (MiniSimDescription miniSim in MiniSimDescription.sMiniSims.Values)
                {
                    if (!GameUtils.IsWorldInstalled(miniSim.mHomeWorld))
                    {
                        miniSim.mHomeWorld = WorldName.UserCreated;
                    }
                }

                UpdateRelations.Perform(Sim.ActiveActor);

                foreach (MiniSimDescription miniSim in new List<MiniSimDescription>(MiniSimDescription.sMiniSims.Values))
                {
                    Update(miniSim);

                    // World switched, end loop
                    if (!object.ReferenceEquals(original, MiniSimDescription.sMiniSims))
                    {
                        break;
                    }
                }
            }
        }
    }*/
}