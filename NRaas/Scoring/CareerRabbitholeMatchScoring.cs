using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CareerRabbitholeMatchScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public CareerRabbitholeMatchScoring()
        { }

        public static RabbitHole GetRabbithole(SimDescription sim)
        {
            if (sim == null) return null;

            Career career = sim.Occupation as Career;
            if (career == null) return null;

            if (career.CareerLoc == null) return null;

            return career.CareerLoc.Owner;
        }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return (GetRabbithole(parameters.Actor) == GetRabbithole(parameters.Other));
        }
    }
}

