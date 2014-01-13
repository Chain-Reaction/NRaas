using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options
{
    public class CounterScaledScoring : SimScaledScoring<SimScoringParameters>
    {
        string mVector;

        string mCounter;

        public CounterScaledScoring()
        { }

        public override bool Cachable
        {
            get { return false; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Vector"))
            {
                error = "Vector missing";
                return false;
            }
            else if (!row.Exists("Counter"))
            {
                error = "Counter missing";
                return false;
            }

            mVector = row.GetString("Vector");
            mCounter = row.GetString("Counter");

            return true;
        }

        protected override int GetScaler(SimScoringParameters parameters)
        {
            foreach (DiseaseVector vector in Vector.Settings.GetVectors(parameters.Actor))
            {
                if (vector.Guid != mVector) continue;

                return vector.GetCounter(mCounter);
            }

            return 0;
        }
    }
}
