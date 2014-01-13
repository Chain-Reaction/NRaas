using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Stages
{
    public class EndStage : VectorBooter.Stage
    {
        public EndStage(XmlDbRow row)
            : base(row)
        { }

        public override int GetNextStage(SimDescription sim, DiseaseVector vector)
        {
            return -1;
        }

        public override bool ValidateStages(Dictionary<string, int> stages)
        {
            return true;
        }
    }
}
