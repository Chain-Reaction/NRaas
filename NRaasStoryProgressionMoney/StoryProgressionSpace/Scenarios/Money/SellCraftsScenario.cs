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
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
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
    public class SellCraftsScenario : SellObjectsScenario
    {
        public SellCraftsScenario()
            : base ()
        { }
        protected SellCraftsScenario(SellCraftsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellCrafts";
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
            bool minerFound = false;

            List<GameObject> list = new List<GameObject>();
            foreach (ICraft obj in Inventories.InventoryDuoFindAll<ICraft, GameObject>(sim))
            {
                // Handled separately
                if ((obj is Ingredient) || (obj is Sims3.Gameplay.Objects.CookingObjects.NectarBottle))
                {
                    continue;
                }
                else if (obj is Photograph)
                {
                    continue;
                }
                else if (obj is Miner)
                {
                    // Retain one Miner object in inventory
                    if (!minerFound)
                    {
                        minerFound = true;
                        continue;
                    }
                }

                GameObject gameObject = obj as GameObject;
                if (gameObject == null) continue;

                list.Add(gameObject);
            }

            if (GetValue<FamilyInventoryOption, bool>())
            {
                if ((sim.Household != null) && (sim.Household.SharedFamilyInventory != null) && (sim.Household.SharedFamilyInventory.Inventory != null))
                {
                    foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.Household.SharedFamilyInventory.Inventory))
                    {
                        if (obj.SculptureComponent == null) continue;

                        list.Add(obj);
                    }
                }
            }

            return list;
        }

        public override Scenario Clone()
        {
            return new SellCraftsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellCraftsScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellCrafts";
            }
        }

        public class FamilyInventoryOption : BooleanScenarioOptionItem<ManagerMoney, SellCraftsScenario>, ManagerMoney.ISalesOption
        {
            public FamilyInventoryOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellFamilyInventoryCrafts";
            }
        }
    }
}
