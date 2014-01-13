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
    public class WeightEqualizationSymptom : WeightBaseSymptom
    {
        public WeightEqualizationSymptom(XmlDbRow row)
            : base(row)
        { }

        public override float GetDelta(Sim sim)
        {
            if (sim.SimDescription.Weight == 0) return 0f;

            float delta = base.GetDelta(sim);

            if (sim.SimDescription.Weight < 0)
            {
                delta = -delta;
            }

            return delta;
        }
    }
}
