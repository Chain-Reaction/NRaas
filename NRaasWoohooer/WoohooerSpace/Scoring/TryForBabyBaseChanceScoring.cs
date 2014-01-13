using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Scoring
{
    public class TryForBabyBaseChanceScoring : Scoring<SimDescription,SimScoringParameters> 
    {
        public TryForBabyBaseChanceScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            return true;
        }

        public override int Score(SimScoringParameters parameters)
        {
            if ((parameters.Actor.IsHuman) && (parameters.Actor.Teen))
            {
                return Woohooer.Settings.mTryForBabyTeenBaseChanceScoring;
            }
            else
            {
                return Woohooer.Settings.mTryForBabyBaseChanceScoringV2[PersistedSettings.GetSpeciesIndex(parameters.Actor)];
            }
        }
    }
}

