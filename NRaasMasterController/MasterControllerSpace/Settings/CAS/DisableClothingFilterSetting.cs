using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.CAS
{
    public class DisableClothingFilterSetting : ListedSettingOption<OutfitCategories, GameObject>, ICASOption
    {
        protected override Proxy GetList()
        {
            return new ListProxy(MasterController.Settings.mDisableClothingFilterV2);
        }

        public override string GetLocalizedValue(OutfitCategories value)
        {
            return Sims.Basic.OutfitBase.Item.GetCategoryName(value);
        }

        protected override bool Allow(OutfitCategories value)
        {
            switch (value)
            {
                case OutfitCategories.Athletic:
                case OutfitCategories.Career:
                case OutfitCategories.Everyday:
                case OutfitCategories.Formalwear:
                case OutfitCategories.Outerwear:
                case OutfitCategories.Sleepwear:
                case OutfitCategories.Swimwear:
                case OutfitCategories.Bridle:
                case OutfitCategories.Racing:
                case OutfitCategories.Jumping:
                    return true;
            }

            return false;
        }

        public override bool ConvertFromString(string value, out OutfitCategories newValue)
        {
            return ParserFunctions.TryParseEnum<OutfitCategories>(value, out newValue, OutfitCategories.None);
        }

        public override string ConvertToString(OutfitCategories value)
        {
            return value.ToString();
        }

        public override string GetTitlePrefix()
        {
            return "DisableClothingFilterSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
