using NRaas.StoryProgressionSpace.Helpers;
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
using Sims3.Gameplay.Seasons;
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
    public class SchoolTwoHourCommuteScenario : TwoHourCommuteScenario
    {
        public SchoolTwoHourCommuteScenario(SimDescription sim)
            : base(sim)
        { }
        protected SchoolTwoHourCommuteScenario(SchoolTwoHourCommuteScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SchoolTwoHourCommute";
        }

        public override Career Job
        {
            get 
            {
                if (Sim.CareerManager == null) return null;

                return Sim.CareerManager.School; 
            }
        }

        protected override float StaggerTime
        {
            get { return 0.5f; }
        }

        protected override TwoHourCommuteScenario.AlarmSimData AlarmData
        {
            get { return GetData<AlarmSimData>(Sim); }
        }

        protected override bool AllowHoliday(Season season)
        {
            return HasValue<AllowSchoolHolidayOption, Season>(season);
        }

        protected static bool TryGiveDayOffForHoliday(Career job)
        {
            DateAndTime queryTime = SimClock.CurrentTime();
            queryTime.Ticks += SimClock.ConvertToTicks(2.1f, TimeUnit.Hours);
            if (job.IsWorkHour(queryTime))
            {
                //HolidayManager instance = HolidayManager.Instance;
                //if ((instance != null) && instance.IsThisDayAHoliday(queryTime))
                {
                    job.TakePaidTimeOff(1);
                    return true;
                }
            }
            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            if ((SeasonsManager.Enabled) && (SeasonsManager.CurrentSeason == Season.Summer))
            {
                if (HasValue<SummerHolidayOption, OccupationNames>(Job.Guid))
                {
                    if (HasValue<SummerHolidayDaysOption, DaysOfTheWeek>(SimClock.CurrentDayOfWeek))
                    {
                        if (TryGiveDayOffForHoliday(Job))
                        {
                            IncStat("Summer Holiday");
                            return false;
                        }
                    }
                    else
                    {
                        IncStat("Not Summer Off Day");
                    }
                }
                else
                {
                    IncStat("Not Summer Off");
                }
            }
            else
            {
                IncStat("Not Summer");
            }

            Add(frame, new SchoolPushScenario(Sim), ScenarioResult.Start);
            Add(frame, new SchoolSetAlarmScenario(Sim), ScenarioResult.Failure);
            return false;
        }

        public override Scenario Clone()
        {
            return new SchoolTwoHourCommuteScenario(this);
        }

        protected new class AlarmSimData : TwoHourCommuteScenario.AlarmSimData
        {
            public AlarmSimData()
            { }
        }

        protected class SchoolSetAlarmScenario : SetAlarmScenario
        {
            public SchoolSetAlarmScenario(SimDescription sim)
                : base (sim)
            { }
            protected SchoolSetAlarmScenario(SchoolSetAlarmScenario scenario)
                : base (scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "SchoolCommuteSetAlarm";
            }

            public override Career Job
            {
                get
                {
                    if (Sim.CareerManager == null) return null;

                    return Sim.CareerManager.School;
                }
            }

            protected override CommuteScenario GetCommuteScenario(bool push)
            {
                return new SchoolCommuteScenario(Sim, push);
            }

            protected override string GetCarpoolMessage(bool selfCommute)
            {
                Career job = Job;

                List<SimDescription> sims = new List<SimDescription>();
                if (Sim.Household != null)
                {
                    sims.AddRange(HouseholdsEx.All(Sim.Household));
                }

                foreach (SimDescription sim in sims)
                {
                    if (sim == Sim) break;

                    if ((sim.Child) || (sim.Teen))
                    {
                        if (sim.CareerManager == null) continue;

                        if (sim.CareerManager.School == null) continue;

                        if (sim.CareerManager.School.CareerLoc == job.CareerLoc)
                        {
                            return null;
                        }
                    }
                }

                if (selfCommute)
                {
                    return Common.Localize("SchoolPush:CommuteComing", Sim.IsFemale, new object[] { Sim });
                }
                else
                {
                    return Common.LocalizeEAString(Sim.IsFemale, "Gameplay/Objects/Vehicles/CarpoolManager:SchoolBusComing", new object[] { Sim });
                }
            }

            public override Scenario Clone()
            {
                return new SchoolSetAlarmScenario(this);
            }
        }

        public class SummerHolidayOption : OccupationManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public SummerHolidayOption()
                : base(new OccupationNames[0])
            { }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP8);
            }

            protected override bool Allow(OccupationNames value)
            {
                School school = CareerManager.GetStaticCareer(value) as School;
                
                return (school != null);
            }

            public override string GetTitlePrefix()
            {
                return "SummerHoliday";
            }
        }

        public class SummerHolidayDaysOption : DaysManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public SummerHolidayDaysOption()
                : base(sAllDays)
            { }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP8);
            }

            public override string GetTitlePrefix()
            {
                return "SummerHolidayDays";
            }
        }

        public class AllowSchoolHolidayOption : MultiEnumManagerOptionItem<ManagerCareer, Season>, ManagerCareer.ISchoolOption
        {
            public AllowSchoolHolidayOption()
                : base(new Season[] { Season.Spring, Season.Summer, Season.Fall, Season.Winter })
            { }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP8);
            }

            public override string GetTitlePrefix()
            {
                return "AllowSchoolHoliday";
            }
        }
    }
}
