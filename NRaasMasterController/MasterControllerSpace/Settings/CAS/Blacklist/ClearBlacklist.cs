using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.MasterControllerSpace.Settings.CAS.Blacklist
{
    public class ClearBlacklist : OptionItem, IBlacklistOption
    {
        public override string GetTitlePrefix()
        {
            return "ClearBlacklist";
        }

        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(MasterController.Settings.BlacklistPartsCount);
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt")))
            {
                return OptionResult.Failure;
            }

            MasterController.Settings.ClearBlacklist();

            Common.Notify(Common.Localize(GetTitlePrefix() + ":Complete"));

            return OptionResult.SuccessClose;
        }
    }
}
