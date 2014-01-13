using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CareerRabbitHoleScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        RabbitHoleType mType;

        public CareerRabbitHoleScoring()
        { }
        public CareerRabbitHoleScoring(int hit, int miss, RabbitHoleType type)
            : base (hit, miss)
        {
            mType = type;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("RabbitHoleType"))
            {
                error = "RabbitHoleType missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<RabbitHoleType>(row.GetString("RabbitHoleType"), out mType, RabbitHoleType.None))
            {
                error = "Unknown RabbitHoleType " + row.GetString("RabbitHoleType");
                return false;
            }

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            SimDescription sim = parameters.Actor;

            if ((sim.Occupation != null) &&
                (sim.Occupation.CareerLoc != null) &&
                (sim.Occupation.CareerLoc.Owner.Guid == mType))
            {
                return true;
            }

            if ((sim.CareerManager != null) && 
                (sim.CareerManager.School != null) &&
                (sim.CareerManager.School.CareerLoc != null) &&
                (sim.CareerManager.School.CareerLoc.Owner.Guid == mType))
            {
                return true;
            }

            return false;
        }
    }
}

