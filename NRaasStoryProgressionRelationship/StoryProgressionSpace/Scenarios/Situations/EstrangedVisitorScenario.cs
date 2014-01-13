using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
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
    public class EstrangedVisitorScenario : VisitorScenario
    {
        public EstrangedVisitorScenario()
        { }
        protected EstrangedVisitorScenario(EstrangedVisitorScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "EstrangedVisitor";
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Relationships.GetParents(sim);
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

        protected override bool Allow(SimDescription sim)
        {
            if (Households.AllowGuardian(sim))
            {
                IncStat("Too Old");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (!base.TargetAllow(target)) return false;

            if (AddScoring("Friendly", target) <= 0)
            {
                IncStat("Friendly Score Fail");
                return false;
            }
            else if (AddScoring("CaresAboutChildren", target) < 0)
            {
                IncStat("Alimony Score Fail");
                return false;
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new EstrangedVisitorScenario(this);
        }

        public class Option : ChanceScenarioOptionItem<ManagerSituation, EstrangedVisitorScenario>
        {
            public Option()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "EstrangedVisitor";
            }
        }
    }
}
