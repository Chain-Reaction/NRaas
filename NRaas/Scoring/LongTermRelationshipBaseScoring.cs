using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public abstract class LongTermRelationshipBaseScoring<SP> : HitMissScoring<SimDescription, SP, SP>
        where SP : SimScoringParameters
    {
        protected HitMissResult<SimDescription, SP> mNotKnown;

        protected int mGate = 0;

        public LongTermRelationshipBaseScoring()
        { }
        public LongTermRelationshipBaseScoring(int notKnownScore, int hitScore, int missScore, int gate)
            : base(hitScore, missScore)
        {
            mGate = gate;

            mNotKnown = new HitMissResult<SimDescription, SP>(notKnownScore);
        }

        public override void Validate()
        {
            if (mNotKnown != null)
            {
                mNotKnown.Validate();
            }

            base.Validate();
        }

        public override bool Cachable
        {
            get
            {
                if (!mNotKnown.Cachable) return false;

                return base.Cachable;
            }
        }

        public override string ToString()
        {
            return base.ToString() + ",Gate " + mGate;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mNotKnown = new HitMissResult<SimDescription, SP>(row, "NotKnown", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            if (!row.Exists("Gate"))
            {
                error = "Gate missing";
                return false;
            }

            mGate = row.GetInt("Gate");

            return base.Parse(row, ref error);
        }

        protected override Dictionary<SimDescription,int> HitCollect(SimDescription obj)
        {
            bool zero = ((mHit.IsZero) && (mMiss.IsZero));

            Dictionary<SimDescription,int> hits = new Dictionary<SimDescription,int>();

            foreach (Relationship relation in Relationship.Get(obj))
            {
                SimDescription sim = relation.GetOtherSimDescription(obj);

                if (zero)
                {
                    hits.Add(sim, (int)relation.LTR.Liking);
                }
                else if (relation.LTR.Liking >= mGate)
                {
                    hits.Add(sim, 200);
                }
                else
                {
                    hits.Add(sim, -200);
                }
            }

            return hits;
        }

        protected bool BaseIsHit(DualSimScoringParameters parameters)
        {
            float liking = 0;

            Relationship relationship = Relationship.Get(parameters.Other, parameters.Actor, false);
            if (relationship != null)
            {
                liking = relationship.LTR.Liking;
            }

            return (liking >= mGate);
        }

        protected int BaseScore(SP parameters, SimDescription other, out bool success)
        {
            success = true;

            if (HasCollection)
            {
                int collection = 0;
                if (GetCollectedScore(parameters.Actor, out collection))
                {
                    switch (collection)
                    {
                        case -200:
                            return mMiss.Score(parameters);
                        case 200:
                            return mHit.Score(parameters);
                        default:
                            return collection;
                    }
                }
                else
                {
                    return mNotKnown.Score(parameters);
                }
            }
            else
            {
                Relationship relationship = Relationship.Get(other, parameters.Actor, false);
                if (relationship == null)
                {
                    return mNotKnown.Score(parameters);
                }
                else if ((mHit.IsZero) && (mMiss.IsZero))
                {
                    return (int)relationship.LTR.Liking;
                }
            }

            success = false;
            return 0;
        }
    }
}

