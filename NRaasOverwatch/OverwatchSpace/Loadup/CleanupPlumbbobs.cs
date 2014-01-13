using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
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
    public class CleanupPlumbbobs : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupPlumbbobs");

            List<PlumbBob> list = new List<PlumbBob>();

            foreach (PlumbBob plumbbob in Sims3.Gameplay.Queries.GetObjects<PlumbBob>())
            {
                if (plumbbob.Charred)
                {
                    Overwatch.Log(" Uncharred");
                    plumbbob.Charred = false;
                }

                list.Add(plumbbob);
            }

            if (list.Count > 1)
            {
                RestartTask.Perform(PlumbBob.SelectedActor, list.Count);

                PlumbBob.Shutdown();

                foreach (PlumbBob plumbbob in list)
                {
                    plumbbob.Dispose();

                    // PlumbBob:Destroy() does nothing
                    //plumbbob.Destroy();

                    // GameObject:Destroy()
                    World.RemoveObjectFromObjectManager(plumbbob.ObjectId);
                    Simulator.DestroyObject(plumbbob.ObjectId);
                }
            }
        }

        public class RestartTask : RepeatingTask
        {
            Sim mSelectedActor;

            DreamCatcher.HouseholdStore mStore = null;

            int mCount;

            protected RestartTask(Sim selectedActor, int count)
            {
                mSelectedActor = selectedActor;
                mCount = count;

                if (mSelectedActor != null)
                {
                    mStore = new DreamCatcher.HouseholdStore(mSelectedActor.Household, true);
                }
            }

            public static void Perform(Sim selectedActor, int count)
            {
                new RestartTask(selectedActor, count).AddToSimulator();
            }

            protected override int Delay
            {
                get
                {
                    return 1000;
                }
            }

            protected override bool OnPerform()
            {
                if (PlumbBob.Singleton == null)
                {
                    PlumbBob.Startup();

                    if (mSelectedActor != null)
                    {
                        DreamCatcher.Select(mSelectedActor, true, true);
                    }

                    mStore.Dispose();

                    Overwatch.Log(" Plumbbobs Deleted: " + mCount);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
