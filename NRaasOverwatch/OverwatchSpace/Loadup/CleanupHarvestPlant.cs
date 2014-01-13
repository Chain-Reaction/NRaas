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
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Roles;
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
    public class CleanupHarvestPlant : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupHarvestPlant");

            foreach (HarvestPlant plant in Sims3.Gameplay.Queries.GetObjects<HarvestPlant>())
            {
                switch (plant.GrowthState)
                {
                    case PlantGrowthState.Mycelium:
                        if (Plant.IsMushroom(plant.Seed)) continue;

                        plant.SetGrowthState(PlantGrowthState.Sprout);

                        Overwatch.Log(" Growth Plant Stage Corrected A");
                        break;
                    case PlantGrowthState.Harvest:
                        if (HarvestPlant.FindSlottedHarvestable(plant) == null)
                        {
                            OmniPlant omniPlant = plant as OmniPlant;
                            if (omniPlant != null)
                            {
                                if (omniPlant.FedObject == null) continue;
                            }

                            plant.PostHarvest();

                            Overwatch.Log(" Growth Plant Stage Corrected B");
                        }
                        break;
                }
            }
        }
    }
}
