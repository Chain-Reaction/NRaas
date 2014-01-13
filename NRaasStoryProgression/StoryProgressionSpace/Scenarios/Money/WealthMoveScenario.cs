using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class WealthMoveScenario : HouseholdScenario
    {
        public WealthMoveScenario()
            : base ()
        { }
        protected WealthMoveScenario(WealthMoveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "WealthMove";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected int MoveRatio
        {
            get { return GetValue<RatioOption,int>(); }
        }

        protected override bool Allow()
        {
            if (MoveRatio <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (SimTypes.IsSpecial(house))
            {
                IncStat("Special");
                return false;
            }
            else if (house == Household.ActiveHousehold)
            {
                IncStat("Active");
                return false;
            }
            else if (house.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Households.Allow(this, house, GetValue<ManagerHousehold.MinTimeBetweenMovesOption, int>()))
            {
                IncStat("Cooldown");
                return false;
            }
            else if (GetValue<IsAncestralOption,bool>(house))
            {
                IncStat("Ancestral");
                return false;
            }
            else if (house.FamilyFunds < (GetValue<NetWorthOption,int>(house) * (MoveRatio / 100f)))
            {
                IncStat("Poor");
                return false;
            }
            else
            {
                SimDescription head = SimTypes.HeadOfFamily(house);
                if (head == null)
                {
                    IncStat("No Head");
                    return false;
                }
                else if (AddScoring("WealthMove", head) <= 0)
                {
                    IncStat("Score Fail");
                    return false;
                }
            }
            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new WealthMoveInLotScenario(HouseholdsEx.All(House), (int)(GetValue<NetWorthOption, int>(House) * (MoveRatio / 100f))), ScenarioResult.Start);
            return false;
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        public override Scenario Clone()
        {
            return new WealthMoveScenario(this);
        }

        protected class WealthMoveInLotScenario : MoveInLotScenario
        {
            int mBetterHome;

            public WealthMoveInLotScenario(ICollection<SimDescription> going, int betterHome)
                : base(going)
            {
                mBetterHome = betterHome;
            }
            protected WealthMoveInLotScenario(WealthMoveInLotScenario scenario)
                : base(scenario)
            {
                mBetterHome = scenario.mBetterHome;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "WealthMoveInLot";
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
                int cost = Lots.GetLotCost(lot);
                if (availableFunds < cost)
                {
                    stats.IncStat("Find Lot: Too expensive");
                    return ManagerLot.CheckResult.Failure;
                }
                else if (cost < mBetterHome)
                {
                    stats.IncStat("Find Lot: Too Cheap");
                    return ManagerLot.CheckResult.Failure;
                }

                return ManagerLot.CheckResult.Success;
            }

            public override Scenario Clone()
            {
                return new WealthMoveInLotScenario(this);
            }
        }

        public class RatioOption : IntegerScenarioOptionItem<ManagerMoney, WealthMoveScenario>
        {
            public RatioOption()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "WealthMoveRatio";
            }
        }
    }
}
