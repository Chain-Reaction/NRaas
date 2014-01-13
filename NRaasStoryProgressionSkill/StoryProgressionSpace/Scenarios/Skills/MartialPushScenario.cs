using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class MartialPushScenario : DualSimSingleProcessScenario, IHasSkill
    {
        bool mReport = true;

        public MartialPushScenario()
        { }
        protected MartialPushScenario(MartialPushScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "MartialArts";
        }

        protected override bool ShouldReport
        {
            get 
            { 
                if (!mReport) return false;

                return base.ShouldReport;
            }
        }

        protected override int ContinueReportChance
        {
            get { return 10; }
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.MartialArts };
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "MartialArts", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (CanCompete(sim))
            {
                SimDescription opponent = FindNextOpponent(sim);
                if (opponent == null) return null;

                return new List<SimDescription>(new SimDescription[] { opponent });
            }
            else
            {
                return new SimScoringList(this, "WantsMartialTraining", Sims.TeensAndAdults, false).GetBestByMinScore(1);
            }
        }

        protected static bool CanCompete(SimDescription sim)
        {
            MartialArts skill = sim.SkillManager.GetElement(SkillNames.MartialArts) as MartialArts;
            if ((skill != null) && (skill.CanParticipateInTournaments))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!CanCompete(Sim)) 
            {
                if (sim.Household == Sim.Household)
                {
                    IncStat("Same Family");
                    return false;
                }
                else if ((sim.SkillManager.GetSkillLevel(SkillNames.MartialArts) >= Sim.SkillManager.GetSkillLevel(SkillNames.MartialArts)))
                {
                    IncStat("Wrong Level");
                    return false;       
                }
            }
            
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.BuffManager.HasElement(BuffNames.Fatigued))
            {
                IncStat("Fatigued");
                return false;
            }
            else if (sim.IsPregnant)
            {
                IncStat("Pregnant");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        public static bool TestDummy(TrainingDummy dummy)
        {
            if (!dummy.InWorld) return false;

            if (dummy.InUse) return false;

            return true;
        }

        protected static bool CompeteFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                NRaas.StoryProgression.Main.Situations.IncStat("First Action: Martial Compete");

                meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Challenge To Spar For Tournament", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        protected static bool TrainFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                NRaas.StoryProgression.Main.Situations.IncStat("First Action: Martial Train");

                TrainingDummy choice = null;

                InteractionInstance interaction = parent.mSimB.InteractionQueue.GetCurrentInteraction();
                if ((interaction != null) && (interaction.Target is TrainingDummy))
                {
                    choice = interaction.Target as TrainingDummy;

                    meetUp.ForceSituationSpecificInteraction(choice, parent.mSimA, MartialArtsTrain.TrainSingleton, null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                }
                else
                {
                    List<TrainingDummy> choices = new List<TrainingDummy>(parent.mLot.GetObjects<TrainingDummy>(TestDummy));
                    if (choices.Count == 0)
                    {
                        return false;
                    }

                    choice = RandomUtil.GetRandomObjectFromList(choices);

                    meetUp.ForceSituationSpecificInteraction(choice, parent.mSimA, new MartialArtsTrain.Definition(parent.mSimB), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        protected static bool TestNextOpponent(Common.IStatGenerator stats, MartialArts mySkill)
        {
            SimDescription tournamentChallenger = ManagerSim.Find(mySkill.mTournamentChallenger);

            if (!TestNextOpponent(stats, mySkill, tournamentChallenger))
            {
                stats.IncStat("Next Opponent Fail");
                return false;
            }

            int possibleWins = mySkill.mSparringTournamentMatchWins + 1;

            int newRank = 0;
            while ((newRank < MartialArts.kTournamentWinsToImproveSparringRank.Length) && (possibleWins >= MartialArts.kTournamentWinsToImproveSparringRank[newRank]))
            {
                newRank++;
            }

            if (newRank > mySkill.mTournamentRank)
            {
                int oldRank = mySkill.mTournamentRank;
                mySkill.mTournamentRank = newRank;
                try
                {
                    if (!TestNextOpponent(stats, mySkill, tournamentChallenger))
                    {
                        stats.IncStat("Next Rank Fail");
                        return false;
                    }
                }
                finally
                {
                    mySkill.mTournamentRank = oldRank;
                }
            }

            return true;
        }
        protected static bool TestNextOpponent(Common.IStatGenerator stats, MartialArts mySkill, SimDescription choice)
        {
            if (GameUtils.GetCurrentWorld() == WorldName.China) return true;

            SimDescription nextOpponent = TournamentManagement.FindSuitableOpponent(mySkill.SkillOwner, Household.sHouseholdList, choice, new TournamentManagement.GetAffinity(mySkill.GetAffinity));

            if ((nextOpponent != null) &&
                (nextOpponent.SkillManager != null) &&
                ((nextOpponent.Household == null) || (!nextOpponent.Household.IsActive)))
            {
                MartialArts oppSkill = nextOpponent.SkillManager.AddElement(SkillNames.MartialArts) as MartialArts;
                if ((oppSkill != null) && (oppSkill.SkillLevel < MartialArts.kMartialArtsSkillForOpponentPerTournamentRank[mySkill.mTournamentRank]))
                {
                    // Next opponent will receive free skills
                    return false;
                }
            }

            return true;
        }

        protected SimDescription FindNextOpponent(SimDescription sim)
        {
            MartialArts mySkill = sim.SkillManager.GetElement(SkillNames.MartialArts) as MartialArts;
            if (mySkill == null)
            {
                IncStat("No Skill");
                return null;
            }

            SimDescription tournamentChallenger = ManagerSim.Find(mySkill.mTournamentChallenger);

            if ((tournamentChallenger == null) ||
                (SimTypes.IsDead(tournamentChallenger)) ||
                (tournamentChallenger.CreatedSim == null) ||
                (tournamentChallenger.CreatedSim.WorldBuilderDeath != null) ||
                (!tournamentChallenger.IsValidDescription) ||
                (SimTypes.IsSelectable(tournamentChallenger)))
            {
                List<Household> households = new List<Household>(Household.sHouseholdList);
                households.Remove(Household.ActiveHousehold);

                tournamentChallenger = TournamentManagement.FindSuitableOpponent(sim, households, tournamentChallenger, new TournamentManagement.GetAffinity(mySkill.GetAffinity));
            }

            if (tournamentChallenger == null)
            {
                IncStat("No Choice");
                return null;
            }

            mySkill.mTournamentChallenger = tournamentChallenger.SimDescriptionId;

            if (!TestNextOpponent(this, mySkill))
            {
                return null;
            }

            if (tournamentChallenger.SkillManager == null)
            {
                return null;
            }

            MartialArts theirSkill = tournamentChallenger.SkillManager.GetElement(SkillNames.MartialArts) as MartialArts;
            if ((theirSkill != null) && (!TestNextOpponent(this, theirSkill)))
            {
                return null;
            }

            return tournamentChallenger;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            if (CanCompete(Sim))
            {
                if (!Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Either, CompeteFirstAction))
                {
                    IncStat("Compete Push Fail");
                }
                else
                {
                    mReport = false;
                    return true;
                }
            }

            List<TrainingDummy> dummies = new List<TrainingDummy>();
            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                dummies.AddRange(lot.GetObjects<TrainingDummy>(TestDummy));
            }

            if (dummies.Count == 0)
            {
                foreach (Lot lot in ManagerLot.GetOwnedLots(Target))
                {
                    dummies.AddRange(lot.GetObjects<TrainingDummy>(TestDummy));
                }

                if (dummies.Count == 0)
                {
                    foreach (TrainingDummy dummy in Sims3.Gameplay.Queries.GetObjects<TrainingDummy>())
                    {
                        if (!TestDummy(dummy)) continue;

                        if (dummy.LotCurrent == null) continue;

                        if (!dummy.LotCurrent.IsCommunityLot) continue;

                        dummies.Add(dummy);
                    }

                    if (dummies.Count == 0)
                    {
                        IncStat("No Dummies");
                        return false;
                    }
                }
            }

            return Situations.PushMeetUp(this, Sim, Target, RandomUtil.GetRandomObjectFromList(dummies).LotCurrent, TrainFirstAction);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new MartialPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, MartialPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MartialPush";
            }
        }
    }
}
