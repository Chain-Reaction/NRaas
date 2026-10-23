using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace;
using NRaas.OverwatchSpace.Interfaces;
using NRaas.OverwatchSpace.Loadup;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class Overwatch : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        // Read by other mods that would otherwise spread the nightly cleanups themselves. Bump when that behaviour changes.
        // 2: one cleanup per frame without spacing, Stuck Check one route check per frame
        public static readonly int StaggeredCleanupVersion = 2;

        static AlarmTask sAlarm = null;

        // The nightly cleanups not run yet, while they are spaced out
        static List<IAlarmOption> sPendingAlarms = null;
        static AlarmTask sNextAlarm = null;
        static int sSpacing = 0;

        static bool sCollectNotices = false;
        static string sAlarmNotice = null;

        static Overwatch()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();

                    if (GameUtils.GetCurrentWorldType() == WorldType.Downtown && sSettings.mAlarmHour == 3f)
                    {
                        sSettings.mAlarmHour = 5f;
                    }
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;

//            HandleLunarCycle.OnOptionsChanged();
        }

        public static void Log(string msg)
        {
            MessageLogger.Append(msg);

            //WriteLog(msg);
        }

        public static void RestartAlarm()
        {
            if (sAlarm != null)
            {
                sAlarm.Dispose();
            }

            sAlarm = new AlarmTask(Settings.mAlarmHour, DaysOfTheWeek.All, OnTimer);
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                kDebugging = Settings.Logging;

                // Spaced cleanups of another world do not carry over
                sPendingAlarms = null;
                sNextAlarm = null;
                sAlarmNotice = null;

                RestartAlarm();

                Butler.kDelayBeforeArriving = 0.02f;
                Butler.kServiceTuning.kIsRecurrent = true;
            }
            catch (Exception exception)
            {
                Common.Exception("OnWorldFinished", exception);
            }
        }

        public static void AlarmNotify(string text)
        {
            if (!Settings.mShowNotices) return;

            if (sCollectNotices)
            {
                sAlarmNotice += Common.NewLine + text;
            }
            else
            {
                Notify(text);
            }
        }

        protected static void OnTimer()
        {
            if (sPendingAlarms != null)
            {
                // Last night's cleanups are still spaced out: run the rest now, in order, before tonight's
                if (sNextAlarm != null)
                {
                    sNextAlarm.Dispose();
                    sNextAlarm = null;
                }

                RunAlarms(false);
            }

            try
            {
                sPendingAlarms = DerivativeSearch.Find<IAlarmOption>();

                // The whole chain stays within 12 hours
                sSpacing = Math.Min(Settings.mAlarmSpacing, 720 / Math.Max(1, sPendingAlarms.Count));
            }
            catch (Exception e)
            {
                Common.Exception("OnTimer", e);
                return;
            }

            RunAlarms(true);
        }

        static void OnNextAlarm()
        {
            sNextAlarm = null;

            RunAlarms(true);
        }

        // Runs the cleanups in order, as one batch like before. Without spacing, each next one runs a frame after the previous
        // one finished; with spacing, sSpacing sim-minutes after. Either way their costs do not add up in one frame. The notices
        // still show as one message at the end.
        static void RunAlarms(bool spaced)
        {
            List<IAlarmOption> items = sPendingAlarms;

            try
            {
                while (items.Count > 0)
                {
                    IAlarmOption alarmed = items[0];
                    items.RemoveAt(0);

                    sCollectNotices = true;
                    alarmed.PerformAlarm();
                    sCollectNotices = false;

                    // A newer run took over while this cleanup slept
                    if (items != sPendingAlarms) return;

                    if (items.Count == 0) break;

                    if ((spaced) && (sSpacing > 0))
                    {
                        sNextAlarm = new AlarmTask(sSpacing, TimeUnit.Minutes, OnNextAlarm);
                        return;
                    }

                    // Without spacing each cleanup gets its own frame (the alarm runs in its own task, so it may sleep)
                    Common.Sleep();

                    if (items != sPendingAlarms) return;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTimer", e);
            }
            finally
            {
                sCollectNotices = false;
            }

            // Not when a newer run took over: that one shows the notices
            if (items != sPendingAlarms) return;

            sPendingAlarms = null;

            if (!string.IsNullOrEmpty(sAlarmNotice))
            {
                Notify(sAlarmNotice);
                sAlarmNotice = null;
            }
        }

        //Externalized to StoryProgression
        public static bool GetStuckCheckEnable(bool defaultValue)
        {
            try
            {
                if (Settings == null) return defaultValue;

                return (Settings.mStuckCheckV2);
            }
            catch (Exception e)
            {
                Common.Exception("GetStuckCheckEnable", e);
                return defaultValue;
            }
        }

        public class MessageLogger : Logger<MessageLogger>
        {
            static MessageLogger sLogger = new MessageLogger();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Messages"; }
            }

            protected override MessageLogger Value
            {
                get { return sLogger; }
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                if (!Common.kDebugging) return 0;

                return base.PrivateLog(builder);
            }
        }
    }
}
