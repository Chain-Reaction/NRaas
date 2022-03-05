using Sims3.Gameplay.Interfaces;
using Sims3.UI;

namespace NRaas.CommonSpace.Options
{
    public abstract class ProtoResetSettings<TTarget> : OperationSettingOption<TTarget>, IPrimaryOption<TTarget>
        where TTarget : class, IGameObject
    {
        public override string GetTitlePrefix()
        {
            return "Reset";
        }

        protected override OptionResult Run(GameHitParameters< TTarget> parameters)
        {
            if (AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt")))
            {
                VersionStamp.ResetSettings();

                SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Complete"));
                return OptionResult.SuccessRetain;
            }

            return OptionResult.Failure;
        }
    }
}
