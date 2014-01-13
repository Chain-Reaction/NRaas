using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class PersonalityOptionScoring : HitMissScoring<SimDescription, ManagerScoringParameters, DualSimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>, IScoring<SimDescription, SimScoringParameters>
    {
        string mOption;

        public PersonalityOptionScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Option"))
            {
                error = "Option Missing";
                return false;
            }

            mOption = row.GetString("Option");

            return base.Parse(row, ref error);
        }

        public int Score(SimScoringParameters parameters)
        {
            return BaseScore(parameters as ManagerScoringParameters);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return BaseScore(parameters as ManagerScoringParameters);
        }

        public override bool IsHit(ManagerScoringParameters parameters)
        {
            return parameters.Manager.GetValue<BooleanOption, bool>(mOption);
        }
    }
}

