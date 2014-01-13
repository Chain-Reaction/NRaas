using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Scoring
{
    public class WoohooBaseChanceScoring : Scoring<SimDescription,SimScoringParameters> 
    {
        public WoohooBaseChanceScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            return true;
        }

        public override int Score(SimScoringParameters parameters)
        {
            if ((parameters.Actor.IsHuman) && (parameters.Actor.Teen))
            {
                return Woohooer.Settings.mWoohooTeenBaseChanceScoring;
            }
            else
            {
                return Woohooer.Settings.mWoohooBaseChanceScoringV2[PersistedSettings.GetSpeciesIndex(parameters.Actor)];
            }
        }
    }
}

