using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class RedHairedBabyScenario : CaughtCheatingBaseScenario
    {
        public RedHairedBabyScenario(SimDescription sim, SimDescription target)
            : base(sim, target, true)
        { }
        protected RedHairedBabyScenario(RedHairedBabyScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RedHairedBaby";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return null;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Partner == null)
            {
                IncStat("No Partner");
                return false;
            }
            else
            {
                int elapsedTime = GetElapsedTime<DayOfLastPartnerOption>(sim);
                if (elapsedTime < GetValue<PregnancyLengthOption,int>())
                {
                    IncStat("Partnered Early");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Partner == Target)
            {
                IncStat("Partner");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public override Scenario Clone()
        {
            return new RedHairedBabyScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RedHairedBaby";
            }

            public override bool Install(ManagerPregnancy manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                if (initial)
                {
                    BirthScenario.OnBirthScenario += OnRun;
                }

                return true;
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                BirthScenario birthScenario = scenario as BirthScenario;

                if (birthScenario.Dad != null)
                {
                    scenario.Add(frame, new RedHairedBabyScenario(birthScenario.Sim, birthScenario.Dad), ScenarioResult.Start);
                }
            }
        }

        public class PregnancyLengthOption : IntegerManagerOptionItem<ManagerPregnancy>, IDebuggingOption
        {
            public PregnancyLengthOption()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "RedHairedBabyPregnancyLength";
            }
        }
    }
}
