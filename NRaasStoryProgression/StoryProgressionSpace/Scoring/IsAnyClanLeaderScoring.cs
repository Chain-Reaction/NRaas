using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class IsAnyClanLeaderScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        string mExclude;

        public IsAnyClanLeaderScoring()
        { }
        public IsAnyClanLeaderScoring(int hit, int miss, string exclude)
            : base(hit, miss)
        {
            mExclude = exclude;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Exclude"))
            {
                error = "Exclude missing";
                return false;
            }

            mExclude = row.GetString("Exclude").ToLower();

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            List<SimPersonality> clans = StoryProgression.Main.Personalities.GetClanLeadership(parameters.Actor);
            
            foreach (SimPersonality clan in clans)
            {
                if (!string.IsNullOrEmpty(mExclude))
                {
                    if (clan.UnlocalizedName.ToLower () == mExclude) continue;
                }
                
                return true;
            }

            return false;
        }
    }
}
