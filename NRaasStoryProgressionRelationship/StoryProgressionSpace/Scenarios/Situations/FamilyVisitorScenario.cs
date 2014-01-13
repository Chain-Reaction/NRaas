using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
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
    public class FamilyVisitorScenario : VisitorScenario
    {
        public FamilyVisitorScenario()
        { }
        protected FamilyVisitorScenario(FamilyVisitorScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "FamilyVisitor";
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!Relationships.IsCloselyRelated(Sim, Target, false))
            {
                IncStat("Unrelated");
                return false;
            }
            else if (ManagerSim.GetLTR(Sim, Target) < 25)
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public override Scenario Clone()
        {
            return new FamilyVisitorScenario(this);
        }

        public class Option : ChanceScenarioOptionItem<ManagerSituation, FamilyVisitorScenario>
        {
            public Option()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "FamilyVisitor";
            }
        }
    }
}
