using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scenarios.Propagation
{
    public class PropagateAlimonyFailScenario : PropagateEnemyScenario
    {
        public PropagateAlimonyFailScenario(SimDescription primary, SimDescription enemy)
            : base(primary, enemy, BuffNames.Disgusted, Origin.FromTheft, -10)
        { }
        protected PropagateAlimonyFailScenario(PropagateAlimonyFailScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PropagateAlimonyFail";
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Household == null) return null;

            return sim.Household.AllSimDescriptions;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (AddScoring("CaresAboutChildren", GetValue<AlimonyChanceOption, int>(Target), ScoringLookup.OptionType.Chance, Target) <= 0)
            {
                ManagerSim.AddBuff(Target, BuffNames.Amused, Origin);
                return true;
            }

            if (ManagerSim.HasTrait(Target, TraitNames.Kleptomaniac))
            {
                ManagerSim.AddBuff(Target, BuffNames.Fascinated, Origin);
            }

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new PropagateAlimonyFailScenario(this);
        }
    }
}
