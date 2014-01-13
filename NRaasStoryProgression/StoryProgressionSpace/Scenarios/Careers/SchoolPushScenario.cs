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
    public class SchoolPushScenario : CareerPushScenario
    {
        public SchoolPushScenario(SimDescription sim)
            : base(sim)
        { }
        protected SchoolPushScenario(SchoolPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "SchoolSlacker";
        }

        public override Occupation Occupation
        {
            get 
            {
                if (Sim.CareerManager == null) return null;

                return Sim.CareerManager.School; 
            }
        }

        protected override bool PostSlackerWarning()
        {
            return Situations.AddSchoolSlacker(Sim);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (JobPerformanceScenario.HandleForeigners(Occupation as School))
            {
                IncStat("Foreign Limited");
            }

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new SchoolPushScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerSituation>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SchoolPush";
            }
        }
    }
}
