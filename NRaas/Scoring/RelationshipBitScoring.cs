using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class RelationshipBitScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        LongTermRelationship.InteractionBits mBit = LongTermRelationship.InteractionBits.None;

        public RelationshipBitScoring()
        {}
        public RelationshipBitScoring(int hit, int miss, LongTermRelationship.InteractionBits bit)
            : base(hit, miss)
        {
            mBit = bit;
        }

        public override string ToString()
        {
            return base.ToString() + "," + mBit;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Bit"))
            {
                error = "Bit missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<LongTermRelationship.InteractionBits>(row.GetString("Bit"), out mBit, LongTermRelationship.InteractionBits.None))
            {
                error = "Unknown Bit " + row.GetString("Bit");
                return false;
            }

            return base.Parse(row, ref error);
        }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            Relationship relationship = Relationship.Get(parameters.Actor, parameters.Other, false);
            if (relationship == null) return false;

            return relationship.LTR.HasInteractionBit (mBit);
        }
    }
}

