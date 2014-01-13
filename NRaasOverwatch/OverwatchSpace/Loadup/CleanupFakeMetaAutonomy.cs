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
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupFakeMetaAutonomy : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupFakeMetaAutonomy");

            if (FakeMetaAutonomy.mToDestroy != null)
            {
                foreach (SimDescription sim in new List<SimDescription>(FakeMetaAutonomy.mToDestroy))
                {
                    if (Annihilation.Cleanse(sim))
                    {
                        Overwatch.Log("Destroyed: " + sim.FullName);

                        FakeMetaAutonomy.mToDestroy.Remove(sim);
                    }
                }
            }

            if ((FakeMetaAutonomy.Instance != null) && (FakeMetaAutonomy.Instance.mPool != null))
            {
                int index = 0;
                while (index < FakeMetaAutonomy.Instance.mPool.Count)
                {
                    SimDescription sim = FakeMetaAutonomy.Instance.mPool[index];

                    bool keep = true;
                    if (sim == null)
                    {
                        keep = false;
                    }
                    else if (sim.Genealogy == null)
                    {
                        keep = false;
                    }

                    if (keep)
                    {
                        index++;
                    }
                    else
                    {
                        Overwatch.Log("Removed: " + sim.FullName);

                        FakeMetaAutonomy.Instance.mPool.RemoveAt(index);
                    }
                }
            }
        }
    }
}
