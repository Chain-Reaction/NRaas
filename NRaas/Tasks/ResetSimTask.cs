using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Stores;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Tasks
{
    public class ResetSimTask : Common.AlarmTask
    {
        Sim mSim;

        static Common.MethodStore sSimReset = new Common.MethodStore("NRaasDresser", "NRaas.Dresser", "AddResetSimDesc", new Type[] { typeof(ulong) });

        public ResetSimTask(Sim sim)
            : base(1, TimeUnit.Seconds)
        {
            mSim = sim;
        }

        protected override void OnPerform()
        {
            mSim = Perform(mSim, true);
        }

        public static void ResetSkillModifiers(SimDescription sim)
        {
            if (sim.SkillManager == null) return;

            Corrections.CorrectOverallSkillModifier(sim);

            sim.SkillManager.mSkillModifiers = new Dictionary<SkillNames, float>();

            TraitManager traitManager = sim.TraitManager;
            if (traitManager != null)
            {
                foreach (Trait trait in traitManager.List)
                {
                    traitManager.AddTraitSkillGainModifiers(sim, trait.Guid);
                }
            }

            Dictionary<GameObject, bool> inventory = new Dictionary<GameObject, bool>();
            if ((sim.CreatedSim != null) && (sim.CreatedSim.Inventory != null))
            {
                foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.CreatedSim.Inventory))
                {
                    inventory.Add(obj, true);
                }
            }

            ulong eventId = (ulong)EventTypeId.kSkillLearnedSkill;

            Dictionary<ulong, List<EventListener>> events;
            if (!EventTracker.Instance.mListeners.TryGetValue(eventId, out events))
            {
                events = null;
            }
            else
            {
                EventTracker.Instance.mListeners.Remove(eventId);
            }

            if ((sim.CreatedSim != null) && (!sim.CreatedSim.HasBeenDestroyed))
            {
                foreach (Skill skill in sim.SkillManager.List)
                {
                    bool isChangingWorlds = GameStates.sIsChangingWorlds;

                    // Workaround for issue in IsIdTravelling
                    GameStates.sIsChangingWorlds = false;
                    try
                    {
                        skill.OnSkillAddition(true);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, null, "Skill: " + skill.Guid.ToString(), e);
                    }
                    finally
                    {
                        GameStates.sIsChangingWorlds = isChangingWorlds;
                    }
                }
            }

            if (events != null)
            {
                EventTracker.Instance.mListeners.Add(eventId, events);
            }

            if ((sim.CreatedSim != null) && (sim.CreatedSim.Inventory != null))
            {
                foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.CreatedSim.Inventory))
                {
                    if (inventory.ContainsKey(obj)) continue;

                    try
                    {
                        sim.CreatedSim.Inventory.RemoveByForce(obj);
                        obj.Destroy(); // Do not use FadeOut(), it hangs the game
                    }
                    catch
                    { }
                }
            }

            if (sim.OccultManager != null)
            {
                OccultVampire vampire = sim.OccultManager.GetOccultType(OccultTypes.Vampire) as OccultVampire;
                if ((vampire != null) && (vampire.AppliedNightBenefits))
                {
                    try
                    {
                        if ((sim.CreatedSim != null) && (!sim.CreatedSim.HasBeenDestroyed))
                        {
                            vampire.SunsetCallback();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
        }

        protected static void ResetPosture(Sim sim)
        {
            if (sim.Posture != null)
            {
                int count = 0;

                Posture posture = sim.Posture;
                while ((posture != null) && (count < 5))
                {
                    try
                    {
                        posture.OnReset(sim);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);                       
                    }

                    count++;
                    posture = posture.PreviousPosture;
                }

                if (posture != null)
                {
                    posture.PreviousPosture = null;
                }

                try
                {
                    sim.Posture = null;
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);

                    sim.mPosture = null;
                }
            }
        }

        protected static void ResetSituations(Sim sim)
        {
            if ((sim.Autonomy != null) &&
                (sim.Autonomy.SituationComponent != null) &&
                (sim.Autonomy.SituationComponent.Situations != null))
            {
                List<Situation> situations = new List<Situation>(sim.Autonomy.SituationComponent.Situations);

                foreach (Situation situation in situations)
                {
                    try
                    {
                        FilmCareerSituation filmSituation = situation as FilmCareerSituation;
                        if (filmSituation != null)
                        {
                            List<Sim> jobTargets = new List<Sim>();
                            foreach (Sim target in filmSituation.mJobTargets)
                            {
                                if (target == null) continue;

                                jobTargets.Add(target);
                            }

                            filmSituation.mJobTargets = jobTargets;
                        }

                        situation.Exit();
                    }
                    catch (Exception exception)
                    {
                        Common.DebugException(sim, exception);
                    }
                }
            }
        }

        protected static void ResetCareer(SimDescription sim)
        {
            try
            {
                Sims3.Gameplay.Careers.Career career = sim.Occupation as Sims3.Gameplay.Careers.Career;
                if (career != null)
                {
                    if (career.HighestCareerLevelAchieved == null)
                    {
                        if ((career.mHighestLevelAchievedBranchName != null) && (career.mHighestLevelAchievedVal != -1))
                        {
                            career.HighestCareerLevelAchieved = career.SharedData.CareerLevels[career.mHighestLevelAchievedBranchName][career.mHighestLevelAchievedVal];
                        }
                        else
                        {
                            career.HighestCareerLevelAchieved = career.CurLevel;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, null, "HighestLevelAchieved", e);
            }
        }

        protected static void ResetInventory(Sim sim)
        {
            if ((sim.Inventory != null) && (ParentsLeavingTownSituation.sAdultsInventories != null))
            {
                List<InventoryItem> inventory;
                if (ParentsLeavingTownSituation.sAdultsInventories.TryGetValue(sim.SimDescription.SimDescriptionId, out inventory))
                {
                    Inventories.RestoreInventoryFromList(sim.Inventory, inventory, false);
                    ParentsLeavingTownSituation.sAdultsInventories.Remove(sim.SimDescription.SimDescriptionId);
                }
            }
        }

        protected static void CleanupSlots(Sim sim)
        {
            foreach (Slot slot in sim.GetContainmentSlots())
            {
                IGameObject obj = sim.GetContainedObject(slot);
                if (obj == null) continue;

                try
                {
                    obj.UnParent();
                    obj.RemoveFromUseList(sim);

                    if (!(obj is Sim))
                    {
                        if ((sim.Inventory == null) || (!sim.Inventory.TryToAdd(obj, false)))
                        {
                            obj.Destroy();
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, obj, e);
                }
            }
        }

        public static void UpdateInterface(Sim sim)
        {
            try
            {
                if ((sim == Sim.ActiveActor) && (HudController.Instance != null))
                {
                    Sims3.Gameplay.UI.HudModel hudModel = HudController.Instance.mHudModel as Sims3.Gameplay.UI.HudModel;
                    if (hudModel != null)
                    {
                        hudModel.OnInteractionQueueDirtied();
                    }
                }

                if ((sim.Household != null) && (sim.Household.HouseholdSimsChanged != null))
                {
                    sim.Household.HouseholdSimsChanged(Sims3.Gameplay.CAS.HouseholdEvent.kSimAdded, sim, null);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
            }

        }

        public static void ResetRole(Sim sim)
        {
            if (sim == null) return;

            if (sim.SimDescription.AssignedRole == null) return;

            Role role = sim.SimDescription.AssignedRole;

            if (role.IsActive)
            {
                try
                {
                    role.mIsActive = false;
                    role.StartRole();
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }                
            }
        }

        public static void ResetRouting(Sim sim)
        {
            if (sim == null) return;

            SimRoutingComponent component = sim.SimRoutingComponent;
            if (component != null)
            {
                if (component.LockedDoorsDuringPlan != null && component.LockedDoorsDuringPlan.Count > 0)
                {
                    foreach (Door door in component.LockedDoorsDuringPlan)
                    {
                        if (door == null)
                        {
                            continue;
                        }                        

                        PortalComponent portalComponent = door.PortalComponent;
                        if (portalComponent != null)
                        {
                            portalComponent.FreeAllRoutingLanes();
                        }

                        door.SetObjectToReset();
                    }
                }

                component.LockedDoorsDuringPlan = new List<Door>();
            }
        }

        public static Sim Perform(Sim sim, bool fadeOut)
        {
            if (sim == null) return null;

            try
            {
                SimDescription simDesc = sim.SimDescription;

                if (Simulator.GetProxy(sim.ObjectId) == null)
                {
                    if (simDesc != null)
                    {
                        sim.Destroy();
                    }                  

                    //sim.mSimDescription = null;
                    return null;
                }

                if (simDesc == null)
                {
                    sim.mSimDescription = new SimDescription();

                    sim.Destroy();                    
                    return null;
                }

                if (sim.LotHome != null)
                {
                    simDesc.IsZombie = false;

                    if (simDesc.CreatedSim != sim)
                    {
                        sim.Destroy();

                        simDesc.CreatedSim = null;                        

                        return null;
                    }
                    else
                    {                        
                        Bed myBed = null;
                        BedData myBedData = null;

                        foreach (Bed bed in sim.LotHome.GetObjects<Bed>())
                        {
                            myBedData = bed.GetPartOwnedBy(sim);
                            if (myBedData != null)
                            {
                                myBed = bed;
                                break;
                            }
                        }

                        ResetPosture(sim);

                        if (simDesc.TraitManager == null)
                        {
                            simDesc.mTraitManager = new TraitManager();
                        }

                        try
                        {
                            simDesc.Fixup();

                            Corrections.CleanupBrokenSkills(simDesc, null);

                            ResetCareer(simDesc);

                            simDesc.ClearSpecialFlags();

                            if (simDesc.Pregnancy == null)
                            {
                                try
                                {
                                    if (simDesc.mMaternityOutfits == null)
                                    {
                                        simDesc.mMaternityOutfits = new OutfitCategoryMap();
                                    }
                                    simDesc.SetPregnancy(0, false);

                                    simDesc.ClearMaternityOutfits();
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim, null, "Pregnancy", e);
                                }
                            }

                            if (sim.CurrentCommodityInteractionMap == null)
                            {
                                try
                                {
                                    LotManager.PlaceObjectOnLot(sim, sim.ObjectId);

                                    if (sim.CurrentCommodityInteractionMap == null)
                                    {
                                        sim.ChangeCommodityInteractionMap(sim.LotHome.Map);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim, null, "ChangeCommodityInteractionMap", e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, null, "Fixup", e);
                        }

                        ResetSituations(sim);

                        CleanupSlots(sim);

                        ResetInventory(sim);

                        if (fadeOut)
                        {
                            bool active = (Sim.ActiveActor == sim);

                            if (sSimReset.Valid)
                            {
                                sSimReset.Invoke<bool>(new object[] { simDesc.SimDescriptionId });
                            }

                            ResetRouting(sim);

                            using (CreationProtection protection = new CreationProtection(simDesc, sim, false, true, false))
                            {
                                sim.Destroy();

                                Common.Sleep();

                                sim = FixInvisibleTask.InstantiateAtHome(simDesc, null);
                            }

                            if (sim != null)
                            {
                                if (active)
                                {
                                    try
                                    {
                                        foreach (Sim member in Households.AllSims(sim.Household))
                                        {
                                            if (member.CareerManager == null) continue;

                                            Occupation occupation = member.CareerManager.Occupation;
                                            if (occupation == null) continue;

                                            occupation.FormerBoss = null;
                                        }

                                        using (DreamCatcher.HouseholdStore store = new DreamCatcher.HouseholdStore(sim.Household, true))
                                        {
                                            PlumbBob.DoSelectActor(sim, true);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Common.Exception(sim, null, "DoSelectActor", e);
                                    }
                                }

                                if ((myBed != null) && (myBedData != null))
                                {
                                    if ((sim.Partner != null) && (sim.Partner.CreatedSim != null))
                                    {
                                        myBed.ClaimOwnership(sim, myBedData);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (sim.Inventory == null)
                            {
                                sim.AddComponent<InventoryComponent>(new object[0x0]);
                            }                           

                            if (Instantiation.AttemptToPutInSafeLocation(sim, false))
                            {
                                ResetRouting(sim);

                                sim.SetObjectToReset();                                

                                // This is necessary to clear certain types of interactions
                                //   (it is also called in SetObjectToReset(), though doesn't always work there)
                                if (sim.InteractionQueue != null)
                                {
                                    sim.InteractionQueue.OnReset();
                                }
                            }
                        }

                        ResetSkillModifiers(simDesc);

                        ResetRole(sim);

                        if (simDesc.IsEnrolledInBoardingSchool())
                        {
                            simDesc.BoardingSchool.OnRemovedFromSchool();
                        }

                        MiniSimDescription miniSim = MiniSimDescription.Find(simDesc.SimDescriptionId);
                        if (miniSim != null)
                        {
                            miniSim.Instantiated = true;
                        }

                        UpdateInterface(sim);

                        return sim;
                    }
                }
                else if (simDesc.Service is Butler)
                {
                    if (Instantiation.AttemptToPutInSafeLocation(sim, true))
                    {
                        sim.Motives.RecreateMotives(sim);
                        sim.SetObjectToReset();
                    }

                    return sim;
                }
                else if (simDesc.IsImaginaryFriend)
                {
                    OccultImaginaryFriend friend;
                    if (OccultImaginaryFriend.TryGetOccultFromSim(sim, out friend))
                    {
                        if (Simulator.GetProxy(friend.mDollId) != null)
                        {
                            friend.TurnBackIntoDoll(OccultImaginaryFriend.Destination.Owner);

                            return null;
                        }
                    }
                }
                else if (simDesc.IsBonehilda)
                {
                    foreach (BonehildaCoffin coffin in Sims3.Gameplay.Queries.GetObjects<BonehildaCoffin>())
                    {
                        if (coffin.mBonehilda == simDesc)
                        {
                            coffin.mBonehildaSim = null;
                            break;
                        }
                    }
                }

                if (fadeOut)
                {
                    sim.Destroy();
                }

                return null;
            }
            catch (Exception exception)
            {
                Common.Exception(sim, exception);
                return sim;
            }
        }

        public class HouseholdSimsProtection : IDisposable
        {
            Dictionary<SimDescription, Sim> mSims = new Dictionary<SimDescription, Sim>();

            public HouseholdSimsProtection(Household house)
            {
                foreach (SimDescription sim in house.AllSimDescriptions)
                {
                    if (sim.mSim == null) continue;

                    mSims.Add(sim, sim.mSim);
                    sim.mSim = null;
                }
            }

            public void Dispose()
            {
                foreach (KeyValuePair<SimDescription, Sim> value in mSims)
                {
                    if ((value.Key.mSim == null) && (!value.Value.HasBeenDestroyed))
                    {
                        value.Key.mSim = value.Value;
                    }
                }
            }
        }

        public class CreationProtection : IDisposable
        {
            static ChangingWorldsSuppression sChangingWorldsSuppression = new ChangingWorldsSuppression();

            SimDescription mSim;

            Household.HouseholdSimsChangedCallback mChangedCallback = null;
            Household mChangedHousehold = null;

            Dictionary<SimDescription, Relationship> mRelations = null;

            Genealogy mGenealogy = null;

            ImaginaryDoll mDoll = null;

            List<InventoryItem> mInventory = null;

            DreamCatcher.DreamStore mDreamStore = null;

            SafeStore mSafeStore = null;

            Vehicle mReservedVehicle = null;

            OpportunitiesChangedCallback mOpportunitiesChanged = null;

            List<BuffInstance> mBuffs;

            TraitChip[] mChips;

            bool mWasFutureSim = false;

            float mAcademicPerformance = -101f;
            float mUniversityStudy = -101f;

            public CreationProtection(SimDescription sim, Sim createdSim, bool performLoadFixup, bool performSelectable, bool performUnselectable)
            {
                try
                {
                    mSim = sim;

                    Corrections.RemoveFreeStuffAlarm(sim);

                    // Stops an issue in "GrantFutureObjects" regarding the use of sIsChangingWorlds=true
                    mWasFutureSim = sim.TraitManager.HasElement(TraitNames.FutureSim);
                    sim.TraitManager.RemoveElement(TraitNames.FutureSim);

                    if (SimTypes.IsSelectable(mSim))
                    {
                        Corrections.CleanupBrokenSkills(mSim, null);
                    }

                    if (OpportunityTrackerModel.gSingleton != null)
                    {
                        mOpportunitiesChanged = OpportunityTrackerModel.gSingleton.OpportunitiesChanged;
                        OpportunityTrackerModel.gSingleton.OpportunitiesChanged = null;
                    }

                    if (mSim.TraitChipManager != null)
                    {
                        mChips = mSim.TraitChipManager.GetAllTraitChips();
                        mSim.TraitChipManager.mTraitChipSlots = new TraitChip[7];
                        mSim.TraitChipManager.mValues.Clear();
                    }

                    if (createdSim != null)
                    {
                        if (createdSim.BuffManager != null)
                        {
                            mBuffs = new List<BuffInstance>();

                            foreach (BuffInstance buff in createdSim.BuffManager.List)
                            {
                                mBuffs.Add(buff);
                            }
                        }

                        if (createdSim.Motives != null)
                        {
                            Motive motive = createdSim.Motives.GetMotive(CommodityKind.AcademicPerformance);
                            if (motive != null)
                            {
                                mAcademicPerformance = motive.Value;
                            }

                            motive = createdSim.Motives.GetMotive(CommodityKind.UniversityStudy);
                            if (motive != null)
                            {
                                mUniversityStudy = motive.Value;
                            }
                        }

                        if (createdSim.Inventory != null)
                        {
                            mInventory = createdSim.Inventory.DestroyInventoryAndStoreInList();
                        }

                        mDreamStore = new DreamCatcher.DreamStore(createdSim, false, false);

                        mReservedVehicle = createdSim.GetReservedVehicle();
                        createdSim.ReservedVehicle = null;
                    }

                    SafeStore.Flag flags = SafeStore.Flag.None;

                    if (performSelectable)
                    {
                        flags |= SafeStore.Flag.Selectable;
                    }

                    if (performLoadFixup)
                    {
                        flags |= SafeStore.Flag.LoadFixup;
                    }

                    if (performUnselectable)
                    {
                        flags |= SafeStore.Flag.Unselectable;
                    }

                    mSafeStore = new SafeStore(mSim, flags);

                    // Stops the startup errors when the imaginary friend is broken
                    mDoll = GetDollForSim(sim);
                    if (mDoll != null)
                    {
                        mDoll.mOwner = null;
                    }

                    mGenealogy = sim.mGenealogy;

                    mRelations = Relationships.StoreRelations(sim, null);

                    // Stops all event processing during the creation process
                    EventTracker.sCurrentlyUpdatingDreamsAndPromisesManagers = true;

                    // Stops the interface from updating during OnCreation
                    if (sim.Household != null)
                    {
                        mChangedCallback = sim.Household.HouseholdSimsChanged;
                        mChangedHousehold = sim.Household;

                        sim.Household.HouseholdSimsChanged = null;
                    }

                    sChangingWorldsSuppression.Push();

                    // Stops SetGeneologyRelationshipBits()
                    sim.mGenealogy = new Genealogy(sim);
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            protected static ImaginaryDoll GetDollForSim(SimDescription owner)
            {
                ulong simDescriptionId = owner.SimDescriptionId;
                foreach (ImaginaryDoll doll in Sims3.Gameplay.Queries.GetObjects<ImaginaryDoll>())
                {
                    if (simDescriptionId == doll.GetOwnerSimDescriptionId())
                    {
                        return doll;
                    }
                }
                return null;
            }

            protected static bool UpdateCareer(Career career, CareerLocation location)
            {
                if (location.Career == null) return false;

                if (location.Career.Guid != career.Guid) return false;

                career.CareerLoc = location;
                if (!location.Workers.Contains(career.OwnerDescription))
                {
                    location.Workers.Add(career.OwnerDescription);
                }

                return true;
            }

            public void Dispose()
            {
                Dispose(true, true);
            }
            public void Dispose(bool postLoad, bool isReset)
            {
                try
                {
                    if (mWasFutureSim)
                    {
                        mSim.TraitManager.AddHiddenElement(TraitNames.FutureSim);
                    }

                    if (mSim.CreatedSim != null)
                    {
                        BuffManager buffManager = mSim.CreatedSim.BuffManager;
                        if ((buffManager != null) && (mBuffs != null))
                        {
                            foreach (BuffInstance buff in mBuffs)
                            {
                                buffManager.AddBuff(buff.Guid, buff.mEffectValue, buff.mTimeoutCount, buff.mTimeoutPaused, buff.mAxisEffected, buff.mBuffOrigin, false);
                            }
                        }

                        if ((mInventory != null) && (mSim.CreatedSim.Inventory != null))
                        {
                            Inventories.RestoreInventoryFromList(mSim.CreatedSim.Inventory, mInventory, true);
                        }

                        if (mDreamStore != null)
                        {
                            mDreamStore.Restore(mSim.CreatedSim);
                        }

                        if (mSafeStore != null)
                        {
                            mSafeStore.Dispose();
                        }

                        if (mSim.DeathStyle != SimDescription.DeathType.None)
                        {
                            Urnstone stone = Urnstones.FindGhostsGrave(mSim);
                            if (stone != null)
                            {
                                stone.GhostSetup(mSim.CreatedSim, true);
                            }
                        }

                        mSim.CreatedSim.ReservedVehicle = mReservedVehicle;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, null, "Inventory", e);
                }

                // Must be after the opportunities are restored
                if ((mOpportunitiesChanged != null) && (OpportunityTrackerModel.gSingleton != null))
                {
                    OpportunityTrackerModel.gSingleton.OpportunitiesChanged = mOpportunitiesChanged;
                }

                try
                {
                    if (!postLoad)
                    {
                        if ((mSim.CreatedSim != null) &&
                            (mSim.CreatedSim.OpportunityManager != null) &&
                            (mSim.CreatedSim.OpportunityManager.Count > 0))
                        {
                            OpportunityTrackerModel.FireOpportunitiesChanged();
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, null, "FireOpportunitiesChanged", e);
                }

                EventTracker.sCurrentlyUpdatingDreamsAndPromisesManagers = false;

                mSim.mGenealogy = mGenealogy;

                if (mDoll != null)
                {
                    mDoll.mOwner = mSim;

                    if (SimTypes.IsSelectable(mSim))
                    {
                        try
                        {
                            mDoll.OnOwnerBecameSelectable();
                        }
                        catch (Exception e)
                        {
                            Common.Exception(mSim, e);

                            SimDescription sim = mDoll.GetLiveFormSimDescription();
                            if (sim != null)
                            {
                                new FixInvisibleTask(sim).AddToSimulator();
                            }
                        }
                    }
                }

                if ((mSim.CreatedSim != null) && (mSim.CreatedSim.Motives != null))
                {
                    if (mAcademicPerformance != -101)
                    {
                        mSim.CreatedSim.Motives.CreateMotive(CommodityKind.AcademicPerformance);
                        Motive motive = mSim.CreatedSim.Motives.GetMotive(CommodityKind.AcademicPerformance);
                        if (motive != null)
                        {
                            motive.Value = mAcademicPerformance;
                        }
                    }

                    if (mUniversityStudy != -101)
                    {
                        mSim.CreatedSim.Motives.CreateMotive(CommodityKind.UniversityStudy);
                        Motive motive = mSim.CreatedSim.Motives.GetMotive(CommodityKind.UniversityStudy);
                        if (motive != null)
                        {
                            motive.Value = mUniversityStudy;
                        }
                    }
                }

                Relationships.RestoreRelations(mSim, mRelations);

                if ((mSim.TraitChipManager != null) && (mChips != null))
                {
                    for (int i = 0; i < mChips.Length; i++)
                    {
                        if (mChips[i] == null) continue;

                        Common.StringBuilder name = new Common.StringBuilder();
                        try
                        {
                            name.Append(mChips[i].GetLocalizedName());

                            mSim.TraitChipManager.AddTraitChip(mChips[i], i);
                        }
                        catch (Exception e)
                        {
                            Common.Exception(mSim, null, name, e);
                        }
                    }
                }

                sChangingWorldsSuppression.Pop();

                if ((mChangedHousehold != null) && (mChangedCallback != null))
                {
                    mChangedHousehold.HouseholdSimsChanged = mChangedCallback;
                }
            }

            public class ChangingWorldsSuppression
            {
                int mCount = 0;

                bool mIsChangingWorlds = false;

                public ChangingWorldsSuppression()
                { }

                public void Push()
                {
                    if (mCount == 0)
                    {
                        mIsChangingWorlds = GameStates.sIsChangingWorlds;
                        GameStates.sIsChangingWorlds = true;
                    }

                    mCount++;
                }

                public void Pop()
                {
                    mCount--;
                    if (mCount <= 0)
                    {
                        mCount = 0;

                        GameStates.sIsChangingWorlds = mIsChangingWorlds;
                    }
                }
            }
        }
    }
}