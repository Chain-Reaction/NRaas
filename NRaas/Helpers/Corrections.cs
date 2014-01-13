using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Corrections
    {
        public delegate void Logger(string text);

        public static void FixCareer(Occupation job, bool allowDrop, Logger log)
        {
            if (job == null) return;

            if (job.Coworkers != null)
            {
                for (int i = job.Coworkers.Count - 1; i >= 0; i--)
                {
                    SimDescription coworker = job.Coworkers[i];

                    if ((!IsValidCoworker(coworker, job is School)) || (coworker == job.OwnerDescription))
                    {
                        job.Coworkers.RemoveAt(i);

                        if (coworker == null)
                        {
                            if (log != null)
                            {
                                log(" Bad Coworker " + job.CareerName + " - <Invalid>");
                            }
                        }
                        else
                        {
                            if (log != null)
                            {
                                log(" Bad Coworker " + job.CareerName + " - " + coworker.FullName);
                            }
                        }
                    }
                }
            }

            if ((allowDrop) && (job is Career))
            {
                bool replace = false;

                if (job.CareerLoc == null)
                {
                    if (log != null)
                    {
                        log(" Bad Location " + job.CareerName + " - " + job.GetType().ToString());
                    }
                    replace = true;
                }
                else
                {
                    RabbitHole hole = job.CareerLoc.Owner;
                    if (hole == null)
                    {
                        if (log != null)
                        {
                            log(" Missing Rabbithole " + job.CareerName + " - " + job.GetType().ToString());
                        }
                        replace = true;
                    }
                    else
                    {
                        RabbitHole proxy = hole.RabbitHoleProxy;
                        if (proxy == null)
                        {
                            if (log != null)
                            {
                                log(" Missing Proxy " + job.CareerName + " - " + job.GetType().ToString());
                            }
                            replace = true;
                        }
                        else
                        {
                            if ((proxy.EnterSlots == null) || (proxy.EnterSlots.Count == 0) ||
                                (proxy.ExitSlots == null) || (proxy.ExitSlots.Count == 0))
                            {
                                if (log != null)
                                {
                                    log(" Missing Slots " + job.CareerName + " - " + job.GetType().ToString());
                                }
                                replace = true;
                            }
                        }
                    }
                }

                if (replace)
                {
                    SimDescription me = job.OwnerDescription;

                    Occupation retiredJob = me.CareerManager.mRetiredCareer;

                    try
                    {
                        CareerLocation location = Sims3.Gameplay.Careers.Career.FindClosestCareerLocation(me, job.Guid);
                        if (location != null)
                        {
                            if (log != null)
                            {
                                log(" Location Replaced " + me.FullName);
                            }

                            if (job.CareerLoc != null)
                            {
                                job.CareerLoc.Workers.Remove(me);
                            }

                            job.CareerLoc = location;

                            location.Workers.Add(me);
                        }
                        else
                        {
                            if (log != null)
                            {
                                log(" Career Dropped " + me.FullName);
                            }

                            job.LeaveJobNow(Career.LeaveJobReason.kJobBecameInvalid);
                        }
                    }
                    finally
                    {
                        me.CareerManager.mRetiredCareer = retiredJob;
                    }
                }
            }
        }

        public static bool IsValidCoworker(SimDescription sim, bool isSchool)
        {
            if (sim == null) return false;

            if (!sim.IsValidDescription) return false;

            if (sim.Household == null) return false;

            if (isSchool)
            {
                if (sim.CareerManager == null) return false;

                if (sim.CareerManager.School == null) return false;
            }
            else
            {
                if (sim.Occupation == null) return false;
            }

            return true;
        }

        public static void CleanupAcademics(Logger log)
        {
            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);

            foreach (SimDescription sim in sims.Values)
            {
                if (sim.CareerManager == null) continue;

                AcademicCareer career = sim.OccupationAsAcademicCareer;
                if (career != null)
                {
                    if ((career.mDegree == null) || (AcademicDegreeManager.GetStaticElement(career.mDegree.AcademicDegreeName) == null))
                    {
                        career.LeaveJobNow(Career.LeaveJobReason.kJobBecameInvalid);
                    }
                }

                AcademicDegreeManager manager = sim.CareerManager.DegreeManager;
                if (manager == null) continue;

                List<AcademicDegreeNames> remove = new List<AcademicDegreeNames>();

                foreach(AcademicDegree degree in manager.List)
                {
                    if (AcademicDegreeManager.GetStaticElement(degree.AcademicDegreeName) == null)
                    {
                        remove.Add(degree.AcademicDegreeName);
                    }
                }

                foreach(AcademicDegreeNames degree in remove)
                {
                    manager.RemoveElement((ulong)degree);

                    if (log != null)
                    {
                        log("Remove Corrupt Degree: " + sim.FullName);
                    }
                }
            }

            if (AcademicCareer.sAcademicSims != null)
            {
                for (int i = AcademicCareer.sAcademicSims.Count - 1; i >= 0; i--)
                {
                    bool remove = false;

                    SimDescription sim;
                    if (!sims.TryGetValue(AcademicCareer.sAcademicSims[i], out sim))
                    {
                        remove = true;
                    }
                    else
                    {
                        AcademicCareer career = sim.OccupationAsAcademicCareer;
                        if (career == null)
                        {
                            remove = true;
                        }
                        else if (career.mDegree == null)
                        {
                            remove = true;
                        }
                        else if (career.Coworkers == null)
                        {
                            remove = true;
                        }
                    }

                    if (remove)
                    {
                        if (log != null)
                        {
                            if (sim != null)
                            {
                                log(" Removed: " + sim.FullName);
                            }
                            else
                            {
                                log(" Removed: " + AcademicCareer.sAcademicSims[i]);
                            }
                        }

                        AcademicCareer.sAcademicSims.RemoveAt(i);
                    }
                }
            }
        }

        public static void CorrectOverallSkillModifier(SimDescription sim)
        {
            sim.SkillManager.mOverallModifier = sim.SkillManager.mMoodModifier;

            if ((sim.CreatedSim != null) && (sim.CreatedSim.BuffManager != null))
            {
                if (sim.CreatedSim.BuffManager.HasElement(BuffNames.IncreasedUnderstanding))
                {
                    sim.SkillManager.mOverallModifier += ImprovedProtestSituation.kSkillIncreasePctRewardFromCause;
                }
            }
        }

        public static void CleanupCommonDoors(Logger log)
        {
            foreach (CommonDoor door in Sims3.Gameplay.Queries.GetObjects<CommonDoor>())
            {
                if (door.LotCurrent == null) continue;

                if (door.mLockType == CommonDoor.tLock.SelectedHousehold)
                {
                    if (door.mLockOwner == null)
                    {
                        door.SetLockType(CommonDoor.tLock.Anybody);

                        if (log != null)
                        {
                            log("  Locked Door Fixed: " + door.LotCurrent.Name);
                        }
                    }
                }
            }
        }

        public static int CleanupRelationship(SimDescription a, Logger log)
        {
            int count = 0;

            Dictionary<SimDescription, Relationship> relations;
            if (Relationship.sAllRelationships.TryGetValue(a, out relations))
            {
                Dictionary<ulong, bool> existing = new Dictionary<ulong, bool>();

                List<SimDescription> remove = new List<SimDescription>();

                foreach (KeyValuePair<SimDescription, Relationship> relation in relations)
                {
                    if (existing.ContainsKey(relation.Key.SimDescriptionId))
                    {
                        remove.Add(relation.Key);
                    }
                    else
                    {
                        existing.Add(relation.Key.SimDescriptionId, true);
                    }
                }

                foreach (SimDescription b in remove)
                {
                    relations.Remove(b);

                    count++;

                    if (log != null)
                    {
                        log("Duplicate Relationship Removed: " + a.FullName + " - " + b.FullName);
                    }
                }
            }

            return count;
        }

        public static void RemoveInvalidCoworkers(Occupation ths)
        {
            if (ths.Coworkers != null)
            {
                // Custom
                for (int i = ths.Coworkers.Count - 1; i >= 0; i--)
                {
                    if ((ths.Coworkers[i] == null) || (ths.Coworkers[i] == ths.OwnerDescription))
                    {
                        ths.Coworkers.RemoveAt(i);
                    }
                }

                List<SimDescription> list = new List<SimDescription>(ths.Coworkers);
                foreach (SimDescription description in list)
                {
                    if ((!description.IsValidDescription || ((description.DeathStyle != SimDescription.DeathType.None) && !description.IsPlayableGhost)) || !description.IsHuman)
                    {
                        ths.RemoveCoworker(description);
                    }
                }
            }
        }

        public static bool CleanupOpportunities(SimDescription sim, bool clean, Logger log)
        {
            if (sim.OpportunityHistory == null) return false;

            if (sim.OpportunityHistory.mCurrentOpportunities == null) return false;

            OpportunityManager manager = null;
            if (sim.CreatedSim != null)
            {
                manager = sim.CreatedSim.OpportunityManager;
            }

            for (int i=0; i<sim.OpportunityHistory.mCurrentOpportunities.Length; i++)
            {
                OpportunityHistory.OpportunityExportInfo info = sim.OpportunityHistory.mCurrentOpportunities[i];
                if (info == null)
                {
                    if (manager != null)
                    {
                        info = new OpportunityHistory.OpportunityExportInfo();

                        sim.OpportunityHistory.mCurrentOpportunities[i] = info;

                        if (log != null)
                        {
                            log(" OpportunityExportInfo Created " + sim.FullName);
                        }
                    }
                }
                else if (clean)
                {
                    if (manager == null)
                    {
                        sim.OpportunityHistory.mCurrentOpportunities[i] = null;

                        if (log != null)
                        {
                            log(" OpportunityExportInfo Removed " + sim.FullName);
                        }
                    }
                }

                if (info == null) continue;

                if (info.ListenerStates == null)
                {
                    info.ListenerStates = new EventListenerExportInfo[3];

                    if (log != null)
                    {
                        log(" ListenerState Array Initialized " + sim.FullName);
                    }
                }

                for (int index = 0; index < 3; index++)
                {
                    if (info.ListenerStates[index] == null)
                    {
                        info.ListenerStates[index] = new EventListenerExportInfo();

                        if (log != null)
                        {
                            log(" ListenerState Element Initialized " + sim.FullName);
                        }
                    }
                }
            }

            return true;
        }

        public static void RemoveFreeStuffAlarm(SimDescription sim)
        {
            // Workaround for error in CelebrityManager:RemoveFreeStuffAlarm
            if ((sim.CelebrityManager != null) && (sim.CelebrityManager.mFreeStuffAlarmHandle != AlarmHandle.kInvalidHandle))
            {
                AlarmManager.Global.RemoveAlarm(sim.CelebrityManager.mFreeStuffAlarmHandle);
                sim.CelebrityManager.mFreeStuffAlarmHandle = AlarmHandle.kInvalidHandle;
            }
        }

        public static void CleanupBrokenSkills(SimDescription sim, Logger log)
        {
            try
            {
                if (sim.SkillManager == null) return;

                List<ulong> remove = new List<ulong>();
                foreach (KeyValuePair<ulong, Skill> value in sim.SkillManager.mValues)
                {
                    Skill skill = value.Value;

                    if (skill == null)
                    {
                        remove.Add(value.Key);
                    }
                    else
                    {
                        Skill staticSkill = SkillManager.GetStaticSkill(skill.Guid);
                        if (staticSkill != null)
                        {
                            if (skill.NonPersistableData == null)
                            {
                                skill.mNonPersistableData = staticSkill.mNonPersistableData;

                                if (skill.NonPersistableData != null)
                                {
                                    if (log != null)
                                    {
                                        log(" Broken Skill " + skill.Guid + " Repaired " + sim.FullName);
                                    }
                                }
                                else
                                {
                                    remove.Add(value.Key);
                                }
                            }

                            if (skill.SkillLevel > staticSkill.MaxSkillLevel)
                            {
                                skill.SkillLevel = staticSkill.MaxSkillLevel;

                                if (log != null)
                                {
                                    log(" Skill Level Reduced To Max " + skill.Guid + ": " + sim.FullName);
                                }
                            }
                        }
                        else
                        {
                            remove.Add(value.Key);
                        }
                    }
                }

                foreach (ulong guid in remove)
                {
                    sim.SkillManager.mValues.Remove(guid);

                    if (log != null)
                    {
                        log("Broken Skill " + guid + " Dropped " + sim.FullName);
                    }
                }

                NectarSkill nectarSkill = sim.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                if (nectarSkill != null)
                {
                    if (nectarSkill.mSimIDsServed == null)
                    {
                        nectarSkill.mSimIDsServed = new List<ulong>();
                    }

                    if (nectarSkill.mHashesMade == null)
                    {
                        nectarSkill.mHashesMade = new List<uint>();
                    }
                }

                RockBand bandSkill = sim.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
                if (bandSkill != null)
                {
                    if (bandSkill.mBandGigsStats != null)
                    {
                        bandSkill.mBandGigsStats.Remove(null);
                    }
                }

                BroomRidingSkill broomRidingSkill = sim.SkillManager.GetSkill<BroomRidingSkill>(SkillNames.BroomRiding);
                if (broomRidingSkill != null)
                {
                    if (broomRidingSkill.mLotsVisited == null)
                    {
                        broomRidingSkill.mLotsVisited = new List<Lot>();

                        if (log != null)
                        {
                            log(" Missing LotsVisited Added: " + sim.FullName);
                        }
                    }

                    for (int i = broomRidingSkill.mLotsVisited.Count - 1; i >= 0; i--)
                    {
                        if (broomRidingSkill.mLotsVisited[i] == null)
                        {
                            broomRidingSkill.mLotsVisited.RemoveAt(i);

                            if (log != null)
                            {
                                log(" Invalid LotsVisited Removed: " + sim.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        public static void CorrectSaveGameLocks()
        {
            if (Sims3.Gameplay.Gameflow.Singleton == null) return;

            Sims3.Gameplay.Gameflow gameFlow = Sims3.Gameplay.Gameflow.Singleton;

            int num = 0x0;
            while ((gameFlow.mSaveGameLocks != null) && (num < gameFlow.mSaveGameLocks.Count))
            {
                InteractionInstance disablingInteraction = gameFlow.mSaveGameLocks[num];
                if (disablingInteraction == null)
                {
                    gameFlow.mSaveGameLocks.RemoveAt(num);
                }
                else
                {
                    Sim instanceActor = disablingInteraction.InstanceActor;
                    if ((instanceActor == null) || (instanceActor.InteractionQueue == null))
                    {
                        Sims3.Gameplay.Gameflow.Singleton.EnableSave(disablingInteraction);
                    }
                    else
                    {
                        num++;
                    }
                }
            }
        }
    }
}

