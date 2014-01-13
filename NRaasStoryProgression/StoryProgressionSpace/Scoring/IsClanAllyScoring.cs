using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class IsClanAllyScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>, IClanScoring
    {
        public IsClanAllyScoring()
        { }

        protected override Dictionary<SimDescription, int> HitCollect(SimDescription obj)
        {
            Dictionary<SimDescription, int> hits = new Dictionary<SimDescription, int>();

            foreach (SimDescription sim in StoryProgression.Main.Personalities.GetClanAlliesFor(obj))
            {
                hits.Add(sim, 1);
            }

            return hits;
        }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return StoryProgression.Main.Personalities.IsFriendly(StoryProgression.Main.Personalities, parameters.Actor, parameters.Other);
        }
    }
}

