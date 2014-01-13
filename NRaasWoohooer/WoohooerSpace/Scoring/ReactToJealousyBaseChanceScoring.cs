using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Scoring
{
    public class ReactToJealousyBaseChanceScoring : Scoring<SimDescription,SimScoringParameters> 
    {
        public ReactToJealousyBaseChanceScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            return true;
        }

        public override int Score(SimScoringParameters parameters)
        {
            return Woohooer.Settings.mReactToJealousyBaseChanceScoring;
        }
    }
}

