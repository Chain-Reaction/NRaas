using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class LiveModeStateEx : LiveModeState
    {
        public LiveModeStateEx(bool forceSave)
            : base(forceSave)
        { }

        public override void LoadUI()
        {
            Common.StringBuilder msg = new Common.StringBuilder("LiveModeStateEx:LoadUI" + Common.NewLine);

            try
            {
                bool reset = false;

                Sim selectedActor = PlumbBob.SelectedActor;
                if (selectedActor != null)
                {
                    msg += Common.NewLine + "SelectedActor: " + selectedActor.FullName;

                    // Elements used by HudModel:Initialize()
                    if (selectedActor.SkillManager == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "SkillManager: null";
                    }
                    else if (selectedActor.SocialComponent == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "SocialComponent: null";
                    }
                    else if (selectedActor.CareerManager == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "CareerManager: null";
                    }
                    else if (selectedActor.TraitManager == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "TraitManager: null";
                    }
                    else if (selectedActor.SimDescription == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "SimDescription: null";
                    }
                    else if (selectedActor.InteractionQueue == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "InteractionQueue: null";
                    }
                    else if (selectedActor.OccultManager == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "OccultManager: null";
                    }
                    else if (selectedActor.CelebrityManager == null)
                    {
                        reset = true;
                        msg += Common.NewLine + "CelebrityManager: null";
                    }
                }
                else
                {
                    msg += Common.NewLine + "No SelectedActor";
                }

                if (reset)
                {
                    msg += Common.NewLine + "Perform Reset";

                    selectedActor = ResetSimTask.Perform(selectedActor, false);

                    if (selectedActor == null)
                    {
                        PlumbBob.DoSelectActor(null, true);
                    }
                    else
                    {
                        DreamCatcher.SelectNoLotCheckImmediate(selectedActor, false, true);
                    }
                }

                base.LoadUI();
                /*
                // From LiveModeState:LoadUI
                mMapTagController = new MapTagControllerEx();
                Simulator.AddObject(mMapTagController);
                HudController.Load();*/
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }

        public override void Startup()
        {
            Common.StringBuilder msg = new Common.StringBuilder("LiveModeStateEx:Startup" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            try
            {
                UIManager.BlackBackground(false);

                msg += "A1";
                Traveler.InsanityWriteLog(msg);

                // StateMachineState:Startup()
                mBaseCallFlag |= BaseCallFlag.kStartup;

                // InWorldSubState:Startup()
                while (sDelayNextStateStartupCount > 0x0)
                {
                    SpeedTrap.Sleep();
                }

                msg += "A2";
                Traveler.InsanityWriteLog(msg);

                EventTracker.SendEvent(new InWorldSubStateEvent(this, true));

                //base.Startup();

                msg += "A3";
                Traveler.InsanityWriteLog(msg);

                ShowUI(true);

                if (CameraController.IsMapViewModeEnabled() && !TutorialModel.Singleton.IsTutorialRunning())
                {
                    CameraController.EnableCameraMapView(true);
                }

                msg += "B";
                Traveler.InsanityWriteLog(msg);

                CASExitLoadScreen.Close();

                bool traveling = false;
                if (GameStates.IsTravelling)
                {
                    if (GameStates.sTravelData == null)
                    {
                        GameStates.ClearTravelStatics();
                    }
                    else
                    {
                        traveling = true;
                        GameStatesEx.OnArrivalAtVacationWorld();
                    }
                }

                msg += "C";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.sNextSimToSelect != null)
                {
                    DreamCatcher.SelectNoLotCheckImmediate(GameStates.sNextSimToSelect, true, true);
                    GameStates.sNextSimToSelect = null;
                }

                msg += "D";
                Traveler.InsanityWriteLog(msg);

                bool flag2 = false;
                if (LoadingScreenController.Instance != null)
                {
                    if ((GameStates.IsPerfTestRunning || (CommandLine.FindSwitch("campos") != null)) || (CommandLine.FindSwitch("camtarget") != null))
                    {
                        msg += "D1";
                        Traveler.InsanityWriteLog(msg);

                        GameUtils.EnableSceneDraw(true);
                        LoadingScreenController.Unload();
                    }
                    else if (traveling)
                    {
                        msg += "D2";
                        Traveler.InsanityWriteLog(msg);

                        uint customFlyThroughIndex = CameraController.GetCustomFlyThroughIndex();
                        if (GameStates.IsNewGame && (customFlyThroughIndex != 0x0))
                        {
                            msg += "D3";
                            Traveler.InsanityWriteLog(msg);

                            WaitForLotLoad(true, false);
                            ShowUI(false);
                            ShowMaptags(false);

                            msg += "D4";
                            Traveler.InsanityWriteLog(msg);

                            AudioManager.MusicMode = MusicMode.Flyby;
                            CameraController.OnCameraFlyThroughFinishedCallback += OnCameraFlyThroughFinishedEvent;
                            CameraController.StartFlyThrough(customFlyThroughIndex, false);
                            flag2 = true;

                            msg += "D5";
                            Traveler.InsanityWriteLog(msg);

                            GameUtils.EnableSceneDraw(true);
                            LoadingScreenController.Unload();
                            while (LoadingScreenController.Instance != null)
                            {
                                Sleep(0.0);
                            }
                        }
                        else
                        {
                            msg += "D6";
                            Traveler.InsanityWriteLog(msg);

                            WaitForLotLoad(true, true);
                            Camera.SetView(CameraView.SimView, true, true);
                        }
                    }
                    else
                    {
                        msg += "D7";
                        Traveler.InsanityWriteLog(msg);

                        Camera.RestorePersistedState();

                        msg += "D8";
                        Traveler.InsanityWriteLog(msg);

                        WaitForLotLoad(false, true);
                    }
                }
                else if (traveling)
                {
                    msg += "D9";
                    Traveler.InsanityWriteLog(msg);

                    WaitForLotLoad(!GameUtils.IsUniversityWorld(), true);
                    Camera.SetView(CameraView.SimView, true, true);
                }

                msg += "E";
                Traveler.InsanityWriteLog(msg);

                UserToolUtils.UserToolGeneric(0xc8, new ResourceKey(0x0L, 0x0, 0x0));
                if (!flag2)
                {
                    GameUtils.Unpause();
                }

                if (AudioManager.MusicMode != MusicMode.Flyby)
                {
                    AudioManager.MusicMode = MusicMode.None;
                }

                msg += "F";
                Traveler.InsanityWriteLog(msg);

                InWorldSubState.EdgeScrollCheck();
                InWorldSubState.OpportunityDialogCheck();

                try
                {
                    DreamsAndPromisesManager.ValidateLifetimeWishes();
                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }

                LifeEventManager.ValidatePendingLifeEvents();
                if (GameUtils.IsInstalled(ProductVersion.EP9))
                {
                    Tutorialette.TriggerLesson(Lessons.Degrees, Sim.ActiveActor);
                }

                if (GameUtils.GetCurrentWorldType() == WorldType.Base)
                {
                    Tutorialette.TriggerLesson(Lessons.Traveling, Sim.ActiveActor);
                }

                if (Sims3.Gameplay.ActiveCareer.ActiveCareer.ActiveCareerShowGeneralLesson)
                {
                    Tutorialette.TriggerLesson(Lessons.Professions, Sim.ActiveActor);
                }

                msg += "G";
                Traveler.InsanityWriteLog(msg);

                Tutorialette.TriggerLesson(Lessons.AgingStageLengths, Sim.ActiveActor);

                Sim selectedActor = PlumbBob.SelectedActor;
                if (selectedActor != null)
                {
                    // Custom
                    if (GameStates.sMovingWorldData != null)
                    {
                        WorldData.MergeFromCrossWorldData();
                    }

                    if (selectedActor.LotHome != null)
                    {
                        if (selectedActor.LotHome.HasVirtualResidentialSlots)
                        {
                            Tutorialette.TriggerLesson(Lessons.ApartmentLiving, selectedActor);
                        }
                    }
                    else
                    {
                        Lot lot = Helpers.TravelUtilEx.FindLot();
                        if (lot != null)
                        {
                            lot.MoveIn(selectedActor.Household);

                            foreach (SimDescription sim in Households.All(selectedActor.Household))
                            {
                                if (sim.CreatedSim == null)
                                {
                                    FixInvisibleTask.Perform(sim, false);
                                }
                                else
                                {
                                    bool replace = (sim.CreatedSim == selectedActor);

                                    ResetSimTask.Perform(sim.CreatedSim, false);

                                    if (replace)
                                    {
                                        selectedActor = sim.CreatedSim;
                                    }
                                }
                            }

                            msg += "MoveIn";
                        }
                    }

                    if (selectedActor.Household != null)
                    {
                        foreach (SimDescription description in Households.Humans(selectedActor.Household))
                        {
                            // Custom
                            if (GameStates.sMovingWorldData != null)
                            {
                                CrossWorldControl.Restore(description);
                            }

                            if (description.Child || description.Teen)
                            {
                                Tutorialette.TriggerLesson(Lessons.Pranks, null);
                                break;
                            }
                        }
                    }

                    if (selectedActor.BuffManager != null)
                    {
                        BuffMummysCurse.BuffInstanceMummysCurse element = selectedActor.BuffManager.GetElement(BuffNames.MummysCurse) as BuffMummysCurse.BuffInstanceMummysCurse;
                        if (element != null)
                        {
                            BuffMummysCurse.SetCursedScreenFX(element.CurseStage, false);
                        }
                    }

                    if (selectedActor.CareerManager != null)
                    {
                        selectedActor.CareerManager.UpdateCareerUI();
                    }

                    if (Traveler.Settings.mAllowSpawnWeatherStone && GameUtils.IsInstalled(ProductVersion.EP7) && GameUtils.IsInstalled(ProductVersion.EP8))
                    {
                        Sims3.Gameplay.UI.Responder.Instance.TrySpawnWeatherStone();
                    }
                }

                msg += "H";
                Traveler.InsanityWriteLog(msg);

                // Custom
                if (GameStates.sMovingWorldData != null)
                {
                    GameStatesEx.UpdateMiniSims(GameStatesEx.GetAllSims());

                    foreach (SimDescription sim in Households.All(Household.ActiveHousehold))
                    {
                        MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
                        if (miniSim != null)
                        {
                            miniSim.Instantiated = true;
                            if (miniSim.HomeWorld != GameUtils.GetCurrentWorld())
                            {
                                miniSim.HomeWorld = GameUtils.GetCurrentWorld();
                            }
                        }

                        if (sim.HomeWorld != GameUtils.GetCurrentWorld())
                        {
                            sim.HomeWorld = GameUtils.GetCurrentWorld();
                        }
                    }
                }

                GameStates.SetupPostMoveData();
                GameStates.ClearMovingData();
                MovingWorldsWizardCheck();

                // Custom
                Household.IsTravelImport = false;

                InWorldSubStateEx.PlaceLotWizardCheck(this);

                msg += "I";
                Traveler.InsanityWriteLog(msg);

                foreach (Sim sim in LotManager.Actors)
                {
                    try
                    {
                        if (sim.HasBeenDestroyed) continue;

                        OccultManager occultManager = sim.OccultManager;
                        if (occultManager != null)
                        {
                            occultManager.RestoreOccultIfNecessary();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, null, msg, e);
                    }
                }

                msg += "J";
                Traveler.InsanityWriteLog(msg);

                // Custom
                foreach (SimDescription description in SimListing.GetResidents(false).Values)
                {
                    if (description.LotHome == null) continue;

                    MiniSimDescription miniSimDescription = description.GetMiniSimDescription();
                    if (miniSimDescription != null)
                    {
                        miniSimDescription.LotHomeId = description.LotHome.LotId;
                    }
                }

                msg += "K";
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
            finally
            {
                CASExitLoadScreen.Close();
            }
        }
    }
}
