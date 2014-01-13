using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class StandardMoveInLotScenario : MoveInLotScenario
    {
        int mMaximumLoan;

        public StandardMoveInLotScenario(List<SimDescription> going, int maximumLoan)
            : base(going)
        {
            mMaximumLoan = maximumLoan;
        }
        public StandardMoveInLotScenario(HouseholdBreakdown breakdown, int maximumLoan)
            : base(breakdown)
        {
            mMaximumLoan = maximumLoan;
        }
        protected StandardMoveInLotScenario(StandardMoveInLotScenario scenario)
            : base(scenario)
        {
            mMaximumLoan = scenario.mMaximumLoan;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "StandardMoveInLot";
            }
            else
            {
                return "MoveIn";
            }
        }

        protected override int MaximumLoan
        {
            get { return mMaximumLoan; }
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
            return new StandardMoveInLotScenario(this);
        }
    }
}
