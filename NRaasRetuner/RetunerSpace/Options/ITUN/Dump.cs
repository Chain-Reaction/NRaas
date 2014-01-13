using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.ITUN
{
    public class Dump : OperationSettingOption<GameObject>, IITUNOption
    {
        InteractionTuning mTuning;

        public Dump(InteractionTuning tuning)
        {
            mTuning = tuning;
        }

        public override string GetTitlePrefix()
        {
            return "Dump";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            msg += Common.NewLine + mTuning.FullInteractionName;
            msg += Common.NewLine + mTuning.ShortInteractionName;
            msg += Common.NewLine + mTuning.FullObjectName;
            msg += Common.NewLine + mTuning.ShortObjectName;
            msg += Common.NewLine + mTuning.mFlags;

            Common.DebugWriteLog(msg);
            return OptionResult.SuccessClose;
        }
    }
}
