using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous;
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
    public class Homeworker
    {
        [Tunable]
        protected static bool kInstantiator = false;

        [Tunable,TunableComment ("The value to set unfinished homework, ranging from 0 to 100")]
        protected static float kCompletionValue = 50f;

        [Tunable, TunableComment("Whether to complete the homework of the active household")]
        protected static bool kActiveCompletion = false;

        protected static AlarmHandle sAlarmHandle = AlarmHandle.kInvalidHandle;

        protected static bool sFirstAlarm = true;

        private static EventListener sBoughtObjectLister = null;

        protected static bool sVerbose = false;

        static Homeworker()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public Homeworker()
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
            sAlarmHandle = AlarmManager.Global.AddAlarmDay(8f, DaysOfTheWeek.All, new AlarmTimerCallback(OnAlarm), "NRaasHomeWorker", AlarmType.NeverPersisted, null);
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
                StyledNotification.Show(new StyledNotification.Format("Homeworker Activated", ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
            }

            int iHomeworkAdjusted = 0;

            List<Sim> sims = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
            foreach (Sim sim in sims)
            {
                if (!kActiveCompletion)
                {
                    if (sim.IsSelectable) continue;
                }

                if (sim.CareerManager == null) continue;

                if (sim.Household == null) continue;

                if (sim.Household.IsServiceNpcHousehold) continue;

                School school = sim.CareerManager.School;
                if (school != null)
                {
                    foreach (Homework obj in sim.Inventory.FindAll<Homework>(false))
                    {
                        if (obj.PercentComplete < kCompletionValue)
                        {
                            obj.PercentComplete = kCompletionValue;
                            iHomeworkAdjusted++;
                        }
                    }
                }
            }

            if ((iHomeworkAdjusted > 0) && (sVerbose))
            {
                string msg = iHomeworkAdjusted.ToString() + " assignments completed to " + kCompletionValue.ToString("F0") + "%";
                StyledNotification.Show(new StyledNotification.Format(msg, ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kGameMessagePositive));
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
                    return new string[] { "Homeworker..." };
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
                SimpleMessageDialog.Show("Homeworker Version", "Version 3\nkCompletionValue=" + kCompletionValue.ToString ("F0") + "\nkActiveCompletion=" + kActiveCompletion.ToString ());
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
                    return new string[] { "Homeworker..." };
                }

                public override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, InteractionObjectPair interaction)
                {
                    if (Homeworker.sVerbose)
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
                Homeworker.sVerbose = !Homeworker.sVerbose;

                string msg = "Homeworker: ";
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
