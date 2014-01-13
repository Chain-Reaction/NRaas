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
    public class RomanceBaseChanceScoring : Scoring<SimDescription,SimScoringParameters> 
    {
        public RomanceBaseChanceScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            return true;
        }

        public override int Score(SimScoringParameters parameters)
        {
            return Woohooer.Settings.mRomanceBaseChanceScoringV2[PersistedSettings.GetSpeciesIndex(parameters.Actor)];
        }
    }
}

