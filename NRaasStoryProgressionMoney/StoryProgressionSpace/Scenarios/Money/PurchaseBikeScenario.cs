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
    public class PurchaseBikeScenario : PurchaseVehicleScenario<AdultBicycle>
    {
        public PurchaseBikeScenario()
        { }
        protected PurchaseBikeScenario(PurchaseBikeScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PurchaseBicycle";
        }

        protected override int Funds
        {
            get
            {
                return (base.Funds - GetValue<Option, int>());
            }
        }

        protected override BuildBuyProduct.eBuySubCategory SubCategory
        {
            get { return BuildBuyProduct.eBuySubCategory.kBuySubCategoryBicycles; }
        }

        protected override bool Test(AdultBicycle obj)
        {
            return obj.AddSavingTheEnvironmentBuff;
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (Inventories.InventoryFind<CarOwnable>(sim) != null)
            {
                IncStat("Car Found");
                return false;
            }
            else if (AddScoring("PurchaseBicycle", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }
            else if (sim.Household != null)
            {
                foreach (SimDescription member in HouseholdsEx.All(sim.Household))
                {
                    if (member.ToddlerOrBelow)
                    {
                        IncStat("Toddler Fail");
                        return false;
                    }
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new PurchaseBikeScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerMoney, PurchaseBikeScenario>, ManagerMoney.IPurchaseOption
        {
            public Option()
                : base(2500)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseBikes";
            }
        }
    }
}
