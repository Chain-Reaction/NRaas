using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class ScoredBreakupScenario : BreakupScenario 
    {
        bool mOpposingClan = false;

        public ScoredBreakupScenario(SimDescription sim, SimDescription target)
            : base (sim, target, false, false)
        { }
        protected ScoredBreakupScenario(ScoredBreakupScenario scenario)
            : base (scenario)
        {
            mOpposingClan = scenario.mOpposingClan;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "ScoredBreakup";
            }
            else if (mOpposingClan)
            {
                return "OpposingBreakup";
            }
            else
            {
                return "Breakup";
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (AddScoring("Breakup Cooldown", GetElapsedTime<DayOfLastRomanceOption>(sim) - MinTimeFromRomanceToBreakup) < 0)
            {
                AddStat("Too Early", GetElapsedTime<DayOfLastRomanceOption>(sim));
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool breakup = false;

            Relationship relation = ManagerSim.GetRelationship(Sim, Target);
            if ((relation != null) && (relation.CurrentLTRLiking <= GetValue<MaximumForceBreakupOption, int>()))
            {
                breakup = true;
            }
            else
            {
                int scoreA = AddScoring("Breakup", Target, Sim);
                int scoreB = AddScoring("Breakup", Sim, Target);

                if ((scoreA > 0) && (scoreB > 0))
                {
                    breakup = true;
                }
                else if ((scoreA > 0) || (scoreB > 0))
                {
                    if ((GetValue<SplitOpposingClanOption, bool>()) && (!Romances.Allow(this, Sim, Target)))
                    {
                        breakup = true;
                        mOpposingClan = true;
                    }
                    else
                    {
                        int chance = (scoreA + scoreB);
                        if ((chance > 0) && (RandomUtil.RandomChance(AddScoring("Chance", chance))))
                        {
                            breakup = true;
                        }
                        else
                        {
                            Add(frame, new ExistingEnemyManualScenario(Sim, Target, -25, 0), ScenarioResult.Start);
                        }
                    }
                }
            }

            if (breakup)
            {
                return base.PrivateUpdate(frame);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new ScoredBreakupScenario(this);
        }

        public class SplitOpposingClanOption : BooleanManagerOptionItem<ManagerPersonality>
        {
            public SplitOpposingClanOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "BreakupOpposingClan";
            }

            public override bool Install(ManagerPersonality manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                if (initial)
                {
                    ScheduledMarriageScenario.OnBreakup += OnRun;
                }

                return true;
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                ScheduledMarriageScenario marriageScenario = scenario as ScheduledMarriageScenario;

                scenario.Add(frame, new ScoredBreakupScenario(marriageScenario.Sim, marriageScenario.Target), ScenarioResult.Start);
            }
        }

        public class MaximumForceBreakupOption : IntegerManagerOptionItem<ManagerRomance>
        {
            public MaximumForceBreakupOption()
                : base(-90)
            { }

            public override string GetTitlePrefix()
            {
                return "MaximumForceBreakup";
            }
        }
    }
}
