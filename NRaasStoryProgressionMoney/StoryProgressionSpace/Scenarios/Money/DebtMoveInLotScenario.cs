using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DebtMoveInLotScenario : MoveInLotScenario
    {
        public DebtMoveInLotScenario(ICollection<SimDescription> going)
            : base(going)
        { }
        protected DebtMoveInLotScenario(DebtMoveInLotScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "DebtMoveInLot";
            }
            else
            {
                return "MoveIn";
            }
        }

        protected override bool CheapestHome
        {
            get { return true; }
        }

        protected override ManagerLot.CheckResult OnLotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds)
        {
            if (currentLotCost < lot.Cost * 0.8)
            {
                stats.IncStat("Find Lot: Too expensive (A)");
                return ManagerLot.CheckResult.Failure;
            }
            else if (availableFunds < lot.Cost)
            {
                stats.IncStat("Find Lot: Too expensive (B)");
                return ManagerLot.CheckResult.Failure;
            }

            return ManagerLot.CheckResult.Success;
        }

        public override Scenario Clone()
        {
            return new DebtMoveInLotScenario(this);
        }
    }
}
