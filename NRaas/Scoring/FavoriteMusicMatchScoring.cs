using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class FavoriteMusicMatchScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public FavoriteMusicMatchScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return (parameters.Actor.mFavouriteMusicType == parameters.Other.mFavouriteMusicType);
        }
    }
}

