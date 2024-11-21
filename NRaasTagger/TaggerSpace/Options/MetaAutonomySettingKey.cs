using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options
{    
    [Persistable]
    public class MetaAutonomySettingKey : IPersistence
    {
        private uint mGUID; // for custom MA Types - Not used       
        private Lot.MetaAutonomyType mMetaAutonomyType; // for EA
        private MetaAutonomySettingKey mDefaults;

        public float mHourOpen = -1;
        public float mHourClose = -1;

        public MetaAutonomySettingKey()
        { }
        public MetaAutonomySettingKey(Lot.MetaAutonomyType type)
        {
            this.mMetaAutonomyType = type;
            this.mGUID = 0;
        }
        public MetaAutonomySettingKey(uint guid)
        {
            this.mGUID = guid;
            this.mMetaAutonomyType = Lot.MetaAutonomyType.None;
        }

        public uint GUID
        {
            get { return mGUID; }
            private set { }
        }       

        public Lot.MetaAutonomyType MetaAutonomyType
        {
            get { return mMetaAutonomyType; }
            private set { }
        }

        public void SetGUID(string val)
        {
            this.mGUID = ResourceUtils.HashString32(val);
        }

        public MetaAutonomySettingKey PopulateWithDefaults()
        {
            List<MetaAutonomyTuning> tlist;
            if (MetaAutonomyManager.sTunings.TryGetValue(this.MetaAutonomyToVenue(), out tlist))
            {
                MetaAutonomyTuning tuning = tlist[0];
                if (tuning != null)
                {
                    this.mHourOpen = tuning.HourOpen;
                    this.mHourClose = tuning.HourClose;

                    this.mDefaults = this;
                }
            }

            return this;
        }

        public void InjectTuning()
        {
            SetOpenHour();
            SetCloseHour();
        }

        public void SetOpenHour()
        {
            Bartending.BarData data;
            if (Bartending.sData.TryGetValue(this.MetaAutonomyType, out data))
            {
                data.mHourOpen = this.mHourOpen;

                List<Bartending.BartendingVenueHelper> dispose = new List<Bartending.BartendingVenueHelper>();
                foreach (Bartending.BartendingVenueHelper helper in Bartending.sHelpers)
                {
                    if (helper.mLotType == this.MetaAutonomyType)
                    {
                        AlarmManager.Global.OnDispose(helper);
                        dispose.Add(helper);
                    }
                    
                    /*
                    List<AlarmHandle> handles;
                    if (AlarmManager.Global.mGameObjectIndex.TryGetValue(helper, out handles))
                    {
                        foreach (AlarmHandle handle in handles)
                        {
                            List<AlarmManager.Timer> alarms;
                            if (AlarmManager.Global.mTimers.TryGetValue(handle, out alarms))
                            {
                                foreach(AlarmManager.Timer alarm in alarms)
                                {
                                    if (alarm.CallBack.ToString().Contains("Bartender"))
                                    {

                                    }
                                }
                            }
                        }
                    }
                     */
                }

                foreach(Bartending.BartendingVenueHelper helper2 in dispose)
                {
                    Bartending.sHelpers.Remove(helper2);                    
                }
                Bartending.sData[this.MetaAutonomyType] = data;
                Bartending.sHelpers.Add(new Bartending.BartendingVenueHelper(this.MetaAutonomyType, data));
            }

            List<MetaAutonomyTuning> tlist;
            if (MetaAutonomyManager.sTunings.TryGetValue(this.MetaAutonomyToVenue(), out tlist))
            {
                MetaAutonomyTuning tuning = tlist[0];
                if (tuning != null)
                {
                    tuning.HourOpen = this.mHourOpen;
                }
            }

            foreach (IRoleGiverExtended roleObj in Queries.GetObjects(typeof(IRoleGiverExtended)))
            {
                if (roleObj.CurrentRole != null)
                {
                    foreach (AlarmHandle handle in roleObj.CurrentRole.mAlarmHandles)
                    {
                        List<AlarmManager.Timer> alarms;
                        if (AlarmManager.Global.mTimers.TryGetValue(handle, out alarms))
                        {
                            AlarmManager.Timer alarm = alarms[0];
                            if (alarm != null)
                            {
                                if (alarm.CallBack.ToString().Contains("StartRole"))
                                {
                                    AlarmManager.Global.SetNewAlarmTime(handle, this.mHourOpen, TimeUnit.Hours);
                                }
                            }
                        }
                    }
                }
            }            
        }

        public void SetCloseHour()
        {
            Bartending.BarData data;
            if (Bartending.sData.TryGetValue(this.MetaAutonomyType, out data))
            {
                data.mHourClose = this.mHourClose; 
               
                List<Bartending.BartendingVenueHelper> dispose = new List<Bartending.BartendingVenueHelper>();
                foreach (Bartending.BartendingVenueHelper helper in Bartending.sHelpers)
                {
                    if (helper.mLotType == this.MetaAutonomyType)
                    {
                        AlarmManager.Global.OnDispose(helper);
                        dispose.Add(helper);
                    }                   
                }

                foreach(Bartending.BartendingVenueHelper helper2 in dispose)
                {
                    Bartending.sHelpers.Remove(helper2);                    
                }
                Bartending.sData[this.MetaAutonomyType] = data;
                Bartending.sHelpers.Add(new Bartending.BartendingVenueHelper(this.MetaAutonomyType, data));
            }            

            List<MetaAutonomyTuning> tlist;
            if (MetaAutonomyManager.sTunings.TryGetValue(this.MetaAutonomyToVenue(), out tlist))
            {
                MetaAutonomyTuning tuning = tlist[0];
                if (tuning != null)
                {
                    tuning.HourClose = this.mHourClose;
                }
            }

            foreach (IRoleGiverExtended roleObj in Queries.GetObjects(typeof(IRoleGiver)))
            {
                if (roleObj.CurrentRole != null)
                {
                    foreach (AlarmHandle handle in roleObj.CurrentRole.mAlarmHandles)
                    {
                        List<AlarmManager.Timer> alarms;
                        if (AlarmManager.Global.mTimers.TryGetValue(handle, out alarms))
                        {
                            AlarmManager.Timer alarm = alarms[0];
                            if (alarm != null)
                            {
                                if (alarm.CallBack.ToString().Contains("EndRole"))
                                {
                                    AlarmManager.Global.SetNewAlarmTime(handle, this.mHourOpen, TimeUnit.Hours);
                                }
                            }
                        }
                    }
                }
            }
        }

        public MetaAutonomyVenueType MetaAutonomyToVenue()
        {
            switch (this.MetaAutonomyType)
            {
                case Lot.MetaAutonomyType.BigPark:
                    return MetaAutonomyVenueType.BigPark;

                case Lot.MetaAutonomyType.SmallPark:
                    return MetaAutonomyVenueType.SmallPark;

                case Lot.MetaAutonomyType.Pool:
                    return MetaAutonomyVenueType.Pool;

                case Lot.MetaAutonomyType.Graveyard:
                    return MetaAutonomyVenueType.Graveyard;

                case Lot.MetaAutonomyType.Library:
                    return MetaAutonomyVenueType.Library;

                case Lot.MetaAutonomyType.Gym:
                    return MetaAutonomyVenueType.Gym;

                case Lot.MetaAutonomyType.ArtGallery:
                    return MetaAutonomyVenueType.ArtGallery;

                case Lot.MetaAutonomyType.FishingVenue:
                    return MetaAutonomyVenueType.FishingVenue;

                case Lot.MetaAutonomyType.BeachVenue:
                    return MetaAutonomyVenueType.BeachVenue;

                case Lot.MetaAutonomyType.MiscellaneousExpectingPeople:
                    return MetaAutonomyVenueType.MiscellaneousExpectingPeople;

                case Lot.MetaAutonomyType.MiscellaneousEmpty:
                    return MetaAutonomyVenueType.MiscellaneousEmpty;

                case Lot.MetaAutonomyType.Dojo:
                    return MetaAutonomyVenueType.Dojo;

                case Lot.MetaAutonomyType.Market:
                    return MetaAutonomyVenueType.Market;

                case Lot.MetaAutonomyType.ChineseGarden:
                    return MetaAutonomyVenueType.ChineseGarden;

                case Lot.MetaAutonomyType.Nectary:
                    return MetaAutonomyVenueType.Nectary;

                case Lot.MetaAutonomyType.MarketSmall:
                    return MetaAutonomyVenueType.MarketSmall;

                case Lot.MetaAutonomyType.BaseCamp:
                    return MetaAutonomyVenueType.BaseCamp;

                case Lot.MetaAutonomyType.ConsignmentStore:
                    return MetaAutonomyVenueType.ConsignmentStore;

                case Lot.MetaAutonomyType.Laundromat:
                    return MetaAutonomyVenueType.Laundromat;

                case Lot.MetaAutonomyType.Salon:
                    return MetaAutonomyVenueType.Salon;

                case Lot.MetaAutonomyType.FireStation:
                    return MetaAutonomyVenueType.FireStation;

                case Lot.MetaAutonomyType.Junkyard:
                    return MetaAutonomyVenueType.Junkyard;

                case Lot.MetaAutonomyType.JunkyardNoVisitors:
                    return MetaAutonomyVenueType.JunkyardNoVisitors;

                case Lot.MetaAutonomyType.Hangout:
                    return MetaAutonomyVenueType.Hangout;

                case Lot.MetaAutonomyType.DiveBarSports:
                    return MetaAutonomyVenueType.DiveBarSports;

                case Lot.MetaAutonomyType.DiveBarIrish:
                    return MetaAutonomyVenueType.DiveBarIrish;

                case Lot.MetaAutonomyType.DiveBarCriminal:
                    return MetaAutonomyVenueType.DiveBarCriminal;

                case Lot.MetaAutonomyType.CocktailLoungeAsian:
                    return MetaAutonomyVenueType.CocktailLoungeAsian;

                case Lot.MetaAutonomyType.CocktailLoungeVampire:
                    return MetaAutonomyVenueType.CocktailLoungeVampire;

                case Lot.MetaAutonomyType.CocktailLoungeCelebrity:
                    return MetaAutonomyVenueType.CocktailLoungeCelebrity;

                case Lot.MetaAutonomyType.DanceClubRave:
                    return MetaAutonomyVenueType.DanceClubRave;

                case Lot.MetaAutonomyType.DanceClubPool:
                    return MetaAutonomyVenueType.DanceClubPool;

                case Lot.MetaAutonomyType.DanceClubLiveMusic:
                    return MetaAutonomyVenueType.DanceClubLiveMusic;

                case Lot.MetaAutonomyType.HorseRanch:
                    return MetaAutonomyVenueType.HorseRanch;

                case Lot.MetaAutonomyType.DogPark:
                    return MetaAutonomyVenueType.DogPark;

                case Lot.MetaAutonomyType.CatJungle:
                    return MetaAutonomyVenueType.CatJungle;

                case Lot.MetaAutonomyType.PetStore:
                    return MetaAutonomyVenueType.PetStore;

                case Lot.MetaAutonomyType.EquestrianCenter:
                    return MetaAutonomyVenueType.EquestrianCenter;

                case Lot.MetaAutonomyType.Bistro:
                    return MetaAutonomyVenueType.Bistro;

                case Lot.MetaAutonomyType.PerformanceClub:
                    return MetaAutonomyVenueType.PerformanceClub;

                case Lot.MetaAutonomyType.PrivateVenue:
                    return MetaAutonomyVenueType.PrivateVenue;

                case Lot.MetaAutonomyType.BigShow:
                    return MetaAutonomyVenueType.BigShow;

                case Lot.MetaAutonomyType.PotionShopConsignmentStore:
                    return MetaAutonomyVenueType.PotionShopConsignmentStore;

                case Lot.MetaAutonomyType.WerewolfBar:
                    return MetaAutonomyVenueType.WerewolfBar;

                case Lot.MetaAutonomyType.Mausoleum:
                    return MetaAutonomyVenueType.Mausoleum;

                case Lot.MetaAutonomyType.Arboretum:
                    return MetaAutonomyVenueType.Arboretum;

                case Lot.MetaAutonomyType.GypsyCaravan:
                    return MetaAutonomyVenueType.GypsyCaravan;

                case Lot.MetaAutonomyType.VaultOfAntiquity:
                    return MetaAutonomyVenueType.VaultOfAntiquity;                

                case Lot.MetaAutonomyType.Dormitory:
                    return MetaAutonomyVenueType.Dormitory;

                case Lot.MetaAutonomyType.Fraternity:
                    return MetaAutonomyVenueType.Fraternity;

                case Lot.MetaAutonomyType.Sorority:
                    return MetaAutonomyVenueType.Sorority;

                case Lot.MetaAutonomyType.StudentUnion:
                    return MetaAutonomyVenueType.StudentUnion;

                case Lot.MetaAutonomyType.RebelHangout:
                    return MetaAutonomyVenueType.RebelHangout;

                case Lot.MetaAutonomyType.UniversityHangout:
                    return MetaAutonomyVenueType.UniversityHangout;

                case Lot.MetaAutonomyType.NerdShop:
                    return MetaAutonomyVenueType.NerdShop;

                case Lot.MetaAutonomyType.Arcade:
                    return MetaAutonomyVenueType.Arcade;

                case Lot.MetaAutonomyType.CoffeeShop:
                    return MetaAutonomyVenueType.CoffeeShop;

                case Lot.MetaAutonomyType.GroupSciencePark:
                    return MetaAutonomyVenueType.GroupSciencePark;

                case Lot.MetaAutonomyType.Resort:
                    return MetaAutonomyVenueType.Resort;

                case Lot.MetaAutonomyType.Diving:
                    return MetaAutonomyVenueType.Diving;

                case Lot.MetaAutonomyType.Port:
                    return MetaAutonomyVenueType.Port;

                case Lot.MetaAutonomyType.BaseCampFuture:
                    return MetaAutonomyVenueType.BaseCampFuture;

                case Lot.MetaAutonomyType.BotEmporium:
                    return MetaAutonomyVenueType.BotEmporium;

                case Lot.MetaAutonomyType.CommunityLivingCenter:
                    return MetaAutonomyVenueType.CommunityLivingCenter;

                case Lot.MetaAutonomyType.Cafeteria:
                    return MetaAutonomyVenueType.Cafeteria;

                case Lot.MetaAutonomyType.GalleryShop:
                    return MetaAutonomyVenueType.GalleryShop;

                case Lot.MetaAutonomyType.RecreationPark:
                    return MetaAutonomyVenueType.RecreationPark;

                case Lot.MetaAutonomyType.Wasteland:
                    return MetaAutonomyVenueType.Wasteland;

                case Lot.MetaAutonomyType.FutureBar:
                    return MetaAutonomyVenueType.FutureBar;

                case Lot.MetaAutonomyType.ServoBotArena:
                    return MetaAutonomyVenueType.ServoBotArena;

                case Lot.MetaAutonomyType.DerelictFutureBeach:
                    return MetaAutonomyVenueType.DerelictFutureBeach;
            }
            return MetaAutonomyVenueType.None;
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("MAType", (uint)mMetaAutonomyType);

            settings.Add("HourOpen", mHourOpen);

            settings.Add("HourClose", mHourClose);
        }

        public void Import(Persistence.Lookup settings)
        {
            if (settings.GetEnum<Lot.MetaAutonomyType>("MAType", Lot.MetaAutonomyType.None) != Lot.MetaAutonomyType.None)
            {
                mMetaAutonomyType = settings.GetEnum<Lot.MetaAutonomyType>("ServiceType", Lot.MetaAutonomyType.None);

                if (settings.Exists("HourOpen"))
                {
                    mHourOpen = settings.GetFloat("HourOpen", -1);
                }

                if (settings.Exists("HourClose"))
                {
                    mHourClose = settings.GetFloat("HourClose", -1);
                }
            }
        }

        public string PersistencePrefix
        {
            get { return null; }
        }
    }
}
