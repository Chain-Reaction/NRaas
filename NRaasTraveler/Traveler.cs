using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace;
using NRaas.TravelerSpace.Helpers;
using NRaas.TravelerSpace.States;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
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
    public class TravelerForceInsanity
    {
        [Tunable, TunableComment("Whether to force insanity debugging on loadup")]
        public static bool kForceInsanity = false;
    }

    public class Traveler : Common, Common.IStartupApp, Common.IPreLoad, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Traveler()
        {
            sEnableLoadLog = true;

            Bootstrap();
        }
        public Traveler()
        { }

        public void OnStartupApp()
        {
            GameStates.sSingleton.mInWorldState = new InWorldStateEx();

            List<StateMachineState> states = GameStates.sSingleton.mStateMachine.mStateList;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                Type type = states[i].GetType();

                if (type == typeof(InWorldState))
                {
                    states[i] = GameStates.sSingleton.mInWorldState;
                    states[i].SetStateMachine(GameStates.sSingleton.mStateMachine);
                }
                else if (type == typeof(TravelDepartureState))
                {
                    states[i] = new TravelDepartureStateEx();
                    states[i].SetStateMachine(GameStates.sSingleton.mStateMachine);
                }
                else if (type == typeof(TravelArrivalState))
                {
                    states[i] = new TravelArrivalStateEx();
                    states[i].SetStateMachine(GameStates.sSingleton.mStateMachine);
                }
                else if (type == typeof(ToInWorldState))
                {
                    states[i] = new ToInWorldStateEx();
                    states[i].SetStateMachine(GameStates.sSingleton.mStateMachine);
                }
                else if (type == typeof(MoveOtherWorldState))
                {
                    states[i] = new MoveOtherWorldStateEx();
                    states[i].SetStateMachine(GameStates.sSingleton.mStateMachine);
                }
            }
        }

        public void OnPreLoad()
        {
            TravelerTask.Create<TravelerTask>();

            CrossWorldControl.sRetention.mInsanityDebugging = TravelerForceInsanity.kForceInsanity;

            //new Common.ImmediateEventListener(EventTypeId.kSimDescriptionDisposed, OnDisposed);
        }
        /*
        protected static void OnDisposed(Event e)
        {
            SimDescriptionEvent dEvent = e as SimDescriptionEvent;
            if (dEvent != null)
            {
                Common.StackLog(new Common.StringBuilder(dEvent.SimDescription.FullName));
            }
        }
        */
        public static void InsanityException(StringBuilder msg, Exception e)
        {
            Common.WriteLog(msg + Common.NewLine + e);

            Common.Exception(msg, e);
        }

        public static void InsanityWriteLog(StringBuilder text)
        {
            if (CrossWorldControl.sRetention.mInsanityDebugging)
            {
                // Do not use DebugWriteLog here, as kDebugging can reset during transition
                Common.WriteLog(text);
            }
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;

            if (!GameUtils.IsOnVacation())
            {
                WorldData.ForceTreasureSpawn();
            }

            UpdateAgeForeign();

            if (GameUtils.IsUniversityWorld())
            {
                AnnexEx.OnWorldLoadFinished();
            }            
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

        public static bool HasBeenSaved()
        {
            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
            if (optionsModel == null) return false;

            return (!string.IsNullOrEmpty(optionsModel.SaveName));
        }

        public static bool SaveGame()
        {
            if (Traveler.Settings.mPromptToSave)
            {
                Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Normal, Sims3.Gameplay.Gameflow.SetGameSpeedContext.User);

                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;

                bool playerMadeTravelRequest = TravelUtil.PlayerMadeTravelRequest;
                TravelUtil.PlayerMadeTravelRequest = false;
                try
                {
                    if ((optionsModel != null) && (!optionsModel.SaveGame(false, true, true)))
                    {
                        return false;
                    }

                    while (optionsModel.mSaveInProgress)
                    {
                        SpeedTrap.Sleep();
                    }
                }
                finally
                {
                    TravelUtil.PlayerMadeTravelRequest = playerMadeTravelRequest;
                }
            }

            return true;
        }

        private static void ReturnToLive()
        {
            WorldName currentWorld = GameUtils.GetCurrentWorld();

            WorldType currentType = WorldType.Undefined;
            GameUtils.WorldNameToType.TryGetValue(currentWorld, out currentType);

            try
            {
                GameUtils.WorldNameToType.Remove(currentWorld);
                GameUtils.WorldNameToType.Add(currentWorld, WorldType.Base);

                EditTownPuck.Instance.ReturnToLive();
            }
            finally
            {
                GameUtils.WorldNameToType.Remove(currentWorld);
                GameUtils.WorldNameToType.Add(currentWorld, currentType);
            }          
        }

        private static void OnReturnToLive(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                EditTownPuck ths = EditTownPuck.Instance;

                eventArgs.Handled = true;
                if (!ths.mExitingGameEntry)
                {
                    ths.mExitingGameEntry = true;
                    Common.FunctionTask.Perform(ReturnToLive);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnReturnToLive", e);
            }
        }

        public static void OnChangeTypeClick(WindowBase w, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                EditTownInfoPanel ths = EditTownInfoPanel.Instance;

                if (ths.mInfo != null)
                {
                    EditTownControllerEx.ChangeLotType(ths.mInfo);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnChangeTypeClick", e);
            }
        }

        public static void UpdateAgeForeign()
        {
            if (MiniSimDescription.sMiniSims != null)
            {
                foreach (MiniSimDescription miniSim in MiniSimDescription.sMiniSims.Values)
                {
                    if (miniSim == null) continue;

                    miniSim.mbAgingEnabled = !Settings.GetAgelessForeign(miniSim);
                }
            }    
        }

        public class TravelerTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                if ((HudController.Instance != null) && (GameUtils.IsOnVacation()))
                {
                    HudModel model = HudController.Instance.mHudModel as HudModel;
                    if (model != null)
                    {
                        if (model.MinuteChanged != null)
                        {
                            foreach (Delegate del in model.MinuteChanged.GetInvocationList())
                            {
                                TimeControl control = del.Target as TimeControl;
                                if (control != null)
                                {
                                    TimeControlEx.sThs = control;

                                    model.MinuteChanged -= TimeControlEx.sThs.OnMinuteChanged;
                                    model.MinuteChanged -= TimeControlEx.OnMinuteChanged;
                                    model.MinuteChanged += TimeControlEx.OnMinuteChanged;

                                    break;
                                }
                            }
                        }
                    }
                }

                InventoryPanel inventory = InventoryPanel.sInstance;
                if ((inventory != null) && (inventory.Visible))
                {
                    if (inventory.mTimeAlmanacButton != null)
                    {
                        if (GameUtils.IsFutureWorld())
                        {
                            inventory.mTimeAlmanacButton.Visible = (GameStates.TravelHousehold == Household.ActiveHousehold);                            
                        }

                        if (inventory.mTimeAlmanacButton.Visible)
                        {
                            inventory.mTimeAlmanacButton.Click -= inventory.OnClickTimeAlmanac;
                            inventory.mTimeAlmanacButton.Click -= FutureDescendantServiceEx.OnClickTimeAlmanac;
                            inventory.mTimeAlmanacButton.Click += FutureDescendantServiceEx.OnClickTimeAlmanac;
                        }
                    }
                }

                EditTownPuck puck = EditTownPuck.Instance;
                if (puck != null)
                {
                    if (puck.mReturnToLiveButton != null)
                    {
                        puck.mReturnToLiveButton.Click -= puck.OnReturnToLive;
                        puck.mReturnToLiveButton.Click -= OnReturnToLive;
                        puck.mReturnToLiveButton.Click += OnReturnToLive;
                    }
                }

                EditTownInfoPanel panel = EditTownInfoPanel.Instance;
                if (panel != null)
                {
                    if ((panel.mActionButtons != null) && (panel.mActionButtons.Length > 8) && (panel.mActionButtons[0x8] != null))
                    {
                        panel.mActionButtons[0x8].Click -= panel.OnChangeTypeClick;
                        panel.mActionButtons[0x8].Click -= OnChangeTypeClick;
                        panel.mActionButtons[0x8].Click += OnChangeTypeClick;
                    }
                }

                OptionsDialog options = OptionsDialog.sDialog;
                if (options != null)
                {
                    if (GameUtils.IsInstalled(ProductVersion.EP8))
                    {
                        Button testButton = options.mSeasonWindow.GetChildByID(0xdf085c3, true) as Button;
                        if ((testButton != null) && (!testButton.Enabled))
                        {
                            using (BaseWorldReversion reversion = new BaseWorldReversion())
                            {
                                foreach (Button weather in options.mEnabledWeatherButtons.Values)
                                {
                                    weather.Enabled = true;
                                }

                                if (options.mFahrenheitRadio != null)
                                {
                                    options.mFahrenheitRadio.Enabled = true;
                                }

                                if (options.mCelciusRadio != null)
                                {
                                    options.mCelciusRadio.Enabled = true;
                                }

                                options.SetupSeasonControls(false, ref options.mOldSeasonData);
                            }
                        }
                    }
                }

                FutureDescendantService instance = FutureDescendantServiceEx.GetInstance();
                if (Sims3.UI.Responder.Instance.InLiveMode && Traveler.Settings.mDisableDescendants && instance.mEventListeners.Count > 0)
                {
                    instance.CleanUpEventListeners();
                }

                return true;
            }
        }
    }
}
