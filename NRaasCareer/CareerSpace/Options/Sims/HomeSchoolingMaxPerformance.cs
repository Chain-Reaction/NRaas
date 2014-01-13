using NRaas.Gameplay.Careers;
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

namespace NRaas.CareerSpace.Options.Sims
{
    public class HomeSchoolingMaxPerformance : OperationSettingOption<Sim>, ISimOption
    {
        Sim mTarget;

        public override string GetTitlePrefix()
        {
            return "HomeSchoolingMaxPerformance";
        }

        public override string Name
        {
            get
            {
                bool maxPerformance = false;

                HomeSchooling career = null;
                if (mTarget != null)
                {
                    career = mTarget.School as HomeSchooling;
                    if (career != null)
                    {
                        maxPerformance = career.mMaxPerformance;
                    }
                }

                if (maxPerformance)
                {
                    return Common.Localize("HomeSchoolingMaxPerformance:Enable", (mTarget != null) ? mTarget.IsFemale : false);
                }
                else
                {
                    return Common.Localize("HomeSchoolingMaxPerformance:Disable", (mTarget != null) ? mTarget.IsFemale : false);
                }
            }
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            mTarget = parameters.mTarget;

            return (parameters.mTarget.School is HomeSchooling);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            HomeSchooling career = parameters.mTarget.School as HomeSchooling;
            if (career == null) return OptionResult.Failure;

            career.mMaxPerformance = !career.mMaxPerformance;

            if (career.mMaxPerformance)
            {
                career.mPerformance = 100;
            }

            return OptionResult.SuccessClose;
        }
    }
}
