using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;
using Sims3.UI;

namespace NRaas.CareerSpace.Options.Assassination
{
    public class ResetSettings : OperationSettingOption<GameObject>, IAssassinationOption
    {
        public override string GetTitlePrefix()
        {
            return "AssassinationResetSettings";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return (Skills.Assassination.StaticGuid != SkillNames.None);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt"))) return OptionResult.Failure;

            Skills.Assassination.ResetSettings();
            return OptionResult.SuccessClose;
        }
    }
}
