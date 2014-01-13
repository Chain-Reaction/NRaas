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
    public class ScheduledMarriageScenario : ScheduledMarriageBaseScenario
    {
        public ScheduledMarriageScenario()
        { }
        protected ScheduledMarriageScenario(ScheduledMarriageScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledMarriage";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        public static event UpdateDelegate OnBreakup;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OnBreakup != null)
            {
                OnBreakup(this, frame);
            };

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new ScheduledMarriageScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerRomance, ScheduledMarriageScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ScheduledMarriage";
            }
        }
    }
}
