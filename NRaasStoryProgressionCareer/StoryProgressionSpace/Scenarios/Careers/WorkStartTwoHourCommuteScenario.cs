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
    public class WorkStartTwoHourCommuteScenario : StartTwoHourCommuteScenario
    {
        public WorkStartTwoHourCommuteScenario(SimDescription sim)
            : base(sim)
        { }
        protected WorkStartTwoHourCommuteScenario(WorkStartTwoHourCommuteScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "WorkStartTwoHourCommute";
        }

        public override Career Job
        {
            get { return Sim.Occupation as Career; }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            Add(frame, new WorkSetAlarmScenario(Sim), ScenarioResult.Start);

            return false;
        }

        public override Scenario Clone()
        {
            return new WorkStartTwoHourCommuteScenario(this);
        }

        protected class WorkSetAlarmScenario : SetAlarmScenario
        {
            public WorkSetAlarmScenario(SimDescription sim)
                : base(sim)
            { }
            protected WorkSetAlarmScenario(WorkSetAlarmScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "WorkTwoHourSetAlarm";
            }

            public override Career Job
            {
                get { return Sim.Occupation as Career; }
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                Manager.AddAlarm(new WorkTwoHourCommuteScenario(Sim));
                return true;
            }

            public override Scenario Clone()
            {
                return new WorkSetAlarmScenario(this);
            }
        }

        public class Installer : ExpansionInstaller<ManagerCareer>
        {
            protected override bool PrivateInstall(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    WorkUpdateScenario.OnWorkStartTwoHourCommuteScenario += OnInstall;

                    CareerUpdateScenario.OnCarpoolUpdateScenario += TwoHourCommuteScenario.CarpoolUpdate;
                }

                return true;
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new WorkStartTwoHourCommuteScenario(s.Sim), ScenarioResult.Failure);
            }
        }
    }
}
