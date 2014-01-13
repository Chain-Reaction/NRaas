using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class PlayConcertScenario : SimScenario
    {
        public PlayConcertScenario()
        { }
        protected PlayConcertScenario(PlayConcertScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PlayConcert";
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
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!HasMetric(sim))
            {
                IncStat("No Metric");
                return false;
            }
            else if ((SimClock.Hours24 < ShowVenue.kPerformConcertAvailableStartingAtHour) || (SimClock.Hours24 >= ShowVenue.kPerformConcertAvailableEndingAtHour))
            {
                IncStat("Out of Time");
                return false;
            }

            return base.Allow(sim);
        }

        protected bool HasMetric(SimDescription sim)
        {
            if (ManagerCareer.HasSkillCareer(sim, SkillNames.Guitar))
            {
                return (sim.Occupation.CareerLevel >= (Music.LevelToGetPaidForConcerts - 3));
            }

            Career job = sim.Occupation as Career;
            if (job != null)
            {
                if (job.CurLevel == null) return false;

                foreach (PerfMetric metric in job.CurLevel.Metrics)
                {
                    if (metric is Music.MetricConcertsPerformed)
                    {
                        return (metric.CalcMetric(job) < 3);
                    }
                }
            }

            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<ShowVenue> list = new List<ShowVenue>(Sims3.Gameplay.Queries.GetObjects<ShowVenue>());
            foreach (ShowVenue hole in list)
            {
                GreyedOutTooltipCallback tooltip = null;
                if (hole.PerformConcertAllowed(ref tooltip))
                {
                    return Situations.PushInteraction(this, Sim, hole, PerformConcert.Singleton);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new PlayConcertScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSituation, PlayConcertScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PushPlayConcert";
            }
        }
    }
}
