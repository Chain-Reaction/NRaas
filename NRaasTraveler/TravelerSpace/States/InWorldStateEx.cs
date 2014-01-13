using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Stores;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
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
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class InWorldStateEx : InWorldState
    {
        public InWorldStateEx()
        { }

        public static void DoTravelActions(StoryProgressionService ths, float yearsGone, List<SimDescription> dyingSims)
        {
            Common.StringBuilder msg = new Common.StringBuilder("DoTravelActions");

            msg += Common.NewLine + "DyingSims: " + dyingSims.Count;
            msg += Common.NewLine + "YearsGone: " + yearsGone;

            GenerateOffspringEx.PossiblyGenerateOffspring(dyingSims, yearsGone);

            int maxActions = StoryProgressionService.kTravellingMaxActions;
            if (!CrossWorldControl.sRetention.mPerformTravelActions)
            {
                maxActions = 0;
            }

            foreach (SimDescription description in dyingSims)
            {
                try
                {
                    msg += Common.NewLine + "Sim: " + description.FullName;

                    KillSim.KillSimExecute(description, SimDescription.DeathType.OldAge);
                }
                catch (Exception e)
                {
                    Common.Exception(description, e);
                }
            }

            if (GameUtils.GetCurrentWorldType() == WorldType.Vacation)
            {
                bool alter = true;

                WorldName currentWorld = GameUtils.GetCurrentWorld();

                switch(currentWorld)
                {
                    case WorldName.China:
                    case WorldName.Egypt:
                    case WorldName.France:
                        alter = false;
                        break;
                }

                if (alter)
                {
                    //maxActions = 5;

                    GameUtils.CheatOverrideCurrentWorld = WorldName.France;

                    try
                    {
                        StoryProgressionService.StaticShutdown();
                        StoryProgressionService.StaticInitialize();

                        ths = StoryProgressionService.sService;
                    }
                    finally
                    {
                        GameUtils.CheatOverrideCurrentWorld = currentWorld;
                    }
                }
            }

            int count = 0;

            float maxValue = float.MaxValue;
            for (int i = 0x0; (maxValue > StoryProgressionService.kTravellingTargetError) && (i < maxActions); i++)
            {
                try
                {
                    maxValue = ths.TryOneAction(true);

                    count++;
                }
                catch (Exception e)
                {
                    Common.DebugException("TryOneAction", e);
                }
            }

            msg += Common.NewLine + "Actions: " + count;

            Common.DebugWriteLog(msg);
        }

        private static void FixUpAfterTravel(float yearsGone, List<SimDescription> dyingSims)
        {
            if (StoryProgressionService.sService != null)
            {
                DoTravelActions(StoryProgressionService.sService, yearsGone, dyingSims);
            }
        }

        public static void LinkToTravelHousehold()
        {
            if (GameStates.TravelHousehold != null)
            {
                GameStates.TravelHousehold.HouseholdSimsChanged -= OnTravelHouseholdChanged;
                GameStates.TravelHousehold.HouseholdSimsChanged += OnTravelHouseholdChanged;
            }
        }

        protected static void OnTravelHouseholdChanged(HouseholdEvent householdEvent, IActor member, Household oldHousehold)
        {
            try
            {
                if (oldHousehold != null)
                {
                    if (GameStates.TravelHousehold == null)
                    {
                        oldHousehold.HouseholdSimsChanged -= OnTravelHouseholdChanged;
                        return;
                    }

                    if (householdEvent == HouseholdEvent.kSimRemoved)
                    {
                        if (Households.NumSims(oldHousehold) == 0)
                        {
                            oldHousehold.HouseholdSimsChanged -= OnTravelHouseholdChanged;

                            if (GameStates.sTravelData != null)
                            {
                                GameStates.sTravelData.mTravelHouse = Household.ActiveHousehold;
                            }

                            LinkToTravelHousehold();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Common.Exception("OnTravelHouseholdChanged", e);
            }
        }

        public override void Startup()
        {
            Common.StringBuilder msg = new Common.StringBuilder("InWorldStateEx:Startup");
            Traveler.InsanityWriteLog(msg);

            try
            {
                // StateMachineState:Startup()
                mBaseCallFlag |= BaseCallFlag.kStartup;
                //base.Startup();

                msg += Common.NewLine + "A";
                Traveler.InsanityWriteLog(msg);

                Sims3.Gameplay.UI.Responder.GameStartup();

                mEnteredFromTravelling = GameStates.IsTravelling;

                bool flag = false;

                try
                {
                    ModalDialog.EnableModalDialogs = false;

                    if (World.IsEditInGameFromWBMode())
                    {
                        Sims3.Gameplay.Autonomy.Autonomy.DisableMoodContributors = true;
                    }

                    msg += Common.NewLine + "B";
                    Traveler.InsanityWriteLog(msg);

                    mStateMachine = StateMachine.Create(0x1, "InWorld");
                    mSubStates[0] = new LiveModeStateEx(GameStates.IsMovingWorlds);  // Custom
                    mSubStates[1] = new BuildModeState();
                    mSubStates[2] = new BuyModeState();
                    mSubStates[12] = new ShoppingModeState();
                    mSubStates[3] = new CASFullModeState();
                    mSubStates[4] = new CASDresserModeState();
                    mSubStates[5] = new CASMirrorModeState();
                    mSubStates[6] = new CASTattooModeState();
                    mSubStates[7] = new CASStylistModeState();
                    mSubStates[8] = new CASTackModeState();
                    mSubStates[9] = new CASCollarModeState();
                    mSubStates[10] = new CASSurgeryFaceModeState();
                    mSubStates[11] = new CASSurgeryBodyModeState();
                    mSubStates[15] = new PlayFlowStateEx(); // Custom
                    mSubStates[14] = new EditTownStateEx(); // Custom
                    mSubStates[16] = new BlueprintModeState();
                    mSubStates[17] = new CASMermaidModeState();
                    mSubStates[0x12] = new CABModeState();
                    mSubStates[0x13] = new CABModeEditState();

                    foreach (InWorldSubState state in mSubStates)
                    {
                        mStateMachine.AddState(state);
                    }

                    msg += Common.NewLine + "C";
                    Traveler.InsanityWriteLog(msg);

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
                    else
                    {
                        CrossWorldControl.sRetention.RestoreHouseholds();
                    }

                    msg += Common.NewLine + "D";
                    Traveler.InsanityWriteLog(msg);

                    if (GameStates.IsTravelling)
                    {
                        msg += Common.NewLine + "E1";
                        Traveler.InsanityWriteLog(msg);

                        bool fail = false;
                        if (!GameStatesEx.ImportTravellingHousehold())
                        {
                            msg += Common.NewLine + "E3";

                            fail = true;
                        }

                        if ((GameStates.TravelHousehold == null) && (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.StartVacation))
                        {
                            msg += Common.NewLine + "E4";

                            fail = true;
                        }

                        if (fail)
                        {
                            msg += Common.NewLine + "E5";
                            Traveler.InsanityWriteLog(msg);

                            GameStates.sStartupState = SubState.EditTown;

                            GameStates.ClearTravelStatics();

                            WorldData.SetVacationWorld(true, true);
                        }
                    }
                    else if ((!GameStates.IsEditingOtherTown) && (GameStates.HasTravelData) && (GameStates.TravelHousehold == null))
                    {
                        switch (GameUtils.GetCurrentWorldType())
                        {
                            case WorldType.Base:
                            case WorldType.Downtown:
                                msg += Common.NewLine + "E2";
                                Traveler.InsanityWriteLog(msg);

                                GameStates.ClearTravelStatics();
                                break;
                        }
                    }

                    // Custom
                    if (GameStates.sMovingWorldData != null)
                    {
                        Household.IsTravelImport = true;
                    }

                    msg += Common.NewLine + "F1";
                    Traveler.InsanityWriteLog(msg);

                    List<Household> households = new List<Household>(Household.sHouseholdList);
                    foreach (Household a in households)
                    {
                        if ((a.LotHome != null) && (a.LotHome.Household == null))
                        {
                            a.LotHome.mHousehold = a;
                        }

                        foreach (SimDescription simA in Households.All(a))
                        {
                            // Must be validated prior to SimDescription:PostLoadFixup()
                            if (simA.GameObjectRelationships != null)
                            {
                                for(int index=simA.GameObjectRelationships.Count-1; index >=0; index--)
                                {
                                    if ((simA.GameObjectRelationships[index] == null) ||
                                        (simA.GameObjectRelationships[index].GameObjectDescription == null) ||
                                        (simA.GameObjectRelationships[index].GameObjectDescription.GameObject == null) ||
                                        (!Objects.IsValid(simA.GameObjectRelationships[index].GameObjectDescription.GameObject.ObjectId)))
                                    {
                                        simA.GameObjectRelationships.RemoveAt(index);
                                    }
                                }
                            }

                            foreach (Household b in households)
                            {
                                if (a == b) continue;

                                if (!b.Contains(simA)) continue;

                                if (b.NumMembers == 1) continue;

                                try
                                {
                                    b.Remove(simA, false);

                                    msg += Common.NewLine + "Duplicate: " + simA.FullName;
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(simA, e);
                                }
                            }
                        }
                    }

                    msg += Common.NewLine + "F2";
                    Traveler.InsanityWriteLog(msg);

                    // Required to ensure that all homeworld specific data is fixed up properly (specifically careers)
                    using (BaseWorldReversion reversion = new BaseWorldReversion())
                    {
                        // Reset this, to allow the Seasons Manager to activate properly
                        SeasonsManager.sSeasonsValidForWorld = SeasonsManager.Validity.Undetermined;

                        try
                        {
                            mPostWorldInitializers = new Initializers("PostWorldInitializers", this);

                            using (CareerStore store = new CareerStore())
                            {
                                //InitializersEx.Initialize(mPostWorldInitializers);
                                mPostWorldInitializers.Initialize();
                            }
                        }
                        catch (Exception e)
                        {
                            Traveler.InsanityException(msg, e);
                        }
                    }

                    msg += Common.NewLine + "G1";
                    Traveler.InsanityWriteLog(msg);

                    try
                    {
                        if (GameStates.TravelHousehold != null)
                        {
                            LinkToTravelHousehold();

                            WorldName worldName = GameUtils.GetCurrentWorld();

                            switch (worldName)
                            {
                                case WorldName.China:
                                case WorldName.Egypt:
                                case WorldName.France:
                                    break;
                                default:
                                    foreach (SimDescription sim in Households.All(GameStates.TravelHousehold))
                                    {
                                        if (sim.VisaManager == null) continue;

                                        WorldData.OnLoadFixup(sim.VisaManager);

                                        sim.VisaManager.SetVisaLevel(worldName, 3);
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Traveler.InsanityException(msg, e);
                    }

                    msg += Common.NewLine + "G2";
                    Traveler.InsanityWriteLog(msg);

                    List<SimDescription> dyingSims = null;
                    if (GameStates.IsTravelling)
                    {
                        dyingSims = GameStatesEx.PostTravelingFixUp();
                    }

                    msg += Common.NewLine + "H";
                    Traveler.InsanityWriteLog(msg);

                    if (GameStates.IsEditingOtherTown)
                    {
                        GameStates.PostLoadEditTownFixup();
                    }

                    msg += Common.NewLine + "I";
                    Traveler.InsanityWriteLog(msg);

                    // We must stop the travel actions from running if the homeworld is an EA vacation world
                    WorldData.SetVacationWorld(true, false);

                    GameStates.NullEditTownDataDataIfEditingOriginalStartingWorld();
                    try
                    {
                        if (GameUtils.IsAnyTravelBasedWorld())
                        {
                            if ((dyingSims != null) && (AgingManager.NumberAgingYearsElapsed != -1f))
                            {
                                float yearsGone = GameStates.NumberAgingYearsElapsed - AgingManager.NumberAgingYearsElapsed;

                                // Custom function
                                FixUpAfterTravel(yearsGone, dyingSims);
                            }
                            AgingManager.NumberAgingYearsElapsed = GameStates.NumberAgingYearsElapsed;
                        }
                    }
                    catch (Exception e)
                    {
                        Traveler.InsanityException(msg, e);
                    }

                    msg += Common.NewLine + "J";
                    Traveler.InsanityWriteLog(msg);

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
                    Traveler.InsanityWriteLog(msg);

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
                    Traveler.InsanityWriteLog(msg);

                    Sims3.Gameplay.Gameflow.SetGameSpeed(pause, Sims3.Gameplay.Gameflow.SetGameSpeedContext.GameStates);
                    NotificationManager.Load();
                    MainMenu.TriggerPendingFsiWorldNotifications();
                }
                finally
                {
                    ModalDialog.EnableModalDialogs = true;
                }

                if (SocialFeatures.Accounts.IsLoggedIn())
                {
                    Notify(0, 0, 0L);
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
                        // Custom
                        if (((!GameUtils.IsOnVacation() && !GameUtils.IsFutureWorld()) || EditTownModel.IsAnyLotBaseCampStatic() || EditTownModelEx.IsAnyUnoccupiedLotStatic()) && flag)
                        {
                            bool directToGame = false;
                            if (!GameStates.IsTravelling) 
                            {
                                // Entering an existing save
                                directToGame = true;
                            }
                            else if (EditTownModel.IsAnyLotBaseCampStatic())
                            {
                                // Normal transition to base camp
                                directToGame = true;
                            }
                            else if (!GameUtils.IsInstalled(ProductVersion.EP9))
                            {
                                // Use custom household selection
                                directToGame = true;
                            }

                            // Custom
                            if ((flag) && (directToGame))
                            {
                                msg += Common.NewLine + "StartupState: LiveMode (A)";

                                GotoLiveMode();
                                break;
                            }
                            else
                            {
                                msg += Common.NewLine + "StartupState: LiveMode (C)";

                                GameUtils.EnableSceneDraw(true);
                                LoadingScreenController.Unload();
                                GotoPlayFlow();
                                break;
                            }
                        }

                        msg += Common.NewLine + "StartupState: LiveMode (B)";

                        GameUtils.EnableSceneDraw(true);
                        LoadingScreenController.Unload();
                        GotoEditTown();
                        break;
                }

                msg += Common.NewLine + "M";
                Traveler.InsanityWriteLog(msg);

                if (Sims3.Gameplay.Gameflow.sShowObjectReplacedWarning)
                {
                    SimpleMessageDialog.Show(Common.LocalizeEAString("Ui/Warning/LoadGame:Warning"), Common.LocalizeEAString("Ui/Warning/LoadGame:ReplacedObjects"), ModalDialog.PauseMode.PauseSimulator);
                    Sims3.Gameplay.Gameflow.sShowObjectReplacedWarning = false;
                }
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
            finally
            {
                //Common.WriteLog(msg);
            }
        }
    }
}
