using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupTimers : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupTimers");

            Dictionary<DateAndTime, Dictionary<MethodInfo, Dictionary<ReferenceWrapper, bool>>> lookup = new Dictionary<DateAndTime, Dictionary<MethodInfo, Dictionary<ReferenceWrapper, bool>>>();

            Dictionary<AlarmHandle, AlarmManager> remove = new Dictionary<AlarmHandle, AlarmManager>();

            Dictionary<AlarmManager, bool> managers = new Dictionary<AlarmManager, bool>();

            managers.Add(AlarmManager.Global, true);

            foreach (Lot lot in LotManager.AllLots)
            {
                if (lot.mSavedData.mAlarmManager == null) continue;

                if (managers.ContainsKey(lot.AlarmManager)) continue;

                managers.Add(lot.AlarmManager, true);
            }

            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(true);

            foreach (AlarmManager manager in managers.Keys)
            {
                foreach (KeyValuePair<AlarmHandle,List<AlarmManager.Timer>> list in manager.mTimers)
                {
                    foreach (AlarmManager.Timer timer in list.Value)
                    {
                        bool removed = false;

                        SimDescription sim = timer.ObjectRef as SimDescription;
                        if (sim != null)
                        {
                            if (!sim.IsValidDescription)
                            {
                                remove[list.Key] = manager;

                                Overwatch.Log(" Invalid Sim " + sim.FullName);
                                removed = true;
                            }
                        }
                        else
                        {
                            GameObject gameObject = timer.ObjectRef as GameObject;
                            if (gameObject != null)
                            {
                                if (gameObject.HasBeenDestroyed)
                                {
                                    remove[list.Key] = manager;

                                    Overwatch.Log(" Destroyed Object " + gameObject.GetType());
                                    removed = true;
                                }
                            }
                        }

                        AlarmTimerCallback callback = timer.CallBack;

                        if (callback == null)
                        {
                            remove[list.Key] = manager;

                            Overwatch.Log(" Removed Empty Alarm");
                            removed = true;
                        }
                        else
                        {
                            Writing.RoyaltyAlarm royaltyAlarm = callback.Target as Writing.RoyaltyAlarm;
                            if (royaltyAlarm != null)
                            {
                                string reason = null;
                                if ((royaltyAlarm.mSkill == null) || (royaltyAlarm.mSkill.SkillOwner == null))
                                {
                                    reason = "No Skill";
                                }
                                else if (!royaltyAlarm.mSkill.SkillOwner.IsValidDescription)
                                {
                                    reason = "Bad Sim";
                                }
                                else if (royaltyAlarm.mSkill.SkillOwner.SkillManager == null)
                                {
                                    reason = "No Manager";
                                }
                                else
                                {
                                    Writing skill = royaltyAlarm.mSkill.SkillOwner.SkillManager.GetSkill<Writing>(SkillNames.Writing);
                                    if (skill != royaltyAlarm.mSkill)
                                    {
                                        reason = "Not Royalty Skill";
                                    }
                                    else if (skill.mRoyaltyAlarm != royaltyAlarm)
                                    {
                                        reason = "Not Royalty Alarm";
                                    }
                                }

                                if (reason != null)
                                {
                                    remove[list.Key] = manager;

                                    Overwatch.Log(" Invalid Royalty Alarm: " + reason);
                                    removed = true;
                                }
                            }
                            else
                            {
                                MethodInfo info = typeof(LunarCycleManager).GetMethod("PossiblySpawnZombie", BindingFlags.Static | BindingFlags.NonPublic);
                                if (callback.Method == info)
                                {
                                    if (LunarCycleManager.mZombieAlarm != timer.Handle)
                                    {
                                        remove[list.Key] = manager;

                                        Overwatch.Log(" Invalid Zombie Alarm");
                                        removed = true;
                                    }
                                }
                                else
                                {
                                    info = typeof(MeteorShower).GetMethod("RandomMeteorShowerCallback", BindingFlags.Static | BindingFlags.Public);
                                    if (callback.Method == info)
                                    {
                                        if (MeteorShower.RandomMeteorShowerAlarmHandler != timer.Handle)
                                        {
                                            remove[list.Key] = manager;

                                            Overwatch.Log(" Invalid Meteor Shower Alarm");
                                            removed = true;
                                        }
                                    }
                                }
                            }

                            if (!removed)
                            {
                                Dictionary<MethodInfo, Dictionary<ReferenceWrapper, bool>> methods;
                                if (!lookup.TryGetValue(timer.AlarmDateAndTime, out methods))
                                {
                                    methods = new Dictionary<MethodInfo, Dictionary<ReferenceWrapper, bool>>();
                                    lookup[timer.AlarmDateAndTime] = methods;
                                }

                                Dictionary<ReferenceWrapper, bool> objects;
                                if (!methods.TryGetValue(callback.Method, out objects))
                                {
                                    objects = new Dictionary<ReferenceWrapper, bool>();
                                    methods[callback.Method] = objects;
                                }

                                ReferenceWrapper reference = new ReferenceWrapper(callback.Target);

                                if (objects.ContainsKey(reference))
                                {
                                    remove[list.Key] = manager;

                                    Overwatch.Log(" Removed Duplicate Alarm: " + timer.AlarmDateAndTime + " " + callback.Method + " (" + callback.Target + ")");
                                }
                                else
                                {
                                    objects[reference] = true;
                                }
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<AlarmHandle, AlarmManager> handle in remove)
            {
                handle.Value.RemoveAlarm(handle.Key);
            }

            // cleanup trick or treating fail
            HolidayManager instance = HolidayManager.Instance;
            if (instance != null && AlarmManager.Global != null)
            {
                if (!instance.IsFallHoliday && TrickOrTreatSituation.NPCTrickOrTreatAlarm != AlarmHandle.kInvalidHandle)
                {
                    Overwatch.Log("Cleaned up run away trick or treat alarm");
                    AlarmManager.Global.RemoveAlarm(TrickOrTreatSituation.NPCTrickOrTreatAlarm);
                    TrickOrTreatSituation.NPCTrickOrTreatAlarm = AlarmHandle.kInvalidHandle;
                }
            }
        }
    }
}
