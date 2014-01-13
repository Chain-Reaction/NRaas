using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public abstract class IsClanBaseScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>, IClanScoring
    {
        string mClan;

        public IsClanBaseScoring()
        { }

        protected string Clan
        {
            get
            {
                return mClan;
            }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Clan"))
            {
                error = "Clan missing";
                return false;
            }

            mClan = row.GetString("Clan").ToLower();

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
    }
}

