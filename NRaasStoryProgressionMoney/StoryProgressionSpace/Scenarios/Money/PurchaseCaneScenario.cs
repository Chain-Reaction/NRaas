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
    public class PurchaseCaneScenario : PurchaseObjectScenario<Cane>
    {
        public PurchaseCaneScenario()
        { }
        protected PurchaseCaneScenario(PurchaseCaneScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PurchaseCane";
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
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

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (sim.Elder)
            {
                if (!Cane.IsAllowedToUseCane(sim.CreatedSim))
                {
                    IncStat("Not Allowed");
                    return false;
                }
                else if (Inventories.InventoryFind<Cane>(sim) != null)
                {
                    IncStat("Has Cane");
                    return false;
                }
            }
            else
            {
                if (Inventories.InventoryFind<Cane>(sim) == null)
                {
                    IncStat("No Cane");
                    return false;
                }
            }

            return true;
        }

        protected override bool Test(Cane obj)
        {
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!Sim.Elder)
            {
                foreach (Cane cane in Inventories.InventoryFindAll<Cane>(Sim))
                {
                    Money.Sell(Sim, cane);
                }

                IncStat("Sold Canes");
                return false;
            }

            if (base.PrivateUpdate(frame))
            {
                if (RandomUtil.RandomChance(Option.kNPCUseCaneChance))
                {
                    Cane cane = Inventories.InventoryFind<Cane>(Sim);
                    if (cane != null)
                    {
                        cane.UsingCane = true;
                        if (RandomUtil.RandomChance01(Cane.kNPCSouthernGentlemanWalkStyleChance))
                        {
                            cane.CurrentCaneWalkStyle = Cane.kSouthernGentlemanCaneWalk;
                        }
                    }
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new PurchaseCaneScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, PurchaseCaneScenario>, ManagerMoney.IPurchaseOption
        {
            public static float kNPCUseCaneChance = -1;

            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseCane";
            }

            public override bool Install(ManagerMoney manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                if (initial)
                {
                    if (kNPCUseCaneChance < 0)
                    {
                        kNPCUseCaneChance = Cane.kNPCUseCaneChance;
                    }

                    Cane.kNPCUseCaneChance = 0;
                }

                return true;
            }
        }
    }
}
