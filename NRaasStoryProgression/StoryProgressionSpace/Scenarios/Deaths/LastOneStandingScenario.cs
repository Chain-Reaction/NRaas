using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class LastOneStandingScenario : SimScenario
    {
        public LastOneStandingScenario(SimDescription sim)
            : base (sim)
        { }
        protected LastOneStandingScenario(LastOneStandingScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "LastOneStanding";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((sim.Household == null) || (sim.LotHome == null))
            {
                IncStat("Homeless");
                return false;
            }

            int count = 0;
            foreach(SimDescription member in sim.Household.AllSimDescriptions)
            {
                if (!Households.AllowGuardian(member)) continue;

                if (Deaths.IsDying(member)) continue;

                count++;
            }

            if (count > 0)
            {
                IncStat("Remaining");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new LastOneStandingScenario(this);
        }
    }
}
