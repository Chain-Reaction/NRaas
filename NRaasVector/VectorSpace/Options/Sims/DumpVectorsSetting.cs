using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options.Sims
{
    public class DumpVectorsSetting : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "DumpVectors";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!Common.kDebugging) return false;

            if (!Vector.Settings.HasVectors(parameters.mTarget)) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            string msg = "Vectors for " + parameters.mTarget.FullName;

            foreach (DiseaseVector vector in Vector.Settings.GetVectors(parameters.mTarget))
            {
                msg += Common.NewLine + Common.NewLine + vector.GetUnlocalizedDescription();
            }

            Common.DebugWriteLog(msg);

            return OptionResult.SuccessClose;
        }
    }
}
