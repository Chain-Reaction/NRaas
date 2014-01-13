using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class ScheduledFailSafeFlirtScenario : ScheduledRegularFlirtScenario
    {
        public ScheduledFailSafeFlirtScenario()
        { }
        protected ScheduledFailSafeFlirtScenario(ScheduledFailSafeFlirtScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledFailSafeFlirt";
        }

        protected override bool Force
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Flirts.FlirtPool;
        }

        public override Scenario Clone()
        {
            return new ScheduledFailSafeFlirtScenario(this);
        }
    }
}
