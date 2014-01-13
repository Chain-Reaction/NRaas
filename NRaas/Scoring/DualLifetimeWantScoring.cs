using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class DualLifetimeWantScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        LifetimeWant mLTW = LifetimeWant.None;

        public DualLifetimeWantScoring()
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

        public override bool IsHit(DualSimScoringParameters parameters)
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

            public int Score(DualLifetimeWantScoring scoring, DualSimScoringParameters parameters)
            {
                return scoring.Score(scoring.mLTW == mLTW, parameters);
            }
        }
    }
}

