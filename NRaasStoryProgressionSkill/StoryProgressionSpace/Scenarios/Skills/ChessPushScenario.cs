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
    public class ChessPushScenario : DualSimSingleProcessScenario, IHasSkill
    {
        public ChessPushScenario()
        { }
        protected ChessPushScenario(ChessPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Chess";
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
                return new SkillNames[] { SkillNames.Chess, SkillNames.Logic };
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
            return new SimScoringList(this, "Logic", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            SimDescription opponent = FindNextOpponent(sim);
            if (opponent == null) return null;

            return new List<SimDescription>(new SimDescription[] { opponent });
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
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

        public static bool TestTable(ChessTable table)
        {
            if (table.LotCurrent == null) return false;

            if (!table.InWorld) return false;

            if (table.NumSimsPlaying() != 0) return false;

            if (table.GetTotalNumChairsAtTable() < 2) return false;

            return true;
        }

        protected static bool FirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                NRaas.StoryProgression.Main.Situations.IncStat("First Action: Chess");

                List<ChessTable> tables = new List<ChessTable>(parent.mLot.GetObjects<ChessTable>(TestTable));
                if (tables.Count == 0)
                {
                    return false;
                }

                meetUp.ForceSituationSpecificInteraction(RandomUtil.GetRandomObjectFromList(tables), parent.mSimA, ChessTable.ChallengeSimToGameOfChess.Singleton, null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(parent.mSimA, parent.mSimB, e);
                return false;
            }
       }

        protected static bool TestNextOpponent(Common.IStatGenerator stats, LogicSkill mySkill)
        {
            if (!TestNextOpponent(stats, mySkill, mySkill.mChessRankingNextChallenger))
            {
                stats.IncStat("Next Opponent Fail");
                return false;
            }

            int possibleWins = mySkill.NumberOfChessTournamentWins + 1;

            int newRank = 0;
            while ((newRank < LogicSkill.kTournamentGamesWinsToImproveRank.Length) && (possibleWins >= LogicSkill.kTournamentGamesWinsToImproveRank[newRank]))
            {
                newRank++;
            }

            if (newRank > mySkill.mChessRank)
            {
                int oldRank = mySkill.mChessRank;
                mySkill.mChessRank = newRank;
                try
                {
                    if (!TestNextOpponent(stats, mySkill, mySkill.mChessRankingNextChallenger))
                    {
                        stats.IncStat("Next Rank Fail");
                        return false;
                    }
                }
                finally
                {
                    mySkill.mChessRank = oldRank;
                }
            }

            return true;
        }
        protected static bool TestNextOpponent(Common.IStatGenerator stats, LogicSkill mySkill, SimDescription choice)
        {
            SimDescription nextOpponent = TournamentManagement.FindSuitableOpponent(mySkill.SkillOwner, Household.sHouseholdList, choice, new TournamentManagement.GetAffinity(mySkill.GetAffinity));

            if ((nextOpponent != null) &&
                (nextOpponent.SkillManager != null) &&
                ((nextOpponent.Household == null) || (!nextOpponent.Household.IsActive)))
            {
                Chess oppSkill = nextOpponent.SkillManager.AddElement(SkillNames.Chess) as Chess;
                if ((oppSkill != null) && (oppSkill.SkillLevel < LogicSkill.kChessSkillForOpponentPerRank[mySkill.mChessRank]))
                {
                    // Next opponent will receive free skills
                    return false;
                }
            }

            return true;
        }

        protected SimDescription FindNextOpponent(SimDescription sim)
        {
            LogicSkill mySkill = sim.SkillManager.GetElement(SkillNames.Logic) as LogicSkill;
            if (mySkill == null)
            {
                IncStat("No Skill");
                return null;
            }

            if ((mySkill.mChessRankingNextChallenger == null) ||
                (SimTypes.IsDead(mySkill.mChessRankingNextChallenger)) ||
                (mySkill.mChessRankingNextChallenger.CreatedSim == null) ||
                (mySkill.mChessRankingNextChallenger.CreatedSim.WorldBuilderDeath != null) ||
                (!mySkill.mChessRankingNextChallenger.IsValidDescription) ||
                (SimTypes.IsSelectable(mySkill.mChessRankingNextChallenger)))
            {
                List<Household> households = new List<Household>(Household.sHouseholdList);
                households.Remove(Household.ActiveHousehold);

                mySkill.mChessRankingNextChallenger = TournamentManagement.FindSuitableOpponent(sim, households, mySkill.mChessRankingNextChallenger, new TournamentManagement.GetAffinity(mySkill.GetAffinity));
            }

            if (mySkill.mChessRankingNextChallenger == null)
            {
                IncStat("No Choice");
                return null;
            }

            if (!TestNextOpponent(this, mySkill))
            {
                return null;
            }

            if (mySkill.mChessRankingNextChallenger.SkillManager == null)
            {
                return null;
            }

            LogicSkill theirSkill = mySkill.mChessRankingNextChallenger.SkillManager.GetElement(SkillNames.Logic) as LogicSkill;
            if ((theirSkill != null) && (!TestNextOpponent(this, theirSkill)))
            {
                return null;
            }

            return mySkill.mChessRankingNextChallenger;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            List<ChessTable> tables = new List<ChessTable>();

            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                tables.AddRange(lot.GetObjects<ChessTable>(TestTable));
            }

            if (tables.Count == 0)
            {
                foreach (Lot lot in ManagerLot.GetOwnedLots(Target))
                {
                    tables.AddRange(lot.GetObjects<ChessTable>(TestTable));
                }

                if (tables.Count == 0)
                {
                    foreach (ChessTable table in Sims3.Gameplay.Queries.GetObjects<ChessTable>())
                    {
                        if (!TestTable(table)) continue;

                        if (!table.LotCurrent.IsCommunityLot) continue;

                        tables.Add(table);
                    }

                    if (tables.Count == 0)
                    {
                        IncStat("No Table");
                        return false;
                    }
                }
            }

            return Situations.PushMeetUp(this, Sim, Target, RandomUtil.GetRandomObjectFromList(tables).LotCurrent, FirstAction);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new ChessPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, ChessPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ChessPush";
            }
        }
    }
}
