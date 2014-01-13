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
    public class DebtScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        HitMissResult<SimDescription, SimScoringParameters> mGate;

        public DebtScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mGate = new HitMissResult<SimDescription, SimScoringParameters>(row, "Gate", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            return base.Parse(row, ref error);
        }

        public override void Validate()
        {
            if (mGate != null)
            {
                mGate.Validate();
            }

            base.Validate();
        }

        public override bool Cachable
        {
            get { return false; }
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            return (StoryProgression.Main.GetValue<DebtOption,int>(parameters.Actor.Household) >= mGate.Score(parameters));
        }
    }
}

