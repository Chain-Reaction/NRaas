using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class HarvestScenario : SimScenario
    {
        public HarvestScenario()
        { }
        protected HarvestScenario(HarvestScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!GetValue<AllowHarvestOption, bool>(sim))
            {
                IncStat("Harvest Denied");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situation Denied");
                return false;
            }
            else if (!Sims.AllowInventory(this, sim, Managers.Manager.AllowCheck.None))
            {
                IncStat("Inventory Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected abstract List<HarvestPlant> GatherPlants();

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool pushed = false;

            Gardening gardeningSkill = Sim.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);

            List<HarvestPlant> plants = GatherPlants();
            if ((plants == null) || (plants.Count == 0))
            {
                IncStat("No Plants");
                return false;
            }

            int index = 0;
            while (index < plants.Count)
            {
                HarvestPlant plant = plants[index];

                if ((plant is Sims3.Gameplay.Objects.Gardening.OmniPlant) || (!Sims3.Gameplay.Objects.Gardening.HarvestPlant.HarvestTest(plant, null)))
                {
                    IncStat("Unharvestable");
                    plants.RemoveAt(index);
                    continue;
                }
                else if (!plant.CanProduceMoreHarvestables())
                {
                    if ((gardeningSkill != null) && (gardeningSkill.SkillLevel <= plant.PlantDef.GetSkillRequirement()))
                    {
                        plant.mLifetimeHarvestablesYielded = 0;

                        IncStat("Replanted");
                    }
                    else
                    {
                        IncStat("Skill Too Low");

                        index++;
                        continue;
                    }
                }

                if (Situations.PushInteraction(this, Sim, plant, HarvestEx.Singleton))
                {
                    plants.RemoveAt(index);

                    pushed = true;
                }
                else
                {
                    IncStat("Failure");
                    break;
                }
            }

            return pushed;
        }
    }
}
