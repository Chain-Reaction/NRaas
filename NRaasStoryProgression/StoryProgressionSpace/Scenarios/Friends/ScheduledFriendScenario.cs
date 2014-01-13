using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
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
    public class ScheduledFriendScenario : ScheduledFriendBaseScenario
    {
        public ScheduledFriendScenario()
        { }
        protected ScheduledFriendScenario(SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected ScheduledFriendScenario(ScheduledFriendScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledFriend";
        }

        protected override bool Allow()
        {
            if (!GetValue<AllowFriendOption, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        public override Scenario Clone()
        {
            return new ScheduledFriendScenario(this);
        }

        public class AllowFriendOption : BooleanScenarioOptionItem<ManagerFriendship, ScheduledFriendScenario>
        {
            public AllowFriendOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowFriendMaking";
            }
        }
    }
}
