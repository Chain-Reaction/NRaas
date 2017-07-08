using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Stores;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class GameStatesEx
    {
        [PersistableStatic]
        static Dictionary<ulong, bool> sAllTravelIds = new Dictionary<ulong, bool>();

        [PersistableStatic]
        static Dictionary<ulong, bool> sResidentIds = new Dictionary<ulong, bool>();

        public static void StartLoadingScreen(bool isReturningHome)
        {
            try
            {
                PersistStatic.MainMenuLoading = false;
                GameStates.EnsureNoModalDialogsUp();
                GameUtils.EnableSceneDraw(false);
                if (GameStates.DestinationTravelWorld != WorldName.Undefined)
                {
                    string travelWorldName = Sims3.Gameplay.UI.Responder.Instance.HudModel.LocationName(GameStates.DestinationTravelWorld, true);
                    if (isReturningHome)
                    {
                        travelWorldName = GameStates.sTravelData.mHomeWorld;
                    }
                    bool isFirstTimeTravelingToFuture = (GameStates.DestinationTravelWorld == WorldName.FutureWorld) && ((CauseEffectService.GetInstance() != null) && (CauseEffectService.GetInstance().GetTimesTraveledToFuture() == 0));
                    LoadingScreenController.LoadTravellingLoadingScreen(travelWorldName, GameStates.DestinationTravelWorld, isReturningHome, isFirstTimeTravelingToFuture);
                }
                SpeedTrap.Sleep(0);
            }
            catch (Exception e)
            {
                Common.Exception("StartLoadingScreen", e);
            }
        }

        public static void MoveToNewWorld(string newWorldName, List<ulong> travelerIds, bool wasGameSaved, bool packFurniture)
        {
            GameStates.StopOperationsThatCannotTravel();
            GameStates.sMovingWorldData = new GameStates.MovingWorldData();
            GameStates.sIsMovingWorlds = true;
            GameStates.sMovingWorldData.mDestWorld = newWorldName;
            GameStates.sMovingWorldData.mTravelHouse = Household.ActiveHousehold;
            GameStates.sMovingWorldData.mGameSaved = wasGameSaved;
            GameStates.sMovingWorldData.mFurniturePacked = packFurniture;
            if (SeasonsManager.Enabled)
            {
                GameStates.sMovingWorldData.mCurrentSeason = SeasonsManager.CurrentSeason;
                int index = 0x0;
                foreach (Season season in Enum.GetValues(typeof(Season)))
                {
                    GameStates.sMovingWorldData.mSeasonEnabled[index] = SeasonsManager.GetSeasonEnabled(season);
                    GameStates.sMovingWorldData.mSeasonLength[index] = SeasonsManager.GetSeasonLength(season);
                    index++;
                }

                index = 0x0;
                foreach (Weather weather in Enum.GetValues(typeof(Weather)))
                {
                    GameStates.sMovingWorldData.mWeatherEnabled[index] = SeasonsManager.IsWeatherEnabled(weather);
                    index++;
                }

                GameStates.sMovingWorldData.mIsCelcius = Sims3.UI.Responder.Instance.OptionsModel.IsCelcius;
            }

            GameStates.sMovingWorldData.mTicksToAdvance = SimClock.CurrentTicks - SimClock.TicksSinceHourOfDay(0f);

            CrossWorldControl.Store(Households.All(Household.ActiveHousehold));

            Common.FunctionTask.Perform(MoveToWorld);
        }

        private static void MoveToWorld()
        {
            PersistStatic.MainMenuLoading = false;
            GameStates.EnsureNoModalDialogsUp();
            GameUtils.EnableSceneDraw(false);
            WorldFileMetadata info = new WorldFileMetadata();
            info.mWorldFile = GameStates.sMovingWorldData.mDestWorld;
            Sims3.UI.Responder.Instance.MainMenuModel.GetWorldFileDetails(ref info);
            LoadingScreenController.LoadNewGameLoadingScreen(info);

            SpeedTrap.Sleep();

            CameraController.DisableObjectFollow();
            bool furniturePacked = GameStates.sMovingWorldData.mFurniturePacked;
            Lot lotHome = GameStates.sMovingWorldData.mTravelHouse.LotHome;
            Dictionary<ObjectGuid, ObjectGuid> originalsToClones = null;
            if (furniturePacked && (lotHome != null))
            {
                GameplayMovingModel.PackFurniture(lotHome, GameStates.sMovingWorldData.mTravelHouse, ref originalsToClones);
            }

            // Transfer data to cross-world lookup
            WorldData.MergeToCrossWorldData();

            // Custom
            MiniSimDescriptionEx.AddMiniSims();

            // Custom
            foreach (SimDescription sim in Households.All(GameStates.sMovingWorldData.mTravelHouse))
            {
                Corrections.CleanupBrokenSkills(sim, null);
            }

            // Custom
            string packageName = BinEx.ExportHousehold(Bin.Singleton, GameStates.sMovingWorldData.mTravelHouse, false, furniturePacked);
            if (packageName != null)
            {
                ulong num = BinModel.Singleton.AddToExportBin(packageName);
                GameStates.sMovingWorldData.binExportId = num;
            }

            GameStates.sOldActiveHousehold = null;
            GameStates.GotoState(GameState.MoveOtherWorld);
        }

        public static void TravelToVacationWorld(WorldName newWorldName, List<ulong> travelerIds, int tripLength, int numDaysSinceLastInDestWorld)
        {
            GameStates.StopOperationsThatCannotTravel();

            GameStates.TravelData origTravelData = GameStates.sTravelData;

            if (origTravelData == null)
            {
                if (newWorldName == WorldName.FutureWorld)
                {
                    CauseEffectService.GetInstance().PreFutureWorldLoadProcess();
                    FutureDescendantServiceEx.BuildDescendantHouseholdSpecs(FutureDescendantService.GetInstance());
                }
            }

            GameStates.sTravelData = new GameStates.TravelData();
            GameStates.sIsChangingWorlds = true;
            GameStates.sTravelData.mState = GameStates.TravelData.TravelState.StartVacation;

            if (origTravelData != null)
            {
                GameStates.sTravelData.mHomeWorld = origTravelData.mHomeWorld;
                GameStates.sTravelData.mHomeWorldMetadataName = origTravelData.mHomeWorldMetadataName;
                GameStates.sTravelData.mTimeInHomeworld = origTravelData.mTimeInHomeworld;
                GameStates.sTravelData.mRealEstateManager = origTravelData.mRealEstateManager;
                GameStates.sTravelData.mNumDaysSinceLastInDestWorld = origTravelData.mNumDaysSinceLastInDestWorld;
            }
            else
            {
                GameStates.sTravelData.mHomeWorld = GameStates.GetCurrentWorldName(true);
                GameStates.sTravelData.mHomeWorldMetadataName = GameStates.GetCurrentWorldName(false);
                GameStates.sTravelData.mTimeInHomeworld = SimClock.CurrentTime();
                GameStates.sTravelData.mRealEstateManager = Household.ActiveHousehold.RealEstateManager;
                GameStates.sTravelData.mRealEstateManager.SavePropertyNamesForTravel();
                GameStates.sTravelData.mNumDaysSinceLastInDestWorld = numDaysSinceLastInDestWorld;
            }

            GameStates.sTravelData.mTravelerIds = travelerIds;
            GameStates.sTravelData.mDestWorld = newWorldName;
            GameStates.sTravelData.mTripLength = tripLength;
            GameStates.sTravelData.mCurrentDayOfTrip = 1;

            if (AgingManager.NumberAgingYearsElapsed != -1f)
            {
                GameStates.sTravelData.mNumberAgingYearsElapsed = AgingManager.NumberAgingYearsElapsed;
            }
            else
            {
                GameStates.sTravelData.mNumberAgingYearsElapsed = 0f;
            }

            List<SimDescription> sims = new List<SimDescription>(Households.All(Household.ActiveHousehold));
            
            Dictionary<ulong,SimDescription> lookup = SimListing.GetResidents(false);

            foreach (ulong id in travelerIds)
            {
                SimDescription sim;
                if (!lookup.TryGetValue(id, out sim)) continue;

                if (sims.Contains(sim)) continue;

                sims.Add(sim);
            }

            CrossWorldControl.sRetention.OnSwitchWorlds(sims);

            CrossWorldControl.Store(sims);

            Common.FunctionTask.Perform(SwitchWorlds);
        }

        private static bool SetupLoadFileName(bool useTravelData)
        {
            WorldName name = useTravelData ? GameStates.sTravelData.mDestWorld : GameStates.sEditOtherWorldData.mDestWorld;

            if ((name == WorldName.FutureWorld) && (GameStates.ShouldResetTheFuture))
            {
                GameStates.sLoadFileName = "Oasis Landing.world";
                GameStates.ShouldResetTheFuture = false;
                return true;
            }
            else
            {
                return WorldData.GetLoadFileName(name, useTravelData);
            }
        }

        private static void VacationArrival(Household travelHouse)
        {
            Common.StringBuilder msg = new Common.StringBuilder("VacationArrival");
            Traveler.InsanityWriteLog(msg);

            foreach (SimDescription description in Households.All(travelHouse))
            {
                try
                {
                    msg += Common.NewLine + description.FullName;
                    Traveler.InsanityWriteLog(msg);

                    WorldName currentWorld = GameUtils.GetCurrentWorld();
                    description.VisaManager.UpdateWorldVisit(currentWorld);
                    if (description.CreatedSim != null)
                    {
                        msg += Common.NewLine + "A";
                        Traveler.InsanityWriteLog(msg);

                        Corrections.CleanupBrokenSkills(description, null);

                        msg += Common.NewLine + "B";
                        Traveler.InsanityWriteLog(msg);

                        using (SafeStore store = new SafeStore(description, SafeStore.Flag.None))
                        {
                            if (!Household.RoommateManager.IsNPCRoommate(description))
                            {
                                description.CreatedSim.OnBecameSelectable();
                            }
                        }

                        msg += Common.NewLine + "C";
                        Traveler.InsanityWriteLog(msg);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(description, e);
                }
            }

            msg += Common.NewLine + "D";
            Traveler.InsanityWriteLog(msg);

            // Custom
            UnstashHousehold(GameStates.sTravelData.mStache, travelHouse);

            msg += Common.NewLine + "E";
            Traveler.InsanityWriteLog(msg);

            travelHouse.RealEstateManager = GameStates.sTravelData.mRealEstateManager;
            //travelHouse.SetupVacationAlarms();

            // From Household:SetupVacationAlarms
            HouseholdEx.OneDayPassed(travelHouse);
            travelHouse.mTriggerVacationEventAlarm = AlarmManager.Global.AddAlarm(Household.kOnVacationTriggerTime, TimeUnit.Minutes, travelHouse.OnTriggerVacationEvent, "Initial On Vacation Trigger", AlarmType.AlwaysPersisted, travelHouse);

            msg += Common.NewLine + "F";
            Traveler.InsanityWriteLog(msg);

            if (GameUtils.IsOnVacation())
            {
                WorldData.ForceSpawners();
            }

            msg += Common.NewLine + "G";
            Traveler.InsanityWriteLog(msg);
        }

        private static void UnstashHousehold(StacheManager stache, Household household)
        {
            Common.StringBuilder msg = new Common.StringBuilder("UnstashHousehold");
            Traveler.InsanityWriteLog(msg);

            if ((stache != null) && (household != null))
            {
                foreach (Sim sim in household.AllActors)
                {
                    msg += Common.NewLine + sim.FullName;
                    Traveler.InsanityWriteLog(msg);

                    if (GameStates.IsIdTravelling(sim.SimDescription.SimDescriptionId))
                    {
                        msg += Common.NewLine + "A";
                        Traveler.InsanityWriteLog(msg);

                        string str = sim.SimDescription.SimDescriptionId.ToString();
                        IPhone phone = sim.Inventory.Find<IPhone>();

                        msg += Common.NewLine + "B";
                        Traveler.InsanityWriteLog(msg);

                        IPhoneSmart smart = phone as IPhoneSmart;
                        if (smart != null)
                        {
                            msg += Common.NewLine + "C";
                            Traveler.InsanityWriteLog(msg);

                            ulong guid = StacheManager.CreateKey<PhoneSkins>(str + "PhoneSkin");
                            if (stache.HasItem(guid))
                            {
                                msg += Common.NewLine + "D";
                                Traveler.InsanityWriteLog(msg);

                                PhoneSkins item = (PhoneSkins)stache.GetItem(guid);
                                smart.SetPhoneSkin(item);
                            }

                            msg += Common.NewLine + "E";
                            Traveler.InsanityWriteLog(msg);

                            guid = StacheManager.CreateKey<bool>(str + "BrokenState");
                            if (stache.HasItem(guid))
                            {
                                msg += Common.NewLine + "F";
                                Traveler.InsanityWriteLog(msg);

                                bool flag = (bool)stache.GetItem(guid);
                                smart.IsBroken = flag;
                            }
                        }

                        IPhoneCell cell = phone as IPhoneCell;
                        if (cell != null)
                        {
                            msg += Common.NewLine + "G";
                            Traveler.InsanityWriteLog(msg);

                            ulong num2 = StacheManager.CreateKey<uint>(str + "RingTone");
                            if (stache.HasItem(num2))
                            {
                                msg += Common.NewLine + "H";
                                Traveler.InsanityWriteLog(msg);

                                uint key = (uint)stache.GetItem(num2);
                                cell.SetRingtoneWithoutAudio(key);
                            }
                        }
                    }
                }

                msg += Common.NewLine + "I";
                Traveler.InsanityWriteLog(msg);
            }
        }

        // Externalized to Register
        public static bool WasActiveFamilyMember(ulong id)
        {
            if (sAllTravelIds == null) return false;

            return sAllTravelIds.ContainsKey(id);
        }

        // Externalized to Register
        public static bool IsHomeworldResident(ulong id)
        {
            if (sResidentIds == null) return false;

            return sResidentIds.ContainsKey(id);
        }

        public static bool ImportTravellingHousehold()
        {
            Common.StringBuilder msg = new Common.StringBuilder("ImportTravellingHousehold" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            try
            {
                // Store in persistence
                CrossWorldControl.sRetention.OnImportHousehold();

                // Must be set prior to selecting a lot for the traveling family
                WorldData.SetVacationWorld(true, true);

                GameStates.sTravelData.mDeadSims = new List<ulong>(GameStates.sTravelData.mTravelerIds);

                msg += Common.NewLine + "A";
                Traveler.InsanityWriteLog(msg);

                sAllTravelIds.Clear();

                Dictionary<ulong, SimDescription> travelers = new Dictionary<ulong, SimDescription>();

                Household travelHouse = null;
                HouseholdContents contents = null;

                if (GameStates.sTravelData.mTravelerIds.Count > 0)
                {
                    try
                    {
                        Household.IsTravelImport = true;

                        // Faking a Base world game ensures that all the data is imported (careers specifically)
                        using (BaseWorldReversion reversion = new BaseWorldReversion())
                        {
                            contents = BinEx.ImportHouseholdForTravel();
                        }
                    }
                    finally
                    {
                        Household.IsTravelImport = false;
                    }

                    msg += Common.NewLine + "B";
                    Traveler.InsanityWriteLog(msg);

                    if (contents == null)
                    {
                        msg += Common.NewLine + "contents == null";
                    }

                    travelHouse = contents.Household;

                    if (travelHouse == null)
                    {
                        msg += Common.NewLine + "travelHouse = null";
                    }
                    else if (travelHouse.AllSimDescriptions == null)
                    {
                        msg += Common.NewLine + "travelHouse.AllSimDescriptions = null";
                    }

                    for (int i = travelHouse.AllSimDescriptions.Count - 1; i >= 0; i--)
                    {
                        SimDescription sim = travelHouse.AllSimDescriptions[i];

                        msg += Common.NewLine + sim.FullName;

                        CrossWorldControl.Restore(sim);

                        if (sim == null)
                        {
                            travelHouse.CurrentMembers.RemoveAt(i);
                        }
                        else
                        {
                            MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
                            if (miniSim != null)
                            {
                                miniSim.Instantiated = true;
                                miniSim.mHomeWorld = sim.HomeWorld;
                            }

                            travelers[sim.SimDescriptionId] = sim;

                            if ((Traveler.Settings.mSetAsUnselectable) && 
                                (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.StartVacation))
                            {
                                if (!CrossWorldControl.sRetention.mTrueActives.ContainsKey(sim.SimDescriptionId))
                                {
                                    sim.SetFlags(SimDescription.FlagField.IsNeverSelectable, true);
                                }
                            }
                        }

                        sAllTravelIds[sim.SimDescriptionId] = true;
                    }

                    CrossWorldControl.Clear();
                }

                msg += Common.NewLine + "C1";
                Traveler.InsanityWriteLog(msg);

                Household.CreatePreviousTravelerHousehold();

                msg += Common.NewLine + "C2";
                Traveler.InsanityWriteLog(msg);

                if ((!GameStates.IsNewGame) && (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.StartVacation))
                {
                    List<SimDescription> list = new List<SimDescription>();
                    WorldName currentWorld = GameUtils.GetCurrentWorld();

                    foreach (SimDescription sim in Household.EverySimDescription())
                    {
                        try
                        {
                            SimDescription traveler = null;
                            if (travelers.TryGetValue(sim.SimDescriptionId, out traveler))
                            {
                                if (object.ReferenceEquals(traveler, sim)) continue;

                                list.Add(sim);
                            }
                            else
                            {
                                MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
                                if (miniSim != null)
                                {
                                    if (WorldData.IsFromDifferentWorld(sim.SimDescriptionId))
                                    {
                                        list.Add(sim);
                                    }

                                    if (!GameStates.IsIdTravelling(sim.SimDescriptionId))
                                    {
                                        miniSim.Instantiated = true;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, null, msg, e);
                        }
                    }

                    msg += Common.NewLine + "C3";
                    Traveler.InsanityWriteLog(msg);

                    foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                    {
                        urnstone.UpdateInstantiatedField();
                    }

                    msg += Common.NewLine + "C4";
                    Traveler.InsanityWriteLog(msg);

                    foreach (SimDescription sim in list)
                    {
                        try
                        {
                            msg += Common.NewLine + sim.FullName;

                            if (sim.CreatedSim != null)
                            {
                                msg += Common.NewLine + "Destroy";

                                sim.CreatedSim.Destroy();
                                SpeedTrap.Sleep();
                            }

                            if (Household.PreviousTravelerHousehold != sim.Household)
                            {
                                msg += Common.NewLine + "Remove";

                                sim.Household.Remove(sim, !sim.Household.IsSpecialHousehold);

                                Household.PreviousTravelerHousehold.AddTemporary(sim);
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, null, msg, e);
                        }
                    }
                }

                WorldData.MergeFromCrossWorldData();

                GameStates.TravelData.TravelState travelState = GameStates.sTravelData.mState;

                if (travelHouse != null)
                {
                    switch (travelState)
                    {
                        case GameStates.TravelData.TravelState.StartVacation:
                            GameStates.sTravelData.mTravelHouse = travelHouse;

                            msg += Common.NewLine + "Start D";
                            Traveler.InsanityWriteLog(msg);

                            List<SimDescription> list2 = new List<SimDescription>();

                            uint familyIndex = (uint)travelHouse.AllSimDescriptions.Count;

                            for (int travelIndex = travelHouse.AllSimDescriptions.Count - 1; travelIndex >= 0; travelIndex--)
                            {
                                SimDescription travelSim = travelHouse.AllSimDescriptions[travelIndex];

                                msg += Common.NewLine + "Try: " + travelSim.FullName;

                                try
                                {
                                    if (!GameStates.IsIdTravelling(travelSim.SimDescriptionId))
                                    {
                                        // Sim did not make the trip

                                        if (travelSim.IsHuman)
                                        {
                                            GameStates.sTravelData.mNumLeftBehindPlusPregnancy++;
                                            if (travelSim.Pregnancy != null)
                                            {
                                                GameStates.sTravelData.mNumLeftBehindPlusPregnancy++;
                                            }
                                        }
                                        else
                                        {
                                            GameStates.sTravelData.mNumPetsLeftBehindPlusPregnancy++;
                                            if (travelSim.Pregnancy != null)
                                            {
                                                GameStates.sTravelData.mNumPetsLeftBehindPlusPregnancy++;
                                            }
                                        }

                                        travelHouse.CurrentMembers.RemoveAt(travelIndex);

                                        msg += Common.NewLine + "Dispose";

                                        travelSim.Dispose();
                                    }
                                    else
                                    {
                                        ulong inventoryIndex = contents.Inventories[travelIndex];

                                        SimDescription existingSim = Household.PreviousTravelerHousehold.FindMember(travelSim.SimDescriptionId);

                                        if (existingSim != null)
                                        {
                                            existingSim.Fixup();

                                            // Existing sim located in town

                                            msg += Common.NewLine + "Found: " + existingSim.FullName;
                                            Traveler.InsanityWriteLog(msg);

                                            if (BinEx.ImportSim(existingSim, Vector3.OutOfWorld, inventoryIndex))
                                            {
                                                // Moved, as Imaginary Friends require that the sim be Created before altering occult
                                                SimDescriptionEx.MergeTravelInformation(existingSim, travelSim, false);

                                                msg += Common.NewLine + "  Imported";
                                                Traveler.InsanityWriteLog(msg);

                                                if ((existingSim.CreatedSim != null) && (existingSim.CreatedSim.BuffManager != null))
                                                {
                                                    existingSim.CreatedSim.BuffManager.LoadBuffsFromTravel(travelSim);
                                                }

                                                existingSim.MergePregnancyInformation(travelSim);

                                                travelHouse.CurrentMembers.RemoveAt(travelIndex);
                                                travelSim.DisposeTravelImportedSimDesc();

                                                list2.Add(existingSim);
                                                GameStates.GhostSetup(existingSim);
                                            }
                                        }
                                        else
                                        {
                                            // Brand new sim in town
                                            msg += Common.NewLine + "New: " + travelSim.FullName;
                                            Traveler.InsanityWriteLog(msg);

                                            if (BinEx.ImportSim(travelSim, Vector3.OutOfWorld, inventoryIndex))
                                            {
                                                msg += Common.NewLine + "  Imported";
                                                Traveler.InsanityWriteLog(msg);

                                                GameStates.GhostSetup(travelSim);
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(travelSim, null, msg, e);
                                }
                            }

                            msg += Common.NewLine + "E";
                            Traveler.InsanityWriteLog(msg);

                            Bin.ImportHouseholdInventories(contents, travelHouse, familyIndex, false);
                            foreach (SimDescription sim in list2)
                            {
                                try
                                {
                                    Household.PreviousTravelerHousehold.RemoveSimKeepHousehold(sim);
                                    travelHouse.AddSilent(sim);
                                    sim.OnHouseholdChanged(travelHouse, false);
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim, null, msg, e);
                                }
                            }

                            msg += Common.NewLine + "F";
                            Traveler.InsanityWriteLog(msg);

                            // Calls custom version
                            VacationArrival(travelHouse);

                            msg += Common.NewLine + "G";
                            Traveler.InsanityWriteLog(msg);

                            break;
                        case GameStates.TravelData.TravelState.EndVacation:

                            msg += Common.NewLine + "End D";
                            Traveler.InsanityWriteLog(msg);

                            // Custom function
                            WorldData.ResetWorldType();

                            List<SimDescription> carTravelers = new List<SimDescription>();
                            List<SimDescription> earlyDepartureSims = null;

                            SimDescription restoreSim = null;

                            int index = -1;
                            foreach (SimDescription travelSim in Households.All(travelHouse))
                            {
                                msg += Common.NewLine + travelSim.FullName;
                                Traveler.InsanityWriteLog(msg);

                                try
                                {
                                    GameStates.sTravelData.mDeadSims.Remove(travelSim.SimDescriptionId);
                                    index++;

                                    SimDescription item = SimDescription.Find(travelSim.SimDescriptionId);

                                    bool ageChanged = travelSim.AgeGenderSpecies != item.AgeGenderSpecies;

                                    Sim createdSim = item.CreatedSim;
                                    if (createdSim == null)
                                    {
                                        msg += Common.NewLine + "D1";
                                        Traveler.InsanityWriteLog(msg);

                                        if (GameStates.sTravelData.mSimDescriptionNeedPostFixUp == null)
                                        {
                                            GameStates.sTravelData.mSimDescriptionNeedPostFixUp = new List<SimDescription>();
                                        }
                                        GameStates.sTravelData.mSimDescriptionNeedPostFixUp.Add(item);

                                        createdSim = Instantiation.Perform(item, Vector3.OutOfWorld, null, null);
                                    }
                                    else
                                    {
                                        msg += Common.NewLine + "D2";
                                        Traveler.InsanityWriteLog(msg);

                                        if (ageChanged)
                                        {
                                            if (GameStates.sTravelData.mSimDescriptionAgedUpFixUp == null)
                                            {
                                                GameStates.sTravelData.mSimDescriptionAgedUpFixUp = new List<GameStates.SimRefreshData>();
                                            }
                                            GameStates.sTravelData.mSimDescriptionAgedUpFixUp.Add(new GameStates.SimRefreshData(item, travelSim, contents.Inventories[index], createdSim.Position, Sim.ActiveActor == createdSim));
                                            createdSim.Destroy();
                                            createdSim = null;
                                            item.CreatedSim = null;
                                        }
                                        else if (createdSim.Household.IsTouristHousehold)
                                        {
                                            if (GameStates.sTravelData.mSimDescriptionNeedPostFixUp == null)
                                            {
                                                GameStates.sTravelData.mSimDescriptionNeedPostFixUp = new List<SimDescription>();
                                            }
                                            GameStates.sTravelData.mSimDescriptionNeedPostFixUp.Add(item);
                                        }

                                        msg += Common.NewLine + "D3";
                                        Traveler.InsanityWriteLog(msg);

                                        SimDescriptionEx.MergeTravelInformation(item, travelSim, true);

                                        msg += Common.NewLine + "D4";
                                        Traveler.InsanityWriteLog(msg);

                                        if (createdSim != null)
                                        {
                                            createdSim.BuffManager.LoadBuffsFromTravel(travelSim);
                                        }

                                        msg += Common.NewLine + "D5";
                                        Traveler.InsanityWriteLog(msg);

                                        item.MergePregnancyInformation(travelSim);
                                        if ((createdSim != null) && (GameStates.ReturningFromWorld == WorldType.Vacation))
                                        {
                                            createdSim.BuffManager.AddElement(BuffNames.WentToLocation, Origin.FromVisitingLocation);
                                            BuffWentToLocation.BuffInstanceWentToLocation element = createdSim.BuffManager.GetElement(BuffNames.WentToLocation) as BuffWentToLocation.BuffInstanceWentToLocation;
                                            if (element != null)
                                            {
                                                element.SetVacationWorldName(GameStates.sTravelData.mDestWorld);
                                            }
                                        }
                                    }

                                    msg += Common.NewLine + "ImportSim";
                                    Traveler.InsanityWriteLog(msg);

                                    if (createdSim != null)
                                    {
                                        BinEx.ImportSim(item, Vector3.OutOfWorld, contents.Inventories[index]);
                                    }

                                    if (!CrossWorldControl.sRetention.IsRestoree(item))
                                    {
                                        restoreSim = item;
                                    }

                                    if ((GameStates.sTravelData.mEarlyDepartureIds != null) && GameStates.sTravelData.mEarlyDepartureIds.Contains(item.SimDescriptionId))
                                    {
                                        if (earlyDepartureSims == null)
                                        {
                                            earlyDepartureSims = new List<SimDescription>();
                                        }
                                        earlyDepartureSims.Add(item);
                                    }
                                    else
                                    {
                                        carTravelers.Add(item);
                                    }

                                    if ((GameStates.ReturningFromWorld == WorldType.University) && (item.CareerManager != null))
                                    {
                                        AcademicDegreeManager degreeManager = item.CareerManager.DegreeManager;
                                        if (degreeManager != null)
                                        {
                                            degreeManager.LastTimeAtUniversity = SimClock.ElapsedCalendarDays();
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(travelSim, null, msg, e);
                                }
                            }

                            SpeedTrap.Sleep();

                            msg += Common.NewLine + "E";
                            Traveler.InsanityWriteLog(msg);

                            travelHouse.RealEstateManager = GameStates.sTravelData.mRealEstateManager;
                            ReturnHomeBehaviorInHomeWorld.SetupReturnHomeBehavior(carTravelers, earlyDepartureSims);
                            if (GameStates.sTravelData.mSimDescriptionNeedPostFixUp != null)
                            {
                                Household activeHousehold = Household.ActiveHousehold;

                                foreach (SimDescription sim in GameStates.sTravelData.mSimDescriptionNeedPostFixUp)
                                {
                                    msg += Common.NewLine + sim.FullName;

                                    sim.Household.RemoveSimKeepHousehold(sim);
                                    activeHousehold.AddTemporary(sim);
                                }
                            }

                            msg += Common.NewLine + "F";
                            Traveler.InsanityWriteLog(msg);

                            index++;
                            if (restoreSim != null)
                            {
                                Household household = restoreSim.Household;
                                if (household != null)
                                {
                                    household.SharedFamilyInventory.Inventory.DestroyItems();
                                    SpeedTrap.Sleep();
                                    Bin.ImportHouseholdInventories(contents, household, (uint)index, false);
                                    household.MergeTravelData(travelHouse);

                                    GameStates.sNextSimToSelect = household.AllActors[0];
                                }
                            }

                            msg += Common.NewLine + "G1";
                            Traveler.InsanityWriteLog(msg);

                            for (int i = travelHouse.AllSimDescriptions.Count - 1; i >= 0; i--)
                            {
                                SimDescription sim = travelHouse.AllSimDescriptions[i];

                                msg += Common.NewLine + sim.FullName;

                                try
                                {
                                    sim.Dispose(false);
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim, null, msg, e);
                                }
                            }

                            msg += Common.NewLine + "G2";
                            Traveler.InsanityWriteLog(msg);

                            travelHouse.Destroy();

                            GameStates.ReturningFromWorld = WorldType.Undefined;
                            break;
                    }
                }

                // Must be performed whether there is a traveling household or not
                switch (travelState)
                {
                    case GameStates.TravelData.TravelState.StartVacation:
                        List<SimDescription> list3 = new List<SimDescription>(Households.All(Household.PreviousTravelerHousehold));
                        foreach (SimDescription sim in list3)
                        {
                            try
                            {
                                msg += Common.NewLine + "Disposed: " + sim.FullName + " (" + sim.SimDescriptionId + ")";

                                Household.PreviousTravelerHousehold.RemoveSimKeepHousehold(sim);
                                sim.Dispose(false, false, false);
                            }
                            catch (Exception e)
                            {
                                Common.Exception(sim, null, msg, e);
                            }
                        }

                        break;
                }

                msg += Common.NewLine + "H";
                Traveler.InsanityWriteLog(msg);

                if ((GameStates.sTravelData.mState == GameStates.TravelData.TravelState.EndVacation) && GameStates.sTravelData.mbItemsAddedToInventory)
                {
                    Sim activeActor = Sim.ActiveActor;
                    if (activeActor != null)
                    {
                        activeActor.AddAlarm(2f, TimeUnit.Minutes, activeActor.ShowItemsAddFromTravelTNS, "Display TNS of Added Items", AlarmType.AlwaysPersisted);
                    }
                }

                msg += Common.NewLine + "I";
                Traveler.InsanityWriteLog(msg);

                PersistStatic.MainMenuLoading = true;
                Tutorialette.ResetTutorialetteCoolDownTimer();

                if (GameStates.TravelHousehold != null)
                {
                    ThumbnailManager.GenerateHouseholdThumbnail(GameStates.TravelHousehold.HouseholdId, GameStates.TravelHousehold.HouseholdId, ThumbnailSizeMask.Large);
                }

                return true;
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
                return false;
            }
        }

        public static void UpdateMiniSims(Dictionary<ulong, SimDescription> allSims)
        {
            foreach (MiniSimDescription sim in MiniSimDescription.sMiniSims.Values)
            {
                if (sim.IsDead)
                {
                    sim.mStatusFlags &= ~MiniSimDescription.SimDescriptionStatus.Contactable;
                }

                if (!WorldData.IsFromDifferentWorld(sim.SimDescriptionId)) continue;

                SimDescription simDesc;
                if (!allSims.TryGetValue(sim.SimDescriptionId, out simDesc))
                {
                    sim.Instantiated = false;
                }
                else if (simDesc.CreatedSim != null)
                {
                    sim.Instantiated = true;
                }
            }
        }

        public static Dictionary<ulong, SimDescription> GetAllSims()
        {
            Dictionary<ulong, SimDescription> allSims = new Dictionary<ulong, SimDescription>();
            foreach (SimDescription description in Household.EverySimDescription())
            {
                allSims[description.SimDescriptionId] = description;
            }

            return allSims;
        }

        public static List<SimDescription> PostTravelingFixUp()
        {
            Common.StringBuilder msg = new Common.StringBuilder("PostTravelingFixup" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            List<SimDescription> list = new List<SimDescription>();
            try
            {
                if (GameStates.TravelComplete != null)
                {
                    GameStates.TravelComplete();
                }

                msg += "A";
                Traveler.InsanityWriteLog(msg);

                Dictionary<ulong, SimDescription> allSims = GetAllSims();

                foreach (SimDescription description in allSims.Values)
                {
                    try
                    {
                        MiniSimDescription msd = MiniSimDescription.Find(description.SimDescriptionId);
                        if ((msd != null) && !GameStates.sTravelData.mTravelerIds.Contains(description.SimDescriptionId))
                        {
                            SimDescriptionEx.MergeTravelInformation(description, msd, allSims);
                            if (msd.mDeathStyle != description.DeathStyle)
                            {
                                list.Add(description);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description, null, msg, e);
                    }
                }

                msg += "B1";
                Traveler.InsanityWriteLog(msg);

                UpdateMiniSims(allSims);

                msg += "B2";
                Traveler.InsanityWriteLog(msg);

                foreach (MiniSimDescription description3 in MiniSimDescription.GetChildrenNeedFixUp())
                {
                    try
                    {
                        SimDescription description4 = SimDescription.Find(description3.mMotherDescId);
                        if (((description4 == null) || GameStates.sTravelData.mTravelerIds.Contains(description3.mMotherDescId)) && ((description3.HouseholdMembers != null) && (description3.HouseholdMembers.Count > 0x0)))
                        {
                            foreach (ulong num in description3.HouseholdMembers)
                            {
                                description4 = SimDescription.Find(num);
                                if (description4 != null)
                                {
                                    break;
                                }
                            }
                        }
                        if ((description4 != null) && (description4.Household != null))
                        {
                            SimDescription simDescription = MiniSims.UnpackSim(description3);
                            if (simDescription != null)
                            {
                                description4.Household.Add(simDescription);
                                description3.Instantiated = true;
                                simDescription.MergeTravelInformation(description3);
                                if (description3.mDeathStyle != simDescription.DeathStyle)
                                {
                                    list.Add(simDescription);
                                }
                                else if ((simDescription.CreatedSim == null) && (simDescription.LotHome != null))
                                {
                                    Sim.MakeSimGoHome(Instantiation.PerformOffLot(simDescription, simDescription.LotHome, null), false);
                                }
                                description3.mMotherDescId = 0x0L;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description3, null, msg, e);
                    }
                }

                msg += "C";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.EndVacation)
                {
                    if (GameStates.sTravelData.mSimDescriptionNeedPostFixUp != null)
                    {
                        Household activeHousehold2 = Household.ActiveHousehold;

                        foreach (SimDescription description6 in GameStates.sTravelData.mSimDescriptionNeedPostFixUp)
                        {
                            try
                            {
                                activeHousehold2.RemoveTemporary(description6);

                                MiniSims.ProtectedAddHousehold(activeHousehold2, description6);
                            }
                            catch (Exception e)
                            {
                                Common.Exception(description6, e);
                            }
                        }
                    }

                    if (GameStates.sTravelData.mSimDescriptionAgedUpFixUp != null)
                    {
                        Household household1 = Household.ActiveHousehold;
                        foreach (GameStates.SimRefreshData data in GameStates.sTravelData.mSimDescriptionAgedUpFixUp)
                        {
                            if (data.mInWorldDesc.CreatedSim == null)
                            {
                                Bin.ImportSim(data.mInWorldDesc, data.mPosition, data.mInventoryId);
                                Sim createdSim = data.mInWorldDesc.CreatedSim;
                                createdSim.SwitchToOutfitWithoutSpin(createdSim.CurrentOutfitCategory);
                                createdSim.BuffManager.LoadBuffsFromTravel(data.mImportedDesc);
                                if (data.mIsSelected)
                                {
                                    PlumbBob.SelectActor(createdSim);
                                }
                            }
                            data.mImportedDesc.DisposeTravelImportedSimDesc();
                        }

                        GameStates.sTravelData.mSimDescriptionAgedUpFixUp.Clear();
                    }

                    msg += "D";
                    Traveler.InsanityWriteLog(msg);

                    foreach (SimDescription sim in Households.All(Household.ActiveHousehold))
                    {
                        try
                        {
                            ulong simDescriptionId = sim.SimDescriptionId;
                            MiniSimDescription miniSim = MiniSimDescription.Find(simDescriptionId);
                            if (miniSim != null)
                            {
                                if (!GameStates.sTravelData.mTravelerIds.Contains(simDescriptionId))
                                {
                                    sim.MergeTravelInformation(miniSim);
                                }
                                miniSim.Instantiated = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, null, msg, e);
                        }
                    }

                    msg += "E";
                    Traveler.InsanityWriteLog(msg);

                    if (GameStates.sTravelData.mTravelingRelationships != null)
                    {
                        Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);

                        foreach (GameStates.TravelRelationship relationship in GameStates.sTravelData.mTravelingRelationships)
                        {
                            SimDescription x;
                            if (sims.TryGetValue(relationship.simXId, out x))
                            {
                                for (int i = 0x0; i < relationship.simyIds.Count; i++)
                                {
                                    SimDescription y;
                                    if (sims.TryGetValue(relationship.simyIds[i], out y))
                                    {
                                        Relationship relation = Relationship.Get(x, y, true);
                                        if (relation != null)
                                        {
                                            Relationship relation2 = relationship.rels[i];
                                            if (relation2 != null)
                                            {
                                                relation.CopyRelationship(relation2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        GameStates.sTravelData.mTravelingRelationships.Clear();
                        GameStates.sTravelData.mTravelingRelationships = null;
                    }

                    msg += "F";
                    Traveler.InsanityWriteLog(msg);

                    float timeoutToAdd = 0f;
                    foreach (Sim sim2 in Households.AllSims(Household.ActiveHousehold))
                    {
                        try
                        {
                            string reason = null;
                            if (CrossWorldControl.sRetention.RestoreHousehold(sim2.SimDescription, ref reason))
                            {
                                msg += Common.NewLine + "Restore: " + sim2.FullName;
                            }
                            else
                            {
                                msg += Common.NewLine + "Restore: " + sim2.FullName + " (" + reason + ")";
                            }

                            SimDescription description11 = sim2.SimDescription;
                            ulong item = description11.SimDescriptionId;
                            if (GameStates.sTravelData.mTravelerIds.Contains(item) && !GameStates.sTravelData.mDeadSims.Contains(item))
                            {
                                BuffInstance element = sim2.BuffManager.GetElement(BuffNames.Mourning);
                                if ((element != null) && (element.TimeoutCount > timeoutToAdd))
                                {
                                    timeoutToAdd = element.TimeoutCount;
                                }

                                BuffHeartBroken.BuffInstanceHeartBroken broken = sim2.BuffManager.GetElement(BuffNames.Undefined | BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken;
                                if (((broken != null) && (broken.BuffOrigin == (Origin.FromWitnessingDeath))) && (broken.TimeoutCount > timeoutToAdd))
                                {
                                    timeoutToAdd = broken.TimeoutCount;
                                }

                                MiniSimDescription description12 = MiniSimDescription.Find(item);
                                if (description12 != null)
                                {
                                    foreach (MiniRelationship relationship3 in description12.MiniRelationships)
                                    {
                                        SimDescription description13 = SimDescription.Find(relationship3.SimDescriptionId);
                                        if (description13 != null)
                                        {
                                            Relationship unsafely = Relationship.GetUnsafely(description11, description13);
                                            RomanceVisibilityState.MergeTravelInformationOnTravelBackHome(relationship3, unsafely);
                                        }
                                        else if ((relationship3.RomanceState != RomanceVisibilityStateType.None) && (relationship3.RomanceStartTime.Ticks == 0x0L))
                                        {
                                            MiniSimDescription description14 = MiniSimDescription.Find(relationship3.SimDescriptionId);
                                            if ((description14 != null) && (description14.HomeWorld != description12.HomeWorld))
                                            {
                                                relationship3.RomanceStartTime = SimClock.CurrentTime();
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim2, null, msg, e);
                        }
                    }

                    msg += "G";
                    Traveler.InsanityWriteLog(msg);

                    Household activeHousehold = null;
                    foreach (ulong num6 in GameStates.sTravelData.mDeadSims)
                    {
                        SimDescription simDesc = SimDescription.Find(num6);
                        try
                        {
                            if (simDesc != null)
                            {
                                MidlifeCrisisManager.OnSimDied(simDesc);
                                if ((simDesc.CreatedSim != null) && (Sim.ActiveActor == simDesc.CreatedSim))
                                {
                                    activeHousehold = Household.ActiveHousehold;
                                    PlumbBob.ForceSelectActor(null);
                                }
                                if ((timeoutToAdd != 0f) || (GameStates.sTravelData.mTravelerIds.Count == GameStates.sTravelData.mDeadSims.Count))
                                {
                                    Urnstone.FinalizeSimDeathRelationships(simDesc, timeoutToAdd);
                                }
                                simDesc.Household.Remove(simDesc);
                                if (Urnstone.FindGhostsGrave(simDesc) == null)
                                {
                                    simDesc.Dispose();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(simDesc, null, msg, e);
                        }
                    }

                    msg += "H";
                    Traveler.InsanityWriteLog(msg);

                    if (activeHousehold != null)
                    {
                        if (activeHousehold.AllActors.Count > 0x0)
                        {
                            GameStates.sNextSimToSelect = RandomUtil.GetRandomObjectFromList(activeHousehold.AllActors);
                        }
                        else
                        {
                            msg += "H1";

                            GameStates.sStartupState = InWorldState.SubState.PlayFlow;
                        }
                    }

                    msg += "I";
                    Traveler.InsanityWriteLog(msg);

                    Household newActiveHousehold = Household.ActiveHousehold ?? activeHousehold;
                    if (newActiveHousehold != null)
                    {
                        GameStates.UnstashHousehold(GameStates.sTravelData.mStache, newActiveHousehold);

                        msg += "I1";
                        Traveler.InsanityWriteLog(msg);

                        foreach (ulong simID in GameStates.sTravelData.mTravelerIds)
                        {
                            SimDescription activeSimDesc = newActiveHousehold.FindMember(simID);

                            if (activeSimDesc != null)
                            {
                                msg += Common.NewLine + activeSimDesc.FullName;
                            }
                            else
                            {
                                msg += Common.NewLine + simID;
                            }
                            Traveler.InsanityWriteLog(msg);

                            try
                            {
                                if (((activeSimDesc != null) || ((GameStates.sTravelData.mDeadSims != null) && GameStates.sTravelData.mDeadSims.Contains(simID))) && ((activeSimDesc != null) && (activeSimDesc.CreatedSim != null)))
                                {
                                    msg += "I2";
                                    Traveler.InsanityWriteLog(msg);

                                    EventTracker.SendEvent(EventTypeId.kSimReturnedFromVacationWorld, activeSimDesc.CreatedSim);
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(activeSimDesc, null, msg, e);
                            }
                        }

                        msg += "J";

                        //GrimReaperSituation.CheckForAbandonedChildren(household2);
                        foreach (Sim sim3 in Households.AllSims(newActiveHousehold))
                        {
                            using (SafeStore store = new SafeStore(sim3.SimDescription, SafeStore.Flag.None))
                            {
                                try
                                {
                                    if (!Household.RoommateManager.IsNPCRoommate(sim3))
                                    {
                                        sim3.OnBecameSelectable();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim3, null, msg, e);
                                }
                            }
                        }
                    }

                    msg += "K";
                    Traveler.InsanityWriteLog(msg);

                    GameStates.GiveWeddingGiftsIfOwed();
                }

                msg += "L";
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }

            GameStates.sTravelData.mStache.RemoveAllItems();

            if (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.EndVacation)
            {
                GameStates.sTravelData = null;
                GameStates.sIsChangingWorlds = false;
            }

            return list;
        }

        private static void SwitchWorlds()
        {
            TravelUtil.PlayerMadeTravelRequest = false;

            Common.StringBuilder msg = new Common.StringBuilder("SwitchWorlds" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            try
            {
                msg += "A";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.TravellingHome)
                {
                sResidentIds.Clear();
                }

                if (sResidentIds.Count == 0 && !GameStates.TravellingHome)
                {
                foreach (KeyValuePair<ulong, SimDescription> sim in SimListing.GetResidents(false))
                {
                    sResidentIds[sim.Key] = true;
                    }
                }

                if (!LoadingScreenController.IsLayoutLoaded())
                {
                    bool isReturningHome = false;
                    StartLoadingScreen(isReturningHome);
                }

                msg += "B";
                Traveler.InsanityWriteLog(msg);

                try
                {
                    if (Traveler.Settings.mStoreVehicles)
                    {
                        if ((GameStates.TravellingHome) && (GameStates.TravelHousehold != null) && (GameStates.TravelHousehold.LotHome != null))
                        {
                            Lot lot = GameStates.TravelHousehold.LotHome;
                            if (!lot.IsCommunityLot)
                            {
                                Inventory inventory = GameStates.TravelHousehold.SharedFamilyInventory.Inventory;

                                List<Sim> sims = Households.AllHumans(GameStates.TravelHousehold);
                                if ((sims != null) && (sims.Count > 0))
                                {
                                    inventory = sims[0].Inventory;
                                }

                                foreach (IOwnableVehicle vehicle in lot.GetObjects<IOwnableVehicle>())
                                {
                                    if (vehicle.InInventory) continue;

                                    Inventories.TryToMove(vehicle, inventory);
                                }
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Common.Exception(msg, e);
                }

                int count = GameStates.sTravelData.mTravelerIds.Count;

                SimDescription[] descriptionArray = new SimDescription[count];

                Household household = Household.ActiveHousehold ?? GameStates.sOldActiveHousehold;

                for (int i = 0x0; i < count; i++)
                {
                    descriptionArray[i] = household.FindMember(GameStates.sTravelData.mTravelerIds[i]);
                }

                msg += "C";
                Traveler.InsanityWriteLog(msg);

                if (count > 0x1)
                {
                    GameStates.sTravelData.mTravelingRelationships = new List<GameStates.TravelRelationship>();
                    for (int j = 0x0; j < count; j++)
                    {
                        GameStates.TravelRelationship item = new GameStates.TravelRelationship();
                        item.simXId = GameStates.sTravelData.mTravelerIds[j];
                        item.simyIds = new List<ulong>();
                        item.rels = new List<Relationship>();
                        for (int k = 0x0; k < count; k++)
                        {
                            if (((k != j) && (descriptionArray[j] != null)) && (descriptionArray[k] != null))
                            {
                                Relationship relationship2 = Relationship.Get(descriptionArray[j], descriptionArray[k], false);
                                if (relationship2 != null)
                                {
                                    item.simyIds.Add(GameStates.sTravelData.mTravelerIds[k]);
                                    item.rels.Add(relationship2);
                                }
                            }
                        }
                        GameStates.sTravelData.mTravelingRelationships.Add(item);
                    }
                }

                msg += "D";
                Traveler.InsanityWriteLog(msg);

                CameraController.DisableObjectFollow();
                GameStates.StoreHouseholdBuffsForTravel(household);

                try
                {
                    foreach (SimDescription sim in Households.All(household))
                    {
                        Corrections.CleanupOpportunities(sim, false, null);
                    }

                    GameStates.StoreOpportunitiesForTravel(household);
                }
                catch (Exception e)
                {
                    Traveler.InsanityException(msg, e);
                }

                msg += "E";
                Traveler.InsanityWriteLog(msg);

                GameStates.StashHousehold(GameStates.sTravelData.mStache, household);

                msg += "E2";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.StartVacation)
                {
                    GameStates.LeavePetUrnstonesAtHome(household);

                    if (descriptionArray.Length > 0)
                    {
                        GameStates.sIsPointSafelyRoutable = new Sim.IsPointSafelyRoutable(Household.ActiveHousehold.LotHome, descriptionArray[0x0].CreatedSim);
                        GameStates.sIsPointSafelyRoutable.SetLimit(0x5);
                        Sim.sIsPointSafelyRoutable = GameStates.sIsPointSafelyRoutable;
                    }

                    GameStates.TravelComplete += GameStates.ClearRoutablePointCache;

                    msg += "E1";
                    Traveler.InsanityWriteLog(msg);

                    for (int m = 0x0; m < descriptionArray.Length; m++)
                    {
                        if (descriptionArray[m] != null)
                        {
                            Sim createdSim = descriptionArray[m].CreatedSim;
                            if (createdSim != null)
                            {
                                createdSim.SetObjectToReset();
                            }
                        }
                    }

                    msg += "E2";
                    Traveler.InsanityWriteLog(msg);

                    SpeedTrap.Sleep();
                    LoadSaveManager.SaveTravel();

                    msg += "E3";
                    Traveler.InsanityWriteLog(msg);

                    GameStates.sTravelData.mSaveName = GameStates.sLoadFileName;
                    if (!GameStatesEx.SetupLoadFileName(true))
                    {
                        PersistStatic.MainMenuLoading = true;
                        GameStates.sTravelData = null;
                        GameStates.sIsChangingWorlds = false;

                        msg += "End";
                        Traveler.InsanityWriteLog(msg);

                        GameUtils.EnableSceneDraw(true);
                        LoadingScreenController.Unload();
                        return;
                    }

                    msg += "E4";
                    Traveler.InsanityWriteLog(msg);

                    Household activeHousehold = Household.ActiveHousehold;
                    if (activeHousehold != null)
                    {
                        string homeworldMetadataName = GameStates.HomeworldMetadataName;
                        UIManager.SetSaveGameMetadata(GameStates.sLoadFileName, activeHousehold.Name, activeHousehold.BioText, homeworldMetadataName, activeHousehold.HouseholdId, activeHousehold.LotId, true);
                    }

                    msg += "E5";
                    Traveler.InsanityWriteLog(msg);

                    string householdDelimitedData = "";
                    UIImage householdThumbnail = new UIImage(0x0);
                    if (!UIManager.GetSaveGameMetadata(GameStates.sLoadFileName, ref householdDelimitedData, ref householdThumbnail, ref householdThumbnail))
                    {
                        UIManager.SetSaveGameMetadata(GameStates.sLoadFileName, "", "", "", 0x0L, 0x0L, true);
                    }
                }

                msg += "F";
                Traveler.InsanityWriteLog(msg);

                // Store the World names for the local sims to persistence
                foreach (Household house in Household.sHouseholdList)
                {
                    if (house.IsSpecialHousehold) continue;

                    foreach (SimDescription sim in Households.All(house))
                    {
                        Traveler.Settings.AddWorldForSim(sim.SimDescriptionId);
                    }
                }

                // Transfer data to cross-world lookup
                WorldData.MergeToCrossWorldData();

                msg += "G1";
                Traveler.InsanityWriteLog(msg);

                // Custom
                MiniSimDescriptionEx.AddMiniSims();

                msg += "G2";
                Traveler.InsanityWriteLog(msg);

                // Custom
                foreach (SimDescription sim in Households.All(household))
                {
                    Corrections.CleanupBrokenSkills(sim, null);
                }

                BinEx.ExportHouseholdForTravel(household);

                if (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.StartVacation)
                {
                    foreach (SimDescription sim in new List<SimDescription> (household.AllSimDescriptions))
                    {
                        string reason = null;
                        if (CrossWorldControl.sRetention.RestoreHousehold(sim, ref reason))
                        {
                            msg += Common.NewLine + "Restored: " + sim.FullName;
                        }
                        else
                        {
                            msg += Common.NewLine + "Restored: " + sim.FullName + " (" + reason + ")";
                        }
                    }
                }

                msg += "G3";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.sTravelData.mState == GameStates.TravelData.TravelState.EndVacation)
                {
                    household.MoveToPreviousTravelerHousehold();

                    foreach (SimDescription description in Household.AllSimsLivingInWorld())
                    {
                        try
                        {
                            if ((description.CreatedSim != null) && (description.CreatedSim.LotHome != description.CreatedSim.LotCurrent))
                            {
                                description.CreatedSim.SetToResetAndSendHome();
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(description, e);
                        }
                    }

                    msg += "G1";
                    Traveler.InsanityWriteLog(msg);

                    SpeedTrap.Sleep();
                    LoadSaveManager.SaveTravel();

                    msg += "G2";
                    Traveler.InsanityWriteLog(msg);

                    if (GameStates.sTravelData.mSaveName.Contains(".world"))
                    {
                        GameStates.sTravelData.mSaveName = "NotSet.sims3";
                    }
                    GameStates.SetLoadFileName(GameStates.sTravelData.mHomeWorld, true);
                }

                msg += "H";
                Traveler.InsanityWriteLog(msg);

                GameStates.sOldActiveHousehold = null;
                GameStates.GotoState(GameState.TravelDeparture);
            }
            catch (Exception e)
            {
                GameUtils.EnableSceneDraw(true);
                LoadingScreenController.Unload();

                Traveler.InsanityException(msg, e);
            }
        }

        public static void UpdateTelemetryAndTriggerTravelBackToHomeWorld()
        {
            new Common.AlarmTask(2, TimeUnit.Minutes, ReturnFromVacationWorld);

            Traveler.SaveGame();
        }

        public static void OnArrivalAtVacationWorld()
        {
            Common.StringBuilder msg = new Common.StringBuilder("OnArrivalAtVacationWorld" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            try
            {
                msg += "A";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.sTravelData == null)
                {
                    msg += Common.NewLine + "GameStates.sTravelData == null";
                }
                else if (GameStates.sTravelData.mTravelHouse == null)
                {
                    msg += Common.NewLine + "GameStates.sTravelData.mTravelHouse == null";
                }
                else if (GameStates.sTravelData.mTravelHouse.AllSimDescriptions == null)
                {
                    msg += Common.NewLine + "GameStates.sTravelData.mTravelHouse.AllSimDescriptions == null";
                }

                int count = GameStates.sTravelData.mTravelHouse.AllSimDescriptions.Count;

                msg += "B";
                Traveler.InsanityWriteLog(msg);

                Dictionary<ulong, SimDescription> dictionary = new Dictionary<ulong, SimDescription>();
                for (int i = 0x0; i < count; i++)
                {
                    SimDescription description = GameStates.sTravelData.mTravelHouse.AllSimDescriptions[i];
                    dictionary.Add(description.SimDescriptionId, description);
                }

                msg += "C";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.sTravelData.mTravelingRelationships != null)
                {
                    foreach (GameStates.TravelRelationship relationship in GameStates.sTravelData.mTravelingRelationships)
                    {
                        SimDescription x;
                        if (dictionary.TryGetValue(relationship.simXId, out x))
                        {
                            for (int j = 0x0; j < relationship.simyIds.Count; j++)
                            {
                                SimDescription y;
                                if (dictionary.TryGetValue(relationship.simyIds[j], out y))
                                {
                                    try
                                    {
                                        Relationship relation = Relationship.Get(x, y, true);
                                        if (relation != null)
                                        {
                                            Relationship relation2 = relationship.rels[j];
                                            if (relation2 != null)
                                            {
                                                relation.CopyRelationship(relation2);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Common.Exception(x, y, msg, e);
                                    }
                                }
                            }
                        }
                    }
                    GameStates.sTravelData.mTravelingRelationships.Clear();
                }

                msg += "D";
                Traveler.InsanityWriteLog(msg);

                Sim simToSelect = null;

                // Custom
                TravelUtilEx.PlaceResults results = TravelUtilEx.LocateHomeAndPlaceSimsAtVacationWorld(GameStates.sTravelData.mTravelHouse, ref simToSelect);
                if (results != TravelUtilEx.PlaceResults.Failure)
                {
                    if (simToSelect != null)
                    {
                        DreamCatcher.SelectNoLotCheckImmediate(simToSelect, true, true);
                    }

                    Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.Gameplay.Gameflow.GameSpeed.Normal, Sims3.Gameplay.Gameflow.SetGameSpeedContext.Gameplay);
                }

                msg += "D2";
                Traveler.InsanityWriteLog(msg);

                if (GameUtils.IsFutureWorld())
                {
                    FutureDescendantServiceEx.PostFutureWorldLoadProcess(FutureDescendantService.GetInstance());
                    CauseEffectService.GetInstance().PostFutureWorldLoadProcess();
                }

                msg += "E";
                Traveler.InsanityWriteLog(msg);

                if (GameStates.TravelHousehold != null)
                {
                    msg += "E1";

                    foreach (Sim sim2 in Households.AllSims(GameStates.TravelHousehold))
                    {
                        try
                        {
                            EventTracker.SendEvent(new GuidEvent<WorldName>(EventTypeId.kSimEnteredVacationWorld, sim2, GameUtils.GetCurrentWorld()));
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim2, null, "kSimEnteredVacationWorld", e);
                        }
                    }
                }

                msg += "F";
                Traveler.InsanityWriteLog(msg);

                TravelUtil.SpecialStoryProgression(GameStates.sTravelData.mNumDaysSinceLastInDestWorld);

                GameStates.sTravelData.mState = GameStates.TravelData.TravelState.OnVacation;

                if (simToSelect == null)
                {
                    Common.FunctionTask.Perform(GameStates.TransitionToEditTown);
                }
                else if ((results == TravelUtilEx.PlaceResults.Residential) && (GameStates.IsNewGame))
                {
                    Common.FunctionTask.Perform(ShowResidentialAlert);
                }
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
            finally
            {
                GameStates.sIsChangingWorlds = false;
            }
        }

        private static void ShowResidentialAlert()
        {
            SimpleMessageDialog.Show(Common.Localize("Root:MenuName").Replace("...", ""), Common.Localize("NoBaseCamp:Error"));
        }

        private static void ReturnFromVacationWorld()
        {
            Common.StringBuilder msg = new Common.StringBuilder("GameStatesEx:ReturnFromVacationWorld" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            try
            {
                if (GameStates.TravelHousehold != Household.ActiveHousehold) return;

                if (Household.ActiveHousehold == null) return;

                bool isReturningHome = true;

                // Custom
                StartLoadingScreen(isReturningHome);

                if (Simulator.CheckYieldingContext(false))
                {
                    SpeedTrap.Sleep(1);
                }

                msg += Common.NewLine + "A";
                Traveler.InsanityWriteLog(msg);

                if (!GameStates.IsGameShuttingDown)
                {
                    GameStates.ReturningFromWorld = GameUtils.GetCurrentWorldType();

                    msg += Common.NewLine + GameStates.ReturningFromWorld.ToString();
                    Traveler.InsanityWriteLog(msg);

                    TravelUtil.PlayerMadeTravelRequest = true;

                    try
                    {
                        List<Role> roles = new List<Role>(RoleManager.GetRolesOfType(Role.RoleType.Tourist));
                        roles.AddRange(RoleManager.GetRolesOfType(Role.RoleType.Explorer));

                        foreach (Role role in roles)
                        {
                            try
                            {
                                if ((role.mSim != null) && (role.mSim.Household != null) && (role.mSim.Household.IsTouristHousehold))
                                {
                                    role.RemoveSimFromRole();

                                    MiniSims.PackUpToMiniSimDescription(role.mSim);
                                }

                                if (role is RoleTourist)
                                {
                                    GameStates.PreReturnHome -= (role as RoleTourist).KillSim;
                                }
                                else if (role is RoleExplorer)
                                {
                                    GameStates.PreReturnHome -= (role as RoleExplorer).KillSim;
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(role.mSim, null, msg, e);
                            }
                        }

                        msg += Common.NewLine + "B";
                        Traveler.InsanityWriteLog(msg);

                        if (GameStates.PreReturnHome != null)
                        {
                            GameStates.PreReturnHome();
                        }
                    }
                    catch (Exception e)
                    {
                        Traveler.InsanityException(msg, e);
                    }

                    msg += Common.NewLine + "C";
                    Traveler.InsanityWriteLog(msg);

                    GameStates.StopOperationsThatCannotTravel();
                    Household household = Household.ActiveHousehold ?? GameStates.sOldActiveHousehold;
                    household.RemoveVacationAlarms();

                    msg += Common.NewLine + "D";
                    Traveler.InsanityWriteLog(msg);

                    foreach (Sim sim in Households.AllSims(household))
                    {
                        try
                        {
                            if (!Household.RoommateManager.IsNPCRoommate(sim))
                            {
                                EventTracker.SendEvent(new TravelUtil.TravelEvent(EventTypeId.kReturnedFromVacation, sim, null, GameStates.sTravelData.mDestWorld, GameStates.sTravelData.mCurrentDayOfTrip));
                                sim.SetObjectToReset();
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, null, msg, e);
                        }
                    }

                    msg += Common.NewLine + "E";
                    Traveler.InsanityWriteLog(msg);

                    if (GameStates.sTravelData.mEarlyDepartures != null)
                    {
                        GameStates.sTravelData.mEarlyDepartureIds = new List<ulong>(GameStates.sTravelData.mEarlyDepartures.Count);
                        foreach (Sim sim2 in new List<Sim> (GameStates.sTravelData.mEarlyDepartures))
                        {
                            GameStates.sTravelData.mEarlyDepartureIds.Add(sim2.SimDescription.SimDescriptionId);
                            household.AddSim(sim2);
                        }
                        GameStates.sTravelData.mEarlyDepartures = null;
                    }

                    if (GameStates.sTravelData.mRealEstateManager == null)
                    {
                        GameStates.sTravelData.mRealEstateManager = household.RealEstateManager;
                    }
                    GameStates.sTravelData.mRealEstateManager.SavePropertyNamesForTravel();

                    msg += Common.NewLine + "G";
                    Traveler.InsanityWriteLog(msg);

                    if ((GameStates.ReturningFromWorld != WorldType.University) && 
                        (GameStates.ReturningFromWorld != WorldType.Future))
                    {
                        Sim sim3 = null;
                        if (household.AllActors.Count > 0x0)
                        {
                            sim3 = household.AllActors[0x0];
                        }

                        if (sim3 != null)
                        {
                            foreach (IBed bed in Sims3.Gameplay.Queries.GetObjects<IBed>())
                            {
                                if (bed.IsTentOwnedByHousehold(sim3.Household))
                                {
                                    if (bed.InInventory)
                                    {
                                        if (!sim3.Inventory.Contains(bed))
                                        {
                                            sim3.Inventory.TryToMove(bed);
                                        }
                                    }
                                    else
                                    {
                                        sim3.Inventory.TryToAdd(bed);
                                    }
                                }
                            }
                        }
                    }

                    Lot lot = LotManager.GetLot(household.LotId);
                    if (lot != null)
                    {
                        lot.MoveOut(household);
                    }
                    Household.RoommateManager.StopAcceptingRoommates(true);

                    msg += Common.NewLine + "H";
                    Traveler.InsanityWriteLog(msg);

                    GameStates.sIsChangingWorlds = true;
                    GameStates.sTravelData.mState = GameStates.TravelData.TravelState.EndVacation;

                    CrossWorldControl.Store(Households.All(household));

                    Common.FunctionTask.Perform(SwitchWorlds);
                }
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
            }
        }
    }
}
