using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class DivorcedScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public override bool IsHit(DualSimScoringParameters parameters)
        {
            Relationship relation = Relationship.Get(parameters.Other, parameters.Actor, false);
            if (relation == null) return false;
            
            if (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.BreakUp)) return true;

            if (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Divorce)) return true;

            return false;
        }
    }
}

