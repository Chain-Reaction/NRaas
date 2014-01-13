using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class HouseholdEx : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new AlarmTask();
        }

        public static void OneDayPassed(Household ths)
        {
            Common.StringBuilder msg = new Common.StringBuilder("OneDayPassed");
            Traveler.InsanityWriteLog(msg);

            if ((!Traveler.Settings.mPauseTravel) && (Traveler.Settings.mTreatAsVacation) && (Household.ActiveHousehold == ths))
            {
                GameStates.CurrentDayOfTrip++;

                msg += Common.NewLine + GameStates.CurrentDayOfTrip;
                Traveler.InsanityWriteLog(msg);

                Common.DebugNotify("Traveler:AlarmTask:OnDayPassed " + GameStates.CurrentDayOfTrip + "/" + GameStates.TripLength);

                if (PlumbBob.SelectedActor != null)
                {
                    (Sims3.Gameplay.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimAgeChanged(PlumbBob.SelectedActor.ObjectId);
                }

                msg += Common.NewLine + "A";
                Traveler.InsanityWriteLog(msg);

                if (GameUtils.IsUniversityWorld())
                {
                    msg += Common.NewLine + "B";
                    Traveler.InsanityWriteLog(msg);

                    OneDayPassedUniversityUpdates(ths);
                }
                else if (GameUtils.IsFutureWorld())
                {
                    msg += Common.NewLine + "C";
                    Traveler.InsanityWriteLog(msg);

                    ths.OneDayPassedFutureUpdates();
                }
                else
                {
                    msg += Common.NewLine + "D";
                    Traveler.InsanityWriteLog(msg);

                    OneDayPassedVacationUpdates(ths);
                }

                msg += Common.NewLine + "E";
                Traveler.InsanityWriteLog(msg);
            }
            else
            {
                Common.DebugNotify("Traveler:AlarmTask:OnDayPassed Paused");
            }
        }

        private static void ReturnFromVacationWorld()
        {
            List<Sim> travelers = new List<Sim>();
            foreach (Sim sim in GameStates.TravelHousehold.AllActors)
            {
                if (!Household.RoommateManager.IsNPCRoommate(sim))
                {
                    travelers.Add(sim);
                }
            }

            List<Sim> roommateSims = GameStates.TravelHousehold.GetRoommateSims();

            TravelUtil.TriggerTravelHomeFromUniversity(travelers, roommateSims);
        }

        private static void BeginReturnFromUniversityFlow()
        {
            new Common.AlarmTask(2, TimeUnit.Minutes, ReturnFromVacationWorld);

            Traveler.SaveGame();
        }

        private static void OneDayPassedUniversityUpdates(Household ths)
        {
            try
            {
                if (GameStates.CurrentDayOfTrip == GameStates.TripLength)
                {
                    Household.ReportCardHelper helper = null;
                    foreach (SimDescription description in Households.All(ths))
                    {
                        if (description == null) continue;

                        if (description.OccupationAsAcademicCareer == null) continue;

                        if (helper == null)
                        {
                            helper = new Household.ReportCardHelper();
                        }

                        helper.AddGPA(description.SimDescriptionId, description.OccupationAsAcademicCareer.GetGradeAsLetter());
                    }

                    if (helper != null)
                    {
                        AlarmManager.Global.AddAlarm(SimClock.HoursUntil(Household.kWhenReportCardArrives), TimeUnit.Hours, helper.ReportCardArrives, "Report Card Arrives", AlarmType.AlwaysPersisted, ths);
                    }

                    // Correction for potential error in OnTermCompleted()
                    Corrections.CleanupAcademics(null);

                    foreach (Household household in Household.sHouseholdList)
                    {
                        if (household == null) continue;

                        if (household.IsPreviousTravelerHousehold) continue;

                        foreach (SimDescription sim in household.SimDescriptions)
                        {
                            if (sim == null) continue;

                            try
                            {
                                AcademicCareer occupationAsAcademicCareer = sim.OccupationAsAcademicCareer;
                                if (occupationAsAcademicCareer != null)
                                {
                                    AcademicCareerEx.OnTermCompleted(occupationAsAcademicCareer);
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(sim, e);
                            }
                        }
                    }

                    AlarmManager.Global.RemoveAlarm(ths.mLastDayAlarm);
                    ths.mLastDayAlarm = AlarmManager.Global.AddAlarm(SimClock.HoursUntil(Household.kWhenOneHourLeftTNSAppears), TimeUnit.Hours, ths.OneHourLeft, "One Hour left TNS", AlarmType.AlwaysPersisted, ths);

                    AlarmManager.Global.RemoveAlarm(ths.mTriggerUniversityReturnFlowAlarm);
                    ths.mTriggerUniversityReturnFlowAlarm = AlarmManager.Global.AddAlarm(SimClock.HoursUntil(Household.kWhenSimsStartLeaving), TimeUnit.Hours, BeginReturnFromUniversityFlow, "Start University Return Home Flow Alarm", AlarmType.AlwaysPersisted, ths);
                }
                else if (GameStates.CurrentDayOfTrip > GameStates.TripLength)
                {
                    bool denyTravel = false;
                    if (!GameStates.IsTravelling)
                    {
                        foreach (Sim sim in ths.AllActors)
                        {
                            if ((sim != null) && sim.IsDying())
                            {
                                denyTravel = true;
                            }
                        }
                    }

                    GameStates.StopSnappingPicturesIfNeccessary();
                    if (Sims3.Gameplay.UI.Responder.Instance.HudModel is HudModel)
                    {
                        Common.FunctionTask.Perform(ths.ShowTripOverDialog);
                    }

                    if (!denyTravel)
                    {
                        Traveler.SaveGame();

                        TravelUtil.PlayerMadeTravelRequest = true;

                        // Custom
                        GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
                    }
                }
                else if (GameStates.CurrentDayOfTrip <= 0x1)
                {
                    AlarmManager.Global.RemoveAlarm(Household.mAtUnivTutorialAlarm);
                    Household.mAtUnivTutorialAlarm = AlarmHandle.kInvalidHandle;
                    AlarmManager.Global.RemoveAlarm(Household.mDormsTutorialAlarm);
                    Household.mDormsTutorialAlarm = AlarmHandle.kInvalidHandle;
                    Household.mAtUnivTutorialAlarm = AlarmManager.Global.AddAlarmDay(Household.kTimeAtUnivTutorial, ~DaysOfTheWeek.None, Household.TriggerAtUniversityTutorial, "At University Lesson", AlarmType.AlwaysPersisted, null);
                    Household.mDormsTutorialAlarm = AlarmManager.Global.AddAlarmDay(Household.kTimeDormsTutorial, ~DaysOfTheWeek.None, Household.TriggerDormsTutorial, "Dorms Lesson", AlarmType.AlwaysPersisted, null);
                }
                else if (GameStates.CurrentDayOfTrip == (GameStates.TripLength - 0x1))
                {
                    float num2 = SimClock.HoursUntil(Household.kWhenOneDayLeftTNSAppears);
                    ths.mLastDayAlarm = AlarmManager.Global.AddAlarm(num2, TimeUnit.Hours, ths.OneDayLeft, "One Day left TNS", AlarmType.AlwaysPersisted, ths);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths, e);
            }
        }

        private static void OneDayPassedVacationUpdates(Household ths)
        {
            if (GameStates.CurrentDayOfTrip == GameStates.TripLength)
            {
                if (!GameStates.IsTravelling)
                {
                    StyledNotification.Format format = new StyledNotification.Format(Common.LocalizeEAString("Gameplay/Vacation:OneDayLeft"), StyledNotification.NotificationStyle.kSystemMessage);
                    StyledNotification.Show(format);
                }

                ths.mLastDayAlarm = AlarmManager.Global.AddAlarm(SimClock.HoursUntil(12f), TimeUnit.Hours, ths.HalfDayLeft, "Half Day left TNS", AlarmType.AlwaysPersisted, ths);
            }
            else if (GameStates.CurrentDayOfTrip > GameStates.TripLength)
            {
                bool denyTravel = false;
                if (!GameStates.IsTravelling)
                {
                    foreach (Sim sim in Households.AllSims(ths))
                    {
                        sim.VisaManager.UpdateDaysSpentInWorld(GameUtils.GetCurrentWorld(), GameStates.CurrentDayOfTrip - 0x1);
                        if (sim.IsDying())
                        {
                            denyTravel = true;
                        }
                    }
                }

                if (!denyTravel)
                {
                    GameStates.StopSnappingPicturesIfNeccessary();
                    Sims3.Gameplay.UI.HudModel hudModel = Sims3.Gameplay.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel;
                    if (hudModel != null)
                    {
                        WorldName currentWorld = GameUtils.GetCurrentWorld();
                        string str = hudModel.LocationName(currentWorld);

                        SimpleMessageDialog.Show(TravelUtil.LocalizeString("TripOverCaption", new object[0x0]), TravelUtil.LocalizeString("TripOverText", new object[] { str }), ModalDialog.PauseMode.PauseSimulator);
                    }
                }

                if (!denyTravel)
                {
                    Traveler.SaveGame();

                    TravelUtil.PlayerMadeTravelRequest = true;

                    // Calls custom function
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
                }
            }
        }

        public class AlarmTask : Common.AlarmTask
        {
            public AlarmTask()
                : base(0, ~DaysOfTheWeek.None)
            { }

            protected override void OnPerform()
            {
                Household house = GameStates.TravelHousehold;
                if (house == null) return;

                HouseholdEx.OneDayPassed(house);
            }
        }
    }
}
