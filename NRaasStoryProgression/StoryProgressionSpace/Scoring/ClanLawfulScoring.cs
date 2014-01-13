using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class ClanLawfulScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        HitMissResult<SimDescription, SimScoringParameters> mNeutral;

        public ClanLawfulScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mNeutral = new HitMissResult<SimDescription, SimScoringParameters>(row, "Neutral", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override int Score(SimScoringParameters parameters)
        {
            switch (StoryProgression.Main.Personalities.GetLawfulness(parameters.Actor))
            {
                case Managers.ManagerPersonality.LawfulnessType.Lawful:
                    return mHit.Score(parameters);
                case Managers.ManagerPersonality.LawfulnessType.Unlawful:
                    return mMiss.Score(parameters);
                default:
                    return mNeutral.Score(parameters);
            }
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}

