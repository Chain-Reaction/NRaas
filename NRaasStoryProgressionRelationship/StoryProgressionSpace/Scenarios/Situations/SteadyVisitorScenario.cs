using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class SteadyVisitorScenario : VisitorScenario
    {
        public SteadyVisitorScenario()
        { }
        protected SteadyVisitorScenario(SteadyVisitorScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "SteadyVisitor";
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> results = new List<SimDescription>();
            results.Add(sim.Partner);
            return results;
        }

        protected override bool Allow()
        {
            if (Situations.GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Friends.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!base.TargetAllow(sim)) return false;

            if (AddScoring("Friendly", sim) <= 0)
            {
                IncStat("Friendly Score Fail");
                return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            frame.Add(Romances, new OldFlirtScenario(Sim, Target, false, ManagerRomance.AffairStory.All, -1), ScenarioResult.Failure);
            //Add(frame, new OldFlirtScenario(Sim, Target, false, ManagerRomance.AffairStory.All, -1), ScenarioResult.Failure);

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new SteadyVisitorScenario(this);
        }

        public class Option : ChanceScenarioOptionItem<ManagerSituation, SteadyVisitorScenario>
        {
            public Option()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "SteadyVisitor";
            }
        }
    }
}
