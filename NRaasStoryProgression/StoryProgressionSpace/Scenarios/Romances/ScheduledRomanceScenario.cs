using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
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
    public class ScheduledRomanceScenario : PartnerScenario
    {
        public ScheduledRomanceScenario()
        { }
        protected ScheduledRomanceScenario(ScheduledRomanceScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledRomance";
        }

        protected override int ContinueChance
        {
            get { return 10; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new ScoredExistingEnemyScenario(Sim, Target), ScenarioResult.Failure);
            Add(frame, new OldFlirtScenario(Sim, Target, true, ManagerRomance.AffairStory.All, -1), ScenarioResult.Failure);
            Add(frame, new ScoredExistingFriendScenario(Sim, Target), ScenarioResult.Failure);
            return false;
        }

        public override Scenario Clone()
        {
            return new ScheduledRomanceScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerRomance, ScheduledRomanceScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ScheduledRomance";
            }
        }
    }
}
