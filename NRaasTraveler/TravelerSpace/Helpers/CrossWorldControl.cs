using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using NRaas.TravelerSpace.Transfers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class CrossWorldControl
    {
        static List<CrossWorldData> sData = new List<CrossWorldData>();

        public static TransitionRetention sRetention = new TransitionRetention();

        public static void Store(ICollection<SimDescription> travelers)
        {
            sData.Clear();

            foreach (SimDescription sim in travelers)
            {
                sData.Add(new CrossWorldData(sim));

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!assembly.GetName().Name.StartsWith("NRaas")) continue;

                        Type common = assembly.GetType("NRaas.CommonSpace.Helpers.Transition");
                        if (common == null) continue;

                        MethodInfo store = common.GetMethod("Store", BindingFlags.Public | BindingFlags.Static);
                        if (store == null) continue;

                        store.Invoke(null, new object[] { sim });
                    }
                    catch (Exception e)
                    {
                        Common.Exception(assembly.GetName().Name, e);
                    }
                }
            }
        }

        public static void Clear()
        {
            sData.Clear();
        }

        public static bool Restore(SimDescription sim)
        {
            foreach (CrossWorldData data in sData)
            {
                if (data.mName == sim.FullName)
                {
                    data.Restore(sim);

                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        try
                        {
                            //if (!assembly.GetName().Name.StartsWith("NRaas")) continue;

                            Type common = assembly.GetType("NRaas.CommonSpace.Helpers.Transition");
                            if (common == null) continue;

                            MethodInfo restore = common.GetMethod("Restore", BindingFlags.Public | BindingFlags.Static);
                            if (restore == null) continue;

                            restore.Invoke(null, new object[] { sim });
                        }
                        catch (Exception e)
                        {
                            Common.Exception(assembly.GetName().Name, e);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public class TransitionRetention
        {
            public bool mInsanityDebugging;

            public bool mPerformTravelActions;

            public Dictionary<string, object> transitionedSettings = new Dictionary<string, object>();

            public Dictionary<ulong, List<OccultTypes>> transitionedOccults = new Dictionary<ulong, List<OccultTypes>>();

            Dictionary<ulong, ulong> mHouseholds = new Dictionary<ulong, ulong>();

            public Dictionary<ulong, bool> mTrueActives = new Dictionary<ulong, bool>();

            public void OnSwitchWorlds(ICollection<SimDescription> travelers)
            {
                mPerformTravelActions = Traveler.Settings.mPerformTravelActions;

                StoreHouseholds(travelers);

                if (GameStates.sTravelData.mDestWorld == WorldName.FutureWorld)
                {
                    // need to figure out the issue with ITransition and expand this into a deriative search
                    // but I don't see a big use beyond this
                    transitionedSettings["DisableDescendants"] = Traveler.Settings.mDisableDescendants;
                    transitionedSettings["ChanceOfOccultMutation"] = Traveler.Settings.mChanceOfOccultMutation;
                    transitionedSettings["ChanceOfOccultHybrid"] = Traveler.Settings.mChanceOfOccultHybrid;
                    transitionedSettings["MaxOccult"] = Traveler.Settings.mMaxOccult;

                    // can't use the transtioned household because progenitors could be outside of the active household
                    foreach (FutureDescendantService.FutureDescendantHouseholdInfo info in FutureDescendantService.sPersistableData.ActiveDescendantHouseholdsInfo)
                    {
                        if (info.HasAncestorFromHousehold(Household.ActiveHousehold) && info.mProgenitorSimIds.Count > 0)
                        {
                            foreach (ulong num in info.mProgenitorSimIds)
                            {
                                SimDescription sim = SimDescription.Find(num);
                                if (sim != null)
                                {
                                    if (sim.OccultManager != null)
                                    {
                                        if (sim.OccultManager.HasAnyOccultType())
                                        {
                                            transitionedOccults.Add(num, new List<OccultTypes>());
                                            transitionedOccults[num].AddRange(OccultTypeHelper.CreateList(sim.OccultManager.CurrentOccultTypes, true));
                                        }                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }

            protected void StoreHouseholds(ICollection<SimDescription> travelers)
            {
                mHouseholds.Clear();
                mTrueActives.Clear();

                Household active = Household.ActiveHousehold;

                foreach (SimDescription sim in travelers)
                {
                    if (sim.Household == null) continue;

                    if (sim.Household == active)
                    {
                        mTrueActives[sim.SimDescriptionId] = true;
                    }
                    else
                    {
                        mHouseholds[sim.SimDescriptionId] = sim.Household.HouseholdId;

                        //sim.Household.RemoveTemporary(sim);

                        active.AddTemporary(sim);
                    }
                }
            }

            public void OnImportHousehold()
            {
                PersistHouseholds();
            }

            protected void PersistHouseholds()
            {
                Traveler.Settings.StoreHouseholds(mHouseholds);
            }

            public void RestoreHouseholds()
            {
                if (Traveler.Settings.TravelerHouseholds != null)
                {
                    mHouseholds = new Dictionary<ulong, ulong>(Traveler.Settings.TravelerHouseholds);
                }
            }

            public bool IsRestoree(SimDescription sim)
            {
                return mHouseholds.ContainsKey(sim.SimDescriptionId);
            }

            public bool RestoreHousehold(SimDescription sim, ref string reason)
            {
                ulong houseId;
                if (!mHouseholds.TryGetValue(sim.SimDescriptionId, out houseId))
                {
                    reason = "Not External";
                    return false;
                }

                Household house = Household.Find(houseId);
                if (house == null)
                {
                    reason = "House Missing";
                    return false;
                }

                if (sim.Household == house)
                {
                    reason = "Same House";
                    return false;
                }

                if (sim.Household != null)
                {
                    sim.Household.Remove(sim, false);
                }

                if (house.FindMember(sim.SimDescriptionId) != null)
                {
                    reason = "Already Part";
                    return false;
                }

                house.Add(sim);
                return true;
            }
        }

        protected class CrossWorldData
        {
            public readonly string mName;

            int mMalePreference;
            int mFemalePreference;
            CASAgeGenderFlags mPregnantGender;

            OutfitCategories mPreviousOutfitCategory = OutfitCategories.None;
            int mPreviousOutfitIndex;

            List<TraitNames> mTraits = new List<TraitNames>();

            RockBandInfo mBandInfo = null;

            float mLifetimeWishTally = 0;

            Dictionary<InsectType, Collecting.GlowBugStats> mGlowBugData;

            List<OccultBaseClass> mOccult;

            int mMushroomsCollected = 0;

            List<uint> mNectarHashesMade = null;

            SavedOutfit.Cache mOutfitCache = null;

            List<uint> mCrossCountryCompetitionsWon = null;
            List<uint> mJumpCompetitionsWon = null;

            Dictionary<string, Photography.SubjectRecord> mSubjectRecords;

            ResourceKey mSkinToneKey;
            float mSkinToneIndex = 0;

            SocialNetworkingSkill.BlogCreationFlags mBlogsCreated;
            uint mNumberOfFollowers;

            public CrossWorldData(SimDescription sim)
            {
                mName = sim.FullName;

                mOutfitCache = new SavedOutfit.Cache(sim);

                SimOutfit outfit = sim.GetOutfit(OutfitCategories.Everyday, 0);
                if (outfit != null)
                {
                    mSkinToneKey = outfit.SkinToneKey;
                    mSkinToneIndex = outfit.SkinToneIndex;
                }
                else
                {
                    mSkinToneKey = sim.SkinToneKey;
                    mSkinToneIndex = sim.SkinToneIndex;
                }

                mMalePreference = sim.mGenderPreferenceMale;
                mFemalePreference = sim.mGenderPreferenceFemale;

                if (sim.CreatedSim != null)
                {
                    if (sim.CreatedSim.mPreviousOutfitKey != null)
                    {
                        try
                        {
                            mPreviousOutfitCategory = sim.GetOutfitCategoryFromResKey(sim.CreatedSim.mPreviousOutfitKey, out mPreviousOutfitIndex);
                        }
                        catch
                        { }
                    }

                    if (sim.CreatedSim.DreamsAndPromisesManager != null)
                    {
                        ActiveDreamNode node = sim.CreatedSim.DreamsAndPromisesManager.LifetimeWishNode;
                        if (node != null)
                        {
                            mLifetimeWishTally = node.InternalCount;
                        }
                    }
                }

                if (sim.Pregnancy != null)
                {
                    mPregnantGender = sim.Pregnancy.mGender;
                }

                foreach (Trait trait in sim.TraitManager.List)
                {
                    if (trait.IsReward) continue;

                    mTraits.Add(trait.Guid);
                }

                SocialNetworkingSkill networkSkill = sim.SkillManager.GetSkill<SocialNetworkingSkill>(SkillNames.SocialNetworking);
                if (networkSkill != null)
                {
                    // This value is set to mNumberOfBlogFollowers for some reason
                    mNumberOfFollowers = networkSkill.mNumberOfFollowers;

                    // Not transitioned at all
                    mBlogsCreated = networkSkill.mBlogsCreated;
                }

                RockBand bandSkill = sim.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
                if (bandSkill != null)
                {
                    mBandInfo = bandSkill.mBandInfo;
                }

                Collecting collecting = sim.SkillManager.GetSkill<Collecting>(SkillNames.Collecting);
                if (collecting != null)
                {
                    // Error in CollectingPropertyStreamWriter:Export() requires that mGlowBugData by transfered manually
                    //   Exported as two Int64, but Imported as a Int64 and Int32
                    mGlowBugData = collecting.mGlowBugData;

                    mMushroomsCollected = collecting.mMushroomsCollected;
                }

                NectarSkill nectar = sim.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                if (nectar != null)
                {
                    mNectarHashesMade = nectar.mHashesMade;
                }

                Photography photography = sim.SkillManager.GetSkill<Photography>(SkillNames.Photography);
                if (photography != null)
                {
                    mSubjectRecords = photography.mSubjectRecords;
                }

                RidingSkill riding = sim.SkillManager.GetSkill<RidingSkill>(SkillNames.Riding);
                if (riding != null)
                {
                    // Error in the Import (Copy/Paste fail by the looks of it), where the CrossCountry Wins are imported instead
                    mCrossCountryCompetitionsWon = new List<uint>(riding.mCrossCountryCompetitionsWon);
                    mJumpCompetitionsWon = new List<uint>(riding.mJumpCompetitionsWon);
                }

                if ((sim.OccultManager != null) && (sim.OccultManager.mOccultList != null))
                {
                    mOccult = new List<OccultBaseClass>(sim.OccultManager.mOccultList);
                }
            }

            public void Restore(SimDescription sim)
            {
                try
                {
                    sim.mGenderPreferenceMale = mMalePreference;
                    sim.mGenderPreferenceFemale = mFemalePreference;

                    if (sim.Pregnancy != null)
                    {
                        sim.Pregnancy.mGender = mPregnantGender;
                    }

                    if (sim.CreatedSim != null)
                    {
                        if (mPreviousOutfitCategory != OutfitCategories.None)
                        {
                            SimOutfit outfit = sim.GetOutfit(mPreviousOutfitCategory, mPreviousOutfitIndex);
                            if (outfit != null)
                            {
                                sim.CreatedSim.mPreviousOutfitKey = outfit.Key;
                            }
                        }

                        if (sim.CreatedSim.DreamsAndPromisesManager != null)
                        {
                            ActiveDreamNode node = sim.CreatedSim.DreamsAndPromisesManager.LifetimeWishNode;
                            if (node != null)
                            {
                                node.InternalCount = mLifetimeWishTally;
                            }
                        }
                    }

                    foreach (TraitNames trait in mTraits)
                    {
                        if (sim.TraitManager.HasElement(trait)) continue;

                        sim.TraitManager.AddElement(trait);
                    }

                    SocialNetworkingSkill networkSkill = sim.SkillManager.GetSkill<SocialNetworkingSkill>(SkillNames.SocialNetworking);
                    if (networkSkill != null)
                    {
                        networkSkill.mNumberOfFollowers = mNumberOfFollowers;
                        networkSkill.mBlogsCreated = mBlogsCreated;
                    }

                    RockBand bandSkill = sim.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
                    if (bandSkill != null)
                    {
                        bandSkill.mBandInfo = mBandInfo;
                    }

                    Collecting collecting = sim.SkillManager.GetSkill<Collecting>(SkillNames.Collecting);
                    if (collecting != null)
                    {
                        collecting.mGlowBugData = mGlowBugData;
                        collecting.mMushroomsCollected = mMushroomsCollected;
                    }

                    NectarSkill nectar = sim.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                    if (nectar != null)
                    {
                        nectar.mHashesMade = mNectarHashesMade;
                    }

                    Photography photography = sim.SkillManager.GetSkill<Photography>(SkillNames.Photography);
                    if (photography != null)
                    {
                        // Forces a recalculation of the completion count
                        photography.mCollectionsCompleted = uint.MaxValue;

                        if (mSubjectRecords != null)
                        {
                            photography.mSubjectRecords = mSubjectRecords;
                        }
                    }

                    RidingSkill riding = sim.SkillManager.GetSkill<RidingSkill>(SkillNames.Riding);
                    if (riding != null)
                    {
                        if (mCrossCountryCompetitionsWon != null)
                        {
                            riding.mCrossCountryCompetitionsWon = mCrossCountryCompetitionsWon.ToArray();
                        }

                        if (mJumpCompetitionsWon != null)
                        {
                            riding.mJumpCompetitionsWon = mJumpCompetitionsWon.ToArray();
                        }
                    }

                    if (mOccult != null)
                    {
                        foreach (OccultBaseClass occult in mOccult)
                        {
                            if (OccultTypeHelper.Add(sim, occult.ClassOccultType, false, false))
                            {
                                OccultTransfer transfer = OccultTransfer.Get(occult.ClassOccultType);
                                if (transfer != null)
                                {
                                    transfer.Perform(sim, occult);
                                }
                            }
                        }
                    }

                    mOccult = null;

                    if (mOutfitCache != null)
                    {
                        foreach (SavedOutfit.Cache.Key outfit in mOutfitCache.Outfits)
                        {
                            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, outfit.mKey, false))
                            {
                                builder.Builder.SkinTone = mSkinToneKey;
                                builder.Builder.SkinToneIndex = mSkinToneIndex;

                                outfit.Apply(builder, true, null, null);
                            }
                        }

                        foreach (SavedOutfit.Cache.Key outfit in mOutfitCache.AltOutfits)
                        {
                            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, outfit.mKey, true))
                            {
                                builder.Builder.SkinTone = mSkinToneKey;
                                builder.Builder.SkinToneIndex = mSkinToneIndex;

                                outfit.Apply(builder, true, null, null);
                            }
                        }

                        int count = 0;
                        int originalCount = mOutfitCache.GetOutfitCount(OutfitCategories.Everyday, false);

                        while ((originalCount > 0) && (originalCount < sim.GetOutfitCount(OutfitCategories.Everyday)) && (count < originalCount))
                        {
                            CASParts.RemoveOutfit(sim, new CASParts.Key(OutfitCategories.Everyday, sim.GetOutfitCount(OutfitCategories.Everyday)-1), false);
                            count++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            public override string ToString()
            {
                return mName;
            }
        }
    }
}
