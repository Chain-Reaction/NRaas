using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class NormalBabyAgingScenario : SimScenario, IAlarmScenario
    {
        public NormalBabyAgingScenario(SimDescription sim)
            : base(sim)
        { }
        protected NormalBabyAgingScenario(NormalBabyAgingScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "NormalBabyAging";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 1f, TimeUnit.Minutes);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.Baby)
            {
                IncStat("Not Baby");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim.AgingYearsSinceLastAgeTransition = 0;

            IncStat("Reset");
            return true;
        }

        public override Scenario Clone()
        {
            return new NormalBabyAgingScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "NormalBabyAging";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
