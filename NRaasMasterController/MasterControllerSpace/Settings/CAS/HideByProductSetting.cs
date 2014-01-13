using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.MasterControllerSpace.Settings.CAS
{
    public class HideByProductSetting : ListedSettingOption<ProductVersion, GameObject>, ICASOption
    {
        public override string GetTitlePrefix()
        {
            return "HideByProductSetting";
        }

        protected override Proxy GetList()
        {
            return new ListProxy(MasterController.Settings.mHideByProduct);
        }

        public override string GetLocalizedValue(ProductVersion value)
        {
            return ProductVersions.GetLocalizedName(value);
        }

        protected override bool Allow(ProductVersion value)
        {
            switch (value)
            {
                case ProductVersion.Undefined:
                case ProductVersion.CurrentTip:
                    return false;
            }

            return true;
        }

        public override bool ConvertFromString(string value, out ProductVersion newValue)
        {
            return ParserFunctions.TryParseEnum<ProductVersion>(value, out newValue, ProductVersion.Undefined);
        }

        public override string ConvertToString(ProductVersion value)
        {
            return value.ToString();
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
