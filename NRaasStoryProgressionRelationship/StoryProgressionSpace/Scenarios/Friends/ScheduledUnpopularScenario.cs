using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class ScheduledUnpopularScenario : UnpopularFriendScenario
    {
        public ScheduledUnpopularScenario()
        { }
        protected ScheduledUnpopularScenario(ScheduledUnpopularScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "ScheduledUnpopular";
            }
            else
            {
                return base.GetTitlePrefix(type);
            }
        }

        protected override bool Allow(SimDescription sim)
        {
            int popular = AddScoring("Popularity", sim);
            if (popular > 0)
            {
                IncStat("Popular");
                return false;
            }
            else if (!RandomUtil.RandomChance(-popular))
            {
                IncStat("Chance Fail");
                return false;
            }

            return base.Allow(sim);
        }

        public override Scenario Clone()
        {
            return new ScheduledUnpopularScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerFriendship, ScheduledUnpopularScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowUnpopularity";
            }
        }
    }
}
