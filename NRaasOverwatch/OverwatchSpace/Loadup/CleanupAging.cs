using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupAging : DelayedLoadupOption
    {
        protected void RemoveDanglingAgeUpAlarms()
        {
            foreach (List<AlarmManager.Timer> list in AlarmManager.Global.mTimers.Values)
            {
                foreach (AlarmManager.Timer timer in list)
                {
                    if (timer.CallBack == null) continue;

                    if (timer.CallBack.Method.ToString().Contains("AgeTransitionWithoutCakeCallback"))
                    {
                        Overwatch.Log("Dropped AgeTransitionWithoutCakeCallback: " + timer.CallBack.Target);

                        List<AlarmHandle> list2;
                        IAlarmOwner objectRef = timer.ObjectRef;
                        if ((objectRef != null) && AlarmManager.Global.mGameObjectIndex.TryGetValue(objectRef, out list2))
                        {
                            if ((list2.Count == 1) && (list2[0] == timer.Handle))
                            {
                                AlarmManager.Global.mGameObjectIndex.Remove(objectRef);
                            }
                            else
                            {
                                list2.Remove(timer.Handle);
                            }
                        }
                        timer.Clear();
                    }
                }
            }
        }

        public static bool IsOldEnoughToAge(AgingManager manager, AgingState state)
        {
            if (state == null) return true;

            if (state.SimDescription.Elder)
            {
                if (state.AgingYearsPassedSinceLastTransition >= manager.SimDaysToAgingYears(state.MinimumElderLifeSpanInSimDays)) return true;
            }
            else
            {
                if (manager.SimIsCloseToAging(state, 0)) return true;
            }

            return false;
        }

        protected static void OnCheckAging()
        {
            AgingManager manager = AgingManager.Singleton;

            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);
            foreach (SimDescription sim in sims.Values)
            {
                if (IsOldEnoughToAge(manager, sim.AgingState)) continue;

                manager.CancelAgingAlarmsForSim(sim.AgingState);
            }
        }

        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupAging");

            new Common.AlarmTask(4, TimeUnit.Hours, OnCheckAging, 4, TimeUnit.Hours);

            Dictionary<SimDescription, AgingData> agingData = new Dictionary<SimDescription, AgingData>();

            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);
            foreach (SimDescription sim in sims.Values)
            {
                if (sim.AgingState != null)
                {
                    agingData.Add(sim, new AgingData(sim.AgingState));
                }

                AgingManager.Singleton.RemoveSimDescription(sim);
            }

            RemoveDanglingAgeUpAlarms();

            foreach (SimDescription sim in sims.Values)
            {
                if (sim.AgingEnabled)
                {
                    if ((SimTypes.InServicePool(sim, Sims3.Gameplay.Services.ServiceType.GrimReaper)) || (sim.IsRaccoon) || (sim.IsDeer))
                    {
                        sim.AgingEnabled = false;
                    }
                    else
                    {
                        AgingManager.Singleton.AddSimDescription(sim);

                        if ((sim.AgingState != null) && (agingData.ContainsKey(sim)))
                        {
                            agingData[sim].Reset(sim, sim.AgingState);
                        }
                    }
                }
                else
                {
                    if (sim.AgingState == null)
                    {
                        sim.AgingState = new AgingState(sim);
                    }
                }
            }
            /*
            if (!GameStates.IsOnVacation)
            {
                foreach (MiniSimDescription description in MiniSimDescription.GetVacationWorldSimDescriptions())
                {
                    if (description.mDeathStyle != SimDescription.DeathType.None) continue;

                    description.mbAgingEnabled = true;
                }
            }*/
        }

        protected class AgingData
        {
            bool mDayPassed = false;
            float mWithoutCake = 0;
            float mMessage = 0;
            float mEarlyMessage = 0;

            public AgingData(AgingState state)
            {
                mDayPassed = state.HasDayPassedSinceLastTransition;

                if (state.AgeTransitionWithoutCakeAlarm != AlarmHandle.kInvalidHandle)
                {
                    mWithoutCake = AlarmManager.Global.GetTimeLeft(state.AgeTransitionWithoutCakeAlarm, TimeUnit.Hours);
                }

                if (state.AgeTransitionMessageAlarm != AlarmHandle.kInvalidHandle)
                {
                    mMessage = AlarmManager.Global.GetTimeLeft(state.AgeTransitionMessageAlarm, TimeUnit.Hours);
                }

                if (state.AgeTransitionEarlyMessageAlarm != AlarmHandle.kInvalidHandle)
                {
                    mEarlyMessage = AlarmManager.Global.GetTimeLeft(state.AgeTransitionEarlyMessageAlarm, TimeUnit.Hours);
                }
            }

            public void Reset(SimDescription sim, AgingState state)
            {
                state.DayPassedSinceLastTransition = mDayPassed;

                if (IsOldEnoughToAge(AgingManager.Singleton, state))
                {
                    if (mWithoutCake != 0)
                    {
                        state.AgeTransitionWithoutCakeAlarm = AlarmManager.Global.AddAlarm(mWithoutCake, TimeUnit.Hours, state.AgeTransitionWithoutCakeCallback, "The Cake is a Lie and Then You Die", AlarmType.AlwaysPersisted, sim);

                        Overwatch.Log("Age-up: " + state.SimDescription.FullName);
                    }

                    if (mMessage != 0)
                    {
                        state.AgeTransitionMessageAlarm = AlarmManager.Global.AddAlarm(mMessage, TimeUnit.Hours, state.ShowAgeTransitionMessageCallback, "Tell Player that the Cake is a Lie", AlarmType.AlwaysPersisted, sim);
                    }
                }
                else
                {
                    if (mWithoutCake != 0)
                    {
                        Overwatch.Log("Age-up Dropped: " + state.SimDescription.FullName);
                    }
                }

                if (mEarlyMessage != 0)
                {
                    state.AgeTransitionEarlyMessageAlarm = AlarmManager.Global.AddAlarm(mEarlyMessage, TimeUnit.Hours, state.ShowAgeTransitionEarlyMessageCallback, "Tell player that the cake will be a lie in a few days", AlarmType.AlwaysPersisted, sim);
                }
            }
        }
    }
}
