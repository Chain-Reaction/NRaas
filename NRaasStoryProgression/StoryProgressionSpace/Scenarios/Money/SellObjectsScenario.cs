using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class SellObjectsScenario : InventorySimScenario
    {
        public SellObjectsScenario()
            : base ()
        { }
        protected SellObjectsScenario(SellObjectsScenario scenario)
            : base (scenario)
        { }

        protected override int ContinueChance
        {
            get { return 50; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int ContinueReportChance
        {
            get { return 25; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool success = false;
            foreach (GameObject obj in GetInventory(Sim))
            {
                int value = Money.Sell(Sim, obj);

                mFunds += value;

                AddStat("Sold", value);
                success = true;
            }

            return success;
        }
    }
}
