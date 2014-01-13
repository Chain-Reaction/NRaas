using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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
    public class ScoredExistingFriendScenario : ExistingFriendScenario
    {
        public ScoredExistingFriendScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected ScoredExistingFriendScenario(ScoredExistingFriendScenario scenario)
            : base (scenario)
        { }

        protected override bool Allow(SimDescription sim)
        {
            int delta = AddScoring("Friendly", sim);
            if (delta > 100)
            {
                delta = 100;
            }

            if (delta <= 0)
            {
                IncStat("Unfriendly");
                return false;
            }
            else if (AddScoring("Popularity", sim) < 0)
            {
                IncStat("Unpopular");
                return false;
            }
            else if (!RandomUtil.RandomChance(delta))
            {
                IncStat("Chance Fail");
                return false;
            }

            return base.Allow(sim);
        }
   
        public override Scenario Clone()
        {
            return new ScoredExistingFriendScenario(this);
        }
    }
}
