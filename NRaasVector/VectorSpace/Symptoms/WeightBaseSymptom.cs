using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public abstract class WeightBaseSymptom : SymptomBooter.Data
    {
        float mMinimum;
        float mMaximum;

        public WeightBaseSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Minimum", Guid))
            {
                mMinimum = row.GetFloat("Minimum");
            }

            if (BooterLogger.Exists(row, "Maximum", Guid))
            {
                mMaximum = row.GetFloat("Maximum");
            }
        }

        public virtual float GetDelta(Sim sim)
        {
            return RandomUtil.GetFloat(mMinimum, mMaximum);
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            float delta = GetDelta(sim);

            float weight = sim.SimDescription.Weight + delta;
            if (weight < -1)
            {
                weight = -1;
            }
            else if (weight > 1)
            {
                weight = 1;
            }

            sim.SimDescription.ForceSetBodyShape(weight, sim.SimDescription.mCurrentShape.Fit);

            sim.RequireImmediateBodyShapeUpdate = true;
            sim.SimDescription.UpdateBodyShape(0f, sim.ObjectId);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Minimum: " + mMinimum;
            result += Common.NewLine + " Maximum: " + mMaximum;

            return result;
        }
    }
}
