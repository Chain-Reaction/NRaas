using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CombinedDualLifetimeWantScoring : CombinedSimScoring<DualLifetimeWantScoring, uint, DualSimScoringParameters>
    {
        public CombinedDualLifetimeWantScoring()
        { }

        protected override uint GetValue(SimDescription sim)
        {
            return sim.LifetimeWish;
        }

        protected override ScoringDelegate GetScoringDelegate(SimDescription sim)
        {
            return new DualLifetimeWantScoring.Delegate(sim).Score;
        }
    }
}

