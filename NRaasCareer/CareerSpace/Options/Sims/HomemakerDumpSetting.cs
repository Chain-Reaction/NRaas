using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;

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
