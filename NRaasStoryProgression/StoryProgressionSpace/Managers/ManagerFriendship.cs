using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerFriendship : Manager
    {
        CelebrityRatioOption[] mCelebrityOptions = new CelebrityRatioOption[5];

        static TraitNames[] sFriendChips = new TraitNames[] { TraitNames.CapacityToLoveChip, TraitNames.FriendlyChip, TraitNames.HumanEmotionChip, TraitNames.HumorChip, TraitNames.SentienceChip, TraitNames.MoodAdjusterChip };
        static TraitNames[] sEnemyChips = new TraitNames[] { TraitNames.EvilChip, TraitNames.HumanEmotionChip, TraitNames.SentienceChip, TraitNames.MoodAdjusterChip, TraitNames.FearOfHumansChip };

        public ManagerFriendship(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Friendships";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerFriendship>(this).Perform(initial);
        }

        public static bool FriendlyFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                StoryProgression.Main.Situations.IncStat("First Action: Friendly");

                if (parent.mSimA.IsHuman)
                {
                    meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Chat", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                }
                else
                {
                    meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Pet Socialize", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        public static bool EnemyFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                StoryProgression.Main.Situations.IncStat("First Action: Enemy");

                string action = null;

                if (parent.mSimA.IsHuman)
                {
                    action = "Slap";
                }
                else if (parent.mSimA.IsHorse)
                {
                    action = "Stamp At";
                }
                else if (parent.mSimA.IsADogSpecies)
                {
                    action = "Growl At";
                }
                else if (parent.mSimA.IsCat)
                {
                    action = "Cat Hiss";
                }

                if (!string.IsNullOrEmpty(action))
                {
                    meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition(action, null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        public static bool FightFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                GreyedOutTooltipCallback callBack = null;
                if (SocialTest.TestFight(parent.mSimA, parent.mSimB, null, true, ref callBack))
                {
                    StoryProgression.Main.Situations.IncStat("First Action: Fight");

                    meetUp.ForceSituationSpecificInteraction(parent.mSimA, parent.mSimB, new SocialInteractionA.Definition("Fight!", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                    return true;
                }
                else
                {
                    return EnemyFirstAction(parent, meetUp);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public bool AllowCelebrity(IScoringGenerator stats, SimDescription sim)
        {
            if (!sim.CanBeCelebrity) return false;

            if (!Allow(stats, sim, AllowCheck.None)) return false;

            if (!GetValue<AllowCelebrityOption, bool>(sim))
            {
                stats.IncStat("Allow: Celebrity Denied");
                return false;
            }

            return true;
        }

        public bool AllowFriend(Common.IStatGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if ((check & AllowCheck.UserDirected) == AllowCheck.None)
            {
                if (Personalities.IsOpposing(stats, actor, target, true))
                {
                    stats.IncStat("Opposing Clan");
                    return false;
                }

                if (actor.IsEP11Bot)
                {
                    if (!HasFriendChips(actor))
                    {
                        stats.IncStat("Allow: Robot");
                        return false;
                    }
                    else if (actor.HasTrait(TraitNames.FearOfHumansChip))
                    {
                        if (!target.IsRobot)
                        {
                            stats.IncStat("Allow: FearOfHumans");
                            return false;
                        }
                    }
                }

                if (target.IsEP11Bot)
                {
                    if (!HasFriendChips(target))
                    {
                        stats.IncStat("Allow: Robot");
                        return false;
                    }
                    else if (target.HasTrait(TraitNames.FearOfHumansChip))
                    {
                        if (!actor.IsRobot)
                        {
                            stats.IncStat("Allow: FearOfHumans");
                            return false;
                        }
                    }
                }
            }

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            if (!actorData.Allowed<AllowCasteFriendOption>(targetData, true))
            {
                stats.IncStat("Caste Friend Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCasteFriendOption>(targetData, true))
            {
                stats.IncStat("Caste Friend Denied");
                return false;
            }

            return true;
        }

        public bool AllowEnemy(Common.IStatGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if ((check & AllowCheck.UserDirected) == AllowCheck.None)
            {
                if (actor.IsEP11Bot)
                {
                    if (!HasEnemyChips(actor))
                    {
                        stats.IncStat("Allow: Robot");
                        return false;
                    }
                }

                if (target.IsEP11Bot)
                {
                    if (!HasEnemyChips(target))
                    {
                        stats.IncStat("Allow: Robot");
                        return false;
                    }
                }
            }

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            if (!actorData.Allowed<AllowCasteEnemyOption>(targetData, true))
            {
                stats.IncStat("Caste Enemy Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCasteEnemyOption>(targetData, true))
            {
                stats.IncStat("Caste Enemy Denied");
                return false;
            }

            return true;
        }

        public static bool HasFriendChips(SimDescription sim)
        {
            return sim.TraitManager.HasAnyElement(sFriendChips);
        }

        public static bool HasEnemyChips(SimDescription sim)
        {
            return sim.TraitManager.HasAnyElement(sEnemyChips);
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
            if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
            {
                stats.IncStat("Allow: Reaper");
                return false;
            }

            if (!settings.GetValue<AllowFriendshipOption, bool>())
            {
                stats.IncStat("Allow: Friendship Denied");
                return false;
            }

            return true;
        }

        public void AccumulateCelebrity(SimDescription sim, int value)
        {
            if (sim == null) return;

            if (sim.CelebrityManager == null) return;

            if (!AllowCelebrity(this, sim)) return;

            if (value > 0)
            {
                if (sim.CelebrityLevel >= CelebrityManager.HighestLevel)
                {
                    //if (sim.CelebrityManager.Points + value > sim.CelebrityManager.GetGoalPointsForCurrentLevel())
                    //{
                        IncStat("Highest Celebrity Pre-Cutoff");
                        return;
                    //}
                }

                int residents = Sims.GetResidentCount(CASAgeGenderFlags.Human);
                if (residents > 0)
                {
                    uint nextLevel = sim.CelebrityLevel + 1;

                    int celebRatio = (AddScoring("Celeb Count " + nextLevel, Sims.GetCelebrityCount(nextLevel)) * 100) / residents;

                    if (AddScoring("Celeb Ratio " + nextLevel, celebRatio) > mCelebrityOptions[nextLevel - 1].Value)
                    {
                        IncStat("Celebrity Ratio Fail: " + nextLevel);
                        return;
                    }
                }

                AddScoring("Celebrity", value);

                try
                {
                    sim.CelebrityManager.AddPoints((uint)value);
                }
                catch (Exception e)
                {
                    Common.DebugException(sim, e);
                }

                if (sim.CelebrityLevel > CelebrityManager.HighestLevel)
                {
                    sim.CelebrityManager.mLevel = CelebrityManager.HighestLevel;

                    IncStat("Highest Celebrity Post-Cutoff");
                }
            }
            else if (value != 0)
            {
                value = -value;

                if (sim.CelebrityManager.mPoints < value)
                {
                    value -= (int)sim.CelebrityManager.mPoints;

                    if (sim.CelebrityManager.Level > 0)
                    {
                        AddScoring("Celebrity", -value);

                        IncStat("Celebrity Level Dropped");

                        sim.CelebrityManager.mLevel--;

                        sim.CelebrityManager.mPoints = sim.CelebrityManager.GetGoalPointsForCurrentLevel() - (uint)value;

                        if (sim.CelebrityManager.OwnerSim == null)
                        {
                            // Workaround for error in CelebrityManager:RemoveFreeStuffAlarm
                            if (sim.CelebrityManager.mFreeStuffAlarmHandle != AlarmHandle.kInvalidHandle)
                            {
                                AlarmManager.Global.RemoveAlarm(sim.CelebrityManager.mFreeStuffAlarmHandle);
                                sim.CelebrityManager.mFreeStuffAlarmHandle = AlarmHandle.kInvalidHandle;
                            }
                        }

                        sim.CelebrityManager.OnLevelModified(sim.CelebrityManager.mLevel + 1, false);
                    }
                    else
                    {
                        sim.CelebrityManager.mPoints = 0;
                    }
                }
                else
                {
                    sim.CelebrityManager.mPoints -= (uint)value;

                    AddScoring("Celebrity", -value);
                }
            }
        }

        public static bool AreFriends(SimDescription a, SimDescription b)
        {
            return AreFriends(a, b, (int)SimScenarioFilter.RelationshipLevel.Friend);
        }
        public static bool AreFriends(SimDescription a, SimDescription b, int minimum)
        {
            Relationship relation = ManagerSim.GetRelationship(a, b);
            if (relation == null) return false;

            return (relation.LTR.Liking > minimum);
        }

        public static bool AreEnemies(SimDescription a, SimDescription b)
        {
            return AreFriends(a, b, (int)SimScenarioFilter.RelationshipLevel.Enemy);
        }
        public static bool AreEnemies(SimDescription a, SimDescription b, int maximum)
        {
            Relationship relation = ManagerSim.GetRelationship(a, b);
            if (relation == null) return false;

            return (relation.LTR.Liking < maximum);
        }

        public List<SimDescription> FindExistingEnemyFor(IScoringGenerator stats, SimDescription sim)
        {
            return FindExistingEnemyFor(stats, sim, 0, false);
        }
        public List<SimDescription> FindExistingEnemyFor(IScoringGenerator stats, SimDescription sim, int maxLiking, bool ignoreBusy)
        {
            List<SimDescription> choices = new List<SimDescription>();

            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(sim));
            if (relations.Count == 0)
            {
                stats.IncStat("Enemy: None");
                return choices;
            }

            stats.AddStat("Enemy: Relations", relations.Count);

            foreach (Relationship relation in relations)
            {
                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null)
                {
                    stats.IncStat("Enemy: Bad Link");
                    continue;
                }

                stats.IncStat(other.FullName, Common.DebugLevel.Logging);

                if (relation.LTR.Liking >= maxLiking)
                {
                    stats.IncStat("Enemy: Liked");
                }
                else if (SimTypes.IsDead(other))
                {
                    stats.IncStat("Enemy: Dead");
                }
                else if ((!ignoreBusy) && (Situations.IsBusy(stats, other, true)))
                {
                    stats.IncStat("Enemy: Busy");
                }
                else if (!Allow(stats, other))
                {
                    stats.IncStat("Enemy: User Denied");
                }
                else
                {
                    choices.Add(other);
                }

                Main.Sleep("ManagerFriendship:FindExistingEnemyFor");
            }

            return choices;
        }

        public List<SimDescription> FindNemesisFor(IScoringGenerator stats, SimDescription sim, bool ignoreBusy)
        {
            List<SimDescription> choices = new List<SimDescription>();

            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(sim));
            if (relations.Count == 0)
            {
                stats.IncStat("Nemesis: None");
                return choices;
            }

            stats.AddStat("Nemesis: Relations", relations.Count);

            foreach (Relationship relation in relations)
            {
                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null)
                {
                    stats.IncStat("Nemesis: Bad Link");
                    continue;
                }

                stats.IncStat(other.FullName, Common.DebugLevel.Logging);

                if (!relation.HasInteractionBitSet(LongTermRelationship.InteractionBits.MakeEnemy))
                {
                    stats.IncStat("Nemesis: Not");
                }
                else if (SimTypes.IsDead(other))
                {
                    stats.IncStat("Nemesis: Dead");
                }
                else if ((!ignoreBusy) && (Situations.IsBusy(stats, other, true)))
                {
                    stats.IncStat("Nemesis: Busy");
                }
                else if (!Allow(stats, other))
                {
                    stats.IncStat("Nemesis: User Denied");
                }
                else
                {
                    choices.Add(other);
                }

                Main.Sleep("ManagerFriendship:FindNemesisFor");
            }

            return choices;
        }

        public List<SimDescription> FindExistingFriendFor(IScoringGenerator stats, SimDescription sim)
        {
            return FindExistingFriendFor(stats, sim, 0, false);
        }
        public List<SimDescription> FindExistingFriendFor(IScoringGenerator stats, SimDescription sim, int minLiking, bool ignoreBusy)
        {
            List<SimDescription> choices = new List<SimDescription>();

            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(sim));
            if (relations.Count == 0) 
            {
                stats.IncStat("Friend: None");
                return choices;
            }

            stats.AddStat("Friend: Relations", relations.Count);

            foreach (Relationship relation in relations)
            {
                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null)
                {
                    stats.IncStat("Friend: Bad Link");
                    continue;
                }

                stats.IncStat(other.FullName, Common.DebugLevel.Logging);

                if (relation.LTR.Liking <= minLiking)
                {
                    stats.IncStat("Friend: Unliked");
                }
                else if (SimTypes.IsDead(other))
                {
                    stats.IncStat("Friend: Dead");
                }
                else if ((!ignoreBusy) && (Situations.IsBusy(stats, other, true)))
                {
                    stats.IncStat("Friend: Busy");
                }
                else if (!Allow(stats, other))
                {
                    stats.IncStat("Friend: User Denied");
                }
                else
                {
                    choices.Add(other);
                }

                Main.Sleep("ManagerFriendship:FindExistingFriendFor");
            }

            return choices;
        }
        
        public class Updates : AlertLevelOption<ManagerFriendship>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerFriendship>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class SpeedOption : SpeedBaseOption<ManagerFriendship>
        {
            public SpeedOption()
                : base(300, true)
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<ManagerFriendship>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerFriendship>
        {
            public DumpScoringOption()
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerFriendship>
        {
            public TicksPassedOption()
            { }
        }

        public interface ICelebrityOption : INotRootLevelOption
        { }

        public class CelebrityListingOption : NestingManagerOptionItem<ManagerFriendship, ICelebrityOption>
        {
            public CelebrityListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityListing";
            }
        }

        public abstract class CelebrityRatioOption : IntegerManagerOptionItem<ManagerFriendship>, ICelebrityOption
        {
            public CelebrityRatioOption(int defValue)
                : base (defValue)
            { }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override string GetTitlePrefix()
            {
                return "CelebRatioLevel";
            }

            public override string Name
            {
                get
                {
                    return Common.Localize(GetTitlePrefix() + ":MenuName", IsFemaleLocalization(), new object[] { Level });
                }
            }

            public override string GetStoreKey()
            {
                return base.GetStoreKey() + Level.ToString();
            }

            protected abstract int Level
            {
                get;
            }

            public override bool Install(ManagerFriendship manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                manager.mCelebrityOptions[Level - 1] = this;

                return true;
            }
        }

        public class CelebrityRatioLevel1Option : CelebrityRatioOption
        {
            public CelebrityRatioLevel1Option()
                : base(100)
            { }

            protected override int Level
            {
                get { return 1; }
            }
        }

        public class CelebrityRatioLevel2Option : CelebrityRatioOption
        {
            public CelebrityRatioLevel2Option()
                : base(50)
            { }

            protected override int Level
            {
                get { return 2; }
            }
        }

        public class CelebrityRatioLevel3Option : CelebrityRatioOption
        {
            public CelebrityRatioLevel3Option()
                : base(25)
            { }

            protected override int Level
            {
                get { return 3; }
            }
        }

        public class CelebrityRatioLevel4Option : CelebrityRatioOption
        {
            public CelebrityRatioLevel4Option()
                : base(12)
            { }

            protected override int Level
            {
                get { return 4; }
            }
        }

        public class CelebrityRatioLevel5Option : CelebrityRatioOption
        {
            public CelebrityRatioLevel5Option()
                : base(6)
            { }

            protected override int Level
            {
                get { return 5; }
            }
        }
    }
}
