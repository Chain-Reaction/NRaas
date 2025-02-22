using NRaas.CommonSpace.Helpers;
using NRaas.SaverSpace;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.VideoRecording;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class SaverOptions
    {
        [Tunable, TunableComment("The number of real-time minutes between saves")]
        public static int kRealMinutesBetweenSaves = 30;

        [Tunable, TunableComment("The number of sim-minutes between saves (unused if RealMinutes is greater than zero)")]
        public static int kSimMinutesBetweenSaves = 1440;

        [Tunable, TunableComment("Comma separated list of hours at which to save (unused if RealMinutes or SimMinutes is greater than zero)")]
        public static float[] kSimSaveHour = new float[] { 5f };

        [Tunable, TunableComment("Number of saves to cycle through")]
        public static int kSaveCycles = 4;

        [Tunable, TunableComment("Whether to switch to map view prior to saving")]
        public static bool kSwitchToMapView = false;

        [Tunable, TunableComment("Whether to prompt while in Build/Buy Mode")]
        public static bool kPromptInBuildBuy = true;        

        [Tunable, TunableComment("Whether to pause when the game initially loads")]
        public static bool kPauseOnLoad = true;

        [Tunable, TunableComment("Whether to pause when the save prompt appears")]
        public static bool kPauseOnSave = false;
    }

    public class Saver : Common, Common.IWorldLoadFinished, Common.IWorldQuit, Common.IPreSave, Common.IPostSave
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static AutoSave sTimer = null;

        static int sCount = 0;

        static bool sSaveWasRequested = false;
        static bool sSaveWasPerformed = false;

        static string sOriginalSaveName = null;

        static List<AlarmTask> sTasks = new List<AlarmTask>();

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Saver()
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
        }

        public static void RestartTimers()
        {
            if (World.IsEditInGameFromWBMode()) return;

            foreach (AlarmTask task in sTasks)
            {
                task.Dispose();
            }

            sTasks.Clear();

            if (sTimer != null)
            {
                sTimer.Stop();
                sTimer = null;
            }

            switch (Saver.Settings.SaveStyle)
            {
                case SaverSpace.Options.SaveStyle.RealTime:
                    if (Saver.Settings.mRealMinutesBetweenSaves > 0)
                    {
                        sTimer = new AutoSave(Saver.Settings.mRealMinutesBetweenSaves);

                        Simulator.AddObject(sTimer);
                    }
                    break;
                case SaverSpace.Options.SaveStyle.SimTime:
                    if (Saver.Settings.mSimMinutesBetweenSaves > 0)
                    {
                        sTasks.Add(new AlarmTask(Saver.Settings.mSimMinutesBetweenSaves, TimeUnit.Minutes, OnSimTimer, Saver.Settings.mSimMinutesBetweenSaves, TimeUnit.Minutes));
                    }
                    break;
                case SaverSpace.Options.SaveStyle.SimHour:
                    foreach (float hour in Saver.Settings.mSimSaveHour)
                    {
                        sTasks.Add(new AlarmTask(hour, DaysOfTheWeek.All, OnSimTimer));
                    }
                    break;
            }

            if ((sTimer == null) && (sTasks.Count == 0))
            {
                SimpleMessageDialog.Show(Common.Localize("Root:MenuName"), Common.Localize("Unset:Prompt"));

                Saver.Settings.SaveStyle = SaverSpace.Options.SaveStyle.RealTime;
                Saver.Settings.mRealMinutesBetweenSaves = 30;

                RestartTimers();
            }
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                if (Saver.Settings.mPauseOnLoad)
                {
                    new AlarmTask(2, TimeUnit.Minutes, GameflowEx.Pause);
                }

                RestartTimers();

                Corrections.CorrectSaveGameLocks();
            }
            catch (Exception exception)
            {
                Common.Exception("OnWorldLoadFinished", exception);
            }
        }

        public void OnWorldQuit()
        {
            if (sTimer != null)
            {
                sTimer.Stop();
                sTimer = null;
            }
        }

        public void OnPreSave()
        {
            sSaveWasPerformed = true;
            sSaveWasRequested = false;

            if (GameStates.GetInWorldSubState() is LiveModeState)
            {
                if (sTimer != null)
                {
                    sTimer.Stop();
                    sTimer = null;
                }
            }
        }

        public void OnPostSave()
        {
            RestartTimers();
        }

        public static void OnCheckForSave()
        {
            if ((sSaveWasRequested) && (!sSaveWasPerformed))
            {
                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                if (optionsModel != null) 
                {
                    optionsModel.SaveName = sOriginalSaveName;
                }

                sCount--;
                sSaveWasRequested = false;
            }
        }

        protected static void OnSimTimer()
        {
            try
            {
                if (VideoRecorder.Instance.Status == Status.Running)
                {
                    new AlarmTask(1, TimeUnit.Hours, OnSimTimer);
                }
                else
                {
                    Save();
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimTimer", exception);
            }
        }

        protected static void Save()
        {
            if (sSaveWasRequested) return;

            try
            {
                /*
                if (!AcceptCancelDialog.Show(Common.LocalizeEAString("Ui/Caption/PromptedSave:PromptEditTown")))
                {
                    sSaveWasRequested = false;
                    sSaveWasPerformed = false;
                    return;
                }
                */
                PuckController controller = null;
                
                Puck puck = Puck.Instance;
                if (puck != null)
                {
                    controller = puck.mPuckController;
                }

                if (GameStates.GetInWorldSubState() is LiveModeState)
                {
                    if (Settings.mSwitchToMapView)
                    {
                        if (!CameraController.IsMapViewModeEnabled())
                        {
                            Sims3.Gameplay.Core.Camera.ToggleMapView();
                            SpeedTrap.Sleep();
                        }
                    }

                    if (Saver.Settings.mPauseOnSave)
                    {
                        GameflowEx.Pause();
                    }
                }
                /*else
                {
                    if (controller != null)
                    {
                        controller.ChangeToMode(PuckController.Mode.Live);
                    }

                    GameflowEx.Pause();

                    while ((GameStates.GetInWorldSubState() is BuyModeState) || (GameStates.GetInWorldSubState() is BuildModeState))
                    {
                        SpeedTrap.Sleep();
                    }
                }*/

                Common.FunctionTask.Perform(OnSave);
            }
            catch (Exception exception)
            {
                Common.Exception("Save", exception);
            }
        }

        protected static void OnSave()
        {
            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
            if (optionsModel == null) return;

            string saveName = optionsModel.SaveName;

            sOriginalSaveName = saveName;

            if (!string.IsNullOrEmpty(saveName))
            {
                saveName = saveName.Replace(".sims3", "");
            }

            if ((saveName != null) && (saveName != ""))
            {
                int i = 0;
                for (i = saveName.Length - 1; i >= 0; i--)
                {
                    string value = saveName[i].ToString();

                    int test;
                    if (!int.TryParse(value, out test)) break;
                }

                int.TryParse(saveName.Substring(i + 1), out sCount);

                saveName = saveName.Substring(0, i + 1);
            }

            sCount++;
            if (sCount > Settings.mSaveCycles)
            {
                sCount = 1;
            }

            bool saveAs = true;
            if (!string.IsNullOrEmpty(saveName))
            {
                optionsModel.SaveName = saveName + sCount.ToString();
            }
            else
            {
                saveAs = false;
            }

            sSaveWasRequested = true;
            sSaveWasPerformed = false;

            if (!optionsModel.SaveGame(true, true, saveAs))
            {
                OnCheckForSave();
            }
            else
            {
                if (GameStates.GetInWorldSubState() is LiveModeState)
                {
                    new AlarmTask(2, TimeUnit.Minutes, OnCheckForSave);
                }
                else
                {
                    sSaveWasRequested = false;
                }
            }
        }

        protected class AutoSave : Task
        {
            private bool mExit;
            private readonly int mSaveInterval;
            private StopWatch mTimer;

            public AutoSave(int interval)
            {
                mSaveInterval = interval;
            }

            protected void Destroy()
            {
                Dispose();

                if (ObjectId != ObjectGuid.InvalidObjectGuid)
                {
                    Simulator.DestroyObject(ObjectId);
                    ObjectId = ObjectGuid.InvalidObjectGuid;
                }

                if (object.ReferenceEquals(sTimer, this))
                {
                    sTimer = null;
                }
            }

            public void Restart()
            {
                if (mTimer != null)
                {
                    mTimer.Restart();
                }
            }

            public override void Dispose()
            {
                if (mTimer != null)
                {
                    mTimer.Dispose();
                    mTimer = null;
                }
            }

            protected static bool IsValidState()
            {
                if (VideoRecorder.Instance.Status == Status.Running) return false;
                
                if (UIManager.GetModalWindow() != null) return false;
                
                if (Sims3.SimIFace.Gameflow.SaveIsDisabled) return false;
                
                if (GameStates.GetInWorldSubState() is LiveModeState) return true;

                if ((GameStates.GetInWorldSubState() is BuildModeState) || 
                    (GameStates.GetInWorldSubState() is BuyModeState) ||
                    (GameStates.GetInWorldSubState() is BlueprintModeState))
                {
                    return Settings.mPromptInBuildBuy;
                }

                return false;
            }

            public override void Simulate()
            {
                try
                {
                    NRaas.SpeedTrap.Begin();

                    mTimer = StopWatch.Create(StopWatch.TickStyles.Minutes);
                    mTimer.Start();

                    while (true)
                    {
                        while ((mTimer != null) && (mTimer.GetElapsedTime() < mSaveInterval))
                        {
                            if (mExit)
                            {
                                break;
                            }
                            SpeedTrap.Sleep();
                        }

                        if (mTimer == null)
                        {
                            break;
                        }

                        Corrections.CorrectSaveGameLocks();

                        do
                        {
                            if (mExit)
                            {
                                break;
                            }
                            SpeedTrap.Sleep();
                        }
                        while (!IsValidState());

                        if (mExit)
                        {
                            break;
                        }

                        Saver.Save();

                        mTimer.Restart();
                    }

                    Destroy();
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception("Simulate", exception);
                }
                finally
                {
                    NRaas.SpeedTrap.End();
                }
            }

            public override void Stop()
            {
                mExit = true;
            }
        }
    }
}
