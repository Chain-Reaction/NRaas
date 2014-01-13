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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Hair
{
    public class InvalidHatCategoriesSetting : ListedSettingOption<OutfitCategories, GameObject>, IHairOption
    {
        protected override Proxy GetList()
        {
            return new ListProxy(Dresser.Settings.mInvalidHatCategories);
        }

        public override string GetTitlePrefix()
        {
            return "InvalidHatCategories";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override string GetLocalizedValue(OutfitCategories value)
        {
            return Common.LocalizeEAString("Ui/Caption/ObjectPicker:" + value);
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Dresser.Settings.mAllowHats) return false;

            return base.Allow(parameters);
        }

        protected override bool Allow(OutfitCategories value)
        {
            switch (value)
            {
                case OutfitCategories.Athletic:
                case OutfitCategories.Everyday:
                case OutfitCategories.Formalwear:
                case OutfitCategories.Sleepwear:
                case OutfitCategories.Swimwear:
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
    }
}
