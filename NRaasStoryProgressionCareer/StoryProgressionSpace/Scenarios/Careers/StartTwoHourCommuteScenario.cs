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
    public abstract class StartTwoHourCommuteScenario : CareerScenario
    {
        public StartTwoHourCommuteScenario(SimDescription sim)
            : base(sim)
        { }
        protected StartTwoHourCommuteScenario(StartTwoHourCommuteScenario scenario)
            : base (scenario)
        { }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (Sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else
            {
                InteractionInstance instance = null;

                try
                {
                    instance = CareerPushScenario.GetWorkInteraction(Job);
                }
                catch (Exception e)
                {
                    Common.DebugException(Sim, e);
                }

                if (instance == null)
                {
                    IncStat("No Interaction");
                    return false;
                }
                else if (Situations.HasInteraction(Sim.CreatedSim, instance.InteractionDefinition, true))
                {
                    IncStat("At Work");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career job = Job;

            if (job.mRegularWorkDayTwoHoursBeforeStartHandle != AlarmHandle.kInvalidHandle)
            {
                AlarmManager.Global.RemoveAlarm(job.mRegularWorkDayTwoHoursBeforeStartHandle);
                job.mRegularWorkDayTwoHoursBeforeStartHandle = AlarmHandle.kInvalidHandle;
                IncStat("EA TwoHour Disabled");
            }

            if (job.mRegularWorkDayGoToWorkHandle != AlarmHandle.kInvalidHandle)
            {
                AlarmManager.Global.RemoveAlarm(job.mRegularWorkDayGoToWorkHandle);
                job.mRegularWorkDayGoToWorkHandle = AlarmHandle.kInvalidHandle;
                IncStat("EA Commute Disabled");
            }
            return false;
        }

        protected abstract class SetAlarmScenario : CareerScenario
        {
            public SetAlarmScenario(SimDescription sim)
                : base (sim)
            { }
            protected SetAlarmScenario(SetAlarmScenario scenario)
                : base(scenario)
            { }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected bool CommuteAlarmRunning()
            {
                foreach (ICareerAlarmSimData data in GetData(Sim).GetList<ICareerAlarmSimData>())
                {
                    if (data.Valid) return true;
                }

                return false;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (CommuteAlarmRunning())
                {
                    IncStat("Running");
                    return false;
                }

                DateAndTime queryTime = SimClock.CurrentTime();
                queryTime.Ticks += SimClock.ConvertToTicks(3f, TimeUnit.Hours);
                if (!Job.IsWorkHour(queryTime))
                {
                    IncStat("Too Early");
                    return false;
                }

                return base.Allow(sim);
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerSituation>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HandleCommute";
            }
        }
    }
}
