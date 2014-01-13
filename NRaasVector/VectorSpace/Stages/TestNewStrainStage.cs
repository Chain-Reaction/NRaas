using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Stages
{
    public class TestNewStrainStage : HitMissStage
    {
        public TestNewStrainStage(XmlDbRow row)
            : base(row)
        { }

        protected override bool IsSuccess(SimDescription sim, DiseaseVector vector)
        {
            bool result = vector.NewStrain;

            vector.NewStrain = false;

            return result;
        }
    }
}
