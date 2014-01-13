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
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupSeasonalLotMarkers : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupSeasonalLotMarkers");

            foreach (SeasonalLotMarker marker in Sims3.Gameplay.Queries.GetObjects<SeasonalLotMarker>())
            {
                if (marker.mSeasonalItems == null) continue;

                foreach (KeyValuePair<SeasonalLotMarker.LotSeason, List<ObjectGuid>> season in marker.mSeasonalItems)
                {
                    if (season.Value == null) continue;

                    for (int i = season.Value.Count - 1; i >= 0; i--)
                    {
                        Sim sim = GameObject.GetObject<GameObject>(season.Value[i]) as Sim;
                        if (sim != null)
                        {
                            Overwatch.Log(" Sim Detached: " + sim.FullName);

                            season.Value.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
