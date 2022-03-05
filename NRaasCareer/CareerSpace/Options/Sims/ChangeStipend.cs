using NRaas.CommonSpace.Options;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.UI;

namespace NRaas.CareerSpace.Options.Sims
{
    public class ChangeStipend : OperationSettingOption<Sim>, ISimOption
    {
        Sim mTarget;

        public override string GetTitlePrefix()
        {
            return "ChangeStipend";
        }

        public override string Name
        {
            get
            {
                return Common.Localize("ChangeStipend:MenuName", (mTarget != null) ? mTarget.IsFemale : false);
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

            string text = StringInputDialog.Show(Name, Common.Localize("ChangeStipend:Prompt", parameters.mActor.IsFemale), unemployed.mStipend.ToString());
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            int value;
            if (!int.TryParse(text, out value))
            {
                Common.Notify(Common.Localize("Numeric:Error"));
                return OptionResult.Failure;
            }

            unemployed.mStipend = value;
            return OptionResult.SuccessClose;
        }
    }
}
