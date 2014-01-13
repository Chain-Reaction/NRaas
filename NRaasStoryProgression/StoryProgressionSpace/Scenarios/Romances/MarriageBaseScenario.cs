using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class MarriageBaseScenario : DualSimScenario
    {
        public MarriageBaseScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected MarriageBaseScenario(MarriageBaseScenario scenario)
            : base (scenario)
        { }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }
            else if (!Romances.AllowMarriage(this, sim, Managers.Manager.AllowCheck.Active))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.IsMarried)
            {
                IncStat("Already Married");
                return false;
            }
            else if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected virtual bool Score()
        {
            int scoreA = AddScoring("Marriage", Sim, Target);
            int scoreB = AddScoring("Marriage", Target, Sim);

            return ((scoreA > 0) && (scoreB > 0));
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (!Score())
            {
                IncStat("Scoring Failure");
                return false;
            }
            else if (TestForChildBlock(Sim, target))
            {
                IncStat("Child Are Married");
                return false;
            }
            else if (!Romances.Allow(this, Sim, target))
            {
                //IncStat("Mixed Age Denied");
                return false;
            }

            return base.TargetAllow(target);
        }

        public static bool TestForChildBlock(SimDescription a, SimDescription b)
        {
            foreach (SimDescription childA in Relationships.GetChildren(a))
            {
                foreach (SimDescription childB in Relationships.GetChildren(b))
                {
                    if (childA.Partner == childB) return true;
                }
            }

            return false;
        }
    }
}
