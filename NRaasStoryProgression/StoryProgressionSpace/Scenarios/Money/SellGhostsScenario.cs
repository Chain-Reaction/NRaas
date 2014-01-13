using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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
    public class SellGhostsScenario : SellObjectsScenario
    {
        public SellGhostsScenario()
            : base ()
        { }
        protected SellGhostsScenario(SellGhostsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellGhosts";
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
            foreach (GameObject obj in Inventories.InventoryFindAll<GameObject>(sim))
            {
                if (obj is Sims3.Gameplay.Objects.Spawners.Spirit.SpiritJar)
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        protected override RabbitHoleType GetRabbitHole()
        {
            return RabbitHoleType.ScienceLab;
        }

        public override Scenario Clone()
        {
            return new SellGhostsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellGhostsScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellGhosts";
            }
        }
    }
}
