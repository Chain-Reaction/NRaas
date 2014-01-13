using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
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
    public class SellVeggiesScenario : SellObjectsScenario
    {
        public SellVeggiesScenario()
        { }
        protected SellVeggiesScenario(SellVeggiesScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellVeggies";
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
            List<NectarMaker> makers = new List<NectarMaker>();
            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                makers.AddRange (lot.GetObjects<NectarMaker>());
            }

            List<GameObject> list = new List<GameObject>(), nectarItems = new List<GameObject>();
            foreach (Ingredient ingredient in Inventories.InventoryFindAll<Ingredient>(sim))
            {
                if (sim.IsVampire)
                {
                    if (ingredient is VampireFruit) continue;
                }

                if (!ingredient.CanBeSold()) continue;

                bool found = false;
                foreach (NectarMaker maker in makers)
                {
                    if (maker.CanAddToInventory(ingredient))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    nectarItems.Add(ingredient);
                }
                else
                {
                    list.Add(ingredient);
                }
            }

            while (nectarItems.Count > 50)
            {
                GameObject choice = RandomUtil.GetRandomObjectFromList(nectarItems);

                nectarItems.Remove(choice);

                list.Add(choice);
            }

            foreach (HarvestMoneyBag bag in Inventories.InventoryFindAll<HarvestMoneyBag>(sim))
            {
                list.Add(bag);
            }

            return list;
        }

        public override Scenario Clone()
        {
            return new SellVeggiesScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellVeggiesScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellInventoryVeggies";
            }
        }
    }
}
