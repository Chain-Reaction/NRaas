﻿using NRaas.CommonSpace.Options;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;

namespace NRaas.CareerSpace.Options.Sims
{
    public class ChangeUnemployedTitle : OperationSettingOption<Sim>, ISimOption
    {
        Sim mTarget;

        public override string GetTitlePrefix()
        {
            return "ChangeUnemployedTitle";
        }

        public override string Name
        {
            get
            {
                return Common.Localize("ChangeUnemployedTitle:MenuName", (mTarget != null) ? mTarget.IsFemale : false);
            }
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            mTarget = parameters.mTarget;

            return (parameters.mTarget.Occupation is Unemployed);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            Unemployed unemployed = parameters.mTarget.Occupation as Unemployed;
            if (unemployed == null) return OptionResult.Failure;

            unemployed.ChangeName();
            return OptionResult.SuccessClose;
        }
    }
}
