using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public abstract class ScheduledEnemyBaseScenario : DualSimScenario
    {
        int mFriendly = 0;

        public ScheduledEnemyBaseScenario()
        { }
        protected ScheduledEnemyBaseScenario(SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected ScheduledEnemyBaseScenario(ScheduledEnemyBaseScenario scenario)
            : base (scenario)
        { 
            mFriendly = scenario.mFriendly;
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int ContinueChance
        {
            get { return 5; }
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }
        protected override bool AllowSpecies(SimDescription sim, SimDescription target)
        {
            if (!sim.IsHuman) return true;

//          The FirstAction pushes require that the initiator be the non-human
            return target.IsHuman;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Friends.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            int delta = AddScoring("Friendly", sim);
            if (delta < -100)
            {
                delta = -100;
            }

            if (delta > 0)
            {
                IncStat("Friendly");
                return false;
            }
            else if (!RandomUtil.RandomChance(-delta))
            {
                IncStat("Chance Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Child != Target.Child)
            {
                IncStat("Wrong Age");
                return false;
            }
            else if (Sim.TeenOrAbove != Target.TeenOrAbove)
            {
                IncStat("Wrong Age");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool TargetSort(SimDescription sim, ref List<SimDescription> targets)
        {
            targets = new SimScoringList(Manager, "Enemy", targets, true, sim).GetBestByMinScore(1);
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Relationship.Get(Sim, Target, false) == null)
            {
                Add(frame, new NewEnemyScenario(this), ScenarioResult.Start);
            }
            else
            {
                Add(frame, new ExistingEnemyScenario(Sim, Target), ScenarioResult.Start);
            }

            return false;
        }
    }
}
