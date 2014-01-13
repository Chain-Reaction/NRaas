using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CombinedTraitScoring : CombinedSimScoring<TraitScoring,int,SimScoringParameters>
    {
        public CombinedTraitScoring()
        { }

        protected override int GetValue(SimDescription sim)
        {
            if (sim.TraitManager == null) return 0;

            return sim.TraitManager.Count;
        }

        protected override ScoringDelegate GetScoringDelegate(SimDescription sim)
        {
            return new TraitScoring.Delegate(sim).Score;
        }
    }
}

