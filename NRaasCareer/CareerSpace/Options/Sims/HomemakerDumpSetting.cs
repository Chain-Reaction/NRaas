using NRaas.CommonSpace.Options;
using NRaas.CareerSpace.Booters;
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
    public class HomemakerDumpSetting : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "HomemakerDump";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (!Common.kDebugging) return false;

            NRaas.CareerSpace.Careers.Homemaker career = parameters.mTarget.Occupation as NRaas.CareerSpace.Careers.Homemaker;
            if (career == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            NRaas.CareerSpace.Careers.Homemaker career = parameters.mTarget.Occupation as NRaas.CareerSpace.Careers.Homemaker;
            if (career == null) return OptionResult.Failure;

            career.DumpLog();

            return OptionResult.SuccessClose;
        }
    }
}
