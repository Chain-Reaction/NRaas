using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class CooldownMarriageScenario : ExpectedMarriageScenario 
    {
        bool mTestScoring;

        public CooldownMarriageScenario(SimDescription sim, SimDescription target, bool testScoring)
            : base (sim, target)
        {
            mTestScoring = testScoring;
        }
        protected CooldownMarriageScenario(CooldownMarriageScenario scenario)
            : base (scenario)
        {
            mTestScoring = scenario.mTestScoring;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Marriage";
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!sim.IsEngaged)
            {
                IncStat("Not Engaged");
                return false;
            }

            if (mTestScoring) 
            {
                Relationship relation = ManagerSim.GetRelationship(Sim, Target);
                if (relation == null) return false;

                if (relation.LTR.Liking < GetValue<LikingGateOption, int>())
                {
                    AddStat("No Like", relation.LTR.Liking);
                    return false;
                }
                else if (AddScoring("Marriage Cooldown", TestElapsedTime<DayOfLastEngagementOption, MinTimeFromEngagementToMarriageOption>(sim)) < 0)
                {
                    AddStat("Too Early", GetElapsedTime<DayOfLastEngagementOption>(sim));
                    return false;
                }
            }

            return base.CommonAllow(sim);
        }

        public override Scenario Clone()
        {
            return new CooldownMarriageScenario(this);
        }

        public class MinTimeFromEngagementToMarriageOption : Manager.CooldownOptionItem<ManagerRomance>
        {
            public MinTimeFromEngagementToMarriageOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownEngagementToMarriage";
            }
        }

        public class LikingGateOption : IntegerManagerOptionItem<ManagerRomance>
        {
            public LikingGateOption()
                : base(75)
            { }

            public override string GetTitlePrefix()
            {
                return "MarriageLikingGate";
            }
        }
    }
}
