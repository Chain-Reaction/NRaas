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
    public class SimTypeMatchScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        SimType mType;

        SimType mOtherType;

        public SimTypeMatchScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mType + "," + mOtherType;
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

            if (!row.Exists("OtherType"))
            {
                error = "OtherType missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<SimType>(row.GetString("OtherType"), out mOtherType, SimType.None))
            {
                error = "Unknown OtherType " + row.GetString("OtherType");
                return false;
            }

            return true;
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            if (!SimTypes.Matches(parameters.Other, mOtherType))
            {
                return 0;
            }

            return base.Score(parameters);
        }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return SimTypes.Matches(parameters.Actor, mType);
        }
    }
}

