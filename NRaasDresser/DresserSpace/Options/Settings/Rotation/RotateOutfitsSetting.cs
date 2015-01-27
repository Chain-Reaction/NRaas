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

namespace NRaas.DresserSpace.Options.Settings.Rotation
{
    public class RotateOutfitsSetting : ListedSettingOption<OutfitCategories, GameObject>, IRotationOption     
    {
        public override string GetLocalizedValue(OutfitCategories value)
        {
            return value.ToString();
        }

        protected override bool Allow(OutfitCategories value)
        {
            switch(value)
            {
                case OutfitCategories.Athletic:
                case OutfitCategories.Everyday:
                case OutfitCategories.Formalwear:
                case OutfitCategories.MartialArts:
                case OutfitCategories.Outerwear:
                case OutfitCategories.Sleepwear:
                case OutfitCategories.Swimwear:
                    return base.Allow(value);
                default: break;
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

        protected override Proxy GetList()
        {
            return new ListProxy(Dresser.Settings.mAllowRotationOutfitCategories);
        }

        public override string GetTitlePrefix()
        {
            return "RotateOutfitsCategories";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}