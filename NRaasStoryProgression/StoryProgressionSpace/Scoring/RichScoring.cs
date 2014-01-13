using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class RichScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        int mAdditional = 0;

        public RichScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + ",Additional=" + mAdditional;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mAdditional = row.GetInt("Additional", mAdditional);

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            int threshold = SimDescription.kSimoleonThresholdForBeingRich + mAdditional;

            return StoryProgression.Main.GetValue<NetWorthOption, int>(parameters.Actor.Household) > threshold;
        }
    }
}

