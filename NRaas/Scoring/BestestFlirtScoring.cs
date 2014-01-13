using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class BestestFlirtScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public BestestFlirtScoring()
        { }
        public BestestFlirtScoring(int hitScore, int missScore)
            : base (hitScore, missScore)
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            Relationship simRelationship = Relationship.Get(parameters.Actor, parameters.Other, false);
            if (simRelationship == null)
            {
                return false;
            }
            else if (!simRelationship.AreRomantic())
            {
                return false;
            }

            float simLiking = simRelationship.LTR.Liking;

            List<Relationship> relationships = new List<Relationship>(Relationship.GetRelationships(parameters.Other));

            foreach (Relationship relationship in relationships)
            {
                if (!relationship.AreRomantic()) continue;

                if (relationship.LTR.Liking > simLiking) return false;
            }

            return true;
        }
    }
}

