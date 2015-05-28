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
    public class PurchaseUmbrellaScenario : PurchaseObjectScenario<Umbrella>
    {
        public PurchaseUmbrellaScenario()
        { }
        protected PurchaseUmbrellaScenario(PurchaseUmbrellaScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PurchaseUmbrella";
        }

        protected override BuildBuyProduct.eBuyCategory Category
        {
            get { return BuildBuyProduct.eBuyCategory.kBuyCategoryEntertainment; }
        }

        protected override BuildBuyProduct.eBuySubCategory SubCategory
        {
            get { return BuildBuyProduct.eBuySubCategory.kBuySubCategoryMiscellaneousEntertainment; }
        }

        protected override int Minimum
        {
            get { return 0; }
        }

        protected override int Maximum
        {
            get { return int.MaxValue; }
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

        protected override bool Test(Umbrella obj)
        {
            if (obj is Parasol && Umbrella.kChanceGetParasolInsteadOfUmbrella == 0)
            {
                return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int count = 0;
            foreach (Umbrella umbrella in Inventories.InventoryFindAll<Umbrella>(Sim))
            {
                if (umbrella.Repairable != null)
                {
                    if (umbrella.Repairable.Broken)
                    {
                        Money.Sell(Sim, umbrella);
                        continue;
                    }
                }

                count++;
            }

            if (count > 0) return false;

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new PurchaseUmbrellaScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, PurchaseUmbrellaScenario>, ManagerMoney.IPurchaseOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseUmbrella";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP8);
            }

            public override bool Install(ManagerMoney manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                if (initial)
                {
                    Umbrella.kChanceNpcInventorySeedWithUmbrella = 0;
                }

                return true;
            }
        }
    }
}
