using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
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
    public class CleanupHomework : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupHomework");

            int count = 0;

            foreach (Sim sim in LotManager.Actors)
            {
                if (sim == null) continue;

                if (sim.Inventory == null) continue;

                List<Homework> objects = Inventories.QuickFind<Homework>(sim.Inventory);

                Homework actual = null;
                
                if (sim.School != null)
                {
                    actual = sim.School.OwnersHomework;
                }

                objects.Remove(actual);

                foreach (Homework obj in objects)
                {
                    try
                    {
                        obj.Destroy();

                        count++;
                    }
                    catch
                    { }
                }
            }

            Overwatch.Log("  Deleted: " + count);
        }
    }
}
