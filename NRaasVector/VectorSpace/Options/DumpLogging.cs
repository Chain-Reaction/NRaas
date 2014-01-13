using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
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
    public class DumpLogging : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Vector.Settings.Debugging;
        }

        public override string GetTitlePrefix()
        {
            return "DumpLogging";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            ScoringLog.Dump(false);
            return OptionResult.SuccessRetain;
        }
    }
}
