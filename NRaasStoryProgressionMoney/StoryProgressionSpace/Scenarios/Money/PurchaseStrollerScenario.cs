using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class PurchaseStrollerScenario : PurchaseObjectScenario<Stroller>
    {
        public PurchaseStrollerScenario()
        { }
        protected PurchaseStrollerScenario(PurchaseStrollerScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PurchaseStroller";
        }

        protected override BuildBuyProduct.eBuyCategory Category
        {
            get { return BuildBuyProduct.eBuyCategory.kBuyCategoryKids; }
        }

        protected override BuildBuyProduct.eBuySubCategory SubCategory
        {
            get { return BuildBuyProduct.eBuySubCategory.kBuySubCategoryMiscellaneousKids; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (Inventories.InventoryFind<Stroller>(sim) != null)
            {
                IncStat("Stroller Found");
                return false;
            }

            bool found = false;

            foreach (SimDescription child in Relationships.GetChildren(sim))
            {
                if (!child.ToddlerOrBelow) continue;

                if (child.Household != sim.Household) continue;

                found = true;
                break;
            }

            if (!found)
            {
                IncStat("Unnecessary");
                return false;
            }

            return true;
        }

        protected override bool Test(Stroller obj)
        {
            return true;
        }

        protected override int Minimum
        {
            get { return 0; }
        }

        protected override int Maximum
        {
            get { return int.MaxValue; }
        }

        public override Scenario Clone()
        {
            return new PurchaseStrollerScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, PurchaseStrollerScenario>, ManagerMoney.IPurchaseOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseStroller";
            }
        }
    }
}
