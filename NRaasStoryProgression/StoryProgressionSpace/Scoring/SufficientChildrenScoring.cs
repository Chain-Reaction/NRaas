using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class SufficientChildrenScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>
    {
        int mGate = 3;

        public SufficientChildrenScoring()
            : base(0, 5)
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Gate"))
            {
                error = "Gate missing";
                return false;
            }

            mGate = row.GetInt("Gate", 0);

            return base.Parse(row, ref error);
        }

        public override bool Cachable
        {
            get { return false; }
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            SimDescription sim = parameters.Actor;

            if ((sim.Adult) || ((sim.IsMale) && (sim.Elder)))
            {
                if (sim.Genealogy.Children.Count >= mGate)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
