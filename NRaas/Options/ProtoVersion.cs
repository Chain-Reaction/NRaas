using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public interface IVersionOption : ICommonOptionItem
    {
        string Prompt
        {
            get;
        }
    }

    public class ExternalVersion
    {
        public static void ExternalVersionPrompt(StringBuilder builder)
        {
            List<IVersionOption> options = Common.DerivativeSearch.Find<IVersionOption>();

            foreach (IVersionOption option in options)
            {
                builder.Append(option.Prompt);
            }
        }
    }

    public interface IPrimaryOption<TTarget> : IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>
        where TTarget : class, IGameObject
    { }

    public abstract class ProtoVersion<TTarget> : InteractionOptionItem<IActor,TTarget,GameHitParameters<TTarget>>, IPrimaryOption<TTarget>, IVersionOption
        where TTarget : class, IGameObject
    {
        public override string GetTitlePrefix()
        {
            return "Version";
        }

        public override string DisplayValue
        {
            get { return EAText.GetNumberString(VersionStamp.sVersion); }
        }

        public virtual string Prompt
        {
            get { return Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { VersionStamp.sVersion }); }
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            SimpleMessageDialog.Show(Name, Prompt);
            return OptionResult.SuccessClose;
        }
    }
}
