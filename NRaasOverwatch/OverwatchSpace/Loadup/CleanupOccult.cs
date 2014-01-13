using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupOccult : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupOccult");

            foreach (SimDescription sim in Household.AllSimsLivingInWorld())
            {
                OccultTypeHelper.ValidateOccult(sim, Overwatch.Log);

                OccultTypeHelper.TestAndRebuildWerewolfOutfit(sim);

                // Compensate for a missing line in OccultWerewolf:FixupOccult
                if ((sim.CreatedSim != null) && (sim.ToddlerOrAbove) && (OccultTypeHelper.HasType(sim, OccultTypes.Werewolf)))
                {
                    World.ObjectSetVisualOverride(sim.CreatedSim.ObjectId, eVisualOverrideTypes.Werewolf, null);
                }
            }
        }
    }
}
