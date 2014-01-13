using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class CaughtCheatingBreakupScenario : CaughtCheatingBreakupBaseScenario 
    {
        public CaughtCheatingBreakupScenario(SimDescription sim, bool affair, bool relatedStay)
            : base(sim, affair, relatedStay)
        { }
        protected CaughtCheatingBreakupScenario(CaughtCheatingBreakupScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "CaughtCheatingBreakup";
            }
            else
            {
                return "Dumped";
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (AddScoring("VictimAffairAcceptance", sim) >= 0)
            {
                IncStat("Accepted");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public override Scenario Clone()
        {
            return new CaughtCheatingBreakupScenario(this);
        }
    }
}
