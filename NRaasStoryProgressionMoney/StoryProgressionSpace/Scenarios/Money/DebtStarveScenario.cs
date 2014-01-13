using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
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
    /*
    public class DebtStarveScenario : HouseholdScenario
    {
        public DebtStarveScenario()
            : base ()
        { }
        protected DebtStarveScenario(DebtStarveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "DebtStarve";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<RatioOption, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            if (!GetValue<ManagerDeath.AllowPushDeathOption, bool>())
            {
                IncStat("Push Denied");
                return false;
            }

            return (AddStat("Ratio", GetValue<NetRatioOption, int>(House)) > GetValue<RatioOption, int>());
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<SimDescription> members = new List<SimDescription>(HouseholdsEx.All(House));

            while (members.Count > 0)
            {
                SimDescription member = RandomUtil.GetRandomObjectFromList(members);
                members.Remove(member);

                if (Deaths.Allow(this, member))
                {
                    Add(frame, new KillScenario(member, null, SimDescription.DeathType.Starve), ScenarioResult.Failure);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new DebtStarveScenario(this);
        }

        private class RatioOption : IntegerScenarioOptionItem<ManagerMoney, DebtStarveScenario>, ManagerMoney.IDebtOption
        {
            public RatioOption()
                : base(75)
            { }

            public override string GetTitlePrefix()
            {
                return "DebtStarveRatio";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ManagerDeath.AllowPushDeathOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
    */
}
