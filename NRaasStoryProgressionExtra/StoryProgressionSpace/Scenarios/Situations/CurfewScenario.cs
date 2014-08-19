using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
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
    public class CurfewScenario : ScheduledSoloScenario, IAlarmScenario
    {
        public CurfewScenario()
        { }
        protected CurfewScenario(CurfewScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "TeenCurfew";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDelayed(this, 0.5f, TimeUnit.Hours);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected static bool IsWitnessed(ManagerProgressionBase manager, IScoringGenerator stats, SimDescription sim, int baseChance)
        {
            if (sim.CreatedSim == null)
            {
                stats.IncStat("Hibernating");
                return false;
            }

            Lot lot = sim.CreatedSim.LotCurrent;
            if (lot == null) return false;

            if (lot.IsWorldLot) return false;

            if (lot.CanSimTreatAsHome(sim.CreatedSim))
            {
                stats.IncStat("At Home");
                return false;
            }

            /*
            if (lot.IsResidentialLot)
            {
                stats.IncStat("Residential");
                return false;
            }
            */

            foreach (Sim witness in lot.GetAllActors())
            {
                if (!witness.IsHuman) continue;

                if (witness.SimDescription.ChildOrBelow) continue;

                if (stats.AddScoring("IgnoreCurfew", witness.SimDescription) > 0) continue;

                if (!RandomUtil.RandomChance(stats.AddScoring("CurfewWitness", witness.SimDescription) + baseChance)) continue;

                bool forceCurfew;
                if (IsUnderCurfew(manager, stats, witness.SimDescription, out forceCurfew)) continue;

                stats.IncStat("Witnessed");
                return true;
            }

            return false;
        }

        protected static bool IsSupervised(Common.IStatGenerator stats, Sim ths)
        {
            if (ths.SimDescription.YoungAdultOrAbove)
            {
                stats.IncStat("Supervised: Too Old");
                return true;
            }

            if (ths.CareerManager != null)
            {
                Career occupationAsCareer = ths.CareerManager.OccupationAsCareer;
                if ((occupationAsCareer != null) && (occupationAsCareer.ShouldBeAtWork()) && (occupationAsCareer.CurLevel.IsSupervised))
                {
                    stats.IncStat("Supervised: Career");
                    return true;
                }
            }

            if ((ths.LotCurrent == null) || (ths.LotCurrent.CanSimTreatAsHome(ths)) || (ths.HasBeenAskedToSleepOver()))
            {
                stats.IncStat("Supervised: Home or Sleepover");
                return true;
            }

            if ((ths.CurrentInteraction is GoHome) || (((ths.CurrentInteraction != null) && (ths.CurrentInteraction.Target != null)) && (ths.CurrentInteraction.Target.LotCurrent != null) && (ths.CurrentInteraction.Target.LotCurrent.CanSimTreatAsHome(ths))))
            {
                stats.IncStat("Supervised: Going Home");
                return true;
            }

            if (ths.LotCurrent.IsWorldLot)
            {
                if (ths.Household != null)
                {
                    float distanceTeenIsSupervisedOnWorldLot = Sim.kDistanceTeenIsSupervisedOnWorldLot;
                    if (ths.SimDescription.Age == CASAgeGenderFlags.Child)
                    {
                        distanceTeenIsSupervisedOnWorldLot = Sim.kChildCurfewDistanceFromAdult;
                    }

                    foreach (Sim sim in ths.Household.Sims)
                    {
                        if (sim.SimDescription.YoungAdultOrAbove && (ths.GetDistanceToObject(sim) <= distanceTeenIsSupervisedOnWorldLot))
                        {
                            stats.IncStat("Supervised: World Lot Sim");
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (Sim sim2 in ths.LotCurrent.GetSims())
                {
                    if (sim2.SimDescription.YoungAdultOrAbove && (sim2.LotHome == ths.LotHome))
                    {
                        stats.IncStat("Supervised: Family Sim");
                        return true;
                    }
                }
            }

            OccultImaginaryFriend friend;
            return ((GameUtils.IsInstalled(ProductVersion.EP4) && OccultImaginaryFriend.TryGetOccultFromSim(ths, out friend)) && !friend.IsReal);
        }

        protected static bool IsUnderCurfew(ManagerProgressionBase manager, Common.IStatGenerator stats, SimDescription sim, out bool forceCurfew)
        {
            forceCurfew = false;

            if (!sim.IsHuman)
            {
                stats.IncStat("Not Human");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                stats.IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.RoutingComponent == null)
            {
                stats.IncStat("No Routing Manager");
                return false;
            }
            else if (sim.CreatedSim.RoutingComponent.RoutingParent != null)
            {
                stats.IncStat("Routing Parent");
                return false;
            }
            else if (sim.CreatedSim.GetSituationOfType<TrickOrTreatSituation>() != null)
            {
                stats.IncStat("Trick Or Treat");
                return false;
            }

            Lot currentLot = sim.CreatedSim.LotCurrent;
            if (currentLot == null) return false;

            if (currentLot.CanSimTreatAsHome(sim.CreatedSim))
            {
                stats.IncStat("At Home");
                return false;
            }

            int startCurfew = manager.GetValue<CurfewStartOption, int>(sim);
            int endCurfew = manager.GetValue<CurfewEndOption, int>(sim);

            if ((startCurfew < 0) || (endCurfew < 0))
            {
                stats.IncStat("User Curfew Fail");
                return false;
            }
            else if (startCurfew != endCurfew)
            {
                if (SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, startCurfew, endCurfew))
                {
                    if (manager.GetValue<ForceCurfewOption, bool>(sim))
                    {
                        forceCurfew = true;

                        stats.IncStat("User Defined Curfew");
                        return true;
                    }
                }
                else
                {
                    stats.IncStat("Not Curfew Time");
                    return false;
                }
            }

            if (!sim.TeenOrBelow)
            {
                stats.IncStat("Adult Curfew");
                return false;
            }

            if (startCurfew == endCurfew)
            {
                if (sim.ChildOrBelow)
                {
                    if (!EACurfewRetention.ChildCurfewIsInEffect())
                    {
                        stats.IncStat("Not Child EA Curfew");
                        return false;
                    }
                }
                else
                {
                    if (!EACurfewRetention.TeenCurfewIsInEffect())
                    {
                        stats.IncStat("Not Teen EA Curfew");
                        return false;
                    }
                }
            }

            if (IsSupervised(stats, sim.CreatedSim))
            {
                stats.IncStat("Supervised");
                return false;
            }

            DaysOfTheWeek dayToCheck = SimClock.Yesterday;
            if (SimClock.Hours24 > 12)
            {
                dayToCheck = SimClock.CurrentDayOfWeek;
            }

            if (!manager.HasValue<ValidDaysOption, DaysOfTheWeek>(dayToCheck))
            {
                stats.IncStat("Not Valid Day");
                return false;
            }

            if (!manager.GetValue<EnforceCurfewForJobOption, bool>())
            {
                stats.IncStat("For Job Disabled");
                return true;
            }
            else
            {
                Occupation career = sim.Occupation;
                if ((career != null) && (!career.HasOpenHours) && (career.IsFollowingDayInTheWorkDayList(dayToCheck)))
                {
                    stats.IncStat("Work Day");
                    return true;
                }

                School school = sim.CareerManager.School;
                if ((school != null) && (school.IsFollowingDayInTheWorkDayList(dayToCheck)))
                {
                    stats.IncStat("School Day");
                    return true;
                }

                stats.IncStat("For Job Allowed");
                return false;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int baseChance = GetValue<WitnessBaseChanceOption, int>();

            foreach (SimDescription sim in Sims.All)
            {
                bool forceCurfew;
                if (!IsUnderCurfew(this, this, sim, out forceCurfew))
                {
                    if (sim.CreatedSim != null)
                    {
                        sim.CreatedSim.BuffManager.RemoveElement(BuffNames.OutAfterCurfew);
                    }
                    continue;
                }

                if ((sim.CreatedSim != null) && (sim.TeenOrBelow))
                {
                    if (AddScoring("IgnoreCurfew", sim) > 0)
                    {
                        if (!sim.CreatedSim.BuffManager.HasElement(BuffNames.OutAfterCurfew))
                        {
                            sim.CreatedSim.BuffManager.AddElement(BuffNames.OutAfterCurfew, Origin.FromBreakingCurfew);
                            sim.CreatedSim.BuffManager.AddElement(BuffNames.FightThePower, Origin.FromBreakingCurfew);

                            IncStat("Added Buff");
                        }

                        EventTracker.SendEvent(EventTypeId.kTeenBreakingCurfew, sim.CreatedSim);
                    }
                    else
                    {
                        forceCurfew = true;
                    }
                }

                if ((!forceCurfew) && (!IsWitnessed(this, this, sim, baseChance)))
                {
                    IncStat("Not Witnessed");
                    continue;
                }

                if ((!forceCurfew) && (SimTypes.IsSelectable(sim)))
                {
                    Police instance = Police.Instance;
                    if (instance != null)
                    {
                        float chance = sim.HasTrait(TraitNames.Rebellious) ? Police.kChanceOfPoliceSpawnAfterCurfewRebellious : Police.kChanceOfPoliceSpawnAfterCurfew;
                        if (RandomUtil.RandomChance01(chance))
                        {
                            if ((!instance.IsAnySimActiveOnLot(sim.CreatedSim.LotCurrent)) && (!PoliceSituation.IsTeenBeingCaught(sim.CreatedSim)))
                            {
                                IncStat("Curfew Police");

                                instance.MakeServiceRequest(sim.CreatedSim.LotCurrent, true, sim.CreatedSim.ObjectId);
                            }
                        }
                        else
                        {
                            IncStat("Curfew Police Chance Fail");
                        }
                    }
                }
                else
                {
                    Situations.PushGoHome(this, sim);
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new CurfewScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerSituation, CurfewScenario>, IDebuggingOption
        {
            static AlarmHandle sMyHandle = AlarmHandle.kInvalidHandle;

            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Curfew";
            }

            protected static void OnDoNothing()
            { }

            protected static void OnReplaceAlarm()
            {
                try
                {
                    if ((SimUpdate.sCurfewCheckPoliceAlarmHandle != AlarmHandle.kInvalidHandle) && 
                        (SimUpdate.sCurfewCheckPoliceAlarmHandle != sMyHandle))
                    {
                        AlarmManager.Global.RemoveAlarm(SimUpdate.sCurfewCheckPoliceAlarmHandle);

                        SimUpdate.sCurfewCheckPoliceAlarmHandle = AlarmManager.Global.AddAlarmRepeating(1, TimeUnit.Days, OnDoNothing, 1, TimeUnit.Days, "Police curfew check", AlarmType.NeverPersisted, null);

                        sMyHandle = SimUpdate.sCurfewCheckPoliceAlarmHandle;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("OnReplaceAlarm", e);
                }
            }

            public override bool Install(ManagerSituation main, bool initial)
            {
                Sim.kChildCurfewDistanceFromAdult = float.MaxValue;

                AlarmManager.Global.AddAlarmRepeating(10, TimeUnit.Minutes, OnReplaceAlarm, 10, TimeUnit.Minutes, "Replace Curfew", AlarmType.NeverPersisted, null);

                return base.Install(main, initial);
            }
        }

        public class EnforceCurfewForJobOption : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public EnforceCurfewForJobOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CurfewByJob";
            }
        }

        public class WitnessBaseChanceOption : IntegerManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public WitnessBaseChanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "CurfewWitnessBaseChance";
            }
        }

        public class ValidDaysOption : DaysManagerOptionItem<ManagerSituation>, ManagerSituation.IGradPromCurfewOption
        {
            public ValidDaysOption()
                : base(DaysOfTheWeek.Monday | DaysOfTheWeek.Tuesday | DaysOfTheWeek.Wednesday | DaysOfTheWeek.Thursday | DaysOfTheWeek.Friday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Sunday)
            { }

            public override string GetTitlePrefix()
            {
                return "TeenCurfewValidDays";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class EACurfewRetention : Common.IPreLoad
        {
            public static float sChildStartCurfewHour;
            public static float sChildEndCurfewHour;

            public static float sTeenStartCurfewHour;
            public static float sTeenEndCurfewHour;

            public void OnPreLoad()
            {
                sTeenStartCurfewHour = Sim.kTeenStartCurfewHour;
                sTeenEndCurfewHour = Sim.kTeenEndCurfewHour;
                sChildStartCurfewHour = Sim.kChildStartCurfewHour;
                sChildEndCurfewHour = Sim.kChildEndCurfewHour;

                // breaks parties without MC Integration
                //Sim.kTeenStartCurfewHour = -2;
                //Sim.kTeenEndCurfewHour = -1;

                //Sim.kChildStartCurfewHour = -2;
                //Sim.kChildEndCurfewHour = -1;
            }

            public static bool ChildCurfewIsInEffect()
            {
                return SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, sChildStartCurfewHour, sChildEndCurfewHour);
            }

            public static bool TeenCurfewIsInEffect()
            {
                return (!GameUtils.IsOnVacation() && SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, sTeenStartCurfewHour, sTeenEndCurfewHour));
            }
        }
    }
}
