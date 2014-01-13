using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class WorkPusher
    {
        [Tunable]
        protected static bool kInstantiator = false;

        [Tunable, TunableComment("Whether to push the sims in the active household")]
        protected static bool kPushActive = false;

        protected static AlarmHandle sAlarmHandle = AlarmHandle.kInvalidHandle;

        protected static bool sFirstAlarm = true;

        private static EventListener sBoughtObjectLister = null;

        protected static bool sVerbose = false;

        protected static Dictionary<Sim, bool> sCarPoolers = new Dictionary<Sim, bool>();
        protected static Dictionary<Sim, bool> sBusPoolers = new Dictionary<Sim, bool>();

        static WorkPusher()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public WorkPusher()
        { }

        public static void AddInteractions(Sims3.Gameplay.Objects.Electronics.Computer obj)
        {
            foreach (InteractionObjectPair pair in obj.Interactions)
            {
                if (pair.InteractionDefinition.GetType() == Version.Singleton.GetType())
                {
                    return;
                }
            }

            obj.AddInteraction(ToggleVerbose.Singleton);
            obj.AddInteraction(Version.Singleton);
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            List<Sims3.Gameplay.Objects.Electronics.Computer> others = new List<Sims3.Gameplay.Objects.Electronics.Computer>(Sims3.Gameplay.Queries.GetObjects<Sims3.Gameplay.Objects.Electronics.Computer>());
            foreach (Sims3.Gameplay.Objects.Electronics.Computer obj in others)
            {
                AddInteractions(obj);
            }

            sBoughtObjectLister = EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectBought));

            sFirstAlarm = true;
            sAlarmHandle = AlarmManager.Global.AddAlarmRepeating(1f, TimeUnit.Minutes, new AlarmTimerCallback(OnAlarm), 1f, TimeUnit.Hours, "NRaasWorkPusher", AlarmType.NeverPersisted, null);

            sCarPoolers.Clear();
            sBusPoolers.Clear();
        }

        protected static ListenerAction OnObjectBought(Sims3.Gameplay.EventSystem.Event e)
        {
            if (e.Id == EventTypeId.kBoughtObject)
            {
                Sims3.Gameplay.Objects.Electronics.Computer obj = e.TargetObject as Sims3.Gameplay.Objects.Electronics.Computer;
                if (obj != null)
                {
                    AddInteractions(obj);
                }
            }

            return ListenerAction.Keep;
        }

        public static void OnAlarm()
        {
            if (sFirstAlarm)
            {
                sFirstAlarm = false;
                StyledNotification.Show(new StyledNotification.Format("Work Pusher Activated", ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
            }

            int iJobSync = 0, iSchoolSync = 0;
            int iJobTwoHourPush = 0, iSchoolTwoHourPush = 0;

            string sJobNames = null, sSchoolNames = null;

            DateAndTime NowTime = SimClock.CurrentTime();
            DateAndTime TwoHourTime = SimClock.CurrentTime();
            TwoHourTime.Ticks += SimClock.ConvertToTicks(2f, TimeUnit.Hours);

            List<Sim> sims = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
            foreach (Sim sim in sims)
            {
                if (sim.CareerManager == null) continue;

                if (sim.Household == null) continue;

                if (sim.Household.IsServiceNpcHousehold) continue;

                {
                    Career job = sim.Job;
                    if (job != null)
                    {
                        float fPrevValue = job.HoursUntilWork;

                        job.SetHoursUntilWork();

                        if (fPrevValue > job.HoursUntilWork + 1f)
                        {
                            iJobSync++;
                        }

                        if (job.ShouldBeAtWork(NowTime))
                        {
                            sCarPoolers.Remove(sim);

                            InteractionInstance instance = job.CreateWorkInteractionInstance();
                            if (instance != null)
                            {
                                if ((((AutonomyRestrictions.GetLevel() >= AutonomyLevel.Two) && kPushActive) || sim.IsNPC) &&
                                    (!sim.InteractionQueue.HasInteractionOfType(instance.InteractionDefinition)))
                                {
                                    VisitSituation.AnnounceTimeToGoToWork(sim);

                                    sim.InteractionQueue.CancelAllInteractions();

                                    sim.InteractionQueue.Add(instance);

                                    sJobNames += "\n" + sim.Name;
                                }
                            }
                        }
                        else
                        {
                            if ((!sCarPoolers.ContainsKey (sim)) && 
                                (job.CurLevel != null) && 
                                (job.ShouldBeAtWork(TwoHourTime)))
                            {
                                // Check to see if the timer is already running, and if so, don't bother
                                if ((job.mRegularWorkDayGoToWorkHandle == AlarmHandle.kInvalidHandle) ||
                                    (AlarmManager.Global.GetTimeLeft(job.mRegularWorkDayGoToWorkHandle, TimeUnit.Minutes) <= 0))
                                {
                                    iJobTwoHourPush++;

                                    if (job.mRegularWorkDayTwoHoursBeforeStartHandle != AlarmHandle.kInvalidHandle)
                                    {
                                        AlarmManager.Global.RemoveAlarm(job.mRegularWorkDayTwoHoursBeforeStartHandle);
                                        job.mRegularWorkDayTwoHoursBeforeStartHandle = AlarmHandle.kInvalidHandle;
                                    }

                                    if ((!sim.IsSelectable || !job.CarpoolEnabled) || !job.CurLevel.HasCarpool)
                                    {
                                        InteractionInstance instance = job.CreateWorkInteractionInstance();
                                        if ((instance != null) &&
                                            (!sim.InteractionQueue.HasInteractionOfType(instance.InteractionDefinition)))
                                        {
                                            sim.InteractionQueue.CancelAllInteractions();
                                        }

                                        float num = ((job.CurLevel.StartTime - NowTime.Hour) + 24f) % 24f;
                                        float time = num - job.AverageTimeToReachWork;
                                        if (time < 0f)
                                        {
                                            time = 0f;
                                        }
                                        job.mRegularWorkDayGoToWorkHandle = AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, new AlarmTimerCallback(job.RegularWorkDayGoToWorkHandle), "Career: time to push go to work", AlarmType.AlwaysPersisted, job.OwnerDescription);
                                    }
                                    else
                                    {
                                        sCarPoolers.Add(sim, true);

                                        StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Objects/Vehicles/CarpoolManager:CarpoolComing", new object[] { sim }), ObjectGuid.InvalidObjectGuid, sim.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                                        StyledNotification.Show(format, job.CareerIconColored);
                                    }
                                }
                            }
                        }
                    }
                }

                {
                    School school = sim.CareerManager.School;
                    if (school != null)
                    {
                        float fPrevValue = school.HoursUntilWork;

                        school.SetHoursUntilWork();

                        if (fPrevValue > school.HoursUntilWork + 1f)
                        {
                            iSchoolSync++;
                        }

                        if (school.ShouldBeAtWork(NowTime))
                        {
                            sBusPoolers.Remove(sim);

                            InteractionInstance instance = school.CreateWorkInteractionInstance();
                            if (instance != null)
                            {
                                if ((((AutonomyRestrictions.GetLevel() >= AutonomyLevel.Two) && kPushActive) || sim.IsNPC) &&
                                    (!sim.InteractionQueue.HasInteractionOfType(instance.InteractionDefinition)))
                                {
                                    VisitSituation.AnnounceTimeToGoToWork(sim);

                                    sim.InteractionQueue.CancelAllInteractions();

                                    sim.InteractionQueue.Add(instance);

                                    sSchoolNames += "\n" + sim.Name;
                                }
                            }
                        }
                        else
                        {
                            if ((!sBusPoolers.ContainsKey (sim)) && 
                                (school.ShouldBeAtWork(TwoHourTime)))
                            {
                                // Check to see if the timer is already running, and if so, don't bother
                                if ((school.mRegularWorkDayGoToWorkHandle == AlarmHandle.kInvalidHandle) ||
                                    (AlarmManager.Global.GetTimeLeft(school.mRegularWorkDayGoToWorkHandle, TimeUnit.Minutes) <= 0))
                                {
                                    iSchoolTwoHourPush++;

                                    if (school.mRegularWorkDayTwoHoursBeforeStartHandle != AlarmHandle.kInvalidHandle)
                                    {
                                        AlarmManager.Global.RemoveAlarm(school.mRegularWorkDayTwoHoursBeforeStartHandle);
                                        school.mRegularWorkDayTwoHoursBeforeStartHandle = AlarmHandle.kInvalidHandle;
                                    }

                                    if (school.PickUpCarpool != null)
                                    {
                                        sBusPoolers.Add(sim, true);

                                        school.PickUpCarpool.TryShowTNS(sim, Localization.LocalizeString("Gameplay/Objects/Vehicles/CarpoolManager:SchoolBusComing", new object[] { sim }), Localization.LocalizeString("Ui/Tooltip/Hud/School:SchoolBusArrives", new object[] { SimClockUtils.GetText((int)(school.CurLevel.StartTime - 1f), 0) }));
                                    }
                                    else
                                    {
                                        InteractionInstance instance = school.CreateWorkInteractionInstance();
                                        if ((instance != null) &&
                                            (!sim.InteractionQueue.HasInteractionOfType(instance.InteractionDefinition)))
                                        {
                                            sim.InteractionQueue.CancelAllInteractions();
                                        }

                                        float num = ((school.CurLevel.StartTime - NowTime.Hour) + 24f) % 24f;
                                        float time = num - school.AverageTimeToReachWork;
                                        if (time < 0f)
                                        {
                                            time = 0f;
                                        }
                                        school.mRegularWorkDayGoToWorkHandle = AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, new AlarmTimerCallback(school.RegularWorkDayGoToWorkHandle), "Career: time to push go to work", AlarmType.AlwaysPersisted, school.OwnerDescription);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            bool bShow = false;

            string msg = "Work Pusher:";
            if (iJobSync > 0)
            {
                msg += "\nWork Alarms Resync'd: " + iJobSync.ToString();
                bShow = true;
            }
            if (iJobTwoHourPush > 0)
            {
                msg += "\nWork Two Hour Alarms: " + iJobTwoHourPush.ToString();
                bShow = true;
            }
            if (sJobNames != null)
            {
                msg += "\nPushed to Work: " + sJobNames;
                bShow = true;
            }
            if (iSchoolSync > 0)
            {
                msg += "\nSchool Alarms Resync'd: " + iSchoolSync.ToString();
                bShow = true;
            }
            if (iSchoolTwoHourPush > 0)
            {
                msg += "\nSchool Two Hour Alarms: " + iSchoolTwoHourPush.ToString();
                bShow = true;
            }
            if (sSchoolNames != null)
            {
                msg += "\nPushed to School: " + sSchoolNames;// iSchoolCount.ToString();
                bShow = true;
            }

            if ((bShow) && (sVerbose))
            {
                StyledNotification.Show(new StyledNotification.Format(msg, ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
            }
        }

        public class Version : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Electronics.Computer>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new VersionDefinition();

            public Version()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class VersionDefinition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Electronics.Computer, Version>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Work Pusher..." };
                }

                public override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, InteractionObjectPair interaction)
                {
                    return "Version";
                }

                public override bool Test(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                SimpleMessageDialog.Show("Work Pusher Version", "Version 7\nkPushActive=" + kPushActive.ToString ());
                return true;
            }
        }

        public class ToggleVerbose : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Electronics.Computer>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new ToggleVerboseDefinition();

            public ToggleVerbose()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class ToggleVerboseDefinition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Electronics.Computer, ToggleVerbose>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Work Pusher..." };
                }

                public override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, InteractionObjectPair interaction)
                {
                    if (WorkPusher.sVerbose)
                    {
                        return "Feedback: Verbose";
                    }
                    else
                    {
                        return "Feedback: Quiet";
                    }
                }

                public override bool Test(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                WorkPusher.sVerbose = !WorkPusher.sVerbose;

                string msg = "Work Pusher: ";
                if (sVerbose)
                {
                    msg += "Now Verbose";
                }
                else
                {
                    msg += "Now Quiet";
                }
                StyledNotification.Show(new StyledNotification.Format(msg, ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
                return true;
            }
        }
    }
}
