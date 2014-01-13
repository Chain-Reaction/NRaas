using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
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
    public class FriendlyOutingScenario : OutingScenario
    {
        public FriendlyOutingScenario()
        { }
        protected FriendlyOutingScenario(FriendlyOutingScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "FriendlyOuting";
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
            if (!Friends.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (AddScoring("Popularity", sim) <= 0)
            {
                IncStat("Unpopular");
                return false;
            }
            else if (AddScoring("Friendly", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Child != Target.Child)
            {
                IncStat("Age Diff");
                return false;
            }
            else if (Households.AllowGuardian(Sim) != Households.AllowGuardian(Target))
            {
                IncStat("Age Diff");
                return false;
            }
            else if (ManagerSim.GetLTR(Sim, Target) < 0)
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (Sim.Child)
            {
                name += "Child";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new FriendlyOutingScenario(this);
        }

        public class Option : ChanceScenarioOptionItem<ManagerSituation, FriendlyOutingScenario>
        {
            public Option()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "FriendlyOuting";
            }
        }
    }
}
