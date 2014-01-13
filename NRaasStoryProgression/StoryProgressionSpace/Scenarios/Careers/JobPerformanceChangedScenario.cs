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
    public class JobPerformanceChangedScenario : SimEventScenario<IncrementalEvent>
    {
        public JobPerformanceChangedScenario()
            : base ()
        { }
        protected JobPerformanceChangedScenario(JobPerformanceChangedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "JobPerformanceChanged";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Immediate; } // To ensure operation occurs prior clamping in Career:AddPerformance()
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kCareerPerformanceChanged);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (Sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if ((Sim.Occupation == null) && ((Sim.CareerManager == null) || (Sim.CareerManager.School == null)))
            {
                IncStat("Unnecessary");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career career = null;

            if (Sim.CreatedSim.CurrentInteraction is WorkInRabbitHole)
            {
                career = Sim.Occupation as Career;
            }
            else if (Sim.CreatedSim.CurrentInteraction is GoToSchoolInRabbitHole)
            {
                career = Sim.CareerManager.School;
            }

            if (career == null)
            {
                IncStat("No Career");
                return false;
            }

            if (!(career is School))
            {
                if (career.Performance >= 100)
                {
                    if (GetValue<PromoteOnTheJobOption, bool>())
                    {
                        IncStat("On The Job Promote");

                        // Don't call PromoteSim() it does not work for raises
                        career.PromoteIfShould();

                        career.ResetPerformance();
                        return true;
                    }
                }
                else if (career.Performance <= -100)
                {
                    if (GetValue<PromoteOnTheJobOption, bool>())
                    {
                        IncStat("On The Job Demote");

                        career.DemoteSim();

                        career.ResetPerformance();
                        return true;
                    }
                }
            }

            float adjustment = Event.Increment;

            if ((adjustment > 0) || ((adjustment < 0) && (GetValue<AlterDecreaseRateOption,bool>())))
            {
                string active = null;
                if (SimTypes.IsSelectable(Sim))
                {
                    if (!GetValue<HandleActiveOption, bool>())
                    {
                        IncStat("Active Denied");
                        return false;
                    }

                    active = "Active ";
                }

                if (!Careers.Allow(this, Sim))
                {
                    // Adjust the full value of the performance
                }
                else if ((!career.IsFinalLevel) || (!GetValue<SuspendRaisesAtMaxOption, bool>()))
                {
                    float scaling = GetValue<ScaledPerformanceOption, int>();
                    if (scaling <= 0)
                    {
                        scaling = 1;
                    }

                    if (GetValue<AdjustToAgeSpanOption, bool>())
                    {
                        scaling *= (LifeSpan.GetHumanAgeSpanLength() / 90f);
                    }

                    float stepped = GetValue<SteppedScaledPerformanceOption, int>() / 100f;
                    if (stepped > 0)
                    {
                        for (int i = 1; i < career.Level; i++)
                        {
                            scaling *= (1 + stepped);
                        }
                    }

                    AddScoring(active + "Scaling 100s", (int)(scaling * 100));

                    if (scaling == 0)
                    {
                        adjustment = 0;
                    }
                    else
                    {
                        adjustment -= (adjustment / scaling);
                    }

                    if ((Math.Abs(Event.Increment) >= 0.1) && (Math.Abs(Event.Increment - adjustment) < 0.1))
                    {
                        if (Event.Increment > 0)
                        {
                            adjustment = Event.Increment - 0.1f;
                        }
                        else
                        {
                            adjustment = Event.Increment + 0.1f;
                        }

                        IncStat("Performance Floor");
                    }
                }

                AddScoring(active + "Actual 100s", (int)(Event.Increment * 100));
                AddScoring(active + "Adjustment 100s", (int)(adjustment * 100));

                AddScoring(active + "Difference 100s", (int)((Event.Increment - adjustment) * 100));

                career.mPerformance -= adjustment;
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new JobPerformanceChangedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, JobPerformanceChangedScenario>, ManagerCareer.IPerformanceOption, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "JobPerformanceChanged";
            }
        }

        public class SuspendRaisesAtMaxOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public SuspendRaisesAtMaxOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "SuspendRaisesAtMax";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class PromoteOnTheJobOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public PromoteOnTheJobOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromoteOnTheJob";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AdjustToAgeSpanOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public AdjustToAgeSpanOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AdjustPerformanceToAgeSpan";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class HandleActiveOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public HandleActiveOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AdjustActivePerformance";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class ScaledPerformanceOption : IntegerManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public ScaledPerformanceOption()
                : base(1)
            { }

            public override string GetTitlePrefix()
            {
                return "JobDifficultyScale";
            }
        }

        public class SteppedScaledPerformanceOption : IntegerManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public SteppedScaledPerformanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "SteppedJobDifficultyScale";
            }
        }

        public class AlterDecreaseRateOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public AlterDecreaseRateOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AlterDecreaseRate";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
