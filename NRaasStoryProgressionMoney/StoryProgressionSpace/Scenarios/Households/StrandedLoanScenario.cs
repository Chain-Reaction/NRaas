using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class StrandedLoanScenario : DualSimScenario
    {
        StrandedCoupleScenario mParent;

        public StrandedLoanScenario(StrandedCoupleScenario scenario)
            : base(scenario.Sim, scenario.Target)
        {
            mParent = scenario;
        }
        protected StrandedLoanScenario(StrandedLoanScenario scenario)
            : base (scenario)
        {
            mParent = scenario.mParent;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "StrandedLoan";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return null;
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            if (!GetValue<UnifiedBillingScenario.UnifiedBillingOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        public static bool CalculateValue(StrandedCoupleScenario parent)
        {
            int maximumLoan = parent.GetValue<Option, int>();

            int netWorth = parent.GetValue<NetWorthOption, int>(parent.Sim.Household);
            if (parent.Sim.Household != parent.Target.Household)
            {
                netWorth += parent.GetValue<NetWorthOption, int>(parent.Target.Household);
            }

            netWorth /= 2;

            parent.AddStat("MaximumLoan", maximumLoan);
            parent.AddStat("NetWorth", netWorth);

            if (netWorth < maximumLoan)
            {
                maximumLoan = netWorth;
            }

            if (maximumLoan <= 0)
            {
                parent.IncStat("NetWorth Too Low");
                return false;
            }

            parent.mMaximumLoan = maximumLoan;
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return CalculateValue(mParent);
        }

        public override Scenario Clone()
        {
            return new StrandedLoanScenario(this);
        }

        public class Option : IntegerManagerOptionItem<ManagerHousehold>
        {
            public Option()
                : base(25000)
            { }

            public override string GetTitlePrefix()
            {
                return "StrandedLoan";
            }

            public override bool Install(ManagerHousehold main, bool initial)
            {
                if (initial)
                {
                    StrandedCoupleScenario.OnLoanScenario += OnRun;
                }

                return base.Install(main, initial);
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                StrandedCoupleScenario s = scenario as StrandedCoupleScenario;
                if (s == null) return;

                StrandedLoanScenario.CalculateValue(s);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<UnifiedBillingScenario.UnifiedBillingOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
