using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TestSpace.States
{
    public class InWorldStateEx : InWorldState
    {
        public InWorldStateEx()
        { }

        public override void Startup()
        {
            string msg = "InWorldStateEx:Startup";
            CRAPJailBreak.InsanityWriteLog(msg);

            try
            {
                // StateMachineState:Startup()
                mBaseCallFlag |= BaseCallFlag.kStartup;
                //base.Startup();

                msg += Common.NewLine + "A";
                CRAPJailBreak.InsanityWriteLog(msg);

                Sims3.Gameplay.UI.Responder.GameStartup();

                bool flag = false;

                try
                {
                    ModalDialog.EnableModalDialogs = false;

                    if (World.IsEditInGameFromWBMode())
                    {
                        Sims3.Gameplay.Autonomy.Autonomy.DisableMoodContributors = true;
                    }

                    msg += Common.NewLine + "B";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    mStateMachine = StateMachine.Create(0x1, "InWorld");
                    mSubStates[0x0] = new LiveModeState();
                    mSubStates[0x1] = new BuildModeState();
                    mSubStates[0x2] = new BuyModeState();
                    mSubStates[0xc] = new ShoppingModeState();
                    mSubStates[0x3] = new CASFullModeState();
                    mSubStates[0x4] = new CASDresserModeState();
                    mSubStates[0x5] = new CASMirrorModeState();
                    mSubStates[0x6] = new CASTattooModeState();
                    mSubStates[0x7] = new CASStylistModeState();
                    mSubStates[0x8] = new CASTackModeState();
                    mSubStates[0x9] = new CASCollarModeState();
                    mSubStates[0xa] = new CASSurgeryFaceModeState();
                    mSubStates[0xb] = new CASSurgeryBodyModeState();
                    mSubStates[0xf] = new PlayFlowState();
                    mSubStates[0xe] = new EditTownState();

                    foreach (InWorldSubState state in mSubStates)
                    {
                        mStateMachine.AddState(state);
                    }

                    msg += Common.NewLine + "C";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    StateMachineManager.AddMachine(mStateMachine);
                    if (GameStates.IsTravelling || GameStates.IsEditingOtherTown)
                    {
                        try
                        {
                            Sims3.Gameplay.WorldBuilderUtil.CharacterImportOnGameLoad.RemapSimDescriptionIds();
                        }
                        catch (Exception e)
                        {
                            Common.DebugException("RemapSimDescriptionIds", e);
                        }
                    }

                    msg += Common.NewLine + "D";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    if (GameStates.IsTravelling)
                    {
                        msg += Common.NewLine + "E1";
                        CRAPJailBreak.InsanityWriteLog(msg);

                        GameStates.ImportTravellingHousehold();
                    }
                    else if ((!GameStates.IsEditingOtherTown) && (GameUtils.GetCurrentWorldType() == WorldType.Base) && (GameStates.HasTravelData) && (GameStates.TravelHousehold == null))
                    {
                        msg += Common.NewLine + "E2";
                        CRAPJailBreak.InsanityWriteLog(msg);

                        GameStates.ClearTravelStatics();
                    }

                    msg += Common.NewLine + "F";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    Sim selectedActor = PlumbBob.SelectedActor;
                    if (selectedActor != null)
                    {
                        msg += Common.NewLine + "SelectedActor: " + selectedActor.FullName;
                        msg += Common.NewLine + "SkillManager: " + (selectedActor.SkillManager != null);
                        msg += Common.NewLine + "SocialComponent: " + (selectedActor.SocialComponent != null);
                        msg += Common.NewLine + "CareerManager: " + (selectedActor.CareerManager != null);
                        msg += Common.NewLine + "TraitManager: " + (selectedActor.TraitManager != null);
                        msg += Common.NewLine + "SimDescription: " + (selectedActor.SimDescription != null);
                        msg += Common.NewLine + "InteractionQueue: " + (selectedActor.InteractionQueue != null);
                        msg += Common.NewLine + "OccultManager: " + (selectedActor.OccultManager != null);
                    }
                    else
                    {
                        msg += Common.NewLine + "No SelectedActor";
                    }

                    CRAPJailBreak.InsanityWriteLog(msg);

                    try
                    {
                        mPostWorldInitializers = new Initializers("PostWorldInitializers", this);

                        /*
                        for (int i = 0; i < mPostWorldInitializers.mInitializerRecords.Count; i++)
                        {
                            Initializers.Record record = mPostWorldInitializers.mInitializerRecords[i];

                            if ((record.mTypeName.Replace(" ", "") == "Sims3.Gameplay.CAS.SimDescription,Sims3GameplaySystems") &&
                                (record.mInitName == "PostLoadFixUp"))
                            {
                                record.mTypeName = typeof(SimDescriptionEx).FullName + "," + typeof(SimDescriptionEx).Assembly.GetName();

                                // Record is a struct, we must replace the old copy with a new one
                                mPostWorldInitializers.mInitializerRecords[i] = record;
                            }
                        }

                        //Initialize(mPostWorldInitializers);
                        */

                        mPostWorldInitializers.Initialize();
                    }
                    catch (Exception e)
                    {
                        CRAPJailBreak.InsanityException(msg, e);
                    }

                    msg += Common.NewLine + "G";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    List<SimDescription> dyingSims = null;
                    if (GameStates.IsTravelling)
                    {
                        dyingSims = GameStates.PostTravelingFixUp();
                    }

                    msg += Common.NewLine + "H";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    if (GameStates.IsEditingOtherTown)
                    {
                        GameStates.PostLoadEditTownFixup();
                    }

                    msg += Common.NewLine + "I";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    GameStates.NullEditTownDataDataIfEditingOriginalStartingWorld();
                    try
                    {
                        if (GameUtils.IsOnVacation())
                        {
                            if ((dyingSims != null) && (AgingManager.NumberAgingYearsElapsed != -1f))
                            {
                                float yearsGone = GameStates.NumberAgingYearsElapsed - AgingManager.NumberAgingYearsElapsed;

                                // Custom function
                                StoryProgressionService.FixUpAfterTravel(yearsGone, dyingSims);
                            }
                            AgingManager.NumberAgingYearsElapsed = GameStates.NumberAgingYearsElapsed;
                        }
                    }
                    catch (Exception e)
                    {
                        CRAPJailBreak.InsanityException(msg, e);
                    }

                    msg += Common.NewLine + "J";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    Sims3.Gameplay.Gameflow.GameSpeed pause = Sims3.Gameplay.Gameflow.GameSpeed.Pause;

                    if (GameStates.StartupState == SubState.LiveMode)
                    {
                        flag = !GameStates.IsEditingOtherTown && (((GameStates.ForceStateChange || !PlayFlowModel.Singleton.GameEntryLive) || (PlumbBob.SelectedActor != null)) || GameStates.IsTravelling);
                        if (flag)
                        {
                            if (Sims3.Gameplay.Gameflow.sGameLoadedFromWorldFile)
                            {
                                pause = Sims3.SimIFace.Gameflow.GameSpeed.Normal;
                            }
                            else
                            {
                                pause = Sims3.Gameplay.Gameflow.sPersistedGameSpeed;
                            }
                        }
                    }

                    msg += Common.NewLine + "K";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    Sims3.Gameplay.Gameflow.sGameLoadedFromWorldFile = false;
                    string s = CommandLine.FindSwitch("speed");
                    if (s != null)
                    {
                        int num2;
                        if (int.TryParse(s, out num2))
                        {
                            pause = (Sims3.Gameplay.Gameflow.GameSpeed)num2;
                        }
                        else
                        {
                            ParserFunctions.TryParseEnum<Sims3.Gameplay.Gameflow.GameSpeed>(s, out pause, Sims3.Gameplay.Gameflow.GameSpeed.Normal);
                        }
                    }

                    msg += Common.NewLine + "L";
                    CRAPJailBreak.InsanityWriteLog(msg);

                    Sims3.Gameplay.Gameflow.SetGameSpeed(pause, Sims3.Gameplay.Gameflow.SetGameSpeedContext.GameStates);
                    NotificationManager.Load();
                }
                finally
                {
                    ModalDialog.EnableModalDialogs = true;
                }

                switch (GameStates.StartupState)
                {
                    case SubState.EditTown:
                        msg += Common.NewLine + "StartupState: EditTown";

                        GameUtils.EnableSceneDraw(true);
                        LoadingScreenController.Unload();
                        GotoEditTown();
                        break;

                    case SubState.PlayFlow:
                        if (World.IsEditInGameFromWBMode())
                        {
                            msg += Common.NewLine + "StartupState: PlayFlow (A)";

                            GameUtils.EnableSceneDraw(true);
                            LoadingScreenController.Unload();
                            GotoLiveMode();
                        }
                        else if (!PlayFlowModel.PlayFlowEnabled || !PlayFlowModel.Singleton.GameEntryLive)
                        {
                            msg += Common.NewLine + "StartupState: PlayFlow (B)";

                            GameUtils.EnableSceneDraw(true);
                            LoadingScreenController.Unload();
                            GotoLiveMode();
                        }
                        else
                        {
                            msg += Common.NewLine + "StartupState: PlayFlow (C)";

                            GotoPlayFlow();
                        }
                        break;

                    case SubState.LiveMode:
                        if (flag)
                        {
                            msg += Common.NewLine + "StartupState: LiveMode (A)";

                            GameUtils.EnableSceneDraw(true);
                            LoadingScreenController.Unload();
                            GotoLiveMode();
                        }
                        else
                        {
                            msg += Common.NewLine + "StartupState: LiveMode (B)";

                            GameUtils.EnableSceneDraw(true);
                            LoadingScreenController.Unload();
                            GotoPlayFlow();
                        }
                        break;
                }

                msg += Common.NewLine + "M";
                CRAPJailBreak.InsanityWriteLog(msg);

                if (Sims3.Gameplay.Gameflow.sShowObjectReplacedWarning)
                {
                    SimpleMessageDialog.Show(Common.LocalizeEAString("Ui/Warning/LoadGame:Warning"), Common.LocalizeEAString("Ui/Warning/LoadGame:ReplacedObjects"), ModalDialog.PauseMode.PauseSimulator);
                    Sims3.Gameplay.Gameflow.sShowObjectReplacedWarning = false;
                }
            }
            catch (Exception e)
            {
                CRAPJailBreak.InsanityException(msg, e);
            }
        }
    }
}
