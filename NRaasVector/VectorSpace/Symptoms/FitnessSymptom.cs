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
    public class FitnessSymptom : SymptomBooter.Data
    {
        float mMinimum;
        float mMaximum;

        public FitnessSymptom(XmlDbRow row)
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

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            float delta = RandomUtil.GetFloat(mMinimum, mMaximum);
            if (delta == 0) return;

            float fitness = sim.SimDescription.mCurrentShape.Fit + delta;
            if (fitness < 0)
            {
                fitness = 0;
            }
            else if (fitness > 1)
            {
                fitness = 1;
            }

            sim.SimDescription.ForceSetBodyShape(sim.SimDescription.mCurrentShape.Weight, fitness);

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
