using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
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
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class StrandedCoupleScenario : MoveScenario 
    {
        bool mReport = true;

        public int mMaximumLoan;

        public StrandedCoupleScenario()
        { }
        public StrandedCoupleScenario (SimDescription sim, SimDescription target, bool report)
            : base (sim, target)
        {
            mReport = report;
        }
        protected StrandedCoupleScenario(StrandedCoupleScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
            mMaximumLoan = scenario.mMaximumLoan;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "StrandedCouple";
        }

        protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
        {
            return new StrandedMoveInLotScenario(going, mMaximumLoan);
        }

        protected override MoveInLotScenario GetMoveInLotScenario(HouseholdBreakdown breakdown)
        {
            return new StrandedMoveInLotScenario(breakdown, mMaximumLoan);
        }

        protected override MoveInScenario GetMoveInScenario(List<SimDescription> going, SimDescription moveInWith)
        {
            return new StrandedMoveInScenario(going, moveInWith);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(sim.Partner);
            return list;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!sim.IsMarried)
            {
                IncStat("Not Married");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Household == sim.Household)
            {
                IncStat("Already Together");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public static UpdateDelegate OnLoanScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OnLoanScenario != null)
            {
                OnLoanScenario(this, frame);

                Add(frame, new FailureScenario(), ScenarioResult.Start);
            }

            base.PrivateUpdate(frame);

            if (mReport)
            {
                Add(frame, new NoHomeScenario(Sim, Target), ScenarioResult.Failure);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new StrandedCoupleScenario(this);
        }

        public class StrandedMoveInScenario : InspectedMoveInScenario
        {
            public StrandedMoveInScenario(List<SimDescription> going, SimDescription moveInWith)
                : base(going, moveInWith)
            { }
            public StrandedMoveInScenario(StrandedMoveInScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "StrandedMoveIn";
                }
                else
                {
                    return base.GetTitlePrefix(type);
                }
            }

            protected override ManagerLot.FindLotFlags Inspect
            {
                get { return base.Inspect | ManagerLot.FindLotFlags.AllowExistingInfractions; }
            }

            public override Scenario Clone()
            {
                return new StrandedMoveInScenario(this);
            }
        }

        public class StrandedMoveInLotScenario : StandardMoveInLotScenario
        {
            public StrandedMoveInLotScenario(List<SimDescription> going, int maximumLoan)
                : base(going, maximumLoan)
            { }
            public StrandedMoveInLotScenario(HouseholdBreakdown breakdown, int maximumLoan)
                : base(breakdown, maximumLoan)
            { }
            public StrandedMoveInLotScenario(StrandedMoveInLotScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "StrandedMoveInLot";
                }
                else
                {
                    return base.GetTitlePrefix(type);
                }
            }

            protected override ManagerLot.FindLotFlags Inspect
            {
                get { return base.Inspect | ManagerLot.FindLotFlags.AllowExistingInfractions; }
            }

            public override Scenario Clone()
            {
                return new StrandedMoveInLotScenario(this);
            }
        }
    }
}
