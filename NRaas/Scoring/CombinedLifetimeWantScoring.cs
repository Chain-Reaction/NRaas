using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CombinedLifetimeWantScoring : CombinedSimScoring<LifetimeWantScoring,uint,SimScoringParameters>
    {
        public CombinedLifetimeWantScoring()
        { }

        protected override uint GetValue(SimDescription sim)
        {
            return sim.LifetimeWish;
        }

        protected override ScoringDelegate GetScoringDelegate(SimDescription sim)
        {
            return new LifetimeWantScoring.Delegate(sim).Score;
        }
    }
}

