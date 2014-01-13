using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public class SuccessScenario : ScheduledSoloScenario
    {
        public SuccessScenario()
        { }
        protected SuccessScenario(SuccessScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Success";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new SuccessScenario (this);
        }
    }
}
