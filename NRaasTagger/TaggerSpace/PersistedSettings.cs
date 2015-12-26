using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.TaggerSpace.Helpers;
using NRaas.TaggerSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TaggerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to enable Sim tags for all Sims in the world")]
        protected static bool kEnableSimTags = false;
        [Tunable, TunableComment("Whether to enable Lot tags for all home lots in the world")]
        protected static bool kEnableLotTags = false;
        [Tunable, TunableComment("Whether to color Sim tags by Sims age")]
        protected static bool kColorTagsByAge = false;
        [Tunable, TunableComment("Whether to color Sim tags by relationship with the active household")]
        protected static bool kColorByRelationship = true;
        [Tunable, TunableComment("Whether to color Lot tags by relationship with the active household")]
        protected static bool kColorLotTagsByRelationship = true;
        [Tunable, TunableComment("Whether to color Sim tags by Sim type")]
        protected static bool kColorBySimType = false;
        [Tunable, TunableComment("Whether to color Sim tags by orientation")]
        protected static bool kColorByOrientation = false;
        [Tunable, TunableComment("Whether to color Sim tags by relationship status")]
        protected static bool kColorByRelationshipStatus = false;
        [Tunable, TunableComment("Whether to enable the Late Night hotspot tag type for StoryProgression personalities")]
        protected static bool kHotspotPersonalityTags = true;
        [Tunable, TunableComment("Whether to color pregnancy tag based on expected gender")]
        protected static bool kColorPregnancyTag = true;
        [Tunable, TunableComment("Whether to use the normal tag without the obnoxious eye catching border for tagged Sims")]
        protected static bool kSubtleTaggedSims = false;

        [Tunable, TunableComment("Whether to show the lot specific interactions")]
        protected static bool kEnableLotInteractions = true;

        [Tunable, TunableComment("Whether to show the sim specific interactions")]
        protected static bool kEnableSimInteractions = true;
        [Tunable, TunableComment("Whether to color Lot tags by available cash (minus due bills and (if installed) SP debt")]
        protected static bool kColorLotTagsByCash = false;
        [Tunable, TunableComment("Whether to color Sim tags by available cash (minus due bills and (if installed) SP debt")]
        protected static bool kColorByCash = false;
        [Tunable, TunableComment("Whether to color Sim tags by job performance")]
        protected static bool kColorByJobPerformance = false;
        [Tunable, TunableComment("Whether to color Sim tags by mood")]
        protected static bool kColorByMood = false;
        [Tunable, TunableComment("Whether to color Sim tags by a particular motive")]
        protected static CommodityKind kColorByCommodity = CommodityKind.None;
        [Tunable, TunableComment("Whether to scale the age in years tag data option to the total length of all seasons")]
        protected static bool kAgeInYearsScalesToSeasons = true;
        [Tunable, TunableComment("A custom year length to scale the age in years tag data option to when kAgeInYearsScalesToSeasons is false (or Seasons isn't installed")]
        protected static int kAgeInYearsCustomLength = 28;
        [Tunable, TunableComment("Whether to force a tag to match all filters or just one")]
        protected static bool kMatchAllActiveFilters = false;

        public bool mEnableSimTags = kEnableSimTags;
        public bool mEnableLotTags = kEnableLotTags;
        public bool mColorTagsByAge = kColorTagsByAge;
        public bool mColorTagsByRelationship = kColorByRelationship;
        public bool mColorLotTagsByRelationship = kColorLotTagsByRelationship;
        public bool mColorTagsBySimType = kColorBySimType;
        public bool mColorTagsByOrientation = kColorByOrientation;
        public bool mColorTagsByRelationshipStatus = kColorByRelationshipStatus;
        public bool mHotspotPersonalityTags = kHotspotPersonalityTags;
        public bool mColorPregnancyTag = kColorPregnancyTag;
        public bool mSubtleTaggedSims = kSubtleTaggedSims;

        public bool mEnableLotInteractions = kEnableLotInteractions;

        public bool mEnableSimInteractions = kEnableSimInteractions;
        public bool mColorLotTagsByCash = kColorLotTagsByCash;
        public bool mColorByCash = kColorByCash;
        public bool mColorByJobPerformance = kColorByJobPerformance;
        public bool mColorByMood = kColorByMood;
        public CommodityKind mColorByCommodity = kColorByCommodity;
        public bool mAgeInYearsScalesToSeasons = kAgeInYearsScalesToSeasons;
        public int mAgeInYearsCustomLength = kAgeInYearsCustomLength;
        public bool mMatchAllActiveFilters = kMatchAllActiveFilters;
        public int mFilterCacheTime = FilterHelper.kFilterCacheTime;

        public Dictionary<uint, TagSettingKey> mCustomTagSettings = new Dictionary<uint, TagSettingKey>();

        // Note when adding types to these Dictionaries... if a user already has settings saved, they won't pick up new types and cause a KeyNotFoundException
        public Dictionary<CASAgeGenderFlags, uint> mAgeColorSettings = new Dictionary<CASAgeGenderFlags, uint>() { { CASAgeGenderFlags.Baby, 4294967295 }, { CASAgeGenderFlags.Toddler, 4294934528 }, { CASAgeGenderFlags.Child, 4278255360 }, { CASAgeGenderFlags.Teen, 4294901760 }, { CASAgeGenderFlags.YoungAdult, 4294967040 }, { CASAgeGenderFlags.Adult, 4278255615 }, { CASAgeGenderFlags.Elder, 4286578943 } };

        public Dictionary<TagDataHelper.TagRelationshipType, uint> mRelationshipColorSettings = new Dictionary<TagDataHelper.TagRelationshipType, uint>() { { TagDataHelper.TagRelationshipType.Unknown, 4294967295 }, { TagDataHelper.TagRelationshipType.Acquaintance, 4294967040 }, { TagDataHelper.TagRelationshipType.Coworker, 4286578943 }, { TagDataHelper.TagRelationshipType.Enemy, 4294901760 }, { TagDataHelper.TagRelationshipType.Family, 4278255615 }, { TagDataHelper.TagRelationshipType.Friend, 4294934528 }, { TagDataHelper.TagRelationshipType.Romantic, 4294934783 } };

        public Dictionary<SimType, uint> mSimTypeColorSettings = new Dictionary<SimType, uint>() { { SimType.Service, 4282270546 }, { SimType.Dead, 4282400567 }, { SimType.Tourist, 4293974031 }, { SimType.Mummy, 4279965975 }, { SimType.SimBot, 4288241419 }, { SimType.Human, 4292334804 }, { SimType.Vampire, 4289400598 }, { SimType.ImaginaryFriend, 4288661481 }, { SimType.Genie, 4284752566 }, { SimType.Fairy, 4280464996 }, { SimType.Werewolf, 4286404647 }, { SimType.Witch, 4289439532 }, { SimType.Zombie, 4279251225 }, { SimType.BoneHilda, 4281809464 }, { SimType.Alien, 4285119617 }, { SimType.Hybrid, 4291420956 }, { SimType.Plantsim, 4279602504 }, { SimType.Mermaid, 4278412238 }, { SimType.Plumbot, 4287466119 }, { SimType.Deer, 4288706379 }, { SimType.WildHorse, 4284736678 }, { SimType.Role, 4293157852 }, { SimType.Dog, 4283519310 }, { SimType.Cat, 4292137620 }, { SimType.Stray, 4285889909 }, { SimType.Horse, 4287986488 }, { SimType.Raccoon, 4291216054 } };

        // I can't figure out why EA won't save these blasted things...
        public Dictionary<string, uint> mSimTypeColorSettingsSave = new Dictionary<string, uint>();

        public Dictionary<TagDataHelper.TagOrientationType, uint> mSimOrientationColorSettings = new Dictionary<TagDataHelper.TagOrientationType, uint>() { { TagDataHelper.TagOrientationType.Asexual, 4288914339 }, { TagDataHelper.TagOrientationType.Bicurious, 4294954048 }, { TagDataHelper.TagOrientationType.Bisexual, 4288368534 }, { TagDataHelper.TagOrientationType.Gay, 4294937600 }, { TagDataHelper.TagOrientationType.Straight, 4278219973 }, { TagDataHelper.TagOrientationType.Undecided, 4291216054 } };

        public Dictionary<SimType, uint> mSimStatusColorSettings = new Dictionary<SimType, uint>() { { SimType.Pregnant, 4278647006 }, { SimType.Single, 4294444578 }, { SimType.Partnered, 4293621778 }, { SimType.Married, 4294636052 } };

        // I can't figure out why EA won't save these blasted things...
        public Dictionary<string, uint> mSimStatusColorSettingsSave = new Dictionary<string, uint>();

        public Dictionary<TagDataHelper.TagDataType, bool> mTagDataSettings = new Dictionary<TagDataHelper.TagDataType, bool>() { { TagDataHelper.TagDataType.LifeStage, true }, { TagDataHelper.TagDataType.AgeInDays, true }, { TagDataHelper.TagDataType.Orientation, true }, { TagDataHelper.TagDataType.Mood, true }, { TagDataHelper.TagDataType.Cash, true }, { TagDataHelper.TagDataType.Debt, true }, { TagDataHelper.TagDataType.NetWorth, true }, { TagDataHelper.TagDataType.CurrentInteraction, true }, { TagDataHelper.TagDataType.DaysTillNextStage, false }, { TagDataHelper.TagDataType.Job, false }, { TagDataHelper.TagDataType.MotiveInfo, false }, { TagDataHelper.TagDataType.Occult, false }, { TagDataHelper.TagDataType.PartnerInfo, false }, { TagDataHelper.TagDataType.PersonalityInfo, true }, { TagDataHelper.TagDataType .PregnancyInfo, false }, { TagDataHelper.TagDataType.LifetimeHappiness, false }, { TagDataHelper.TagDataType.LifetimeWant, false }, { TagDataHelper.TagDataType.Zodiac, false } };

        public List<ulong> mTaggedSims = new List<ulong>();

        public Dictionary<ulong, string> mAddresses = new Dictionary<ulong, string>();
        
        public List<string> mCurrentLotFilters = new List<string>();
        public List<string> mCurrentSimFilters = new List<string>();

        public Dictionary<ulong, List<string>> mCustomSimTitles = new Dictionary<ulong, List<string>>();

        public Dictionary<Lot.MetaAutonomyType, MetaAutonomySettingKey> mMetaAutonomySettings = new Dictionary<Lot.MetaAutonomyType, MetaAutonomySettingKey>();        

        protected bool mDebugging = false;

        public PersistedSettings()
        {            
        }

        public bool TypeHasCustomSettings(uint LotType)
        {
            return mCustomTagSettings.ContainsKey(LotType);
        }            

        public bool HasSimFilterActive()
        {
            return (mCurrentSimFilters.Count > 0);            
        }

        public bool HasLotFilterActive()
        {
            return (mCurrentLotFilters.Count > 0);
        }

        public bool DoesSimMatchSimFilters(ulong sim)
        {
            if (PlumbBob.SelectedActor != null && PlumbBob.SelectedActor.SimDescription != null)
            {
                if (Tagger.Settings.mMatchAllActiveFilters)
                {
                    return FilterHelper.DoesSimMatchFilters(sim, PlumbBob.SelectedActor.SimDescription.SimDescriptionId, Tagger.Settings.mCurrentSimFilters, true);
                }
                else
                {
                    return FilterHelper.DoesSimMatchFilters(sim, PlumbBob.SelectedActor.SimDescription.SimDescriptionId, Tagger.Settings.mCurrentSimFilters, false);
                }
            }

            return false;
        }

        public bool DoesHouseholdMatchLotFilters(List<SimDescription> descs)
        {
            if (PlumbBob.SelectedActor != null && PlumbBob.SelectedActor.SimDescription != null)
            {
                if (Tagger.Settings.mMatchAllActiveFilters)
                {
                    return FilterHelper.DoesAnySimMatchFilters(PlumbBob.SelectedActor.SimDescription.SimDescriptionId, descs, Tagger.Settings.mCurrentLotFilters, true);
                }
                else
                {
                    return FilterHelper.DoesAnySimMatchFilters(PlumbBob.SelectedActor.SimDescription.SimDescriptionId, descs, Tagger.Settings.mCurrentLotFilters, false);
                }
            }

            return false;
        }

        public void ValidateActiveFilters(bool worldLoad)
        {
            foreach (string filter in new List<string>(mCurrentLotFilters))
            {
                if (!FilterHelper.IsValidFilter(filter))
                {
                    mCurrentLotFilters.Remove(filter);                    
                }
            }

            foreach (string filter in new List<string>(mCurrentSimFilters))
            {
                if (!FilterHelper.IsValidFilter(filter))
                {
                    mCurrentSimFilters.Remove(filter);
                }
            }

            if (worldLoad)
            {
                // I still have no idea why EA eats these but we'll use this hacky work around for now
                foreach (KeyValuePair<string, uint> pair in Tagger.Settings.mSimTypeColorSettingsSave)
                {
                    SimType newValue;
                    ParserFunctions.TryParseEnum<SimType>(pair.Key, out newValue, SimType.None);

                    if (newValue != SimType.None)
                    {
                        Tagger.Settings.mSimTypeColorSettings[newValue] = pair.Value;
                    }
                }

                foreach (KeyValuePair<string, uint> pair in Tagger.Settings.mSimStatusColorSettingsSave)
                {
                    SimType newValue;
                    ParserFunctions.TryParseEnum<SimType>(pair.Key, out newValue, SimType.None);

                    if (newValue != SimType.None)
                    {
                        Tagger.Settings.mSimStatusColorSettings[newValue] = pair.Value;
                    }
                }
            }
        }

        public uint GetCustomTagColor(uint lotType)
        {
            TagStaticData data;
            if (!Tagger.staticData.TryGetValue(lotType, out data))
            {               
                return 0;
            }

            TagSettingKey key;
            if (this.mCustomTagSettings.TryGetValue(lotType, out key) && key.Color > 0)
            {                
                return key.Color;
            }
            else
            {                
                return data.Color;
            }
        }

        public void AddAddress(ulong id, string address)
        {
            if (address == null) return;

            if (mAddresses == null) mAddresses = new Dictionary<ulong, string>();

            if (mAddresses.ContainsKey(id))
                mAddresses[id] = address;
            else
                mAddresses.Add(id, address);
        }

        public int TagDataAgeInYearsLength
        {
            get
            {
                if (Tagger.Settings.mAgeInYearsScalesToSeasons && GameUtils.IsInstalled(ProductVersion.EP8))
                {
                    return (int)SeasonsManager.GetYearLength();
                }
                else
                {
                    return Tagger.Settings.mAgeInYearsCustomLength;                    
                }
            }
        }

        public bool SimHasCustomTitles(ulong descId)
        {
            return Tagger.Settings.mCustomSimTitles.ContainsKey(descId);
        }

        public void SetCustomTitles(Sim sim)
        {
            if (sim != null)
            {
                List<string> titles;
                Tagger.Settings.mCustomSimTitles.TryGetValue(sim.SimDescription.SimDescriptionId, out titles);

                SimInfo info = Sims3.UI.Responder.Instance.HudModel.GetSimInfo(sim.ObjectId);
                if (info != null && titles != null)
                {
                    if (titles.Count > 0)
                    {
                        string name = sim.FullName;
                        foreach (string title in titles)
                        {
                            name = name + ", " + title;
                        }

                        info.mName = name;

                        Sims3.Gameplay.UI.HudModel model = Sims3.Gameplay.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel;
                        if (model != null)
                        {
                            foreach (SimInfo info2 in model.mSimList)
                            {
                                if (info2.mGuid == info.mGuid)
                                {
                                    model.mSimList.Remove(info2);
                                    model.mSimList.Add(info);
                                    model.SimNameChanged(info.mGuid);
                                    break;
                                }
                            }
                        }

                        int index = 0;
                        foreach (Skewer.SkewerItem item in Skewer.Instance.mHouseholdItems)
                        {
                            if (item.mSimInfo != null && item.mSimInfo.mGuid == info.mGuid)
                            {
                                item.mSimInfo.mName = name;
                                Skewer.Instance.mHouseholdItems[index] = item;                               
                            }
                        }
                    }
                    else
                    {
                        info.mName = sim.FullName;
                    }
                }
            }
        }

        public MetaAutonomySettingKey GetMASettings(Lot.MetaAutonomyType type)
        {
            return GetMASettings(type, true);
        }

        public MetaAutonomySettingKey GetMASettings(Lot.MetaAutonomyType type, bool returnDefault)
        {
            MetaAutonomySettingKey key;
            if (mMetaAutonomySettings.TryGetValue(type, out key))
            {
                return key;
            }

            if (returnDefault)
            {
                return new MetaAutonomySettingKey(type).PopulateWithDefaults();
            }

            return null;
        }        

        public void AddOrUpdateMASettings(Lot.MetaAutonomyType type, MetaAutonomySettingKey settings)
        {
            if (GetMASettings(type, false) != null)
            {
                mMetaAutonomySettings[type] = settings;
            }
            else
            {
                mMetaAutonomySettings.Add(type, settings);
            }            
        }

        /*
        public MetaAutonomySettingKey GetMASettings(MetaAutonomyVenueType type, uint customType)
        {
            if (type == MetaAutonomyVenueType.None)
            {
                return GetMASettingsForCustomType(customType, true);
            }

            return GetMASettingsForEAType(type, true);
        }

        public MetaAutonomySettingKey GetMASettingsForEAType(MetaAutonomyVenueType type, bool returnDefault)
        {
            MetaAutonomySettingKey key;
            if (mEAMetaAutonomySettings.TryGetValue(type, out key))
            {
                return key;
            }

            if (returnDefault)
            {
                return new MetaAutonomySettingKey(type).PopulateWithDefaults();
            }

            return null;
        }

        public MetaAutonomySettingKey GetMASettingsForCustomType(uint type, bool returnDefault)
        {
            MetaAutonomySettingKey key;
            if (mCustomMetaAutonomySettings.TryGetValue(type, out key))
            {
                return key;
            }

            if (returnDefault)
            {
                return new MetaAutonomySettingKey(type).PopulateWithDefaults();
            }

            return null;
        }

        public void AddOrUpdateMASettings(MetaAutonomyVenueType type, uint customType, MetaAutonomySettingKey settings)
        {
            if (type != MetaAutonomyVenueType.None)
            {
                if (GetMASettingsForEAType(type, false) != null)
                {
                    mEAMetaAutonomySettings[type] = settings;
                }
                else
                {
                    mEAMetaAutonomySettings.Add(type, settings);
                }
            }
            else
            {
                if (GetMASettingsForCustomType(customType, false) != null)
                {
                    mCustomMetaAutonomySettings[customType] = settings;
                }
                else
                {
                    mCustomMetaAutonomySettings.Add(customType, settings);
                }
            }
        }
         */

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }
    }
}