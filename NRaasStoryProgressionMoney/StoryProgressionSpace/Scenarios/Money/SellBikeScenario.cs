using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    // This scenario was created to sell bikes of families that contain toddlers, due to a routing error
    public class SellBikeScenario : SellObjectsScenario
    {
        public SellBikeScenario()
            : base ()
        { }
        protected SellBikeScenario(SellBikeScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellBike";
            }
            else
            {
                return null;
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.Household.AllSimDescriptions.Find((e) => { return e.ToddlerOrBelow; }) == null)
            {
                IncStat("No Toddler");
                return false;
            }

            return true;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (AdultBicycle bike in Inventories.InventoryDuoFindAll<AdultBicycle, Vehicle>(sim))
            {
                list.Add(bike);
            }

            return list;
        }

        public override Scenario Clone()
        {
            return new SellBikeScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellFishScenario>, IDebuggingOption, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellInventoryBike";
            }
        }
    }
}
