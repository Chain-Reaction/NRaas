using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class SellDraftsScenario : SellObjectsScenario
    {
        public SellDraftsScenario()
            : base ()
        { }
        protected SellDraftsScenario(SellDraftsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellDrafts";
            }
            else
            {
                return "SellStuff";
            }
        }

        public override List<string> GetStoryPrefixes()
        {
            List<string> stories = base.GetStoryPrefixes();

            stories.Add("SellStuff");

            return stories;
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
            foreach (DraftingTable.Canvas obj in Inventories.InventoryFindAll<DraftingTable.Canvas>(sim))
            {
                list.Add(obj);
            }

            return list;
        }

        protected override RabbitHoleType GetRabbitHole()
        {
            return RabbitHoleType.ScienceLab;
        }

        public override Scenario Clone()
        {
            return new SellDraftsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellDraftsScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellDrafts";
            }
        }
    }
}
