using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class LifetimeWantScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        LifetimeWant mLTW = LifetimeWant.None;

        public LifetimeWantScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mLTW;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!base.Parse(row, ref error)) return false;

            if (!row.Exists("LTW"))
            {
                error = "LTW missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<LifetimeWant>(row.GetString("LTW"), out mLTW, LifetimeWant.None))
            {
                error = "Unknown LTW " + row.GetString("LTW");
                return false;
            }

            return true;
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            return (parameters.Actor.LifetimeWish == (uint)mLTW);
        }

        public class Delegate
        {
            LifetimeWant mLTW;

            public Delegate(SimDescription sim)
            {
                mLTW = (LifetimeWant)sim.LifetimeWish;
            }

            public int Score(LifetimeWantScoring scoring, SimScoringParameters parameters)
            {
                return scoring.Score(scoring.mLTW == mLTW, parameters);
            }
        }
    }
}

