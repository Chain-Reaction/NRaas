using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class PromScenario : ScheduledSoloScenario, IAlarmScenario
    {
        public PromScenario()
        { }
        protected PromScenario(PromScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Prom";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, PromSituation.kTimeOfEventAnnouncement);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (Household.ActiveHousehold == null) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (PromSituation.sInstance == null)
            {
                DateAndTime checkDay = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Days, PromSituation.kNumberOfDaysForAnnouncement);

                if (!HasValue<ValidDaysOption, DaysOfTheWeek>(checkDay.DayOfWeek))
                {
                    IncStat("Wrong Day: " + checkDay.DayOfWeek);
                    return false;
                }

                int elapsed = (SimClock.ElapsedCalendarDays() - GetValue<DayOfLastOption, int>());
                if (elapsed < GetValue<CooldownOption, int>())
                {
                    AddStat("Cooldown", elapsed);
                    return false;
                }

                if (!RandomUtil.RandomChance(GetValue<ChanceOption, int>()))
                {
                    IncStat("Chance Fail");
                    return false;
                }

                if (PromSituation.HasValidConditionsToThrow())
                {
                    IncStat("Created");

                    PromSituation.Create();

                    GetOption<DayOfLastOption>().SetValue(SimClock.ElapsedCalendarDays());

                    return true;
                }
                else
                {
                    IncStat("Not Satisfied");
                    return false;
                }
            }
            else if (PromSituation.GetDayOfScheduledProm() == SimClock.CurrentDayOfWeek)
            {
                if (GetValue<DayOffOption, bool>())
                {
                    foreach (SimDescription sim in HouseholdsEx.All(Household.ActiveHousehold))
                    {
                        Occupation career = sim.Occupation;
                        if (career != null)
                        {
                            float endHour = career.FinishTime;
                            if (endHour != -1)
                            {
                                if (endHour > PromSituation.kTimeOfEventStart)
                                {
                                    career.TakePaidTimeOff(1);

                                    IncStat("Time Off");
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new PromScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerSituation, PromScenario>, ManagerSituation.IGradPromCurfewOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Prom";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool Install(ManagerSituation main, bool initial)
            {
                PromSituation.Shutdown();
                PromSituation.sAlarm = AlarmHandle.kInvalidHandle;

                return base.Install(main, initial);
            }
        }

        public class ChanceOption : IntegerManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public ChanceOption()
                : base((int)(PromSituation.kChanceOfSchedulingProm * 100))
            { }

            public override string GetTitlePrefix()
            {
                return "PromChance";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class DayOfLastOption : IntegerManagerOptionItem<ManagerSituation>, IDebuggingOption, ManagerSituation.IGradPromCurfewOption
        {
            public DayOfLastOption()
                : base(-10)
            { }

            public override string GetTitlePrefix()
            {
                return "PromDayOfLast";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class CooldownOption : IntegerManagerOptionItem<ManagerSituation>, ICooldownOptionItem, ManagerSituation.IGradPromCurfewOption
        {
            public CooldownOption()
                : base((int)PromSituation.kCooldownTime)
            { }

            public override string GetTitlePrefix()
            {
                return "PromCooldown";
            }

            public bool AdjustsForAgeSpan
            {
                get { return true; }
            }

            public bool AdjustsForSpeed
            {
                get { return false; }
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class DayOffOption : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public DayOffOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PromDayOff";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class ValidDaysOption : DaysManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public ValidDaysOption()
                : base(DaysOfTheWeek.Friday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Sunday)
            { }

            public override string GetTitlePrefix()
            {
                return "PromValidDays";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
