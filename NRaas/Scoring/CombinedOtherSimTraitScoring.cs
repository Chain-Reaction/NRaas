using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CombinedOtherSimTraitScoring : CombinedSimScoring<OtherSimTraitScoring, int, DualSimScoringParameters>
    {
        public CombinedOtherSimTraitScoring()
        { }

        protected override int GetValue(SimDescription sim)
        {
            if (sim.TraitManager == null) return 0;

            return sim.TraitManager.Count;
        }

        protected override ScoringDelegate GetScoringDelegate(SimDescription sim)
        {
            return new OtherSimTraitScoring.Delegate(sim).Score;
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            // Actors reversed
            return base.Score(new DualSimScoringParameters(parameters.Other, parameters.Actor));
        }
    }
}

