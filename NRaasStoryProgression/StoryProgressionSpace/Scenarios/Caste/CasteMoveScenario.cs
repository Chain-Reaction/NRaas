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

namespace NRaas.StoryProgressionSpace.Scenarios.Caste
{
    public class CasteMoveScenario : HouseholdScenario
    {
        public CasteMoveScenario()
            : base()
        { }
        protected CasteMoveScenario(CasteMoveScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "CasteMove";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

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
            else if (Lots.AllowCastes(this, house.LotHome, HouseholdsEx.All(house)))
            {
                IncStat("Unnecessary");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new CasteMoveInLotScenario(HouseholdsEx.All(House)), ScenarioResult.Start);
            return false;
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        public override Scenario Clone()
        {
            return new CasteMoveScenario(this);
        }

        protected class CasteMoveInLotScenario : MoveInLotScenario
        {
            public CasteMoveInLotScenario(ICollection<SimDescription> going)
                : base(going)
            { }
            protected CasteMoveInLotScenario(CasteMoveInLotScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "CasteMoveInLot";
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

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                manager = Lots;

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new CasteMoveInLotScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerCaste, CasteMoveScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CasteMove";
            }
        }
    }
}
