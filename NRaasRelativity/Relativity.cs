using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.RelativitySpace;
using NRaas.RelativitySpace.Helpers;
using NRaas.RelativitySpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Relativity : Common, Common.IWorldLoadFinished, Common.IWorldQuit, Common.IPreSave
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static Adjuster sTimer = null;

        public static int sOneMinute = (int)SimClock.ConvertToTicks(1f, TimeUnit.Minutes);

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Relativity()
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

            PersistedSettings.ResetSpeeds();

            PriorValues.sFactorChanged = true;

            Settings.ParseIntervals();
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;

            Settings.EnsureBaseMotives();

            new AlarmTask(1f, TimeUnit.Minutes, OnTimer);
        }

        public void OnWorldQuit()
        {
            if (sTimer != null)
            {
                sTimer.Stop();
            }
        }

        public void OnPreSave()
        {
            if (sTimer != null)
            {
                sTimer.RevertCommodities();
            }
        }

        protected static void OnTimer()
        {
            try
            {
                if (sTimer != null)
                {
                    sTimer.Destroy();
                    sTimer = null;
                }

                sTimer = new Adjuster();
                Simulator.AddObject(sTimer);
            }
            catch (Exception exception)
            {
                Common.Exception("OnTimer", exception);
            }
        }

        protected class Adjuster : RepeatingTask
        {
            bool mExit;

            long mPreviousTime;

            PriorValues mPriorValues = new PriorValues();

            public Adjuster()
            { }

            public void Destroy()
            {
                Dispose();

                if (ObjectId != ObjectGuid.InvalidObjectGuid)
                {
                    Simulator.DestroyObject(ObjectId);
                    ObjectId = ObjectGuid.InvalidObjectGuid;
                }
            }

            protected override bool OnPerform()
            {
                while ((SimClock.CurrentTicks - mPreviousTime) < (sOneMinute - 1))
                {
                    if (mExit)
                    {
                        break;
                    }
                    SpeedTrap.Sleep();
                }

                int overrideSpeed = 0;

                Sims3.Gameplay.Gameflow.ClockSpeedModel model = Sims3.UI.Responder.Instance.ClockSpeedModel as Sims3.Gameplay.Gameflow.ClockSpeedModel;
                if (model != null)
                {
                    if (Settings.mPausingOnCompletion)
                    {
                        if (Sim.ActiveActor != null)
                        {
                            if (Sim.ActiveActor.CurrentInteraction == null)
                            {
                                Settings.mPausingOnCompletion = false;
                                GameflowEx.Pause();
                            }
                        }
                    }

                    if (Settings.mSkipOnHumanSleep)
                    {
                        bool allSleeping = true;

                        if ((Sim.ActiveActor == null) || (!Sim.ActiveActor.IsHuman))
                        {
                            allSleeping = false;
                        }
                        else
                        {
                            foreach (Sim sim in Households.AllHumans(Household.ActiveHousehold))
                            {
                                if (!(sim.CurrentInteraction is ISleeping))
                                {
                                    allSleeping = false;
                                }
                            }
                        }

                        if (allSleeping)
                        {
                            if (model.CurrentGameSpeed != Sims3.SimIFace.Gameflow.GameSpeed.Triple)
                            {
                                Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Triple, Sims3.Gameplay.Gameflow.SetGameSpeedContext.Automation);
                            }

                            Settings.mSkippingSleep = true;
                        }
                        else if (Settings.mSkippingSleep)
                        {
                            switch (model.CurrentGameSpeed)
                            {
                                case Sims3.SimIFace.Gameflow.GameSpeed.Triple:
                                case Sims3.SimIFace.Gameflow.GameSpeed.Skip:
                                    Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Normal, Sims3.Gameplay.Gameflow.SetGameSpeedContext.Automation);
                                    break;
                            }
                        }
                    }

                    Sims3.SimIFace.Gameflow.GameSpeed speed = model.CurrentGameSpeed;

                    switch (speed)
                    {
                        case Sims3.SimIFace.Gameflow.GameSpeed.Pause:
                            Settings.mPausingOnCompletion = false;
                            Settings.mSkippingSleep = false;
                            break;
                        case Sims3.SimIFace.Gameflow.GameSpeed.Normal:
                            Settings.mSkippingSleep = false;
                            break;
                        case Sims3.SimIFace.Gameflow.GameSpeed.Double:
                            if (Settings.mSwitchSpeedOnFast > 0)
                            {
                                overrideSpeed = Settings.mSwitchSpeedOnFast;
                            }
                            Settings.mSkippingSleep = false;
                            Settings.mPausingOnCompletion = false;
                            break;
                        case Sims3.SimIFace.Gameflow.GameSpeed.Triple:
                            if (Settings.mSwitchSpeedOnFast > 0)
                            {
                                overrideSpeed = Settings.mSwitchSpeedOnFast;
                            }

                            Settings.mPausingOnCompletion = false;
                            break;
                        case Sims3.SimIFace.Gameflow.GameSpeed.Skip:
                            if (Settings.mSwitchSpeedOnFast > 0)
                            {
                                overrideSpeed = Settings.mSwitchSpeedOnFast;
                            }

                            Settings.mPausingOnCompletion = false;
                            if (Settings.mPauseOnCompletion)
                            {
                                if (Sim.ActiveActor != null)
                                {
                                    if (Sim.ActiveActor.CurrentInteraction != null)
                                    {
                                        Settings.mPausingOnCompletion = true;
                                    }
                                }
                            }
                            break;
                    }
                }

                using (TestSpan span = new TestSpan(Relativity.Logger.sLogger, "Simulate"))
                {
                    if (mExit)
                    {
                        return false;
                    }

                    int speed = Settings.GetSpeed(SimClock.CurrentTime(), overrideSpeed);

                    PersistedSettings.sRelativeFactor = 0;
                    if (speed != 0)
                    {
                        PersistedSettings.sRelativeFactor = sOneMinute / (float)speed;
                    }

                    if (speed != mPriorValues.mPreviousSpeed)
                    {
                        PriorValues.sFactorChanged = true;
                    }

                    if (PriorValues.sFactorChanged)
                    {
                        PriorValues.sFactorChanged = false;

                        mPriorValues.RevertCommodityGains();

                        TuningAlterations.Revert();

                        TuningAlterations.Apply();
                    }

                    mPriorValues.ApplyCommodityGains();

                    mPriorValues.mPreviousSpeed = speed;

                    if (speed < 0)
                    {
                        speed = 0;
                    }

                    SimClock.TicksAdvanced += (speed - sOneMinute);

                    mPreviousTime = SimClock.CurrentTicks;
                }

                return true;
            }

            protected override void OnPostSimulate()
            {
                Destroy();

                mPriorValues.RevertCommodityGains();
            }

            public void RevertCommodities()
            {
                mPriorValues.RevertCommodityGains();
            }

            public override void Stop()
            {
                mExit = true;
            }
        }

        public class Logger : Common.Logger<Logger>, IAddStatGenerator
        {
            public readonly static Logger sLogger = new Logger();

            StatBin.StatEntry mEntry = new StatBin.StatEntry("Simulate");

            public static void Append(string msg)
            {
                if (!Common.kDebugging) return;

                sLogger.PrivateAppend(msg);
            }
            public static void Append(Common.StringBuilder msg)
            {
                if (!Common.kDebugging) return;

                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Delta Log"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }

            public Common.DebugLevel DebuggingLevel
            {
                get { return DebugLevel.Stats; }
            }

            public float AddStat(string stat, float val)
            {
                mEntry.Add(val);
                return val;
            }
            public float AddStat(string stat, float val, Common.DebugLevel minLevel)
            {
                mEntry.Add(val);
                return val;
            }

            protected override int PrivateLog(StringBuilder builder)
            {
                Append(mEntry.ToString());

                return base.PrivateLog(builder);
            }
        }
    }
}
