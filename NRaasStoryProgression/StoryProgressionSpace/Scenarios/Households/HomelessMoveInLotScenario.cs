using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class HomelessMoveInLotScenario : MoveInLotScenario
    {
        public HomelessMoveInLotScenario(ICollection<SimDescription> going)
            : base(going)
        { }
        protected HomelessMoveInLotScenario(HomelessMoveInLotScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "HomelessMoveInLot";
            }
            else
            {
                return "MoveIn";
            }
        }

        protected override ManagerLot.FindLotFlags Inspect
        {
            get
            {
                if (GetValue<ManagerHousehold.HomelessInspectionOption, bool>())
                {
                    return ManagerLot.FindLotFlags.Inspect;
                }
                else
                {
                    return ManagerLot.FindLotFlags.None;
                }
            }
        }

        protected override bool CheapestHome
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
            {
                IncStat("Move In Denied");
                return false;
            }
            else if (Sims.HasEnough(Movers))
            {
                IncStat("Town Full");
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
            return new HomelessMoveInLotScenario(this);
        }
    }

}
