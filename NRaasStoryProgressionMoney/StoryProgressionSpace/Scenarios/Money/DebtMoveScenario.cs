using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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
    public class DebtMoveScenario : HouseholdScenario
    {
        public DebtMoveScenario()
            : base ()
        { }
        protected DebtMoveScenario(DebtMoveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DebtMove";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected virtual int DangerRatio
        {
            get { return GetValue<RatioOption,int>(); }
        }

        protected override bool Allow()
        {
            if (DangerRatio <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            if (!Households.Allow(this, house, GetValue<ManagerHousehold.MinTimeBetweenMovesOption, int>()))
            {
                IncStat("User Denied");
                return false;
            }
            else if (GetValue<IsAncestralOption,bool>(House))
            {
                IncStat("Ancestral");
                return false;
            }

            return (AddStat("Ratio", GetValue<NetRatioOption,int>(House)) > DangerRatio);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new DebtMoveInLotScenario(HouseholdsEx.All(House)), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new DebtMoveScenario(this);
        }

        private class RatioOption : IntegerScenarioOptionItem<ManagerMoney, DebtMoveScenario>, ManagerMoney.IDebtOption
        {
            public RatioOption()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "DebtMoveRatio";
            }
        }
    }
}
