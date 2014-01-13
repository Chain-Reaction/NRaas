using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class WorkPushScenario : CareerPushScenario
    {
        public WorkPushScenario(SimDescription sim)
            : base(sim)
        { }
        protected WorkPushScenario(WorkPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "WorkSlacker";
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((sim.IsPregnant) && (!GetValue<PushPregnantOption, bool>()))
            {
                IncStat("Pregnant Denied");
                return false;
            }

            return base.Allow(sim);
        }

        public override Occupation Occupation
        {
            get 
            { 
                // Cast intentionally to weed out non Career occupations
                return Sim.Occupation as Career; 
            }
        }

        protected override bool PostSlackerWarning()
        {
            return Situations.AddWorkSlacker(Sim);
        }

        public override Scenario Clone()
        {
            return new WorkPushScenario(this);
        }

        public class PushPregnantOption : BooleanManagerOptionItem<ManagerSituation>
        {
            public PushPregnantOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WorkPushPregnant";
            }
        }
    }
}
