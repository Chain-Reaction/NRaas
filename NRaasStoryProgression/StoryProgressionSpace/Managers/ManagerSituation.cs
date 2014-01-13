using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerSituation : Manager
    {
        public enum MeetUpType : int
        {
            Residential,
            Commercial,
            Either
        }
       
        protected Dictionary<SimDescription, bool> mJobWorkPush = new Dictionary<SimDescription, bool>();
        protected Dictionary<SimDescription, bool> mSchoolWorkPush = new Dictionary<SimDescription, bool>();

        protected static bool sInitial = true;

        public ManagerSituation(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Situations";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerSituation>(this).Perform(initial);
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            if (sInitial)
            {
                sInitial = false;
            }

            CleanupSituations(true, true);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public List<SimDescription> GetFree(IScoringGenerator stats, List<SimDescription> sims, bool enteringSituation)
        {
            List<SimDescription> list = new List<SimDescription>();

            foreach (SimDescription sim in sims)
            {
                if (IsBusy(stats, sim, enteringSituation)) continue;

                list.Add(sim);
            }

            stats.AddStat("Free Sims", list.Count);

            return list;
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (fullUpdate)
            {
                mJobWorkPush.Clear();
                mSchoolWorkPush.Clear();

                CleanupSituations(false, initialPass);
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public bool AddSchoolSlacker(SimDescription sim)
        {
            if (!mSchoolWorkPush.ContainsKey(sim))
            {
                mSchoolWorkPush.Add(sim, false);
                return false;
            }
            else if (!mSchoolWorkPush[sim])
            {
                mSchoolWorkPush[sim] = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddWorkSlacker(SimDescription sim)
        {
            if (!mJobWorkPush.ContainsKey(sim))
            {
                mJobWorkPush.Add(sim, false);
                return false;
            }
            else if (!mJobWorkPush[sim])
            {
                mJobWorkPush[sim] = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);
        }

        public bool PushToObject<T>(IScoringGenerator stats, SimDescription sim)
            where T : GameObject
        {
            List<T> objs = new List<T>();

            foreach (T obj in Sims3.Gameplay.Queries.GetObjects<T>())
            {
                if (!obj.InWorld) continue;

                if (obj.LotCurrent == null) continue;

                if (obj.LotCurrent.IsWorldLot) continue;

                objs.Add(obj);
            }

            if (objs.Count == 0)
            {
                stats.IncStat("No Choices");
                return false;
            }

            return PushVisit(stats, sim, RandomUtil.GetRandomObjectFromList(objs).LotCurrent);
        }

        public bool PushGoHere(IScoringGenerator stats, SimDescription sim, Vector3 position)
        {
            if (sim.CreatedSim == null) return false;

            if (position == Vector3.Empty)
            {
                stats.IncStat("Go Here Push: No Destination");
                return false;
            }

            if (IsRidingHorse(sim))
            {
                MountedTerrainInteraction interaction = Terrain.RideHere.SkipGoHereTestsSingleton.CreateInstance(Terrain.Singleton, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as MountedTerrainInteraction;
                if (interaction == null)
                {
                    stats.IncStat("Go Here Push: No Interaction");
                    return false;
                }

                interaction.SetDestination (position);

                return PushInteraction(stats, sim, AllowCheck.None, interaction);
            }
            else
            {
                InteractionDefinition definition = Terrain.GoHere.GetSingleton(sim.CreatedSim, position);
                if (definition == null)
                {
                    stats.IncStat("Go Here Push: Definition Fail");
                    return false;
                }

                TerrainInteraction interaction = definition.CreateInstance(Terrain.Singleton, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as TerrainInteraction;
                if (interaction == null)
                {
                    stats.IncStat("Go Here Push: No Interaction");
                    return false;
                }

                interaction.Destination = position;

                return PushInteraction(stats, sim, AllowCheck.None, interaction);
            }
        }

        public bool PushGoHome(Common.IStatGenerator stats, SimDescription sim)
        {
            if (sim.CreatedSim == null) return false;

            if (Sim.MakeSimGoHome(sim.CreatedSim, false, new InteractionPriority(InteractionPriorityLevel.Autonomous)))
            {
                if (SimTypes.IsSelectable(sim))
                {
                    stats.IncStat("Go Home Push: Active");
                }
                else
                {
                    stats.IncStat("Go Home Push: Inactive");
                }
                return true;
            }

            stats.IncStat("Go Home Push: Fail");
            return false;
        }

        public bool PushMassVisit(IScoringGenerator stats, SimDescription host, List<Sim> followers, Lot lot)
        {
            if (host == null)
            {
                stats.IncStat("No Host");
                return false;
            }

            return PushInteraction(stats, host, lot, new MassVisitLot.Definition(followers));
        }

        protected void CleanupSituations(bool startup, bool initialPass)
        {
            if (startup)
            {
                if (Firefighter.sFirefighterDictionary != null)
                {
                    foreach (Firefighter.FirefighterInformation info in Firefighter.sFirefighterDictionary.Values)
                    {
                        List<Sim> sims = new List<Sim>(info.FirefightersActiveOnLot);
                        info.FirefightersActiveOnLot.Clear();

                        foreach (Sim sim in sims)
                        {
                            if (sim == null) continue;

                            if (sim.Autonomy == null) continue;

                            if (sim.Autonomy.SituationComponent == null) continue;

                            if (sim.Autonomy.SituationComponent.Situations == null) continue;

                            bool found = false;
                            foreach (Situation situation in sim.Autonomy.SituationComponent.Situations)
                            {
                                if (situation is Sims3.Gameplay.Services.FirefighterSituation)
                                {
                                    found = true;
                                    break;
                                }
                                else if (situation is NRaas.StoryProgressionSpace.Situations.FirefighterSituation)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found) continue;

                            info.FirefightersActiveOnLot.Add(sim);
                        }
                    }
                }
            }

            Dictionary<Situation, bool> allSituations = new Dictionary<Situation, bool>();

            {
                List<Situation> allSituationList = new List<Situation>(Situation.sAllSituations);
                foreach (Situation sit in allSituationList)
                {
                    Sims3.Gameplay.Moving.MovingSituation moving = sit as Sims3.Gameplay.Moving.MovingSituation;
                    if (moving != null)
                    {
                        if (moving.Lot == LotManager.ActiveLot)
                        {
                            try
                            {
                                moving.Exit();
                            }
                            catch (Exception e)
                            {
                                Common.DebugException("Moving", e);
                            }
                        }
                    }
                    else if ((!startup) && (initialPass))
                    {
                        AgeUpNpcSituation ageUp = sit as AgeUpNpcSituation;
                        if (ageUp != null)
                        {
                            if (ageUp.SimDescription == null)
                            {
                                try
                                {
                                    ageUp.Exit();
                                }
                                catch (Exception e)
                                {
                                    Common.DebugException(ageUp.Actor, e);
                                }
                            }
                            else if (SimTypes.IsSelectable(ageUp.SimDescription))
                            {
                                try
                                {
                                    ageUp.Exit();
                                }
                                catch (Exception e)
                                {
                                    Common.DebugException(ageUp.Actor, e);
                                }
                            }
                        }
                        else
                        {
                            Sims3.Gameplay.Services.FirefighterSituation EAfireSituation = sit as Sims3.Gameplay.Services.FirefighterSituation;
                            if (EAfireSituation != null)
                            {
                                try
                                {
                                    EAfireSituation.Exit();
                                }
                                catch (Exception e)
                                {
                                    Common.DebugException("Fire Situation", e);
                                }
                            }

                            NRaas.StoryProgressionSpace.Situations.FirefighterSituation customFireSituation = sit as NRaas.StoryProgressionSpace.Situations.FirefighterSituation;
                            if (customFireSituation != null)
                            {
                                try
                                {
                                    customFireSituation.Exit();
                                }
                                catch (Exception e)
                                {
                                    Common.DebugException("Fire Situation", e);
                                }
                            }
                        }
                    }
                }
            }

            foreach (Lot lot in LotManager.AllLots)
            {
                if (lot.mOnPortalPathPlanHandler != null)
                {
                    foreach (PortalPathPlanHandler handler in lot.mOnPortalPathPlanHandler.GetInvocationList())
                    {
                        Situation target = handler.Target as Situation;
                        if (target == null) continue;

                        allSituations[target] = true;
                    }
                }
            }

            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                if (sim.Autonomy == null) continue;

                if (sim.Autonomy.SituationComponent == null) continue;

                foreach (Situation situation in sim.Autonomy.SituationComponent.Situations)
                {
                    allSituations[situation] = true;
                }
            }

            List<Situation> situations = new List<Situation>(Situation.sAllSituations);
            foreach (Situation situation in situations)
            {
                allSituations[situation] = true;
            }

            foreach (Situation situation in allSituations.Keys)
            {
                bool delete = false;

                SimDescription restartAgeup = null;

                // These situations can get stuck, dispose of them if the game was saved with one running
                AgeUpNpcSituation ageup = situation as AgeUpNpcSituation;
                if ((startup) && (ageup != null))
                {
                    if (ageup.SimDescription == null)
                    {
                        delete = true;
                    }
                    else if (!SimTypes.IsSelectable(ageup.SimDescription))
                    {
                        delete = true;

                        restartAgeup = ageup.SimDescription;
                    }
                }

                PrivacySituation privacy = situation as PrivacySituation;
                if (privacy != null)
                {
                    if (privacy.SyncTarget == null)
                    {
                        delete = true;
                    }
                }

                GenericPrivacySituation generic = situation as GenericPrivacySituation;
                if (generic != null)
                {
                    if (generic.Participants != null)
                    {
                        int index = 0;
                        while (index < generic.Participants.Count)
                        {
                            if (generic.Participants[index] == null)
                            {
                                generic.Participants.RemoveAt(index);
                            }
                            else
                            {
                                index++;
                            }
                        }
                    }
                }

                if (delete)
                {
                    try
                    {
                        situation.Exit();

                        IncStat("Situation Disposed");

                        if (restartAgeup != null)
                        {
                            IncStat("AgeUpSituation Restarted");

                            Sims.Reset(restartAgeup);

                            AgeUp.Execute(restartAgeup, false, false, null);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(restartAgeup, e);
                    }
                }
            }
        }

        protected bool ReadyingForWork(SimDescription sim)
        {
            foreach (ICareerAlarmSimData data in GetData(sim).GetList<ICareerAlarmSimData>())
            {
                if (data.Valid) return true;
            }
            return false;
        }

        public static bool HasBlockingSituation(Sim sim, Situation toIgnore)
        {
            foreach (Situation situation in sim.Autonomy.SituationComponent.Situations)
            {
                if (situation == toIgnore) continue;

                Situation root = situation.Root;

                GroupingSituation group = root as GroupingSituation;
                if (group != null)
                {
                    if (SimTypes.IsSelectable(group.Leader)) return true;

                    if (group.mSimsInDate != null)
                    {
                        foreach (SimDescription member in group.mSimsInDate)
                        {
                            if (SimTypes.IsSelectable(member)) return true;
                        }
                    }
                }
                else
                {
                    NpcParty party = root as NpcParty;
                    if (party != null)
                    {
                        if (party.Child is NpcParty.WaitForSelectableGuestToArrive) continue;
                    }
                    else
                    {
                        GoHereWithSituation goHereWith = root as GoHereWithSituation;
                        if (goHereWith != null)
                        {
                            if (!SimTypes.IsSelectable(goHereWith.Leader)) continue;
                        }
                        else
                        {
                            if (root is VisitSituation) continue;

                            if (root is SocialGroupInteractionSituation) continue;

                            if (root is PlayTagSituation) continue;

                            if (root is BorrowHorseSituation) continue;

                            if (root is PicnicBasketSituation) continue;

                            if (root is PrivacySituation) continue;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public delegate bool OnIsUnderCurfewFunc(Common.IStatGenerator stats, SimDescription sim, out bool forceCurfew);

        public static event OnIsUnderCurfewFunc OnIsUnderCurfew;

        protected bool IsUnderCurfew(Common.IStatGenerator stats, SimDescription sim, out bool forceCurfew)
        {
            if (OnIsUnderCurfew != null)
            {
                return OnIsUnderCurfew(stats, sim, out forceCurfew);
            }
            else
            {
                if (sim.ChildOrBelow)
                {
                    if (SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, Sim.ChildStartCurfewHour, Sim.ChildEndCurfewHour))
                    {
                        forceCurfew = true;

                        stats.IncStat("Child EA Curfew");
                        return true;
                    }
                }
                else if (sim.Teen)
                {
                    if (Sim.TeenCurfewIsInEffect)
                    {
                        forceCurfew = true;

                        stats.IncStat("Teen EA Curfew");
                        return true;
                    }
                }
            }

            forceCurfew = false;
            return false;
        }

        public static bool HasInteraction(SimDescription sim, InteractionDefinition definition)
        {
            if (sim == null) return false;

            if (sim.CreatedSim == null) return false;

            if (sim.CreatedSim.InteractionQueue == null) return false;

            return sim.CreatedSim.InteractionQueue.HasInteractionOfType(definition);
        }

        public bool IsBusy(IScoringGenerator stats, SimDescription sim, bool enteringSituation)
        {
            if (sim == null) return true;

            return IsBusy(stats, sim.CreatedSim, enteringSituation, null);
        }
        public bool IsBusy(IScoringGenerator stats, Sim sim, bool enteringSituation)
        {
            return IsBusy(stats, sim, enteringSituation, null);
        }
        public bool IsBusy(IScoringGenerator stats, Sim sim, bool enteringSituation, Situation toIgnore)
        {
            if (sim == null) return false;

            SimDescription simDesc = sim.SimDescription;
            if (simDesc == null) return true;

            IncStat("Busy: Try");

            string reason = null;

            if (!Allow(stats, sim, enteringSituation ? AllowCheck.Active : AllowCheck.None))
            {
                IncStat("Busy: User Denied");
                stats.IncStat("Busy: User Denied");
                return true;
            }
            else if (PromSituation.IsHeadedToProm(sim))
            {
                IncStat("Busy: Prom");
                stats.IncStat("Busy: Prom");
                return true;
            }
            else if (simDesc.IsEnrolledInBoardingSchool())
            {
                IncStat("Busy: Boarding School");
                stats.IncStat("Busy: Boarding School");
                return true;
            }
            else if ((ParentsLeavingTownSituation.Adults != null) && (ParentsLeavingTownSituation.Adults.Contains(simDesc.SimDescriptionId)))
            {
                IncStat("Busy: Free Vacation");
                stats.IncStat("Busy: Free Vacation");
                return true;
            }
            else if ((sim.Autonomy.AutonomyDisabled) && (!(sim.Parent is Vehicle)))
            {
                IncStat("Busy: Autonomy Disabled");
                stats.IncStat("Busy: Autonomy Disabled");
                return true;
            }
            else if (!ManagerSim.ValidSim(simDesc, out reason))
            {
                IncStat("Busy: " + reason);
                stats.IncStat("Busy: " + reason);
                return true;
            }
            else if ((simDesc.IsPregnant) && (simDesc.Pregnancy.mHourOfPregnancy > 48))
            {
                IncStat("Busy: Pregnant");
                stats.IncStat("Busy: Pregnant");
                return true;
            }

            if (enteringSituation)
            {
                if (ReadyingForWork(simDesc))
                {
                    IncStat("Busy: Readying For Work");
                    stats.IncStat("Busy: Readying For Work");
                    return true;
                }
            }

            if (HasBlockingSituation(sim, toIgnore))
            {
                if ((sim.Autonomy != null) &&
                    (sim.Autonomy.SituationComponent != null) &&
                    (sim.Autonomy.SituationComponent.Situations != null))
                {
                    foreach (Situation situation in sim.Autonomy.SituationComponent.Situations)
                    {
                        IncStat("Busy: Situation " + situation.ToString());
                        stats.IncStat("Busy: Situation " + situation.ToString());
                    }
                }

                IncStat("Busy: In Situation");
                stats.IncStat("Busy: In Situation");
                return true;
            }
            else if (simDesc.ToddlerOrBelow)
            {
                IncStat("Busy: Too Young");
                stats.IncStat("Busy: Too Young");
                return true;
            }
            else if (sim.InteractionQueue == null)
            {
                IncStat("Busy: No Queue");
                stats.IncStat("Busy: No Queue");
                return true;
            }

            bool forceCurfew;
            if ((enteringSituation) && (IsUnderCurfew(stats, simDesc, out forceCurfew)))
            {
                if (simDesc.Teen)
                {
                    if (stats.AddScoring("IgnoreScoring", AddScoring("IgnoreCurfew", simDesc)) <= 0)
                    {
                        IncStat("Busy: Teen Curfew");
                        stats.IncStat("Busy: Teen Curfew");
                        return true;
                    }
                }
                else
                {
                    IncStat("Busy: Child Curfew");
                    stats.IncStat("Busy: Child Curfew");
                    return true;
                }
            }

            InteractionInstance interaction = sim.CurrentInteraction;
            if (interaction != null)
            {
                if (interaction is ICountsAsWorking)
                {
                    IncStat("Busy: At Work");
                    stats.IncStat("Busy: At Work");
                    return true;
                }
                else if (interaction is ISleeping)
                {
                    IncStat("Busy: Asleep");
                    stats.IncStat("Busy: Asleep");
                    return true;
                }
                else if (interaction is PromSituation.GoToProm)
                {
                    IncStat("Busy: At Prom");
                    stats.IncStat("Busy: At Prom");
                    return true;
                }
                else if (interaction.mPriority.Level > InteractionPriorityLevel.NonCriticalNPCBehavior)
                {
                    IncStat("Busy: Priority Level: " + interaction.mPriority.Level);
                    stats.IncStat("Busy: Priority Level: " + interaction.mPriority.Level);
                    return true;
                }
            }

            IncStat("Busy: Not");
            stats.IncStat("Busy: Not");
            return false;
        }

        public static Lot FindLotType(CommercialLotSubType type)
        {
            List<Lot> lots = new List<Lot>();

            foreach (Lot lot in LotManager.AllLots)
            {
                if (!lot.IsCommunityLot) continue;

                if (lot.CommercialLotSubType == type)
                {
                    lots.Add(lot);
                }
            }

            if (lots.Count == 0) return null;

            return RandomUtil.GetRandomObjectFromList(lots);
        }

        public static RabbitHole FindRabbitHole(RabbitHoleType type)
        {
            List<RabbitHole> holes = new List<RabbitHole>();
            foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
            {
                if (hole.Guid != type) continue;

                holes.Add(hole);
            }

            if (holes.Count == 0) return null;

            return RandomUtil.GetRandomObjectFromList(holes);
        }

        public bool PushToRabbitHole(IScoringGenerator stats, SimDescription sim, RabbitHoleType type, bool allowFriend, bool allowFlirt)
        {
            return PushToRabbitHole(stats, sim, FindRabbitHole(type), allowFriend, allowFlirt);
        }
        public bool PushToRabbitHole(IScoringGenerator stats, SimDescription sim, RabbitHole rabbitHole, bool allowFriend, bool allowFlirt)
        {
            if (rabbitHole == null)
            {
                stats.IncStat("PushToRabbitHole: No Hole");
                return false;
            }

            if (!Allow(stats, sim))
            {
                stats.IncStat("PushToRabbitHole: User Denied");
                return true;
            }

            if (allowFlirt)
            {
                List<SimDescription> flirts = Flirts.FindExistingFor(this, sim, false);

                SimDescription flirt = null;
                if ((flirts != null) && (flirts.Count > 0))
                {
                    flirt = RandomUtil.GetRandomObjectFromList(flirts);
                }

                if ((flirt != null) && (Sims.Instantiate(flirt, null, false)))
                {
                    stats.IncStat("Try RabbitHoleWithDate");

                    RabbitHole.EnterRabbitHoleWithDate interaction = new RabbitHole.EnterRabbitHoleWithDate.Definition().CreateInstance(rabbitHole, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true) as RabbitHole.EnterRabbitHoleWithDate;

                    interaction.OtherSim = flirt.CreatedSim;

                    if (PushInteraction(stats, sim, AllowCheck.Active, interaction))
                    {
                        stats.IncStat("Success RabbitHoleWithDate");
                        return true;
                    }
                }
            }

            if (allowFriend)
            {
                List<SimDescription> friends = Friends.FindExistingFriendFor(this, sim);

                SimDescription friend = null;
                if ((friends != null) && (friends.Count > 0))
                {
                    friend = RandomUtil.GetRandomObjectFromList(friends);
                }
                
                if ((friend != null) && (Sims.Instantiate(friend, null, false)))
                {
                    stats.IncStat("Try VisitRabbitHoleWith");

                    RabbitHole.VisitRabbitHoleWith.Definition definition = new RabbitHole.VisitRabbitHoleWith.Definition();
                    definition.VisitTuniing = CityHall.kVisitRabbitHoleTuning;

                    RabbitHole.VisitRabbitHoleWith interaction = definition.CreateInstance(rabbitHole, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true) as RabbitHole.VisitRabbitHoleWith;

                    interaction.SelectedObjects = new List<object>();
                    interaction.SelectedObjects.Add(friend.CreatedSim);

                    if (PushInteraction(stats, sim, AllowCheck.Active, interaction))
                    {
                        stats.IncStat("Success VisitRabbitHoleWith");
                        return true;
                    }
                }
            }

            {
                stats.IncStat("Try VisitRabbitHole");

                RabbitHole.VisitRabbitHole.Definition definition = new RabbitHole.VisitRabbitHole.Definition();
                definition.VisitTuning = CityHall.kVisitRabbitHoleTuning;

                if (PushInteraction(stats, sim, rabbitHole, definition))
                {
                    stats.IncStat("Success VisitRabbitHole");
                    return true;
                }
                else
                {
                    stats.IncStat("PushToRabbitHole: Fail");
                    return true; // intentional
                }
            }
        }

        public bool GreetSimOnLot(SimDescription sim, Lot lot)
        {
            if (lot == null) return false;

            if (lot.IsPlayerHomeLot) return false;

            if (lot.IsCommunityLot) return false;

            if (!Sims.Instantiate(sim, lot, false)) return false;

            if (sim.CreatedSim == null) return false;

            if (sim.CreatedSim.LotCurrent == null) return false;

            sim.CreatedSim.GreetSimOnLot(lot);
            return true;
        }

        public bool PushInteraction<TTarget>(IScoringGenerator stats, SimDescription sim, TTarget target, InteractionDefinition definition)
            where TTarget : IGameObject
        {
            return PushInteraction<TTarget>(stats, sim, target, new StandardPush(definition));
        }
        public bool PushInteraction<TTarget>(IScoringGenerator stats, SimDescription sim, TTarget target, InteractionPush definition)
            where TTarget : IGameObject
        {
            return PushInteraction<TTarget>(stats, sim, target, AllowCheck.Active, definition);
        }
        public bool PushInteraction<TTarget>(IScoringGenerator stats, SimDescription sim, TTarget target, AllowCheck check, InteractionDefinition definition)
            where TTarget : IGameObject
        {
            return PushInteraction<TTarget>(stats, sim, target, check, new StandardPush(definition));
        }
        public bool PushInteraction<TTarget>(IScoringGenerator stats, SimDescription sim, TTarget target, AllowCheck check, InteractionPush definition)
            where TTarget : IGameObject
        {
            GreetSimOnLot(sim, target.LotCurrent);

            if (sim.CreatedSim == null)
            {
                stats.IncStat("Push Hibernating");
                return false;
            }

            InteractionInstance interaction = null;
            
            try
            {
                interaction = definition.CreateInstance(sim, target);
            }
            catch (Exception e)
            {
                Common.DebugException(sim.CreatedSim, target, e);

                stats.IncStat("Push Exception");
                return false;
            }

            return PushInteraction(stats, sim, check, interaction);
        }
        public bool PushInteraction<TTarget>(IScoringGenerator stats, SimDescription sim, TTarget target, InteractionPush definition, AllowCheck check, out InteractionInstance instance)
            where TTarget : IGameObject
        {
            instance = null;

            if (sim.CreatedSim == null)
            {
                stats.IncStat("Push Hibernating");
                return false;
            }

            try
            {
                instance = definition.CreateInstance(sim, target);
            }
            catch (Exception e)
            {
                Common.Exception(sim.CreatedSim, target, definition.GetType().ToString(), e);

                stats.IncStat("Push Exception");
                return false;
            }

            return PushInteraction(stats, sim, check, instance);
        }
        public bool PushInteraction(IScoringGenerator stats, SimDescription sim, AllowCheck check, InteractionInstance instance)
        {
            if (sim == null)
            {
                stats.IncStat("Push No Sim");
                return false;
            }

            if (instance == null)
            {
                stats.IncStat("Push No Interaction");
                return false;
            }

            if (!Allow(stats, sim, check))
            {
                stats.IncStat("Push User Denied");
                return true;
            }

            if (!Sims.Instantiate(sim, null, false))
            {
                stats.IncStat("Push Instantiate Fail");
                return false;
            }

            if (sim.CreatedSim == null)
            {
                stats.IncStat("Push Hibernating");
                return false;
            }

            if (sim.CreatedSim.LotCurrent == null)
            {
                stats.IncStat("Push No Current Lot");
                return false;
            }

            if (sim.CreatedSim.InteractionQueue == null)
            {
                stats.IncStat("Push No Queue");
                return false;
            }

            if (sim.CreatedSim.InteractionQueue.Count >= 8)
            {
                stats.IncStat("Push Queue Full");
                return false;
            }

            InteractionInstanceParameters parameters = instance.GetInteractionParameters();

            GreyedOutTooltipCallback greyedOutCallback = null;
            InteractionTestResult result = instance.InteractionDefinition.Test(ref parameters, ref greyedOutCallback);
            if (!IUtil.IsPass(result))
            {
                IncStat("Push " + result + " (" + instance.InteractionDefinition.GetType() + ")");
                stats.IncStat("Push " + result + " (" + instance.InteractionDefinition.GetType() + ")");

                if (greyedOutCallback != null)
                {
                    IncStat("Push " + greyedOutCallback() + " (" + instance.InteractionDefinition.GetType() + ")");
                    stats.IncStat("Push " + greyedOutCallback() + " (" + instance.InteractionDefinition.GetType() + ")");
                }
            }

            List<Situation> situations = new List<Situation>();

            if (sim.CreatedSim.Autonomy == null)
            {
                stats.IncStat("Autonomy Manager Fail");
                return false;
            }
            else if (sim.CreatedSim.Autonomy.SituationComponent == null)
            {
                stats.IncStat("Situation Manager Fail");
                return false;
            }

            situations.AddRange(sim.CreatedSim.Autonomy.SituationComponent.Situations);

            foreach (Situation situation in situations)
            {
                if (situation is GroupingSituation)
                {
                    GroupingSituation grouping = situation as GroupingSituation;
                    grouping.LeaveGroup(sim.CreatedSim);

                    stats.IncStat("Grouping Situation Dropped");
                }
            }

            try
            {
                string name = instance.InteractionDefinition.GetType().ToString();

                if (!sim.CreatedSim.InteractionQueue.Add(instance))
                {
                    IncStat("Push Add Fail (" + instance.InteractionDefinition.GetType() + ")");
                    stats.IncStat("Push Add Fail (" + instance.InteractionDefinition.GetType() + ")");
                    return false;
                }
                else
                {
                    //Common.DebugNotify(delegate { return "Push\n" + sim.FullName + "\n" + instance.InteractionDefinition.GetType() + "\n" + stats.ToString(); }, sim.CreatedSim);

                    if (SimTypes.IsSelectable(sim))
                    {
                        IncStat("PushInteraction Active Success (" + instance.InteractionDefinition.GetType() + ")");
                        stats.IncStat("PushInteraction Active Success (" + instance.InteractionDefinition.GetType() + ")");
                    }
                    else
                    {
                        IncStat("PushInteraction Success (" + instance.InteractionDefinition.GetType() + ")");
                        stats.IncStat("PushInteraction Success (" + instance.InteractionDefinition.GetType() + ")");
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Common.DebugException (sim, e);

                stats.IncStat("Push Add Exception");
                return false;
            }
        }

        public static bool IsRidingHorse(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.CreatedSim == null) return false;

            if (sim.CreatedSim.Posture == null) return false;

            Sim horse = sim.CreatedSim.Posture.Container as Sim;
            if (horse == null) return false;

            return horse.IsHorse;
        }

        public bool PushVisit(IScoringGenerator stats, SimDescription sim, Lot lot)
        {
            if (lot == null) return false;

            if (sim.CreatedSim != null)
            {
                if (lot == sim.CreatedSim.LotCurrent)
                {
                    stats.IncStat("Visit Already There");
                    return true;
                }
            }

            if (IsBusy(stats, sim, true))
            {
                stats.IncStat("Visit Busy");
                return false;
            }

            if (IsRidingHorse(sim))
            {
                GreetSimOnLot(sim, lot);

                Sim createdSim = Instantiation.PerformOffLot(sim, lot, null);
                if (createdSim == null)
                {
                    stats.IncStat("Visit Instantiation Fail");
                    return false;
                }

                Vector3 position;
                if (!World.GetLotMainEntranceLocation(lot.LotId, out position))
                {
                    position = lot.EntryPoint();

                    if (position == Vector3.Invalid)
                    {
                        stats.IncStat("No Lot Entry Point");
                        return false;
                    }
                }

                return PushGoHere(stats, sim, position);
            }
            else
            {
                return PushInteraction(stats, sim, lot, GetVisitInteraction(sim, lot));
            }
        }

        public static InteractionDefinition GetVisitInteraction(Sim sim, Lot lot)
        {
            if (sim == null) return null;

            return GetVisitInteraction(sim.SimDescription, lot);
        }
        public static InteractionDefinition GetVisitInteraction(SimDescription sim, Lot lot)
        {
            if (lot.IsCommunityLot)
            {
                return VisitCommunityLot.Singleton;
            }
            else
            {
                if ((sim.CreatedSim != null) && (sim.CreatedSim.IsGreetedOnLot(lot)))
                {
                    return GoToLot.Singleton;
                }
                else
                {
                    return VisitLot.Singleton;
                }
            }
        }

        protected bool InstantiateForMeetUp(IScoringGenerator stats, SimDescription a, SimDescription b, ref bool force)
        {
            if ((a == null) || (b == null)) return false;

            if ((!Allow(stats, a)) || (!Allow(stats, b)))
            {
                stats.IncStat("MeetUp: User Denied");
                return false;
            }
            else if ((a.CreatedSim == null) && (b.CreatedSim == null))
            {
                stats.IncStat("MeetUp: Both Homeless");
                return false;
            }
            else if (a.CreatedSim == null)
            {
                if (IsBusy(stats, b, true))
                {
                    stats.IncStat("MeetUp: Busy");
                    return false;
                }

                if (!Sims.Instantiate(a, b.CreatedSim.LotCurrent, false))
                {
                    stats.IncStat("MeetUp: Instantiate Fail");
                    return false;
                }
                force = true;
            }
            else if (b.CreatedSim == null)
            {
                if (IsBusy(stats, a, true))
                {
                    stats.IncStat("MeetUp: Busy");
                    return false;
                }

                if (!Sims.Instantiate(b, a.CreatedSim.LotCurrent, false))
                {
                    stats.IncStat("MeetUp: Instantiate Fail");
                    return false;
                }
                force = true;
            }

            return true;
        }

        public bool Allow(IScoringGenerator stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (sim.Household == null)
            {
                stats.IncStat("Allow: No Home");
                return false;
            }
            else if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
            {
                stats.IncStat("Allow: Reaper");
                return false;
            }

            return AllowPush(stats, sim, settings);
        }
        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (((check & AllowCheck.Active) == AllowCheck.Active) && (SimTypes.IsSelectable(sim)))
            {
                check &= ~AllowCheck.Active;
            }

            return base.PrivateAllow(stats, sim, check);
        }

        public bool AllowPush(Common.IStatGenerator stats, SimDescription sim)
        {
            return AllowPush(stats, sim, Options.GetSim(sim));
        }
        protected bool AllowPush(Common.IStatGenerator stats, SimDescription sim, SimData settings)
        {
            if (SimClock.IsNightTime())
            {
                if (!settings.GetValue<AllowPushAtNightOption, bool>())
                {
                    stats.IncStat("Allow: Push Night Denied");
                    return false;
                }
            }
            else
            {
                if (!settings.GetValue<AllowPushAtDayOption, bool>())
                {
                    stats.IncStat("Allow: Push Day Denied");
                    return false;
                }
            }

            if (SimTypes.IsSelectable(sim))
            {
                Sim createdSim = sim.CreatedSim;
                if ((createdSim != null) && (createdSim.Motives != null))
                {
                    if (createdSim.Motives.InMotiveDistress)
                    {
                        stats.IncStat("Allow: In Distress");
                        return false;
                    }
                }
            }

            return true;
        }

        protected override string IsOnActiveLot(SimDescription sim, bool testViewLot)
        {
            return base.IsOnActiveLot(sim, false);
        }

        public bool PushGathering(IScoringGenerator stats, SimDescription sim, Lot lot)
        {
            stats.IncStat("Try PushGathering");

            if (lot.Household != null)
            {
                if (!Allow(stats, sim)) return false;

                if (sim.LotHome == lot)
                {
                    stats.IncStat("Gathering: Home Lot");
                    return false;
                }

                if (!Sims.Instantiate(sim, lot, false))
                {
                    stats.IncStat("Gathering: Instantiate Fail");
                    return false;
                }

                if (sim.CreatedSim == null) return false;

                VisitSituation.Create(sim.CreatedSim, lot);

                stats.IncStat("Residential Gather");
                return true;
            }
            else
            {
                if (PushInteraction(stats, sim, lot, VisitCommunityLot.Singleton))
                {
                    stats.IncStat("Commercial Gather");
                    return true;
                }
            }

            return false;
        }

        public bool PushMeetUp(IScoringGenerator stats, SimDescription a, SimDescription b, MeetUpType lotType, GoToLotSituation.FirstActionDelegate firstAction)
        {
            return PushMeetUp(stats, a, b, lotType, firstAction, false);
        }
        public bool PushMeetUp(IScoringGenerator stats, SimDescription a, SimDescription b, MeetUpType lotType, GoToLotSituation.FirstActionDelegate firstAction, bool force)
        {
            if (!InstantiateForMeetUp(stats, a, b, ref force)) return false;

            return PushMeetUp(stats, a.CreatedSim, b.CreatedSim, lotType, firstAction, force);
        }

        public bool PushMeetUp(IScoringGenerator stats, SimDescription a, SimDescription b, Lot lot, GoToLotSituation.FirstActionDelegate firstAction)
        {
            return PushMeetUp(stats, a, b, lot, firstAction, false);
        }
        public bool PushMeetUp(IScoringGenerator stats, SimDescription a, SimDescription b, Lot lot, GoToLotSituation.FirstActionDelegate firstAction, bool force)
        {
            if (!InstantiateForMeetUp(stats, a, b, ref force)) return false;

            return PushMeetUp(stats, a.CreatedSim, b.CreatedSim, lot, firstAction, force);
        }

        protected bool PushMeetUp(IScoringGenerator stats, Sim a, Sim b, MeetUpType lotType, GoToLotSituation.FirstActionDelegate firstAction)
        {
            return PushMeetUp(stats, a, b, lotType, firstAction, false);
        }
        protected bool PushMeetUp(IScoringGenerator stats, Sim a, Sim b, MeetUpType lotType, GoToLotSituation.FirstActionDelegate firstAction, bool force)
        {
            if ((a == null) || (b == null)) return false;

            Lot lot = null;
            if ((lotType == MeetUpType.Residential) || ((lotType == MeetUpType.Either) && (RandomUtil.CoinFlip())))
            {
                lot = Lots.GetHomeLot(a, b, true);
            }

            if ((lot == null) || (lotType == MeetUpType.Commercial))
            {
                lot = Lots.GetCommunityLot(a, b);
            }

            if (lot == null)
            {
                stats.IncStat("MeetUp: Lot Failure");
                return false;
            }

            return PushMeetUp(stats, a, b, lot, firstAction, force);
        }

        protected bool PushMeetUp(IScoringGenerator stats, Sim a, Sim b, Lot lot, GoToLotSituation.FirstActionDelegate firstAction)
        {
            return PushMeetUp(stats, a, b, lot, firstAction, false);
        }
        protected bool PushMeetUp(IScoringGenerator stats, Sim a, Sim b, Lot lot, GoToLotSituation.FirstActionDelegate firstAction, bool force)
        {
            if ((a == null) || (b == null)) return false;

            AddTry("MeetUp");

            if ((IsBusy(stats, a, true)) || (IsBusy(stats, b, true)))
            {
                stats.IncStat("MeetUp: Busy");
                return false;
            }
            else if ((a.LotCurrent == b.LotCurrent) && (!force))
            {
                AddSuccess("MeetUp A");
                return true;
            }

            try
            {
                new GoToLotSituation(a, b, lot, firstAction);

                AddSuccess("MeetUp B");
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(a, b, e);

                stats.IncStat("MeetUp: Exception");
                return false;
            }
        }

        public bool HasInteraction(Sim sim, InteractionDefinition interaction, bool mustBeFirst)
        {
            List<InteractionDefinition> interactions = new List<InteractionDefinition>();

            if (interaction != null)
            {
                interactions.Add(interaction);
            }

            return HasInteraction(sim, interactions, mustBeFirst);
        }
        public bool HasInteraction(Sim sim, List<InteractionDefinition> interactions, bool mustBeFirst)
        {
            if (sim == null) return false;

            try
            {
                if (sim.InteractionQueue == null) return false;

                if ((interactions == null) || (interactions.Count == 0))
                {
                    return false;
                }

                foreach (InteractionDefinition interaction in interactions)
                {
                    if (interaction == null)
                    {
                        IncStat("Definition Failure " + sim.Name, Common.DebugLevel.High);
                        continue;
                    }
                    else if (sim.InteractionQueue.InteractionList == null)
                    {
                        IncStat("InteractionList Failure " + sim.Name, Common.DebugLevel.High);
                        continue;
                    }

                    Type type = interaction.GetType();
                    foreach (InteractionInstance instance in sim.InteractionQueue.InteractionList)
                    {
                        if (instance == null)
                        {
                            IncStat("Instance Failure " + sim.Name, Common.DebugLevel.High);
                            CleanUpInteractionQueue(sim);
                            break;
                        }
                        else if (instance.InteractionDefinition == null)
                        {
                            IncStat("Definition Failure " + sim.Name + " : " + instance.GetType().ToString(), Common.DebugLevel.High);
                            CleanUpInteractionQueue(sim);
                            break;
                        }
                        else if (instance.InteractionDefinition.GetType() == type)
                        {
                            return true;
                        }

                        if (mustBeFirst)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(sim, exception);
            }
            return false;
        }

        public void CleanUpInteractionQueue(Sim sim)
        {
            if (sim == null) return;

            if (sim.InteractionQueue == null) return;

            List<InteractionInstance> interactions = sim.InteractionQueue.InteractionList as List<InteractionInstance>;

            int index = 0;
            while (index < interactions.Count)
            {
                InteractionInstance instance = interactions[index];
                if ((instance == null) || (instance.InteractionDefinition == null))
                {
                    interactions.RemoveAt(index);

                    IncStat("Interaction Dropped");
                }
                else
                {
                    index++;
                }
            }
        }

        public class Updates : AlertLevelOption<ManagerSituation>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerSituation>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class SpeedOption : SpeedBaseOption<ManagerSituation>
        {
            public SpeedOption()
                : base(1000, false)
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerSituation>
        {
            public TicksPassedOption()
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerSituation>
        {
            public DumpScoringOption()
            { }
        }

        public interface IGradPromCurfewOption : INotRootLevelOption
        { }

        public class GradPromCurfewOption : NestingManagerOptionItem<ManagerSituation, IGradPromCurfewOption>
        {
            public GradPromCurfewOption()
            { }

            public override string GetTitlePrefix()
            {
                return "GradPromCurfewListing";
            }
        }

        public interface IGatheringOption : INotRootLevelOption
        { }

        public class GatheringOption : NestingManagerOptionItem<ManagerSituation, IGatheringOption>
        {
            public GatheringOption()
            { }

            public override string GetTitlePrefix()
            {
                return "GatheringListing";
            }
        }

        public abstract class InteractionPush
        {
            protected readonly InteractionDefinition mDefinition;

            public InteractionPush(InteractionDefinition definition)
            {
                mDefinition = definition;
            }

            public abstract InteractionInstance CreateInstance(SimDescription sim, IGameObject target);
        }

        public class StandardPush : InteractionPush
        {
            public StandardPush(InteractionDefinition definition)
                : base(definition)
            { }

            public override InteractionInstance CreateInstance(SimDescription sim, IGameObject target)
            {
                return mDefinition.CreateInstance(target, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.Autonomous), false, true);
            }
        }

        public class WithCallbackPush : InteractionPush
        {
            ICallbackScenario mOnStarted;
            ICallbackScenario mOnCompleted;
            ICallbackScenario mOnFailed;

            public WithCallbackPush(StoryProgressionObject manager, InteractionDefinition definition, ICallbackScenario onStarted, ICallbackScenario onCompleted, ICallbackScenario onFailed)
                : base(definition)
            {
                mOnStarted = onStarted;
                if (mOnStarted != null)
                {
                    mOnStarted.Manager = manager;
                }

                mOnCompleted = onCompleted;
                if (mOnCompleted != null)
                {
                    mOnCompleted.Manager = manager;
                }

                mOnFailed = onFailed;
                if (mOnFailed != null)
                {
                    mOnFailed.Manager = manager;
                }
            }

            public override InteractionInstance CreateInstance(SimDescription sim, IGameObject target)
            {
                Callback onStarted = null;
                if (mOnStarted != null)
                {
                    onStarted = mOnStarted.Callback;
                }

                Callback onCompleted = null;
                if (mOnCompleted != null)
                {
                    onCompleted = mOnCompleted.Callback;
                }

                Callback onFailed = null;
                if (mOnFailed != null)
                {
                    onFailed = mOnFailed.Callback;
                }

                return mDefinition.CreateInstanceWithCallbacks(target, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.Autonomous), false, true, onStarted, onCompleted, onFailed);
            }
        }
    }
}

