using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupGenericPerformanceObjects : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupGenericPerformanceObjects");

            foreach (GenericPerformanceObject obj in Sims3.Gameplay.Queries.GetObjects<GenericPerformanceObject>())
            {
                if (obj.SimOwner != null) continue;

                Inventory inventory = Inventories.ParentInventory(obj);
                if (inventory == null) continue;

                Sim sim = inventory.Owner as Sim;
                if (sim == null) continue;

                obj.SimOwner = sim.SimDescription;

                Overwatch.Log("  Sim reattached : " + obj.GetLocalizedName());
            }
        }
    }
}
