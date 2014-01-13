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
using Sims3.Gameplay.Objects.Electronics;
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
    public class WriteReportPushScenario : SimScenario
    {
        public WriteReportPushScenario()
        { }
        protected WriteReportPushScenario(WriteReportPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "WriteReports";
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
                if ((metric is LawEnforcement.MetricReportsWritten) || (metric is Journalism.MetricStoriesAndReviews))
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
            Computer computer = ManagerLot.GetUsableComputer(Sim);
            if (computer == null)
            {
                IncStat("No Computer");
                return false;
            }

            Career occupation = Sim.Occupation as Career;

            if (!occupation.CanWriteReport())
            {
                if (Sim.Occupation is LawEnforcement)
                {
                    LawEnforcement career = Sim.Occupation as LawEnforcement;
                
                    if (!career.CanInterview())
                    {
                        IncStat("Police Level Fail");
                        return false;
                    }

                    foreach (SimDescription sim in Sims.Adults)
                    {
                        if (career.HasSimBeenInterviewed(sim)) continue;

                        career.SimInterviewed(sim);
                        break;
                    }
                }
                else if (Sim.Occupation is Journalism)
                {
                    Journalism career = Sim.Occupation as Journalism;

                    if (!career.CanInterview())
                    {
                        IncStat("Journalism Level Fail");
                        return false;
                    }

                    foreach (SimDescription sim in Sims.Adults)
                    {
                        if (career.HasSimBeenInterviewed(sim)) continue;

                        career.SimInterviewed(sim);
                        break;
                    }
                }
            }

            return Situations.PushInteraction<Computer>(this, Sim, computer, Computer.WriteReport.Singleton);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new WriteReportPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, WriteReportPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WriteReportPush";
            }
        }
    }
}
