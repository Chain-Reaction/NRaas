using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public class SimScoringParameters : ListedScoringParameters<SimDescription>
    {
        public SimScoringParameters(SimDescription scoreAgainst)
            : this(scoreAgainst, false)
        { }
        public SimScoringParameters(SimDescription scoreAgainst, bool absolute)
            : base(scoreAgainst, absolute)
        { }

        public override string ToString()
        {
            string result = base.ToString();

            if (Actor != null)
            {
                result += " " + Actor.FullName;
            }

            return result;
        }
    }

    public class SimListedScoringMethod : GenericSimListedScoringMethod<SimScoringParameters, SimScoringParameters>, IScoringMethod<SimDescription, DualSimScoringParameters>
    {
        public SimListedScoringMethod()
        { }
        protected SimListedScoringMethod(bool hasCache)
            : base(hasCache)
        { }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
    }
}

