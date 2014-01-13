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
    public class SellFishingJunkScenario : SellObjectsScenario
    {
        public SellFishingJunkScenario()
            : base ()
        { }
        protected SellFishingJunkScenario(SellFishingJunkScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellFishingJunk";
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
                if ((obj is Sims3.Gameplay.Objects.Miscellaneous.Candle) ||
                    (obj is Sims3.Gameplay.Objects.Miscellaneous.RubberDucky) ||
                    (obj is Sims3.Gameplay.Objects.Miscellaneous.BubbleBath) ||
                    (obj is Sims3.Gameplay.Objects.CookingObjects.Cake) ||
                    (obj is Sims3.Gameplay.PetObjects.CatHuntFailureObject))
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        public override Scenario Clone()
        {
            return new SellFishingJunkScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellFishingJunkScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellFishingJunk";
            }
        }
    }
}
