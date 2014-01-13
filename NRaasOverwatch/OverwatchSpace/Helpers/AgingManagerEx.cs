using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Helpers
{
    public class AgingManagerEx
    {
        private static void CommonDayPassedUpdates(AgingManager ths, AgingState state, List<AgingState> statesToGraduate, ref bool silentGraduation, bool noSchools)
        {
            try
            {
                SimDescription simDescription = state.SimDescription;
                if (simDescription.UserDaysInCurrentAge < 0x7fffffff)
                {
                    simDescription.UserDaysInCurrentAge++;
                }

                // Custom
                //   Fix for midlife crisis moodlets left running on inactives
                if ((simDescription.CreatedSim != null) && (simDescription.CreatedSim.DreamsAndPromisesManager == null))
                {
                    BuffManager buffManager = simDescription.CreatedSim.BuffManager;
                    if (buffManager != null)
                    {
                        buffManager.RemoveElement(BuffNames.HavingAMidlifeCrisis);
                        buffManager.RemoveElement(BuffNames.HavingAMidlifeCrisisWithPromise);
                    }
                }

                MidlifeCrisisManager.OnDayPassed(simDescription);

                if (GameUtils.IsInstalled(ProductVersion.EP4))
                {
                    if ((simDescription.YoungAdult && (simDescription.UserDaysInCurrentAge == AgingManager.kDaysBeforeGraduation)) && (simDescription.GraduationType == GraduationType.None))
                    {
                        if (noSchools)
                        {
                            state.SimDescription.GraduationType = GraduationType.NoSchool;
                        }
                        else
                        {
                            state.ShouldBeGraduating = true;
                        }
                    }

                    if ((statesToGraduate != null) && state.ShouldBeGraduating)
                    {
                        statesToGraduate.Add(state);
                        if ((simDescription.CreatedSim != null) && simDescription.CreatedSim.IsSelectable)
                        {
                            silentGraduation = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(state.SimDescription, e);
            }
        }

        public static void DayPassedCallback(AgingManager ths)
        {
            List<AgingState> statesToGraduate = new List<AgingState>();
            bool silentGraduation = true;
            bool noSchools = Sims3.Gameplay.Queries.CountObjects<ISchoolRabbitHole>() == 0x0;
            if (ths.Enabled)
            {
                float hoursPassedOfDay = SimClock.HoursPassedOfDay;
                float agingYearsPassed = 1f / ths.SimDaysPerAgingYear;
                if (hoursPassedOfDay > AgingManager.kHourToShowBirthdayMessage)
                {
                    foreach (AgingState state in ths.AgingStates)
                    {
                        if (state != null)
                        {
                            state.AgingYearsPassedSinceLastTransition += agingYearsPassed;
                        }
                    }
                    return;
                }

                if (!GameUtils.IsFutureWorld())
                {
                    ths.AgeVacationWorldSims(agingYearsPassed);
                }

                foreach (AgingState state in ths.AgingStates)
                {
                    try
                    {
                        if (ths.IsAgingStateValid(state))
                        {
                            state.AgingYearsPassedSinceLastTransition += agingYearsPassed;
                            state.DayPassedSinceLastTransition = true;
                            Sim createdSim = state.SimDescription.CreatedSim;
                            if (createdSim != null)
                            {
                                Sims3.Gameplay.UI.HudModel hudModel = Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel;
                                if (hudModel != null)
                                {
                                    hudModel.OnSimAgeChanged(createdSim.ObjectId);
                                }
                            }

                            // Custom
                            CommonDayPassedUpdates(ths, state, statesToGraduate, ref silentGraduation, noSchools);

                            if (((state.AgeTransitionEarlyMessageAlarm != AlarmHandle.kInvalidHandle) || (state.AgeTransitionWithoutCakeAlarm != AlarmHandle.kInvalidHandle)) || (state.AgeTransitionMessageAlarm != AlarmHandle.kInvalidHandle))
                            {
                                float timeLeft = AlarmManager.Global.GetTimeLeft(state.AgeTransitionWithoutCakeAlarm, TimeUnit.Hours);
                                if ((timeLeft > 0f) && (timeLeft <= 24f))
                                {
                                    continue;
                                }
                                ths.CancelAgingAlarmsForSim(state);
                            }

                            IAlarmOwner simDescription = state.SimDescription;
                            if (ths.SimIsOldEnoughToTransition(state))
                            {
                                if (state.SimDescription.Age == CASAgeGenderFlags.Elder)
                                {
                                    EventTracker.SendEvent(new MiniSimDescriptionEvent(EventTypeId.kSimGettingOld, state.SimDescription));

                                    if (AgingStateEx.IsInactiveActive(state))
                                    {
                                        AgingStateEx.AgeTransitionWithoutCakeTask.Perform(state);
                                    }
                                    else
                                    {
                                        state.AgeTransitionWithoutCakeAlarm = AlarmManager.Global.AddAlarm(RandomUtil.GetFloat(12f, 24f), TimeUnit.Hours, state.AgeTransitionWithoutCakeCallback, "The Cake is a Lie and Then You Die", AlarmType.AlwaysPersisted, simDescription);
                                        AlarmManager.Global.AlarmWillYield(state.AgeTransitionWithoutCakeAlarm);
                                    }
                                }
                                else
                                {
                                    if (AgingStateEx.IsInactiveActive(state))
                                    {
                                        AgingStateEx.AgeTransitionWithoutCakeTask.Perform(state);
                                    }
                                    else
                                    {
                                        float time = AgingManager.kHourToShowBirthdayMessage - hoursPassedOfDay;
                                        float maxValue = float.MaxValue;
                                        if (createdSim != null)
                                        {
                                            maxValue = (24f + createdSim.HoursUntilWakeupTime) - AgingManager.kHourToAgeWithoutCake;
                                        }

                                        maxValue = Math.Min(maxValue, AgingManager.kLatestHourToAgeWithoutCake - hoursPassedOfDay);

                                        state.AgeTransitionWithoutCakeAlarm = AlarmManager.Global.AddAlarmRepeating(maxValue, TimeUnit.Hours, state.AgeTransitionWithoutCakeCallback, 15f, TimeUnit.Minutes, "The Cake is a Lie", AlarmType.AlwaysPersisted, simDescription);
                                        AlarmManager.Global.AlarmWillYield(state.AgeTransitionWithoutCakeAlarm);

                                        if (!state.SimDescription.IsEnrolledInBoardingSchool())
                                        {
                                            state.AgeTransitionMessageAlarm = AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, state.ShowAgeTransitionMessageCallback, "Tell Player that the Cake is a Lie", AlarmType.AlwaysPersisted, simDescription);
                                            AlarmManager.Global.AlarmWillYield(state.AgeTransitionMessageAlarm);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!state.SimDescription.Elder && ths.SimIsCloseToAging(state, AgingManager.kDaysEarlyToShowBirthdayMessage))
                                {
                                    float num6 = AgingManager.kHourToShowBirthdayMessage - hoursPassedOfDay;
                                    state.AgeTransitionEarlyMessageAlarm = AlarmManager.Global.AddAlarm(num6, TimeUnit.Hours, state.ShowAgeTransitionEarlyMessageCallback, "Tell player that the cake will be a lie in a few days", AlarmType.AlwaysPersisted, simDescription);
                                    AlarmManager.Global.AlarmWillYield(state.AgeTransitionEarlyMessageAlarm);
                                }

                                if (state.SimDescription.Teen && ths.SimIsCloseToAging(state, AgingManager.kDaysEarlyToShowGraduationMessage))
                                {
                                    float num7 = AgingManager.kHourToShowGraduationMessage - hoursPassedOfDay;
                                    state.GraduationEarlyMessageAlarm = AlarmManager.Global.AddAlarm(num7, TimeUnit.Hours, state.ShowGraduationEarlyMessageCallback, "Tell player that graduation will be in a few days", AlarmType.AlwaysPersisted, simDescription);
                                    AlarmManager.Global.AlarmWillYield(state.GraduationEarlyMessageAlarm);
                                }
                            }

                            if (((createdSim != null) && createdSim.IsSelectable) && (state.SimDescription.Teen && ths.SimIsCloseToAging(state, AgingManager.kDaysBeforeAgingToDelayGraduation)))
                            {
                                statesToGraduate = null;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(state.SimDescription, e);
                    }
                }
            }
            else
            {
                foreach (AgingState state3 in ths.AgingStates)
                {
                    if (ths.IsAgingStateValid(state3))
                    {
                        state3.DayPassedSinceLastTransition = true;

                        // Custom
                        CommonDayPassedUpdates(ths, state3, statesToGraduate, ref silentGraduation, noSchools);
                    }
                }
            }

            if ((statesToGraduate != null) && (statesToGraduate.Count > 0x0))
            {
                foreach (AgingState state4 in statesToGraduate)
                {
                    try
                    {
                        if (noSchools)
                        {
                            state4.SimDescription.GraduationType = GraduationType.NoSchool;
                        }
                        else if (silentGraduation)
                        {
                            state4.SimDescription.GraduationType = GraduationType.Graduate;
                            Sim sim2 = state4.SimDescription.CreatedSim;
                            if (sim2 != null)
                            {
                                sim2.SetDefaultGraduatedStateIfNeccessary();
                            }
                        }
                        else
                        {
                            School.GraduateSim(state4.SimDescription);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(state4.SimDescription, e);
                    }
                    finally
                    {
                        state4.ShouldBeGraduating = false;
                    }
                }
            }
        }
    }
}
