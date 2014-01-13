using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DebtTransferDeedsScenario : TransferPropertyScenario
    {
        public DebtTransferDeedsScenario()
            : base (15)
        { }
        protected DebtTransferDeedsScenario(DebtTransferDeedsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DebtTransferDeeds";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool AllowGoToJail
        {
            get { return false; }
        }

        public override int InvestigateMinimum
        {
            get { return 0; }
        }

        public override int InvestigateMaximum
        {
            get { return 0; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override int Maximum
        {
            get 
            {
                if (Target == null) return 0;

                return Target.FamilyFunds;
            }
        }

        protected override bool ActualTransfer
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.Adults;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.Adults;
        }

        protected override bool Allow()
        {
            if (GetValue<RatioOption,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!GetValue<AllowPurchaseDeedsOption, bool>(sim))
            {
                IncStat("Purchase Denied");
                return false;
            }
            else if (AddStat("Ratio", GetValue<NetRatioOption,int>(sim.Household)) < GetValue<RatioOption,int>()) 
            {
                IncStat("Gated");
                return false;
            }

            return base.Allow(sim);
        }

        protected override int GetCost(PropertyData data)
        {
            int result = 0;

            if (data.PropertyType == RealEstatePropertyType.VacationHome)
            {
                Lot lot = LotManager.GetLot(data.LotId);
                if (lot != null)
                {
                    result = ManagerLot.GetUnfurnishedLotCost(lot, int.MaxValue);
                }
            }

            return (int)(result * (GetValue<SurchargeRateOption, int>() / 100f));
        }

        public override Scenario Clone()
        {
            return new DebtTransferDeedsScenario(this);
        }

        public class RatioOption : IntegerScenarioOptionItem<ManagerMoney, DebtTransferDeedsScenario>, ManagerMoney.IDebtOption
        {
            public RatioOption()
                : base(30)
            { }

            public override string GetTitlePrefix()
            {
                return "DebtTransferDeedRatio";
            }
        }

        public class SurchargeRateOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IDebtOption
        {
            public SurchargeRateOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "DeedSurchargeRate";
            }
        }
    }
}
