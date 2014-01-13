using NRaas.CommonSpace.Helpers;
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

namespace NRaas.DresserSpace.Options.Settings
{
    public abstract class ProductVersionSetting<TObject> : ListedSettingOption<ProductVersion, TObject>
        where TObject : class, IGameObject
    {
        public override string GetLocalizedValue(ProductVersion value)
        {
            return ProductVersions.GetLocalizedName(value);
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
    }
}
