using NRaas.CommonSpace;
using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.TaggerSpace;
using NRaas.TaggerSpace.Booters;
using NRaas.TaggerSpace.Helpers;
using NRaas.TaggerSpace.MapTags;
using NRaas.TaggerSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class Tagger : Common, Common.IStartupApp, Common.IPreLoad, Common.IWorldLoadFinished, Common.IDelayedWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        public static Dictionary<uint, TagStaticData> staticData = new Dictionary<uint, TagStaticData>();        

        static Tagger()
        {
            Bootstrap();

            BooterHelper.Add(new LotTypeBooter(BooterHelper.sBootStrapFile, false));

            World.sOnLotTypeChangedEventHandler += new EventHandler(OnLotTypeChanged);
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

            Tagger.RemoveTags();
        }

        public void OnStartupApp()
        {            
            EnumInjection.InjectEnums<MapTagType>(new string[] { "CustomTagNRaas" }, new object[] { 0xB4 }, false);            
        }

        public void OnPreLoad()
        {
            foreach (KeyValuePair<uint, TagStaticData> data in staticData)
            {                
                InjectionHelper.InjectCommunityType(data.Value.name);
                InjectionHelper.InjectRealEstateData(data.Value.name);
            }            

            InjectionHelper.InjectMapTag("CustomTagNRaas");            
        }

        public ListenerAction OnLiveMode(Event e)
        {
            InWorldSubStateEvent event2 = e as InWorldSubStateEvent;
            if (event2 != null)
            {
                if (event2.State.StateId == 0)
                {
                    SetupMapTags(true);
                }
            }

            return ListenerAction.Remove;
        }

        public ListenerAction OnSimInstantiated(Event e)
        {
            Sim sim = e.TargetObject as Sim;
            if (sim != null && CameraController.IsMapViewModeEnabled())
            {
                MapTagHelper.SetupSimTag(sim);
            }

            return ListenerAction.Keep;
        }

        public static void OnSimSelected(Event e)
        {            
            SetupMapTags(false);

            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                Tagger.Settings.SetCustomTitles(sim);
            }            
        }

        public static void OnActiveHouseholdMoved(Event e)
        {
            SetupMapTags(false);            
        }

        public static void SetupMapTags()
        {
            SetupMapTags(false, false);
        }

        public static void SetupMapTags(bool initial)
        {
            SetupMapTags(initial, false);
        }

        public static void SetupMapTags(bool initial, bool lotOnly)
        {
            MapTagsModel model = MapTagsModel.Singleton;
            if (model != null && !GameStates.IsGameShuttingDown)
            {
                if (initial)
                {                    
                    Sims3.UI.Responder.Instance.MapTagsModel.MapTagAdded += new MapTagChangedDelegate(MapTagHelper.OnMapTagAdded);
                    Sims3.UI.Responder.Instance.MapTagsModel.MapTagRefreshAll += OnRefreshed;
                    CameraController.OnCameraMapViewEnabledCallback += OnMapView;
                    new Common.ImmediateEventListener(EventTypeId.kEventSimSelected, OnSimSelected);
                    new Common.ImmediateEventListener(EventTypeId.kLotChosenForActiveHousehold, OnActiveHouseholdMoved);
                }

                InitTags(lotOnly);
            }
        }

        public static void InitTags(bool lotOnly)
        {
            MapTagsModel model = MapTagsModel.Singleton;
            if (model != null)
            {
                if (CameraController.IsMapViewModeEnabled())
                {
                    Tagger.Settings.ValidateActiveFilters(false);
                    if (!lotOnly)
                    {
                        if (Tagger.staticData.Count > 0)
                        {
                            foreach (IMapTag tag in model.GetCurrentMapTags())
                            {
                                MapTagHelper.OnMapTagAdded(tag);
                            }
                        }

                        if (Tagger.Settings.mEnableSimTags || Tagger.Settings.mTaggedSims.Count > 0)
                        {
                            if (Tagger.Settings.mColorByCash)
                            {
                                TagDataHelper.GenerateMoneyGraphData();
                            }

                            foreach (Sim sim in LotManager.Actors)
                            {
                                if (sim.SimDescription.CreatedSim != null && !sim.HasBeenDestroyed)
                                {
                                    MapTagHelper.SetupSimTag(sim);
                                }
                            }
                        }
                    }

                    if (Tagger.Settings.mEnableLotTags)
                    {
                        if (Tagger.Settings.mColorLotTagsByCash)
                        {
                            TagDataHelper.GenerateMoneyGraphData();
                        }

                        foreach (Lot lot in LotManager.AllLots)
                        {
                            if (lot.Household != null && lot.ObjectId != ObjectGuid.InvalidObjectGuid)
                            {
                                MapTagHelper.SetupLotTag(lot);
                            }
                        }
                    }                    
                }
                else
                {                    
                    RemoveTags();
                }
            }
        }      
  
        public static void RemoveTags()
        {
            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return;

            MapTagManager mtm = active.MapTagManager;
            if (mtm == null) return;

            if(!Tagger.Settings.mEnableLotTags)
            {
                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.Household != null && lot.ObjectId != ObjectGuid.InvalidObjectGuid)
                    {
                        MapTag tag = mtm.GetTag(lot);
                       
                        if(tag != null && tag is TrackedLot)
                        {
                            mtm.RemoveTag(tag);
                        }
                    }
                }
            }

            if (!Tagger.Settings.mEnableSimTags)
            {
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.SimDescription.CreatedSim != null)
                    {
                        MapTag tag = mtm.GetTag(sim);

                        if (tag != null && tag is TrackedSim && !Tagger.Settings.mTaggedSims.Contains(sim.SimDescription.SimDescriptionId))
                        {
                            mtm.RemoveTag(tag);
                        }
                    }
                }
            }
        }

        public static void OnMapView(bool enabled)
        {
            if (enabled)
            {                
                SetupMapTags(false);
            }
        }

        public static void OnRefreshed(object sender, EventArgs e)
        {
            SetupMapTags(false);
        }

        // doesn't seem to be firing...
        public static void OnLotTypeChanged(object sender, EventArgs args)
        {            
            // until I can work on custom MA types, this should push some Sims to the lots (and urge them to buy stuff if the items exist)
            World.OnLotTypeChangedEventArgs args2 = args as World.OnLotTypeChangedEventArgs;
            if (args2 != null)
            {
                Lot lot = LotManager.GetLot(args2.LotId);
                if (lot != null)
                {
                    if (MapTagHelper.ShouldReplace(lot))
                    {                        
                        SetMetaAutonomyType(lot, Lot.MetaAutonomyType.MarketSmall);
                    }
                }
            }
        }

        public static void SetMetaAutonomyType(Lot lot, Lot.MetaAutonomyType type)
        {
            if (lot == null)
            {
                return;
            }

            lot.mMetaAutonomyType = type;
            Autonomy.AddPublicMetaObject(lot);

            MetaAutonomyTuning tuning = MetaAutonomyManager.GetTuning(lot.GetMetaAutonomyVenueType());
            if (tuning != null)
            {
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.HasReasonToGoToVenue(tuning))
                    {
                        MetaAutonomyManager.AddSimToVenue(sim, lot);
                    }
                    else
                    {
                        MetaAutonomyManager.RemoveSimFromVenue(sim, lot);
                    }
                }
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            foreach (Lot lot in LotManager.AllLots)
            {
                if (MapTagHelper.ShouldReplace(lot))
                {
                    if (Tagger.Settings.TypeHasCustomSettings((uint)lot.CommercialLotSubType))
                    {
                        TagSettingKey key = Tagger.Settings.mCustomTagSettings[(uint)lot.CommercialLotSubType];
                        if (key.MetaAutonomyType > 0)
                        {
                            SetMetaAutonomyType(lot, (Lot.MetaAutonomyType)key.MetaAutonomyType);
                        }
                    }
                    else
                    {                        
                        SetMetaAutonomyType(lot, Lot.MetaAutonomyType.Hangout);
                    }
                }
            }

            foreach (KeyValuePair<Lot.MetaAutonomyType, MetaAutonomySettingKey> key in Tagger.Settings.mMetaAutonomySettings)
            {
                key.Value.InjectTuning();
            }
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;
            FilterHelper.kFilterCacheTime = Tagger.Settings.mFilterCacheTime;

            EventTracker.AddListener(EventTypeId.kEnterInWorldSubState, new ProcessEventDelegate(this.OnLiveMode));
            EventTracker.AddListener(EventTypeId.kSimInstantiated, new ProcessEventDelegate(this.OnSimInstantiated));

            List<ulong> invalidIds = new List<ulong>();
            foreach (ulong lotId in Tagger.Settings.mAddresses.Keys)
            {
                Lot lot;
                if (LotManager.sLots.TryGetValue(lotId, out lot))
                {
                    lot.mAddressLocalizationKey = Tagger.Settings.mAddresses[lotId];
                }
                else
                {
                    invalidIds.Add(lotId);
                }
            }
            foreach (ulong id in invalidIds)
            {
                Tagger.Settings.mAddresses.Remove(id);

                BooterLogger.AddTrace("Removed: " + id);
            }

            List<ulong> invalidSims = new List<ulong>();
            foreach (ulong descId in Tagger.Settings.mTaggedSims)
            {
                SimDescription desc = SimDescription.Find(descId);
                MiniSimDescription desc2 = MiniSimDescription.Find(descId);
                if (desc == null && desc2 == null)
                {
                    invalidSims.Add(descId);
                }
            }

            foreach (KeyValuePair<ulong, List<string>> pair in Tagger.Settings.mCustomSimTitles)
            {
                SimDescription desc = SimDescription.Find(pair.Key);
                MiniSimDescription desc2 = MiniSimDescription.Find(pair.Key);
                if (desc == null && desc2 == null)
                {
                    invalidSims.Add(pair.Key);
                }
            }

            // stop KeyNotFoundExceptions... in hindsight should have wrapped these in a get function...
            foreach (TagDataHelper.TagDataType flag in Enum.GetValues(typeof(TagDataHelper.TagDataType)))
            {
                if (!Tagger.Settings.mTagDataSettings.ContainsKey(flag))
                {
                    Tagger.Settings.mTagDataSettings.Add(flag, false);
                }
            }

            Tagger.Settings.ValidateActiveFilters(true);           
        }

        // Externalized to GoHere
        public static bool IsBusinessType(uint type)
        {
            TagStaticData data;
            if (staticData.TryGetValue(type, out data))
            {
                return data.isBusinessType;
            }

            return false;
        }        
    }
}
