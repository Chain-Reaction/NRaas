using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Booters;
using NRaas.TravelerSpace.States;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Helpers
{
    public class WorldData : Common.IPreLoad, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        static Dictionary<WorldName, WorldNameData> sData = new Dictionary<WorldName, WorldNameData>();

        static Dictionary<WorldName, WorldType> sOrigWorldNameToType = null;

        static List<BookBaseStoreItem> sAdjustedStoreItems = null;

        static bool sDefaultBarTuned = false;

        static bool sInitial = true;
        public static Dictionary<ulong, string> sWorldForSims = new Dictionary<ulong, string>();

        static List<UIImage> sConfirmImages = new List<UIImage>();

        static Household mOldHouse = null;

        static WorldName sPreviousWorld = WorldName.Undefined;

        public static void OnLoadFixup(VisaManager manager)
        {
            foreach (KeyValuePair<ulong, Visa> element in manager.mValues)
            {
                Visa defaultVisa = VisaManager.GetStaticVisaData((WorldName)element.Key);
                if (defaultVisa != null)
                {
                    element.Value.mWorldName = defaultVisa.mWorldName;
                }
            }

            manager.OnLoadFixup();
        }

        private static bool GetWorldFileDetails(ref WorldFileMetadata info)
        {
            string mWorldFile = info.mWorldFile;
            if (!mWorldFile.ToLower().EndsWith(".world"))
            {
                mWorldFile = mWorldFile + ".world";
            }

            info.mDescription = null;
            info.mWorldThumb = null;
            info.mLotCount = 0x0;

            WorldName worldName = WorldName.Undefined;
            ulong worldNameKey = 0x0L;

            string entryKey = UIManager.GetWorldMetadata(mWorldFile, ref info.mLotCount, ref info.mWorldThumb, ref worldNameKey, ref info.mWorldType, ref worldName);
            if (!GameUtils.WorldNameToType.ContainsKey(worldName))
            {
                GameUtils.WorldNameToType.Add(worldName, info.mWorldType);
            }

            ILocalizationModel localizationModel = Sims3.Gameplay.UI.Responder.Instance.LocalizationModel;
            if (entryKey != null)
            {
                if (localizationModel.HasLocalizationString(entryKey))
                {
                    info.mDescription = localizationModel.LocalizeString(entryKey, new object[0x0]);
                }
                else
                {
                    info.mDescription = entryKey;
                }
            }

            if ((worldNameKey != 0x0L) && localizationModel.HasLocalizationString(worldNameKey))
            {
                info.mCaption = localizationModel.LocalizeString(worldNameKey);
            }

            if (string.IsNullOrEmpty(info.mCaption))
            {
                BooterLogger.AddTrace("Untranslated " + mWorldFile + ": " + worldNameKey);

                if (mWorldFile.ToLower().EndsWith(".world"))
                {
                    int num2 = mWorldFile.LastIndexOfAny(new char[] { '\\', '/' });
                    info.mCaption = ((num2 >= 0x0) && (mWorldFile.Length > 0x0)) ? mWorldFile.Substring(num2 + 0x1) : mWorldFile.Substring(0x0, mWorldFile.Length - 0x6);
                }
                else
                {
                    info.mCaption = mWorldFile;
                }
            }

            if (info.mDescription == null)
            {
                info.mDescription = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Fusce pulvinar. Donec faucibus, dolor eu porta lobortis ante nibh vulputate justo, velvolutpat odio sapien venenatis justo. Quisque sit amet felis acelit imperdiet hendrerit. Nam ut mauris et nisi varius portitor. Fusce blandit, diam id varius portitor, sem elit bibendum dolor, congue molestie sapien lorem vel dolor. Fusce laoreet est non urna euismod commodo. Donec enim quam, blandit at, placerat in, eleifend vitae, justo. Suspendisse potenti. In blandit pede sit amet sem. Nam condimentum sapien sit amet erat.";
            }

            if (info.mWorldThumb == null)
            {
                info.mWorldThumb = UIManager.LoadUIImage(ResourceKey.CreatePNGKey("game_entry_town_placeholder", 0x0));
            }
            return true;
        }

        public static string GetWorldFile(WorldName worldName)
        {
            WorldNameData data;
            if (sData.TryGetValue(worldName, out data))
            {
                return data.mWorldFile;
            }
            else
            {
                return null;
            }
        }

        public static UIImage GetConfirmImage(int index)
        {
            if (index > sConfirmImages.Count)
            {
                if (index < 3)
                {
                    return UIManager.LoadUIImage(ResourceKey.CreatePNGKey(TravelUtil.DestinationInfoConfirmImage[index], 0x0));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return sConfirmImages[index];
            }
        }

        protected static int OnSort(WorldFileMetadata left, WorldFileMetadata right)
        {
            return left.mCaption.CompareTo(right.mCaption);
        }

        public void OnPreLoad()
        {
            sDefaultBarTuned = false;

            foreach (BookComicData bookData in BookData.BookComicDataList.Values)
            {
                bookData.AllowedWorldTypes.Add(WorldType.Vacation);
            }

            foreach (Opportunity opp in OpportunityManager.sLocationBasedOpportunityList.Values)
            {
                if (opp.SharedData.mTargetWorldRequired == WorldName.SunsetValley)
                {
                    opp.SharedData.mTargetWorldRequired = WorldName.Undefined;
                }
            }

            if (ServiceNPCSpecifications.sServiceSpecifications != null)
            {
                foreach (ServiceType type in Enum.GetValues(typeof(ServiceType)))
                {
                    ServiceNPCSpecifications.ServiceSpecifications spec;
                    if (ServiceNPCSpecifications.sServiceSpecifications.TryGetValue(type.ToString(), out spec))
                    {
                        spec.InvalidWorlds.Clear();
                    }
                }
            }

            foreach (List<MotiveTuning> tempTuning in MotiveTuning.sTuning.Values)
            {
                foreach (MotiveTuning tuning in tempTuning)
                {
                    tuning.WorldRestrictionType = WorldRestrictionType.None;
                }
            }

            Bartending.BarData defaultData;
            if (Bartending.TryGetBarData(Lot.MetaAutonomyType.None, out defaultData))
            {
                sDefaultBarTuned = (defaultData.mFoods.Count > 0);
            }

            // Unlock all subTypes for all worlds
            foreach (Lot.CommercialSubTypeData commData in Lot.sCommnunityTypeData)
            {
                commData.WorldAllowed = null;
                commData.WorldTypeAllowed = null;
            }

            // Unlock all subTypes for all worlds
            foreach (Lot.ResidentialSubTypeData residentData in Lot.sResidentialTypeData)
            {
                residentData.WorldAllowed = null;
                residentData.WorldTypeAllowed = null;
            }

            foreach (ActionData action in ActionData.sData.Values)
            {
                if (action.mAllowedWorldTypes != null)
                {
                    List<WorldType> types = new List<WorldType>(action.mAllowedWorldTypes);

                    if(types.Count > 0)
                    {
                        if ((!types.Contains(WorldType.Vacation)))
                        {
                            types.Add(WorldType.Vacation);
                        }

                        if ((!types.Contains(WorldType.Future)))
                        {
                            types.Add(WorldType.Future);
                        }

                        action.mAllowedWorldTypes = types.ToArray();
                    }
                }                
            }

            STCData.SetNumSocialsDuringConversation(int.MaxValue);

            foreach (InsectData insect in InsectData.sData.Values)
            {
                if (insect.RequiredWorld != WorldName.Undefined)
                {
                    insect.mRequiredWorld = WorldName.Undefined;

                    BooterLogger.AddTrace("  Unlocked: " + insect.Name);
                }
            }

            foreach (RockGemMetalData rock in RockGemMetalBase.sData.Values)
            {
                if (rock.RequiredWorld != WorldName.Undefined)
                {
                    rock.mRequiredWorld = WorldName.Undefined;

                    BooterLogger.AddTrace("  Unlocked: " + rock.Name);
                }
            }

            List<KeyValuePair<WorldName, WorldNameData>> data = new List<KeyValuePair<WorldName, WorldNameData>>();

            if (GameUtils.IsInstalled(ProductVersion.EP1))
            {
                data.Add(new KeyValuePair<WorldName, WorldNameData>(WorldName.China, new WorldNameData("China.world", "China_0x0859db4c", TravelUtil.DestinationInfoImage[0], TravelUtil.DestinationInfoName[0], TravelUtil.DestinationInfoDescription[0], UIManager.LoadUIImage(ResourceKey.CreatePNGKey(TravelUtil.DestinationInfoConfirmImage[0], 0x0)), TravelUtil.DestinationInfoComfirmDescription[0])));
                data.Add(new KeyValuePair<WorldName, WorldNameData>(WorldName.Egypt, new WorldNameData("Egypt.world", "Egypt_0x0859db48", TravelUtil.DestinationInfoImage[1], TravelUtil.DestinationInfoName[1], TravelUtil.DestinationInfoDescription[1], UIManager.LoadUIImage(ResourceKey.CreatePNGKey(TravelUtil.DestinationInfoConfirmImage[1], 0x0)), TravelUtil.DestinationInfoComfirmDescription[1])));
                data.Add(new KeyValuePair<WorldName, WorldNameData>(WorldName.France, new WorldNameData("France.world", "France_0x0859db50", TravelUtil.DestinationInfoImage[2], TravelUtil.DestinationInfoName[2], TravelUtil.DestinationInfoDescription[2], UIManager.LoadUIImage(ResourceKey.CreatePNGKey(TravelUtil.DestinationInfoConfirmImage[2], 0x0)), TravelUtil.DestinationInfoComfirmDescription[2])));
            }

            List<WorldFileMetadata> worlds = new List<WorldFileMetadata>();

            WorldFileSearch search = new WorldFileSearch(0x0);
            foreach (string str in search)
            {
                WorldFileMetadata info = new WorldFileMetadata();

                info.mWorldFile = str;
                if (GetWorldFileDetails(ref info))
                {
                    worlds.Add(info);
                }
            }

            worlds.Sort(OnSort);

            foreach(WorldFileMetadata info in worlds)
            {
                string name = info.mWorldFile.Replace(".world", "");

                WorldName worldName = WorldName.Undefined;

                try
                {
                    worldName = unchecked((WorldName)ResourceUtils.HashString32(name.Replace(" ", "")));
                }
                catch
                {
                    continue;
                }

                string saveFile = name;
                switch (name.ToLower())
                {
                    case "islaparadiso":
                        if (!GameUtils.IsInstalled(ProductVersion.EP10)) continue;

                        worldName = WorldName.IslaParadiso;

                        saveFile += "_0x0c50c382";
                        break;
                    case "sims university":
                        if (!GameUtils.IsInstalled(ProductVersion.EP9)) continue;

                        worldName = WorldName.University;

                        saveFile += "_0x0e41c954";
                        break;
                    case "bridgeport":
                        if (!GameUtils.IsInstalled(ProductVersion.EP3)) continue;

                        worldName = WorldName.NewDowntownWorld;

                        saveFile += "_0x09ffe3d7";
                        break;
                    case "twinbrook":
                        if (!GameUtils.IsInstalled(ProductVersion.EP2)) continue;

                        worldName = WorldName.TwinBrook;

                        saveFile += "_0x09b610fa";
                        break;
                    case "appaloosaplains":
                        if (!GameUtils.IsInstalled(ProductVersion.EP5)) continue;

                        worldName = WorldName.AppaloosaPlains;

                        saveFile += "_0x0c50c56d";
                        break;
                    case "riverview":
                        worldName = WorldName.RiverView;

                        saveFile += "_0x0859db43";
                        break;
                    case "sunset valley":
                        worldName = WorldName.SunsetValley;

                        saveFile += "_0x0859db3c";
                        break;
                    case "moonlight falls":
                        worldName = WorldName.MoonlightFalls;

                        saveFile += "_0x09b61110";
                        break;
                    case "starlight shores":
                        if (!GameUtils.IsInstalled(ProductVersion.EP6)) continue;

                        worldName = WorldName.StarlightShores;

                        saveFile += "_0x09b610ff";
                        break;
                    case "hidden springs":
                    case "barnacle bay":
                    case "lunar lakes":
                    case "lucky palms":
                        saveFile += "_0x08866eb8";
                        break;
                    case "roaring heights":
                        worldName = WorldName.DOT11;

                        saveFile += "_0x0de07e86";
                        break;
                    case "midnight hollow":
                        worldName = WorldName.DOT10;

                        saveFile += "_0x0de07e7d";
                        break;
                    case "dragon valley":
                        worldName = WorldName.DOT09;

                        saveFile += "_0x0de07c9c";
                        break;
                    case "aurora skies":
                        worldName = WorldName.DOT08;

                        saveFile += "_0x0de07c8b";
                        break;
                    case "monte vista":
                        worldName = WorldName.DOT07;

                        saveFile += "_0x0de07c83";
                        break;
                    case "sunlit tides":
                        worldName = WorldName.DOT06;

                        saveFile += "_0x0de07c78";
                        break;
                    case "oasis landing":
                        worldName = WorldName.FutureWorld;

                        saveFile += "_0x0f36012a";
                        break;
                    case "egypt":
                    case "china":
                    case "france":
                        continue;
                    default:
                        saveFile = FileNameBooter.GetFileName(saveFile);
                        break;
                }

                string infoIcon = "glb_i_suburb";
                switch (info.mWorldType)
                {
                    case WorldType.Downtown:
                        infoIcon = "glb_i_downtown";
                        break;
                    case WorldType.Vacation:
                        infoIcon = "glb_i_vacation";
                        break;
                    default:
                        break;
                }

                if (!VisaManager.sDictionary.ContainsKey((ulong)worldName))
                {
                    Visa defaultVisa = new Visa();
                    defaultVisa.mWorldName = worldName;
                    defaultVisa.mNonPersistableData = new NonPersistableVisaData();
                    defaultVisa.NonPersistableData.LevelUpStrings = new string[3] { "", "", "" };
                    defaultVisa.NonPersistableData.PointsForNextLevel = new int[3] { 0, 0, 0 };

                    Visa chinaVisa = VisaManager.GetStaticVisaData(WorldName.China);
                    if (chinaVisa != null)
                    {
                        defaultVisa.NonPersistableData.LevelUpStrings = new List<string>(chinaVisa.NonPersistableData.LevelUpStrings).ToArray();
                        defaultVisa.NonPersistableData.PointsForNextLevel = new List<int>(chinaVisa.NonPersistableData.PointsForNextLevel).ToArray();
                    }

                    VisaManager.sDictionary.Add((ulong)worldName, defaultVisa);

                    data.Add(new KeyValuePair<WorldName, WorldNameData>(worldName, new WorldNameData(info.mWorldFile, saveFile, infoIcon, info.mCaption, info.mDescription, info.mWorldThumb, Common.Localize("Itinerary:Name", false, new object[] { info.mCaption }))));
                }
            }

            TravelUtil.kVacationWorldNames = new WorldName[data.Count];

            TravelUtil.DestinationInfoImage = new string[data.Count];
            TravelUtil.DestinationInfoName = new string[data.Count];
            TravelUtil.DestinationInfoDescription = new string[data.Count];
            TravelUtil.DestinationInfoConfirmImage = new string[data.Count];
            TravelUtil.DestinationInfoComfirmDescription = new string[data.Count];

            TravelUtil.DestinationInfoIndex = new int[data.Count];

            int index = 0;
            foreach (KeyValuePair<WorldName, WorldNameData> value in data)
            {
                TravelUtil.kVacationWorldNames[index] = value.Key;

                TravelUtil.DestinationInfoIndex[index] = index;

                TravelUtil.DestinationInfoImage[index] = value.Value.mDestinationInfoImage;
                TravelUtil.DestinationInfoName[index] = value.Value.mDestinationInfoName;
                TravelUtil.DestinationInfoDescription[index] = value.Value.mDestinationInfoDescription;
                TravelUtil.DestinationInfoConfirmImage[index] = "";
                TravelUtil.DestinationInfoComfirmDescription[index] = value.Value.mDestinationInfoConfirmDescription;

                sConfirmImages.Add(value.Value.mDestinationInfoConfirmImage);

                BooterLogger.AddTrace(value.Value.ToString());
                BooterLogger.AddTrace("  WorldName: " + (ulong)value.Key);

                index++;

                sData.Add(value.Key, value.Value);
            }

            //BooterLogger.AddError("WorldData");

            sOrigWorldNameToType = GameUtils.WorldNameToType;

            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                switch (tuning.Availability.WorldRestrictionType)
                {
                    case WorldRestrictionType.Allow:
                        if (!tuning.Availability.WorldRestrictionWorldTypes.Contains(WorldType.Vacation))
                        {
                            tuning.Availability.WorldRestrictionWorldTypes.Add(WorldType.Vacation);

                            BooterLogger.AddTrace("Tuning Altered: Allowed (A) " + tuning.FullInteractionName + " : " + tuning.FullObjectName);
                        }
                        else if (!tuning.Availability.WorldRestrictionWorldTypes.Contains(WorldType.Downtown))
                        {
                            tuning.Availability.WorldRestrictionWorldTypes.Add(WorldType.Downtown);

                            BooterLogger.AddTrace("Tuning Altered: Allowed (C) " + tuning.FullInteractionName + " : " + tuning.FullObjectName);
                        }

                        break;
                    case WorldRestrictionType.Disallow:
                        if ((tuning.Availability.WorldRestrictionWorldTypes.Contains(WorldType.Vacation)) ||
                            (tuning.Availability.WorldRestrictionWorldTypes.Contains(WorldType.Downtown)) ||
                            (tuning.Availability.WorldRestrictionWorldTypes.Contains(WorldType.University)))
                        {
                            tuning.Availability.WorldRestrictionWorldTypes.Remove(WorldType.Vacation);
                            tuning.Availability.WorldRestrictionWorldTypes.Remove(WorldType.Downtown);
                            tuning.Availability.WorldRestrictionWorldTypes.Remove(WorldType.University);

                            BooterLogger.AddTrace("Tuning Altered: Allowed (B) " + tuning.FullInteractionName + " : " + tuning.FullObjectName);
                        }

                        break;
                }
            }
        }

        public static void MergeFromCrossWorldData()
        {
            Traveler.Settings.MergeFromCrossWorldData(sWorldForSims);
        }

        public static void MergeToCrossWorldData()
        {
            Traveler.Settings.MergeToCrossWorldData(sWorldForSims);
        }

        public static bool IsFromDifferentWorld(ulong id)
        {
            string worldFile;
            if (sWorldForSims.TryGetValue(id, out worldFile))
            {
                return (worldFile.ToLower() != World.GetWorldFileName().ToLower());
            }
            else
            {
                return false;
            }
        }

        public void OnWorldLoadFinished()
        {
            if (sInitial)
            {
                MergeToCrossWorldData();
                sInitial = false;
            }

            SetVacationWorld(false, true);

            mOldHouse = Household.ActiveHousehold;

            new Common.DelayedEventListener(EventTypeId.kEventSimSelected, OnSelected);

            InWorldStateEx.LinkToTravelHousehold();

            if (!sDefaultBarTuned)
            {
                Bartending.BarData defaultData;
                if (Bartending.TryGetBarData(Lot.MetaAutonomyType.None, out defaultData))
                {
                    List<Bartending.BarData> choices = new List<Bartending.BarData>();

                    foreach (Bartending.BarData data in Bartending.sData.Values)
                    {
                        if (!data.IsPizzaAvailable) continue;

                        if (data.mFoods.Count == 0) continue;

                        choices.Add(data);
                    }

                    if (choices.Count > 0)
                    {
                        Bartending.BarData choice = RandomUtil.GetRandomObjectFromList(choices);

                        defaultData.mFoodQualityMinimum = choice.mFoodQualityMinimum;
                        defaultData.mFoodQualityMaximum = choice.mFoodQualityMaximum;
                        defaultData.mIsPizzaAvailable = true;
                        defaultData.mPriceMultiplier = choice.mPriceMultiplier;
                        defaultData.mPriceCapMultiplier = choice.mPriceCapMultiplier;
                        defaultData.mFoods = choice.mFoods;
                    }
                }
            }
        }

        protected static void OnSelected(Event e)
        {
            if (GameStates.IsTravelling) return;

            if (GameStates.IsOnVacation)
            {
                SetVacationWorld(false, false);
            }
            else
            {
                Sim actor = e.Actor as Sim;
                if ((actor != null) && (actor.Household != mOldHouse))
                {
                    if (actor.Household != null)
                    {
                        foreach (SimDescription sim in Households.All(actor.Household))
                        {
                            foreach (SimDescription other in Households.All(actor.Household))
                            {
                                if (sim == other) continue;

                                try
                                {
                                    // A Relationship must be created between all travelers or the transition fails
                                    Relationship.Get(sim, other, true);
                                }
                                catch
                                { }
                            }
                        }
                    }

                    mOldHouse = actor.Household;
                }
            }
        }

        public static void SetVacationWorld(bool ignoreHousehold, bool updateSims)
        {
            Common.StringBuilder msg = new Common.StringBuilder("SetVacationWorld" + Common.NewLine);

            try
            {
                ResetWorldType();

                WorldType originalType = GameUtils.GetCurrentWorldType();

                string worldFile = World.GetWorldFileName();

                msg += worldFile + Common.NewLine;

                WorldName currentWorld = WorldName.Undefined;

                msg += "A";

                foreach (KeyValuePair<WorldName, WorldNameData> data in sData)
                {
                    // Do not alter the future world
                    if (data.Key == WorldName.FutureWorld) continue;

                    if (data.Value.mWorldFile == worldFile)
                    {
                        currentWorld = data.Key;
                    }
                    else
                    {
                        if (originalType != WorldType.Future)
                        {
                            // Make all other worlds vacation worlds to allow the Core to age the sims while playing home-town
                            GameUtils.WorldNameToType[data.Key] = WorldType.Vacation;
                        }
                        else
                        {
                            // Alter all worlds to *not* allow aging
                            GameUtils.WorldNameToType[data.Key] = WorldType.Base;
                        }
                    }
                }

                msg += "B";

                msg += Common.NewLine + "CurrentWorld: " + currentWorld + Common.NewLine;

                bool adjustBaseBooks = false;

                // Change the books so the base world books can be seen in Traveler worlds or while "Treat As Vacation" is false
                switch (currentWorld)
                {
                    case WorldName.China:
                    case WorldName.Egypt:
                    case WorldName.France:
                        if (!Traveler.Settings.mTreatAsVacation)
                        {
                            adjustBaseBooks = true;
                        }
                        break;
                    default:
                        adjustBaseBooks = true;
                        break;
                }

                if ((GameStates.HasTravelData) && (Traveler.Settings.mTreatAsVacation) && ((ignoreHousehold) || (GameStates.TravelHousehold == Household.ActiveHousehold)))
                {
                    switch(currentWorld)
                    {
                        case WorldName.University:
                            GameUtils.WorldNameToType[currentWorld] = WorldType.University;
                            break;
                        case WorldName.FutureWorld:
                            GameUtils.WorldNameToType[currentWorld] = WorldType.Future;
                            break;
                        default:
                            GameUtils.WorldNameToType[currentWorld] = WorldType.Vacation;
                            break;
                    }

                    GameUtils.CheatOverrideCurrentWorld = currentWorld;
                }
                else
                {
                    switch (GameUtils.GetCurrentWorldType())
                    {
                        case WorldType.Vacation:
                        case WorldType.University:
                        case WorldType.Future:
                            GameUtils.WorldNameToType[currentWorld] = WorldType.Base;

                            GameUtils.CheatOverrideCurrentWorld = currentWorld;
                            break;
                    }
                }

                msg += Common.NewLine + "CurrentWorldType: " + GameUtils.GetCurrentWorldType() + Common.NewLine;

                if (sAdjustedStoreItems == null)
                {
                    sAdjustedStoreItems = new List<BookBaseStoreItem>();

                    foreach (List<StoreItem> list in Bookstore.mItemDictionary.Values)
                    {
                        foreach (StoreItem item in list)
                        {
                            BookBaseStoreItem baseItem = item as BookBaseStoreItem;
                            if (baseItem == null) continue;

                            if (baseItem.mAllowedWorldTypes == null) continue;

                            if (!baseItem.mAllowedWorldTypes.Contains(WorldType.Base)) continue;

                            if (baseItem.mAllowedWorldTypes.Contains(WorldType.Vacation)) continue;

                            sAdjustedStoreItems.Add(baseItem);
                        }
                    }
                }

                foreach (BookBaseStoreItem item in sAdjustedStoreItems)
                {
                    if (adjustBaseBooks)
                    {
                        if (!item.mAllowedWorldTypes.Contains(WorldType.Vacation))
                        {
                            item.mAllowedWorldTypes.Add(WorldType.Vacation);
                        }
                    }
                    else
                    {
                        item.mAllowedWorldTypes.Remove(WorldType.Vacation);
                    }
                }

                msg += "C";

                if (PetPoolManager.sPetConfigManager != null)
                {
                    foreach (PetPoolConfig config in PetPoolManager.sPetConfigManager.Values)
                    {
                        if (config == null) continue;

                        if (config.mAllowedWorldTypes == null)
                        {
                            config.mAllowedWorldTypes = new List<string>();
                        }

                        config.mAllowedWorldTypes.Remove(WorldType.Vacation.ToString());
                        config.mAllowedWorldTypes.Remove(WorldType.University.ToString());
                        config.mAllowedWorldTypes.Remove(WorldType.Future.ToString());

                        if (!config.mAllowedWorldTypes.Contains(WorldType.Downtown.ToString()))
                        {
                            if (originalType != WorldType.Downtown)
                            {
                                config.mAllowedWorldTypes.Add(WorldType.Vacation.ToString());
                                config.mAllowedWorldTypes.Add(WorldType.University.ToString());
                                config.mAllowedWorldTypes.Add(WorldType.Future.ToString());
                            }
                        }
                        else
                        {
                            config.mAllowedWorldTypes.Add(WorldType.Vacation.ToString());
                            config.mAllowedWorldTypes.Add(WorldType.University.ToString());
                            config.mAllowedWorldTypes.Add(WorldType.Future.ToString());
                        }

                        config.mDisallowedWorldNames = null;
                    }
                }

                msg += "D";

                Navigation navigation = Navigation.Instance;

                if ((navigation != null) && (navigation.mInfoStateButtons.Length >= 2) && (navigation.mInfoStateButtons[1] != null))
                {
                    /*if (GameUtils.IsOnVacation())
                    {
                        navigation.mInfoStateButtons[1].TooltipText = Common.LocalizeEAString("Ui/Caption/Hud/Navigation:OnVacationNoWork");
                        navigation.mInfoStateButtons[1].Enabled = false;
                    }
                    else*/
                    {
                        navigation.mInfoStateButtons[1].TooltipText = null;
                        navigation.mInfoStateButtons[1].Enabled = true;
                    }
                }

                msg += "E";

                if (updateSims)
                {
                    currentWorld = GameUtils.GetCurrentWorld();

                    foreach (SimDescription sim in Household.EverySimDescription())
                    {
                        if (sim.Household == null) continue;

                        if (sim.Household.IsTravelHousehold) continue;

                        if (sim.Household.IsTouristHousehold) continue;

                        sim.mHomeWorld = currentWorld;
                    }
                }

                msg += "F";

                // Startup the service manually

                if (SocialWorkerPetPutUp.Instance == null)
                {
                    SocialWorkerPetPutUp.Create();
                }

                if (SocialWorkerPetAdoption.Instance == null)
                {
                    SocialWorkerPetAdoption.Create();
                }

                if (SocialWorkerAdoption.Instance == null)
                {
                    SocialWorkerAdoption.Create();
                }

                if (Babysitter.Instance == null)
                {
                    Babysitter.Create();
                }

                if (Repoman.Instance == null)
                {
                    Repoman.Create();
                }

                if (NewspaperDelivery.Instance == null)
                {
                    NewspaperDelivery.Create();
                }
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
            finally
            {
                //Common.StackLog(msg);
            }
        }

        public static void ForceTreasureSpawn()
        {
            try
            {
                TreasureSpawnPoint.OnTravelComplete();
            }
            catch (Exception e)
            {
                Common.Exception("", e);
            }
        }

        public static void ForceSpawners()
        {
            ForceTreasureSpawn();

            Sim selectedActor = PlumbBob.Singleton.mSelectedActor;

            try
            {
                PlumbBob.Singleton.mSelectedActor = null;

                // Force all spawners to spawn something
                foreach (ObjectSpawner spawner in Sims3.Gameplay.Queries.GetObjects<ObjectSpawner>())
                {
                    if (spawner.LotCurrent == null) continue;

                    if (spawner.TuningObjectSpawner.SpawnChance <= 0) continue;

                    int maxSpirits = SpiritMaker.kMaxSpirits;

                    try
                    {
                        if (spawner.LotCurrent.IsWorldLot)
                        {
                            SpiritMaker.kMaxSpirits = 0;
                        }

                        for (int i = 0x0; (i < ObjectSpawner.kInitialVacationRolls) && (spawner.mObjectSet.Count < spawner.TuningObjectSpawner.MaxSpawnCapacity); i++)
                        {
                            spawner.TrySpawnObject();
                        }
                        spawner.UpdateSpawnAlarm();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(spawner, e);
                    }
                    finally
                    {
                        SpiritMaker.kMaxSpirits = maxSpirits;
                    }
                }
            }
            finally
            {
                PlumbBob.Singleton.mSelectedActor = selectedActor;
            }
        }

        public void OnWorldQuit()
        {
            ResetWorldType();

            mOldHouse = null;

            sPreviousWorld = WorldName.Undefined;
        }

        public static bool IsPreviousWorld()
        {
            return (sPreviousWorld == GameUtils.GetCurrentWorld());
        }

        public static void SetPreviousWorld()
        {
            sPreviousWorld = GameUtils.GetCurrentWorld();
        }

        public static void ResetWorldType()
        {
            if (sOrigWorldNameToType != null)
            {
                GameUtils.WorldNameToType = new Dictionary<WorldName, WorldType>(sOrigWorldNameToType);

                GameUtils.CheatOverrideCurrentWorld = WorldName.Undefined;

                if (!GameStates.HasTravelData) 
                {
                    switch (GameUtils.GetCurrentWorldType())
                    {
                        case WorldType.Vacation:
                        case WorldType.University:
                        case WorldType.Future:
                            GameUtils.WorldNameToType[GameUtils.GetCurrentWorld()] = WorldType.Base;
                            break;
                    }
                }
            }
        }

        public static bool GetLoadFileName(WorldName world, bool useTravelData)
        {
            WorldNameData data = null;
            if (sData.TryGetValue(world, out data))
            {
                GameStates.GetLoadFileName(data.mWorldFile, data.mSaveFile, useTravelData);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Externalized to [Register]
        public static Dictionary<WorldName, string> GetWorlds(Dictionary<WorldName, string> worlds)
        {
            foreach (KeyValuePair<WorldName, WorldNameData> data in sData)
            {
                worlds[data.Key] = GetLocationName(data.Key);
            }

            return worlds;
        }

        public static string GetLocationName(WorldName world)
        {
            string key = "Gameplay/Visa/TravelUtil:" + world + "Full";

            if (Localization.HasLocalizationString(key))
            {
                return Common.LocalizeEAString(key);
            }
            else
            {
                WorldNameData data;
                if (sData.TryGetValue(world, out data))
                {
                    return data.mDestinationInfoName;
                }
                else
                {
                    return key;
                }
            }
        }

        public static string GetWorldDataToString()
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            foreach (KeyValuePair<WorldName, WorldNameData> data in sData)
            {
                msg += Common.NewLine + "WorldName: " + data.Key;
                msg += data.Value;
            }

            return msg.ToString();
        }

        public class WorldNameData
        {
            public readonly string mWorldFile;
            public readonly string mSaveFile;

            public readonly string mDestinationInfoImage;
            public readonly string mDestinationInfoName;
            public readonly string mDestinationInfoDescription;
            public readonly UIImage mDestinationInfoConfirmImage;
            public readonly string mDestinationInfoConfirmDescription;

            public WorldNameData(string worldFile, string saveFile, string destinationInfoImage, string destinationInfoName, string destinationInfoDescription, UIImage destinationInfoConfirmImage, string destinationInfoConfirmDescription)
            {
                mWorldFile = worldFile;
                mSaveFile = saveFile;
                mDestinationInfoImage = destinationInfoImage;
                mDestinationInfoName = destinationInfoName;
                mDestinationInfoDescription = destinationInfoDescription;
                mDestinationInfoConfirmImage = destinationInfoConfirmImage;
                mDestinationInfoConfirmDescription = destinationInfoConfirmDescription;
            }

            public override string ToString()
            {
                string text = null;
                
                text += Common.NewLine + "  WorldFile: " + mWorldFile;
                text += Common.NewLine + "  SaveFile: " + mSaveFile;
                text += Common.NewLine + "  Caption: " + mDestinationInfoName;

                return text;
            }
        }
    }
}