using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.ScoringMethods
{
    public class PreferredBabyCountScoringMethod : SimListedScoringMethod
    {
        public override int Score(SimScoringParameters parameters)
        {
            int score = base.Score(parameters);
            score += StoryProgression.Main.GetValue<BaseNumberOfChildrenOption, int>(parameters.Actor);

            int maximum = StoryProgression.Main.GetValue<MaximumNumberOfChildrenOption,int>(parameters.Actor);
            if (maximum > 0)
            {
                if (score > maximum)
                {
                    score = maximum;
                }
            }

            return score;
        }
    }
}
