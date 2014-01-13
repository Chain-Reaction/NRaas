using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.ScoringMethods
{
    public class CareerScoringParameters : SimScoringParameters
    {
        Dictionary<OccupationNames, int> mScores = new Dictionary<OccupationNames, int>();

        public CareerScoringParameters(SimDescription scoreAgainst)
            : base(scoreAgainst)
        { }

        public Dictionary<OccupationNames, int> Scores
        {
            get { return mScores; }
        }

        public void Add(OccupationNames occupation, int score)
        {
            int value = 0;
            if (!mScores.TryGetValue(occupation, out value))
            {
                mScores.Add(occupation, score);
            }
            else
            {
                mScores[occupation] = value + score;
            }
        }
    }

    public class CareerScoringMethod : GenericSimListedScoringMethod<CareerScoringParameters, CareerScoringParameters>
    {
        public CareerScoringMethod()
        { }
    }
}
