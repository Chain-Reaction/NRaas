using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class TwoHourCommuteScenario : CareerAlarmScenario
    {
        public TwoHourCommuteScenario(SimDescription sim)
            : base (sim)
        {}
        protected TwoHourCommuteScenario(TwoHourCommuteScenario scenario)
            : base (scenario)
        { }

        protected virtual float StaggerTime
        {
            get { return 0; }
        }

        protected override float GetTime()
        {
            Career job = Job;

            DateAndTime nowTime = SimClock.CurrentTime();

            AddStat("StartTime", job.CurLevel.StartTime);
            AddStat("NowTime", nowTime.Hour);
            AddStat("AvgTime", job.AverageTimeToReachWork);

            float oneHourAhead = (job.AverageTimeToReachWork + 1) + RandomUtil.GetFloat(StaggerTime);

            float time = ((job.CurLevel.StartTime - oneHourAhead) - nowTime.Hour);
            if (time < 0f)
            {
                time += 24f;
                if (time > oneHourAhead)
                {
                    time = 0f;
                }
            }

            return time;
        }

        protected static bool CarpoolEnabled(Occupation job)
        {
            return (job.mPickUpCarpool != null);
        }

        public static void CarpoolUpdate(Scenario paramScenario, ScenarioFrame frame)
        {
            CareerUpdateScenario scenario = paramScenario as CareerUpdateScenario;

            Occupation job = scenario.Job;

            if ((SimTypes.IsSelectable(scenario.Sim)) && 
                (scenario.GetValue<AllowCarpoolOption, bool>(scenario.Sim)) && 
                (!scenario.Lots.HasPersonalVehicle(scenario.Sim)))
            {
                CareerUpdateScenario.SchedulingSimData data = scenario.Scheduling;

                if (!data.mScheduled)
                {
                    job.RescheduleCarpool();
                    data.mScheduled = true;

                    scenario.IncStat("Scheduled");
                }
            }
            else if (CarpoolEnabled(job))
            {
                job.RemoveCarpool();
                scenario.Scheduling.mScheduled = false;

                if (SimTypes.IsSelectable(scenario.Sim))
                {
                    scenario.IncStat("Dropped Active");
                }
                else
                {
                    scenario.IncStat("Dropped Inactive");
                }
            }
        }

        protected abstract bool AllowHoliday(Season season);

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((SeasonsManager.Enabled) && (AllowHoliday(SeasonsManager.CurrentSeason)))
            {
                if (Job.TryGiveDayOffForHoliday())
                {
                    IncStat("Holiday");
                    return false;
                }
            }

            base.PrivateUpdate(frame);
            return true;
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
                if (sim.Household == null)
                {
                    IncStat("No Home");
                    return false;
                }
                else if (SimTypes.IsSpecial(sim))
                {
                    IncStat("Special");
                    return false;
                }
                else if (CommuteAlarmRunning ())
                {
                    IncStat("Running");
                    return false;
                }

                Career job = Job;

                DateAndTime queryTime = SimClock.CurrentTime();
                queryTime.Ticks += SimClock.ConvertToTicks(2f, TimeUnit.Hours);
                if (!job.IsWorkHour(queryTime))
                {
                    IncStat("Too Early");
                    return false;
                }

                return base.Allow(sim);
            }

            protected abstract CommuteScenario GetCommuteScenario(bool push);

            protected abstract string GetCarpoolMessage(bool selfCommute);

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                Career job = Job;

                if (Sim.CreatedSim == null)
                {
                    Sims.Instantiate(Sim, null, false);
                }

                if (Sim.CreatedSim == null)
                {
                    IncStat("Hibernating");
                    return false;
                }

                if ((job.mCalledInSick) || (job.mIsFakeSickDay))
                {
                    IncStat("Sick Day");
                    return false;
                }

                Careers.VerifyTone(job);

                bool selfCommute = false;

                if ((!GetValue<AllowCarpoolOption,bool>(Sim)) ||
                    (!CarpoolEnabled(job)) ||
                    (!job.CurLevel.HasCarpool) ||
                    (!SimTypes.IsSelectable (Sim)))
                {
                    if (!SimTypes.IsSelectable (Sim))
                    {
                        try
                        {
                            // Don't queue stomp on their birthday
                            if ((Sim.YearsSinceLastAgeTransition != 0) &&
                                (Sim.CreatedSim.InteractionQueue != null))
                            {
                                Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
                            }
                        }
                        catch (Exception e)
                        {
                            Common.DebugException(Sim, e);
                        }
                    }

                    if (SimTypes.IsSelectable (Sim))
                    {
                        IncStat("Active Alarm");
                    }
                    else
                    {
                        IncStat("Inactive Alarm");
                    }

                    selfCommute = true;
                }
                else
                {
                    IncStat("Carpool");
                }

                if ((SimTypes.IsSelectable(Sim)) && (GetValue<ShowCarpoolMessageOption, bool>()))
                {
                    string msg = GetCarpoolMessage(selfCommute);
                    if (msg != null)
                    {
                        StyledNotification.Format format = new StyledNotification.Format(msg, ObjectGuid.InvalidObjectGuid, Sim.CreatedSim.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                        StyledNotification.Show(format, job.CareerIconColored);
                    }
                }

                Manager.AddAlarm(GetCommuteScenario(selfCommute));

                return selfCommute;
            }
        }

        public class ShowCarpoolMessageOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public ShowCarpoolMessageOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowCarpoolMessage";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
