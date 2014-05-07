using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
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
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class PurchaseCarScenario : PurchaseVehicleScenario<IOwnableVehicle>
    {
        public PurchaseCarScenario()
        { }
        protected PurchaseCarScenario(PurchaseCarScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "PurchaseCar";
            }
            else
            {
                AdultBicycle bike = Object as AdultBicycle;
                if (bike != null)
                {
                    return "PurchaseMotorcycle";
                }
                else
                {
                    return "PurchaseCar";
                }
            }
        }

        public override List<string> GetStoryPrefixes()
        {
            List<string> prefixes = base.GetStoryPrefixes();

            prefixes.Add("PurchaseMotorcycle");

            return prefixes;
        }

        protected override int Minimum
        {
            get
            {
                return GetValue<MinOption,int>();
            }
        }

        protected override int Funds
        {
            get
            {
                return (base.Funds - GetValue<Option,int>());
            }
        }

        protected override BuildBuyProduct.eBuySubCategory SubCategory
        {
            get { return BuildBuyProduct.eBuySubCategory.kBuySubCategoryCars | BuildBuyProduct.eBuySubCategory.kBuySubCategoryMiscellaneousVehicles; }
        }

        protected override bool Test(IOwnableVehicle obj)
        {
            Vehicle vehicle = obj as Vehicle;
            if (vehicle == null) return false;

            if (!Sim.IsWitch)
            {
                if (obj is MagicBroom)
                {
                    return false;
                }
            }

            if (obj is SirenEnabledVehicle) return false;

            if (obj is CarMotiveMobile) return false;

            if (obj is Boat) return false;            

            if (obj is CarOwnable)
            {
                return true;
            }
            else
            {
                // Biycles have the environmental buff set
                if (vehicle.AddSavingTheEnvironmentBuff) return false;
            }

            return true;
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            if (Common.IsOnTrueVacation())
            {
                IncStat("Vacation");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (AddScoring("PurchaseBicycle", sim) > 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        public override Scenario Clone()
        {
            return new PurchaseCarScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerMoney, PurchaseCarScenario>, ManagerMoney.IPurchaseOption
        {
            public Option()
                : base(25000)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseVehicles";
            }
        }

        public class MinOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IPurchaseOption
        {
            public MinOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseVehiclesMinimum";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<Option, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
