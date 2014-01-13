using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class TraitMismatchScoring : TraitBaseScoring<DualSimScoringParameters>
    {
        TraitNames mOtherTrait = TraitNames.Unknown;

        public TraitMismatchScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mOtherTrait;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("OtherTrait"))
            {
                error = "OtherTrait missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<TraitNames>(row.GetString("OtherTrait"), out mOtherTrait, TraitNames.Unknown))
            {
                error = "Unknown OtherTrait " + row.GetString("OtherTrait");
                return false;
            }

            return base.Parse(row, ref error);
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            if (!parameters.Other.TraitManager.HasElement(mOtherTrait))
            {
                return 0;
            }

            return base.Score(parameters);
        }
    }
}

