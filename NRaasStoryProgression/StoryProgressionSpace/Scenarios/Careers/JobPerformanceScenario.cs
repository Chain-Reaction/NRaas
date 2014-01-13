using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
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
    public class JobPerformanceScenario : SimScenario
    {
        public JobPerformanceScenario()
            : base ()
        { }
        protected JobPerformanceScenario(JobPerformanceScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "JobPerformance";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            // Run every 10 sim-minutes
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((Sim.Occupation == null) && ((Sim.CareerManager == null) || (Sim.CareerManager.School == null)))
            {
                IncStat("Unnecessary");
                return false;
            }

            return base.Allow(sim);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            List<SimDescription> sims = new List<SimDescription>(Careers.Employed);
            ManagerSim.Union(sims, Careers.SchoolChildren);

            return sims;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career career = Sim.Occupation as Career;

            if (Careers.Allow(this, Sim))
            {
                UpdatePerformance(career);
            }
            else if (career != null)
            {
                career.LastPerfChange = 0;

                career.ShouldDemote = false;
                career.ShouldPromote = false;
            }

            if (Sim.CareerManager != null)
            {
                UpdatePerformance(Sim.CareerManager.School);
            }

            return true;
        }

        public static bool HandleForeigners(School school)
        {
            if (school == null) return false;

            if (school.OwnerDescription.Genealogy.Parents.Count != Relationships.GetParents(school.OwnerDescription).Count)
            {
                if (school.Performance >= school.SchoolTuning.GradeThresholdB)
                {
                    school.mPerformance = school.SchoolTuning.GradeThresholdA - 0.01f;
                    school.LastPerfChange = 0;
                    return true;
                }
            }

            return false;
        }

        protected bool UpdatePerformance(Career career)
        {
            if (!ManagerCareer.ValidCareer(career)) return false;

            if (SimTypes.IsSelectable(career.OwnerDescription)) return false;

            Sim createdSim = career.OwnerDescription.CreatedSim;
            if ((createdSim != null) &&
                (!career.SpecialWorkDay) &&
                (!(createdSim.CurrentInteraction is Pregnancy.GoToHospital)))
            {
                if (HandleForeigners(career as School))
                {
                    IncStat("Foreign Limited");
                    return false;
                }

                float perfPerHour = 0f;

                DateAndTime time = SimClock.CurrentTime();
                if (career.IsAtWork && (SimClock.IsTimeBetweenTimes(time.Hour, career.CurLevel.StartTime, career.CurLevel.FinishTime() + career.OvertimeHours) || career.IsSpecialWorkTime))
                {
                    if (!career.ShouldBeAtWork() || career.IsSpecialWorkTime)
                    {
                        perfPerHour = Math.Max(perfPerHour, career.OvertimePerfPerHour);
                    }
                    else
                    {
                        float lastMetricAverageCalculated = 0f;
                        foreach (PerfMetric metric in career.CurLevel.Metrics)
                        {
                            lastMetricAverageCalculated += metric.CalcMetric(career);
                        }
                        lastMetricAverageCalculated /= career.CurLevel.Metrics.Count;
                        career.LastMetricAverageCalculated = lastMetricAverageCalculated;

                        AddScoring("Metric Calc 100s", (int)(lastMetricAverageCalculated * 100));

                        if (lastMetricAverageCalculated > 0f)
                        {
                            perfPerHour = (lastMetricAverageCalculated / 3f) * career.MaxPerfFlowPerHour;
                        }
                        else
                        {
                            perfPerHour = (lastMetricAverageCalculated / -3f) * career.MinPerfFlowPerHour;
                        }
                    }

                    perfPerHour += career.PerformanceBonusPerHour;
                    perfPerHour += career.OwnerDescription.TraitManager.HasElement(TraitNames.MultiTasker) ? TraitTuning.MultiTaskerWorkPerformanceAdd : 0f;

                    if (((createdSim != null) && (createdSim.BuffManager != null)) && createdSim.BuffManager.HasElement(BuffNames.MeditativeFocus))
                    {
                        perfPerHour += MartialArts.kMeditativeFocusCareerPerformanceModifier;
                    }
                }
                else if (career.IsMissingWork())
                {
                    InteractionInstance currentInteraction = createdSim.CurrentInteraction;
                    if (currentInteraction != null)
                    {
                        bool flag;
                        if (career is School)
                        {
                            flag = !(currentInteraction is GoToSchoolInRabbitHole);
                        }
                        else
                        {
                            flag = !(currentInteraction is WorkInRabbitHole);
                            if (flag)
                            {
                                EventTracker.SendEvent(EventTypeId.kNotAtWork, createdSim);
                            }
                        }

                        if (flag)
                        {
                            perfPerHour = career.MissWorkPerfPerHour * (career.OwnerDescription.TraitManager.HasElement(TraitNames.Vacationer) ? TraitTuning.VacationerMissingWorkPerformanceMultiplier : 1f);
                        }
                    }
                }
                else if ((career.PerformanceBonusPerHour != 0f) && !career.IsAllowedToWork())
                {
                    career.PerformanceBonusPerHour = 0f;
                }

                // Interval are 10 sim-minutes long
                perfPerHour *= GetValue<Option, int>();
                perfPerHour /= 6f;

                career.AddPerformance(perfPerHour);

                AddScoring("Perf Change 100s", (int)(perfPerHour * 100));
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new JobPerformanceScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerCareer, JobPerformanceScenario>, ManagerCareer.IPerformanceOption, IDebuggingOption
        {
            int mCount = 1;

            public Option()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "JobPerformanceInterval";
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (mCount >= Value)
                {
                    mCount = 1;
                    base.PrivateUpdate(fullUpdate, initialPass);
                }
                else
                {
                    mCount++;
                }
            }
        }
    }
}
