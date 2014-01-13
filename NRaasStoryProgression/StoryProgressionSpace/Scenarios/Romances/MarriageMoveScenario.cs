using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Households;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class MarriageMoveScenario : MoveScenario
    {
        public MarriageMoveScenario()
        { }
        public MarriageMoveScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected MarriageMoveScenario(MarriageMoveScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "MarriageMove";
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Household == Target.Household)
            {
                IncStat("Together");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
        {
            return new MarriageMoveMoveInLotScenario(going);
        }

        protected override MoveInLotScenario GetMoveInLotScenario(HouseholdBreakdown breakdown)
        {
            return new MarriageMoveMoveInLotScenario(breakdown);
        }

        protected override MoveInScenario GetMoveInScenario(List<SimDescription> going, SimDescription moveInWith)
        {
            return new StrandedCoupleScenario.StrandedMoveInScenario(going, moveInWith);
        }

        public override Scenario Clone()
        {
            return new MarriageMoveScenario(this);
        }

        protected class MarriageMoveMoveInLotScenario : MoveInLotScenario
        {
            public MarriageMoveMoveInLotScenario(List<SimDescription> going)
                : base(going)
            { }
            public MarriageMoveMoveInLotScenario(HouseholdBreakdown breakdown)
                : base(breakdown)
            { }
            protected MarriageMoveMoveInLotScenario(MarriageMoveMoveInLotScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "MarriageMoveMoveInLot";
                }
                else
                {
                    return "MoveIn";
                }
            }

            protected override bool CheapestHome
            {
                get { return false; }
            }

            protected override ManagerLot.CheckResult OnLotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds)
            {
                if (availableFunds < Lots.GetLotCost(lot))
                {
                    stats.IncStat("Find Lot: Too expensive");
                    return ManagerLot.CheckResult.Failure;
                }

                return ManagerLot.CheckResult.Success;
            }

            public override Scenario Clone()
            {
                return new MarriageMoveMoveInLotScenario(this);
            }
        }
    }
}
