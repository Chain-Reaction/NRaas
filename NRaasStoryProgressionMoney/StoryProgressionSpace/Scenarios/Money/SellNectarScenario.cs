using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class SellNectarScenario : SellObjectsScenario
    {
        public SellNectarScenario()
        { }
        protected SellNectarScenario(SellNectarScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellNectar";
            }
            else
            {
                return "SellStuff";
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (NectarBottle Nectar in Inventories.InventoryFindAll<NectarBottle>(sim))
            {
                if (!Nectar.CanBeSold()) continue;

                list.Add(Nectar);
            }

            return list;
        }

        public override Scenario Clone()
        {
            return new SellNectarScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellNectarScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellInventoryNectar";
            }
        }
    }
}
