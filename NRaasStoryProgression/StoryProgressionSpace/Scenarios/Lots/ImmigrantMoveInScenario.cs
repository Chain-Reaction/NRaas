using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ImmigrantMoveInScenario : MoveInLotScenario
    {
        public ImmigrantMoveInScenario(List<SimDescription> sims)
            : base(sims)
        { }
        protected ImmigrantMoveInScenario(ImmigrantMoveInScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ImmigrantMoveIn";
        }

        protected override bool CheapestHome
        {
            get { return true; }
        }

        protected override bool TestDead
        {
            get { return false; }
        }

        protected override ManagerLot.FindLotFlags Inspect
        {
            get
            {
                // Removes the Pets inspection
                return ManagerLot.FindLotFlags.Inspect;
            }
        }

        protected override ManagerLot.CheckResult OnLotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds)
        {
            Vector2 range = GetValue<LotPriceRangeOption, Vector2>();
            if ((range.x == 0) && (range.y == 0)) return ManagerLot.CheckResult.IgnoreCost;

            int cost = Lots.GetLotCost(lot);
            if ((range.x <= cost) && (cost <= range.y))
            {
                return ManagerLot.CheckResult.IgnoreCost;
            }
            else
            {
                stats.IncStat("Out of Range");
                return ManagerLot.CheckResult.Failure;
            }
        }

        public static event UpdateDelegate OnAfterMoveIn;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (base.PrivateUpdate(frame))
            {
                List<ISimDescription> sims = new List<ISimDescription>();

                Household house = null;

                foreach(SimDescription sim in Movers)
                {
                    house = sim.Household;

                    sims.Add(sim);
                }

                if (house != null)
                {
                    Vector2 fundsRange = GetValue<CashRangeOption,Vector2>();

                    int funds = (int)RandomUtil.GetFloat(fundsRange.x, fundsRange.y);
                    if (funds == 0)
                    {
                        if (house.FamilyFunds <= 0)
                        {
                            house.ModifyFamilyFunds(CASLogic.GetHouseholdStartingFunds(sims));
                        }
                    }
                    else
                    {
                        house.SetFamilyFunds(funds, false);
                    }

                    Options.StampImmigrantHousehold(house);
                }

                if (OnAfterMoveIn != null)
                {
                    OnAfterMoveIn(this, frame);
                }

                ManagerSim.ForceRecount();
                return true;
            }

            foreach (SimDescription sim in Movers)
            {
                sim.Dispose();
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new ImmigrantMoveInScenario(this);
        }

        public class LotPriceRangeOption : RangeManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public LotPriceRangeOption()
                : base(new Vector2(0, 10000000), new Vector2(0, 10000000))
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrantLotCostRange";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class CashRangeOption : RangeManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public CashRangeOption()
                : base(new Vector2(10000, 50000), new Vector2(0, 10000000))
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrantCashRange";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
