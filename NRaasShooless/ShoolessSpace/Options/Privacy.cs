using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Options
{
    public class Privacy : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        GameObject mTarget;

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;

            if (parameters.mTarget.LotCurrent == null) return false;

            if (Common.IsRootMenuObject(parameters.mTarget)) return true;

            if (!parameters.mTarget.InWorld) return false;

            if (!Shooless.Settings.GetGlobalPrivacy()) return false;

            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string Name
        {
            get
            {
                if (Shooless.Settings.GetPrivacy(mTarget))
                {
                    return Shooless.Localize("Privacy:On");
                }
                else
                {
                    return Shooless.Localize("Privacy:Off");
                }
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (Common.IsRootMenuObject(parameters.mTarget))
            {
                Shooless.Settings.ToggleGlobalPrivacy();
            }
            else
            {
                Shooless.Settings.TogglePrivacy(parameters.mTarget);
            }
            return OptionResult.SuccessRetain;
        }
    }
}


