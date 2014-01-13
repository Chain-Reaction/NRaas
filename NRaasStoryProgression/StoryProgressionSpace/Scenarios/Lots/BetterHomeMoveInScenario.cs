using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class BetterHomeMoveInScenario : MoveInLotScenario
    {
        Household mOldHouse = null;

        public BetterHomeMoveInScenario(Household house)
            : base(HouseholdsEx.All(house))
        {
            mOldHouse = house;
        }
        protected BetterHomeMoveInScenario(BetterHomeMoveInScenario scenario)
            : base (scenario)
        {
            mOldHouse = scenario.mOldHouse;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "BetterHome";
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

        protected override bool Allow()
        {
            if (!GetValue<InspectedOption,bool>(mOldHouse))
            {
                IncStat("Uninspected");
                return false;
            }

            return base.Allow();
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
            return new BetterHomeMoveInScenario(this);
        }
    }
}
