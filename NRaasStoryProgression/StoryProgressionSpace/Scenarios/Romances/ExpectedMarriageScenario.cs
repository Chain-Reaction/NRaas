using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class ExpectedMarriageScenario : MarriageScenario 
    {
        public ExpectedMarriageScenario(SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected ExpectedMarriageScenario(ExpectedMarriageScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "ExpectedMarriage";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Romances.Partnered;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(sim.Partner);
            return list;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Add(frame, new MarriageMoveScenario(Sim, Target), ScenarioResult.Start);
            Add(frame, new NoHoneymoonScenario(Sim, Target), ScenarioResult.Failure);
            return true;
        }

        public override Scenario Clone()
        {
            return new ExpectedMarriageScenario(this);
        }
    }
}
