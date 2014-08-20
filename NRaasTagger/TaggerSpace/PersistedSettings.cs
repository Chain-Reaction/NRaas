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
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
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

        public Dictionary<uint, TagSettingKey> mCustomTagSettings = new Dictionary<uint, TagSettingKey>();

        // Note when adding types to these Dictionaries... if a user already has settings saved, they won't pick up new types and cause a KeyNotFoundException
        public Dictionary<CASAgeGenderFlags, uint> mAgeColorSettings = new Dictionary<CASAgeGenderFlags, uint>() { { CASAgeGenderFlags.Baby, 4294967295 }, { CASAgeGenderFlags.Toddler, 4294934528 }, { CASAgeGenderFlags.Child, 4278255360 }, { CASAgeGenderFlags.Teen, 4294901760 }, { CASAgeGenderFlags.YoungAdult, 4294967040 }, { CASAgeGenderFlags.Adult, 4278255615 }, { CASAgeGenderFlags.Elder, 4286578943 } };

        public Dictionary<TagDataHelper.TagRelationshipType, uint> mRelationshipColorSettings = new Dictionary<TagDataHelper.TagRelationshipType, uint>() { { TagDataHelper.TagRelationshipType.Unknown, 4294967295 }, { TagDataHelper.TagRelationshipType.Acquaintance, 4294967040 }, { TagDataHelper.TagRelationshipType.Coworker, 4286578943 }, { TagDataHelper.TagRelationshipType.Enemy, 4294901760 }, { TagDataHelper.TagRelationshipType.Family, 4278255615 }, { TagDataHelper.TagRelationshipType.Friend, 4294934528 }, { TagDataHelper.TagRelationshipType.Romantic, 4294934783 } };

        public Dictionary<SimType, uint> mSimTypeColorSettings = new Dictionary<SimType, uint>() { { SimType.Service, 4282270546 }, { SimType.Dead, 4282400567 }, { SimType.Tourist, 4293974031 }, { SimType.Mummy, 4279965975 }, { SimType.SimBot, 4288241419 }, { SimType.Human, 4292334804 }, { SimType.Vampire, 4289400598 }, { SimType.ImaginaryFriend, 4288661481 }, { SimType.Genie, 4284752566 }, { SimType.Fairy, 4280464996 }, { SimType.Werewolf, 4286404647 }, { SimType.Witch, 4289439532 }, { SimType.Zombie, 4279251225 }, { SimType.BoneHilda, 4281809464 }, { SimType.Alien, 4285119617 }, { SimType.Hybrid, 4291420956 }, { SimType.Plantsim, 4279602504 }, { SimType.Mermaid, 4278412238 }, { SimType.Plumbot, 4287466119 }, { SimType.Deer, 4288706379 }, { SimType.WildHorse, 4284736678 }, { SimType.Role, 4293157852 }, { SimType.Dog, 4283519310 }, { SimType.Cat, 4292137620 }, { SimType.Stray, 4285889909 }, { SimType.Horse, 4287986488 }, { SimType.Raccoon, 4291216054 } };

        public Dictionary<TagDataHelper.TagOrientationType, uint> mSimOrientationColorSettings = new Dictionary<TagDataHelper.TagOrientationType, uint>() { { TagDataHelper.TagOrientationType.Asexual, 4288914339 }, { TagDataHelper.TagOrientationType.Bicurious, 4294954048 }, { TagDataHelper.TagOrientationType.Bisexual, 4288368534 }, { TagDataHelper.TagOrientationType.Gay, 4294937600 }, { TagDataHelper.TagOrientationType.Straight, 4278219973 }, { TagDataHelper.TagOrientationType.Undecided, 4291216054 } };

        public Dictionary<SimType, uint> mSimStatusColorSettings = new Dictionary<SimType, uint>() { { SimType.Pregnant, 4278647006 }, { SimType.Single, 4294444578 }, { SimType.Partnered, 4293621778 }, { SimType.Married, 4294636052 } };

        public Dictionary<TagDataHelper.TagDataType, bool> mTagDataSettings = new Dictionary<TagDataHelper.TagDataType, bool>() { { TagDataHelper.TagDataType.LifeStage, true }, { TagDataHelper.TagDataType.AgeInDays, true }, { TagDataHelper.TagDataType.Orientation, true }, { TagDataHelper.TagDataType.Mood, true }, { TagDataHelper.TagDataType.Cash, true }, { TagDataHelper.TagDataType.Debt, true }, { TagDataHelper.TagDataType.NetWorth, true }, { TagDataHelper.TagDataType.CurrentInteraction, true }, { TagDataHelper.TagDataType.DaysTillNextStage, false }, { TagDataHelper.TagDataType.Job, false }, { TagDataHelper.TagDataType.MotiveInfo, false }, { TagDataHelper.TagDataType.Occult, false }, { TagDataHelper.TagDataType.PartnerInfo, false }, { TagDataHelper.TagDataType.PersonalityInfo, true }, { TagDataHelper.TagDataType .PregnancyInfo, false } };

        public List<ulong> mTaggedSims = new List<ulong>();

        public Dictionary<ulong, string> mAddresses = new Dictionary<ulong, string>();

        //public List<SimFilter> filters = new List<SimFilter>();

        protected bool mDebugging = false;

        [Persistable(false)]
        static SimFilters sFilters = null;

        public bool TypeHasCustomSettings(uint LotType)
        {
            return mCustomTagSettings.ContainsKey(LotType);
        }

        // not implemented yet (and may be a bit yet :) )
        [Persistable(false)]
        public class SimFilters
        {
            public bool Matches()
            {
                return true;
            }

            public bool Matches(List<SimDescription> sims)
            {
                return true;
            }

            public bool Matches(SimDescription sim)
            {
                return true;
            }
        }

        public SimFilters Filters
        {
            get
            {
                if(sFilters == null)
                {
                     return new SimFilters();
                } 

                return sFilters;
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