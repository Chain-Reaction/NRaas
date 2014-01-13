using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class LecturePushScenario : SimScenario
    {
        public LecturePushScenario()
        { }
        public LecturePushScenario(SimDescription sim)
            : base (sim)
        { }
        protected LecturePushScenario(LecturePushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Lecture";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 60; }
        }

        protected override int MaximumReschedules
        {
            get { return 12; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.Employed;
        }

        protected static bool HasMetric(SimDescription sim)
        {
            Education career = sim.Occupation as Education;
            if (career == null) return false;

            foreach (PerfMetric metric in career.CurLevel.Metrics)
            {
                if (metric is Education.MetricLecturesGiven)
                {
                    return (metric.CalcMetric(career) < 3);
                }
            }

            return false;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!HasMetric(sim))
            {
                IncStat("No Metric");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<RabbitHole> schools = new List<RabbitHole>(Sims3.Gameplay.Queries.GetObjects<SchoolRabbitHole>());
            if (schools.Count == 0) return false;

            RabbitHole choice = RandomUtil.GetRandomObjectFromList(schools);
            
            return Situations.PushInteraction(this, Sim, choice, Education.GiveLectureInAtRabbitHole.Singleton);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new LecturePushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSituation, LecturePushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "LecturePush";
            }
        }
    }
}
