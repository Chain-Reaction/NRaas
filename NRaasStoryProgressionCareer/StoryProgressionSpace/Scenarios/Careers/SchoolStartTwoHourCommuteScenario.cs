using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
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
    public class SchoolStartTwoHourCommuteScenario : StartTwoHourCommuteScenario
    {
        public SchoolStartTwoHourCommuteScenario(SimDescription sim)
            : base(sim)
        { }
        protected SchoolStartTwoHourCommuteScenario(SchoolStartTwoHourCommuteScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SchoolStartTwoHourCommute";
        }

        public override Career Job
        {
            get
            {
                if (Sim.CareerManager == null) return null;

                return Sim.CareerManager.School;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            Add(frame, new SchoolSetAlarmScenario(Sim), ScenarioResult.Start);

            return true;
        }

        public override Scenario Clone()
        {
            return new SchoolStartTwoHourCommuteScenario(this);
        }

        protected class SchoolSetAlarmScenario : SetAlarmScenario
        {
            public SchoolSetAlarmScenario(SimDescription sim)
                : base(sim)
            { }
            protected SchoolSetAlarmScenario(SchoolSetAlarmScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "SchoolTwoHourSetAlarm";
            }

            public override Career Job
            {
                get 
                {
                    if (Sim.CareerManager == null) return null;

                    return Sim.CareerManager.School; 
                }
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return (Manager.AddAlarm(new SchoolTwoHourCommuteScenario(Sim)) != null);
            }

            public override Scenario Clone()
            {
                return new SchoolSetAlarmScenario(this);
            }
        }

        public class Installer : ExpansionInstaller<ManagerCareer>
        {
            protected override bool PrivateInstall(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    SchoolUpdateScenario.OnSchoolStartTwoHourCommuteScenario += OnInstall;
                }

                return true;
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new SchoolStartTwoHourCommuteScenario(s.Sim), ScenarioResult.Failure);
            }
        }
    }
}
