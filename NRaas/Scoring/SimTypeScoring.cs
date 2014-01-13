using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class SimTypeScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        SimType mType;

        public SimTypeScoring()
        { }
        public SimTypeScoring(int hit, int miss, SimType type)
            : base (hit, miss)
        { 
            mType = type;
        }

        public override string ToString()
        {
            return base.ToString() + "," + mType;
        }

        public override bool Cachable
        {
            get { return false; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!base.Parse(row, ref error)) return false;

            if (!row.Exists("Type"))
            {
                error = "Type missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<SimType>(row.GetString("Type"), out mType, SimType.None))
            {
                error = "Unknown Type " + row.GetString("Type");
                return false;
            }

            return true;
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            return SimTypes.Matches(parameters.Actor, mType);
        }
    }
}

