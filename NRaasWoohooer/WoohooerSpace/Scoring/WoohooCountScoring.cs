using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Scoring
{
    public class WoohooCountScoring : Scoring<SimDescription,SimScoringParameters> 
    {
        public WoohooCountScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            return true;
        }

        public override bool Cachable
        {
            get { return false; }
        }

        public override int Score(SimScoringParameters parameters)
        {
            return Woohooer.Settings.GetCount(parameters.Actor) * Woohooer.Settings.mWoohooCountScoreFactor[PersistedSettings.GetSpeciesIndex(parameters.Actor)];
        }
    }
}

