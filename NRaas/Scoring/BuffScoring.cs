using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class BuffScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>
    {
        BuffNames mBuff = BuffNames.Undefined;

        public BuffScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mBuff;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!base.Parse(row, ref error)) return false;

            if (!row.Exists("Buff"))
            {
                error = "Buff missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<BuffNames>(row.GetString("Buff"), out mBuff, BuffNames.Undefined))
            {
                error = "Unknown Buff " + row.GetString("Buff");
                return false;
            }

            return true;
        }

        public override bool Cachable
        {
            get { return false; }
        }

        public override bool IsHit(SimScoringParameters parameters)
        {            
            if (parameters.Actor.CreatedSim == null) return false;
            
            return parameters.Actor.CreatedSim.BuffManager.HasElement(mBuff);
        }
    }
}

