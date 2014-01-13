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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Opportunities;
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
    public class CleanupCompletedOpportunities : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupCompletedOpportunities");

            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);
            foreach (SimDescription sim in sims.Values)
            {
                if (sim.Household == null) continue;

                if (sim.CreatedSim == null) continue;

                if (sim.CreatedSim.OpportunityManager == null) continue;

                foreach (Opportunity opp in sim.CreatedSim.OpportunityManager.List)
                {
                    if (sim.Household.mCompletedHouseholdOpportunities.Remove((ulong)opp.Guid))
                    {
                        Overwatch.Log("Removed Opportunity: " + opp.Guid);
                    }
                }
            }
        }
    }
}
