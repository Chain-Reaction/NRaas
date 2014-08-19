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

        static AlarmTask sAlarm = null;

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
            try
            {
                sCollectNotices = true;

                List<IAlarmOption> items = DerivativeSearch.Find<IAlarmOption>();

                foreach (IAlarmOption alarmed in items)
                {
                    alarmed.PerformAlarm();
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
