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
using Sims3.Gameplay.Interfaces;
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
    public class GraduationScenario : ScheduledSoloScenario, IAlarmScenario
    {
        public GraduationScenario()
        { }
        protected GraduationScenario(GraduationScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Graduation";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 1);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (Household.ActiveHousehold == null) return false;

            if (!HasValue<ValidDaysOption, DaysOfTheWeek>(SimClock.CurrentDayOfWeek))
            {
                IncStat("Wrong Day");
                return false;
            }

            return base.Allow();
        }

        protected static void GiveDayOff(SimDescription sim)
        {
            Occupation career = sim.Occupation;
            if ((career != null) && (!career.HasOpenHours) && (career.IsWorkDay) && (!career.HasVacationOrLeave))
            {
                career.TakePaidTimeOff(1);
            }

            if (sim.CareerManager != null)
            {
                School school = sim.CareerManager.School;
                if ((school != null) && (!school.HasOpenHours) && (school.IsWorkDay) && (!school.HasVacationOrLeave))
                {
                    school.TakePaidTimeOff(1);
                }
            }
        }

        protected static void GiveDaysOff(StoryProgressionObject manager)
        {
            if (School.sGraduatingSims == null) return;

            foreach (SimDescription sim in School.sGraduatingSims.Keys)
            {
                GiveDayOff(sim);

                if ((!SimTypes.IsSelectable(sim)) || (manager.GetValue<DayOffOption, bool>()))
                {
                    foreach (SimDescription member in HouseholdsEx.All(sim.Household))
                    {
                        if (member == sim) continue;

                        GiveDayOff(member);
                    }
                }
            }
        }

        private static void StartGraduationCallback()
        {
            try
            {
                StoryProgression.Main.GetOption<ForceGradAlarmOption>().SetValue(false);

                Common.DebugNotify("StartGraduationCallback");

                if (School.sGraduatingSims != null)
                {
                    foreach (SimDescription description2 in new List<SimDescription>(School.sGraduatingSims.Keys))
                    {
                        School.GraduationInformation information = School.sGraduatingSims[description2];
                        if ((information == null) || (information.State == School.GraduationState.None))
                        {
                            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.Aging);
                            List<ICityHall> list2 = new List<ICityHall>(Sims3.Gameplay.Queries.GetObjects<ICityHall>());
                            if (list2.Count > 0x0)
                            {
                                RabbitHole target = list2[0x0] as RabbitHole;
                                if (target != null)
                                {
                                    Sim actor = description2.CreatedSim;
                                    if (actor == null)
                                    {
                                        actor = Instantiation.Perform(description2, target.LotCurrent, null);
                                    }

                                    if (actor != null)
                                    {
                                        School.sGraduatingSims[description2].State = School.GraduationState.EnRoute;
                                        School.GraduateInCityHall entry = School.GraduateInCityHall.Singleton.CreateInstance(target, actor, priority, true, true) as School.GraduateInCityHall;
                                        entry.MustRun = true;
                                        actor.InteractionQueue.Add(entry);
                                    }
                                    else
                                    {
                                        School.sGraduatingSims.Remove(description2);
                                    }
                                }
                            }
                            else
                            {
                                Sim sim3 = description2.CreatedSim;
                                if (sim3 == null)
                                {
                                    description2.GraduationType = School.sGraduatingSims[description2].IsValedictorian ? GraduationType.Valedictorian : GraduationType.Graduate;
                                    School.sGraduatingSims.Remove(description2);
                                }
                                else
                                {
                                    School.GraduateInPlace place = School.GraduateInPlace.Singleton.CreateInstance(sim3, sim3, priority, true, false) as School.GraduateInPlace;
                                    sim3.InteractionQueue.Add(place);
                                }
                            }
                        }
                    }

                    if (School.sGraduatingSims.Count == 0x0)
                    {
                        School.sGraduatingSims = null;
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("StartGraduationCallback", e);
            }
        }

        private static void ShowGraduationMessageCallback()
        {
            try
            {
                StoryProgression.Main.GetOption<GradMessageAlarmOption>().SetValue(false);

                Common.DebugNotify("ShowGraduationMessageCallback");

                if (School.sGraduatingSims != null)
                {
                    List<SimDescription> list = new List<SimDescription>(School.sGraduatingSims.Keys);

                    foreach (SimDescription desc in list)
                    {
                        if ((desc.CareerManager == null) || (School.sGraduatingSims[desc] == null))
                        {
                            School.sGraduatingSims.Remove(desc);
                        }
                    }

                    if (Sims3.Gameplay.Queries.GetObjects<ISchoolRabbitHole>().Length == 0x0)
                    {
                        foreach (SimDescription description in list)
                        {
                            description.GraduationType = GraduationType.NoSchool;
                        }
                    }
                    else
                    {
                        foreach (SimDescription description2 in list)
                        {
                            Sim createdSim = description2.CreatedSim;
                            if ((createdSim != null) /*&& createdSim.IsSelectable*/)
                            {
                                GiveDaysOff(StoryProgression.Main);

                                createdSim.ShowTNSIfSelectable(TNSNames.GraduationWarning, null, createdSim, new object[] { createdSim });

                                StoryProgression.Main.GetOption<ForceGradAlarmOption>().SetValue(true);

                                School.CalculateValedictorianAndRewards();
                                return;
                            }
                        }

                        foreach (SimDescription description3 in list)
                        {
                            description3.GraduationType = GraduationType.Graduate;
                            if (description3.CreatedSim != null)
                            {
                                description3.CreatedSim.SetDefaultGraduatedStateIfNeccessary();
                            }
                        }
                    }

                    School.sGraduatingSims = null;
                }
            }
            catch (Exception e)
            {
                Common.Exception("ShowGraduationMessageCallback", e);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<SimDescription> choices = new List<SimDescription>();

            bool active = false;

            foreach (SimDescription sim in Household.EveryHumanSimDescription())
            {
                if (SimTypes.IsService(sim)) continue;

                if (SimTypes.IsDead(sim)) continue;

                if (sim.YoungAdult)
                {
                    if (sim.GraduationType != GraduationType.None) continue;

                    choices.Add(sim);

                    if (SimTypes.IsSelectable(sim))
                    {
                        active = true;
                    }
                }
                else if (sim.AdultOrAbove)
                {
                    if (sim.GraduationType == GraduationType.None)
                    {
                        sim.GraduationType = GraduationType.Graduate;

                        if (sim.CreatedSim != null)
                        {
                            sim.CreatedSim.SetDefaultGraduatedStateIfNeccessary();
                        }
                    }
                }
            }

            if (choices.Count == 0)
            {
                IncStat("No choices");
                return false;
            }

            if (active)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("Graduation:Prompt", false, new object[] { choices.Count })))
                {
                    IncStat("User Cancelled");
                    return false;
                }
            }

            if (School.sGraduatingSims == null)
            {
                School.sGraduatingSims = new Dictionary<SimDescription, School.GraduationInformation>();
            }

            int maxPer = GetValue<MaxPerOption, int>();

            foreach (SimDescription sim in choices)
            {
                if ((!SimTypes.IsSelectable(sim)) && (School.sGraduatingSims.Count > maxPer)) continue;

                if (School.sGraduatingSims.ContainsKey(sim)) continue;

                School.sGraduatingSims.Add(sim, new School.GraduationInformation());
            }

            GetOption<GradMessageAlarmOption>().SetValue(true);

            return true;
        }

        public override Scenario Clone()
        {
            return new GraduationScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerSituation, GraduationScenario>, ManagerSituation.IGradPromCurfewOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Graduation";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override bool Install(ManagerSituation main, bool initial)
            {
                if (initial)
                {
                    AgingManager.kDaysBeforeGraduation = int.MaxValue;
                }

                return base.Install(main, initial);
            }
        }

        public class GradMessageAlarmOption : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption, IDebuggingOption
        {
            public GradMessageAlarmOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "GradMessageAlarm";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override void SetValue(bool value, bool persist)
            {
                base.SetValue(value, persist);

                if (value)
                {
                    float time = School.kHourToShowGraduationMessage - SimClock.HoursPassedOfDay;

                    AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, ShowGraduationMessageCallback, "Tell player that graduation is today", AlarmType.NeverPersisted, null);
                }
            }
        }

        public class ForceGradAlarmOption : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption, IDebuggingOption
        {
            public ForceGradAlarmOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ForceGradAlarm";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }

            public override void SetValue(bool value, bool persist)
            {
                base.SetValue(value, persist);

                if (value)
                {
                    float time = School.kHourToHoldGraduation - SimClock.HoursPassedOfDay;

                    AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, StartGraduationCallback, "Force graduation", AlarmType.NeverPersisted, null);
                }
            }
        }

        public class DayOffOption : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public DayOffOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "GraduationDayOff";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }
        }

        public class InactiveGraduationOption : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public InactiveGraduationOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "InactiveGraduation";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }
        }

        public class MaxPerOption : IntegerManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption, IDebuggingOption
        {
            public MaxPerOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "MaxPerGraduation";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP4);
            }
        }

        public class ValidDaysOption : DaysManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public ValidDaysOption()
                : base(DaysOfTheWeek.Friday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Sunday)
            { }

            public override string GetTitlePrefix()
            {
                return "GraduationValidDays";
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
