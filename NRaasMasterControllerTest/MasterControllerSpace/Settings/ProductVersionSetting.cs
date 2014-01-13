using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class ProductVersionSetting : ListedSettingOption<ProductVersion, GameObject>, ISettingOption
    {
        public override string GetTitlePrefix()
        {
            return "ProductVersion";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new CustomProxy();
        }

        public override string GetLocalizedValue(ProductVersion value)
        {
            string result = null;
            if (Common.Localize("ProductVersion:" + value, false, new object[0], out result))
            {
                return result;
            }
            else
            {
                return value.ToString();
            }
        }

        protected override bool Allow(ProductVersion value)
        {
            if (value == ProductVersion.Undefined) return false;

            if (value == ProductVersion.CurrentTip) return false;

            return base.Allow(value);
        }

        public override bool ConvertFromString(string value, out ProductVersion newValue)
        {
            return ParserFunctions.TryParseEnum<ProductVersion>(value, out newValue, ProductVersion.Undefined);
        }

        public override string ConvertToString(ProductVersion value)
        {
            return value.ToString();
        }

        public class CustomProxy : Proxy
        {
            public override string GetDisplayValue(ListedSettingOption<ProductVersion, GameObject> option)
            {
                return "";
            }

            public override string GetExportValue(ListedSettingOption<ProductVersion, GameObject> option)
            {
                return null;
            }

            public override void Clear()
            {
                GameUtils.sProductFlags = ProductVersion.Undefined;
            }

            public override bool Contains(ProductVersion value)
            {
                return ((GameUtils.sProductFlags & value) != ProductVersion.Undefined);
            }

            public override void Add(ProductVersion value)
            {
                GameUtils.sProductFlags |= value;
            }

            public override void Remove(ProductVersion value)
            {
                GameUtils.sProductFlags &= ~value;
            }
        }
    }
}
