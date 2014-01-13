using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class RunScenario : ScheduledSoloScenario
    {
        List<WeightOption> mScenarios = null;

        bool mFirst = true;

        SimPersonality mPersonality;

        public RunScenario(SimPersonality personality)
        {
            mPersonality = personality;
            mScenarios = personality.GetScenarios();
        }
        protected RunScenario(RunScenario scenario)
            : base (scenario)
        {
            mFirst = scenario.mFirst;
            mPersonality = scenario.mPersonality;
            mScenarios = scenario.mScenarios;  // intentionally reference the list
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Run";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 30; }
        }

        protected override int MaximumReschedules
        {
            get 
            {
                if (mScenarios == null) return 0;

                return mScenarios.Count; 
            }
        }

        protected override bool Allow()
        {
            if (!mPersonality.ProgressionEnabled) return false;

            return base.Allow();
        }

        protected override bool Matches(Scenario scenario)
        {
            RunScenario runScenario = scenario as RunScenario;
            if (runScenario == null) return false;

            return (mPersonality == runScenario.mPersonality);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimPersonality clan = Manager as SimPersonality;

            foreach (SimDescription sim in clan.GetClanMembers(false))
            {
                clan.CheckRetention(sim);
            }

            if ((mScenarios == null) || (mScenarios.Count == 0))
            {
                IncStat("No Choices");
                return false;
            }

            WeightOption scenario = RandomUtil.GetWeightedRandomObjectFromList(mScenarios.ToArray()) as WeightOption;
            if (scenario == null)
            {
                IncStat("Bad Choice");
                return false;
            }

            if (!mFirst)
            {
                if (scenario is SimPersonality.IMustBeFirstChoiceOption)
                {
                    IncStat("Must Be First");
                    return false;
                }
            }

            mFirst = false;

            mScenarios.Remove(scenario);

            Add(frame, scenario.GetScenario(), ScenarioResult.Start);
            Add(frame, new ResetChanceScenario(mPersonality), ScenarioResult.Success);
            return true;
        }

        public override Scenario Clone()
        {
            return new RunScenario(this);
        }

        public class ResetChanceScenario : SuccessScenario
        {
            SimPersonality mPersonality = null;

            public ResetChanceScenario(SimPersonality personality)
            {
                mPersonality = personality;
            }
            public ResetChanceScenario(ResetChanceScenario scenario)
                : base(scenario)
            {
                mPersonality = scenario.mPersonality;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "ResetChance";
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                mPersonality.ChanceOfEvent = 0;

                IncStat("Chance Reset");

                return base.PrivateUpdate(frame);
            }

            public override Scenario Clone()
            {
                return new ResetChanceScenario(this);
            }
        }
    }
}
