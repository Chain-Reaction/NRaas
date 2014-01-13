using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options
{
    public class DumpVectors : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Vector.Settings.Debugging;
        }

        public override string GetTitlePrefix()
        {
            return "DumpVectors";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder builder = new Common.StringBuilder();

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                builder.Append(Common.NewLine + Common.NewLine + vector.ToString());
            }

            foreach (SymptomBooter.Data symptom in SymptomBooter.Symptoms)
            {
                builder.Append(Common.NewLine + Common.NewLine + symptom.ToString());
            }

            foreach (ResistanceBooter.Data resistance in ResistanceBooter.Resistances)
            {
                builder.Append(Common.NewLine + Common.NewLine + resistance.ToString());
            }

            builder.Append(Common.NewLine + Vector.Settings.Dump());

            Common.DebugWriteLog(builder);

            return OptionResult.SuccessRetain;
        }
    }
}
