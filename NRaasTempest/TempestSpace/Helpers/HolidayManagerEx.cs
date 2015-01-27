using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class HolidayManagerEx : Common.IDelayedWorldLoadFinished, Common.IWorldQuit
    {
        static Common.AlarmTask sHolidayAlarm = null;

        public void OnDelayedWorldLoadFinished()
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP8) || !SeasonsManager.Enabled) return;

            if (HolidayManager.sInstance == null) return;

            if (HolidayManager.sInstance.mSeasonStartedListener != null)
            {
                EventTracker.RemoveListener(HolidayManager.sInstance.mSeasonStartedListener);
                HolidayManager.sInstance.mSeasonStartedListener = null;
            }
            
            new Common.DelayedEventListener(EventTypeId.kSeasonTransition, OnSeasonStarted);

            if (HolidayManager.sInstance.mCurrentSeasonLengthChanged != null)
            {
                EventTracker.RemoveListener(HolidayManager.sInstance.mCurrentSeasonLengthChanged);
                HolidayManager.sInstance.mCurrentSeasonLengthChanged = null;
            }

            new Common.DelayedEventListener(EventTypeId.kCurrentSeasonLengthChanged, OnSeasonLengthChanged);

            new Common.AlarmTask(23.99f, DaysOfTheWeek.All, OnNewDay);

            SetUpHolidayManager(SeasonsManager.CurrentSeason, false);
        }

        public void OnWorldQuit()
        {
            if (sHolidayAlarm != null)
            {
                sHolidayAlarm.Dispose();
                sHolidayAlarm = null;
            }
        }

        private static void OnSeasonStarted(Event e)
        {
            SeasonTransitionEvent event2 = e as SeasonTransitionEvent;
            if (event2 != null)
            {
                SetUpHolidayManager(event2.CurrentSeason, true);
            }
        }

        private static void OnSeasonLengthChanged(Event e)
        {
            if (GameStates.IsInWorld())
            {
                SetUpHolidayManager(SeasonsManager.CurrentSeason, true);
            }
        }

        // This alarm is fired at 23.99 on the previous day
        protected static void OnNewDay()
        {
            HolidayManager ths = HolidayManager.sInstance;
            if (ths != null) 
            {
                // Clean up in case the previous holiday alarm is munched
                ths.OnHolidayDone();
            }

            SetUpHolidayManager(false);

            if (SeasonsManager.Enabled)
            {
                if (Tempest.Settings.GetHolidays(SeasonsManager.CurrentSeason).mNoSchool)
                {
                    foreach (SimDescription sim in Household.AllSimsLivingInWorld())
                    {
                        if (sim.CareerManager == null) continue;

                        School school = sim.CareerManager.School;
                        if (school == null) continue;

                        if (!school.IsWorkDay) continue;

                        school.TakePaidTimeOff(1);
                    }
                }
            }
        }

        public static void SetUpHolidayManager(bool clearHoliday)
        {
            if (SeasonsManager.Enabled)
            {
                SetUpHolidayManager(SeasonsManager.CurrentSeason, clearHoliday);
            }
        }
        private static void SetUpHolidayManager(Season currentSeason, bool clear)
        {
            HolidayManager ths = HolidayManager.sInstance;
            if (ths == null) return;

            if (SeasonsManager.sInstance == null) return;

            if (clear)
            {
                ths.ClearCurrentHoliday();
            }

            if (SeasonsManager.GetSeasonEnabled(currentSeason))
            {
                if (ths.mStartDateTimeOfHoliday == DateAndTime.Invalid)
                {
                    ths.OnHolidayDone();

                    CalculateDateTimeOfHoliday(ths, ref currentSeason);

                    if (ths.mStartDateTimeOfHoliday != DateAndTime.Invalid)
                    {
                        ths.mCurrentSeason = currentSeason;
                        ths.SetHoliday();
                    }

                    if (TrickOrTreatSituation.NPCTrickOrTreatAlarm != AlarmHandle.kInvalidHandle)
                    {
                        AlarmManager.Global.RemoveAlarm(TrickOrTreatSituation.NPCTrickOrTreatAlarm);
                        TrickOrTreatSituation.NPCTrickOrTreatAlarm = AlarmHandle.kInvalidHandle;
                    }
                }

                if (ths.mStartDateTimeOfHoliday != DateAndTime.Invalid)
                {
                    DateAndTime time = SimClock.CurrentTime();
                    long ticks = (ths.mStartDateTimeOfHoliday - time).Ticks;

                    Common.DebugNotify("Initiate Holiday Alarm: " + Common.NewLine + SimClock.CurrentTime() + Common.NewLine + ths.mStartDateTimeOfHoliday + Common.NewLine + ticks);

                    if (ticks > 0)
                    {
                        ths.ClearAllAlarms();
                        //ths.StartAlarms();

                        if (sHolidayAlarm != null)
                        {
                            sHolidayAlarm.Dispose();
                        }

                        sHolidayAlarm = new Common.AlarmTask(SimClock.ConvertFromTicks(ticks, TimeUnit.Hours), TimeUnit.Hours, OnHolidayStarted);

                        long ticksToWarning = ticks - SimClock.ConvertToTicks((float)HolidayManager.kDaysBeforeHolidayToShowWarningTNS, TimeUnit.Days);
                        if (ticksToWarning > 0)
                        {
                            ths.mShowWarningAlarm = AlarmManager.Global.AddAlarm(SimClock.ConvertFromTicks(ticksToWarning, TimeUnit.Hours), TimeUnit.Hours, ths.OnShowWarningTns, "ShowWarningTns", AlarmType.AlwaysPersisted, null);
                        }
                    }
                }
            }
        }

        protected static void OnHolidayStarted()
        {
            Common.DebugNotify("OnHolidayStarted");

            HolidayManager ths = HolidayManager.sInstance;
            if (ths == null) return;

            if (SeasonsManager.sInstance == null) return;

            ths.OnHolidayStarted();

            if (!Tempest.Settings.mAllowHolidayParties)
            {
                NpcSeasonalPartyManager.ClearData(true);
            }

            sHolidayAlarm.Dispose();
            sHolidayAlarm = null;
        }

        private static void CalculateDateTimeOfHoliday(HolidayManager ths, ref Season season)
        {
            DateAndTime startTime = SimClock.Subtract(SimClock.CurrentTime(), TimeUnit.Hours, SimClock.CurrentTime().Hour);
            startTime = SimClock.Subtract(startTime, TimeUnit.Days, Tempest.GetCurrentSeasonDay() - 1);

            List<Pair<Season, uint>> days = new List<Pair<Season, uint>>();

            HolidaySettings settings = Tempest.Settings.GetHolidays(season);

            Common.StringBuilder result = new Common.StringBuilder("Season: " + season);

            result += Common.NewLine + "CurrentTime: " + SimClock.CurrentTime();
            result += Common.NewLine + "StartTime: " + startTime;
            result += Common.NewLine + "ExpectedEndTime: " + SeasonsManager.ExpectedEndTime;
            result += Common.NewLine + "GetCurrentSeasonDay: " + Tempest.GetCurrentSeasonDay();
            result += Common.NewLine + SimClock.ElapsedTime(TimeUnit.Days, startTime);

            foreach (HolidaySettings.Holiday day in settings.Days)
            {                
                uint actualDay = day.GetActualDay(season);                
                if (actualDay == 0) continue;

                days.Add(new Pair<Season, uint>(day.mSeason, actualDay)); // was actualDay -1
            }

            days.Sort(SortByDay);

            foreach (Pair<Season, uint> day in days)
            {                
                ths.mStartDateTimeOfHoliday = SimClock.Add(startTime, TimeUnit.Days, day.Second);

                result += Common.NewLine + "Days: " + day.Second + " Time: " + ths.mStartDateTimeOfHoliday;

                if (ths.mStartDateTimeOfHoliday.Ticks < SimClock.CurrentTicks)
                {
                    ths.mStartDateTimeOfHoliday = DateAndTime.Invalid;
                }
                else
                {
                    result += Common.NewLine + " Success";

                    season = day.First;
                    break;
                }
            }

            Common.DebugNotify(result);
        }

        public static int SortByDay(Pair<Season, uint> l, Pair<Season, uint> r)
        {
            return l.Second.CompareTo(r.Second);
        }
    }
}
