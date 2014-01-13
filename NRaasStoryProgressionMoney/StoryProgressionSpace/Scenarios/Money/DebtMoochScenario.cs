using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
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
    public class DebtMoochScenario : MoochScenario
    {
        public DebtMoochScenario()
            : base (-20)
        { }
        protected DebtMoochScenario(DebtMoochScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DebtMooch";
        }

        protected override string AccountingKey
        {
            get { return "DebtMooch"; }
        }

        protected override int Minimum
        {
            get 
            {
                return GetValue<DebtOption,int>(Sim.Household) / 4; 
            }
        }

        protected override int Maximum
        {
            get 
            {
                if (Sim == null) return 1;

                return GetValue<DebtOption,int>(Sim.Household); 
            }
        }

        protected override float SkillIncrease
        {
            get { return TraitTuning.MoochTraitSmallMoneyMoochSkill; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected int DangerRatio
        {
            get { return GetValue<RatioOption,int>(); }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return new SimScoringList(Manager, "MoochVictim", Sims.Adults, false, sim).GetBestByMinScore(1);
        }

        protected override bool Allow()
        {
            if (DangerRatio <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (AddStat("Ratio", GetValue<NetRatioOption,int>(sim.Household)) < DangerRatio) 
            {
                IncStat("Gated");
                return false;
            }
            else if (AddScoring("Mooch", sim) <= 0) 
            {
                IncStat("Poor Score");
                return false;
            }

            return base.Allow(sim);
        }

        public override Scenario Clone()
        {
            return new DebtMoochScenario(this);
        }

        public class RatioOption : IntegerScenarioOptionItem<ManagerMoney, DebtMoochScenario>, ManagerMoney.IDebtOption
        {
            public RatioOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "DebtMoochRatio";
            }
        }
    }
}
