using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.Sims.Basic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class CopyCategoryClothing : SimFromList, IIntermediateOption
    {
        static CopyCategoryClothing()
        { }

        public override string GetTitlePrefix()
        {
            return "CopyCategoryClothing";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.GetOutfit(OutfitCategories.Everyday, 0) == null) return false;

            if (me.IsUsingMaternityOutfits) return false;

            if (SimTypes.IsSkinJob(me)) return false;

            if (me.Household == null) return false;

            if (me.Household == Household.ActiveHousehold) return true;

            return (!SimTypes.IsTourist(me));
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<ChangeOutfit.Item> allOptions = new List<ChangeOutfit.Item>();

            SavedOutfit.Cache cache = new SavedOutfit.Cache(me);
            foreach (SavedOutfit.Cache.Key outfit in cache.Outfits)
            {
                switch (outfit.Category)
                {
                    case OutfitCategories.Everyday:
                    case OutfitCategories.Formalwear:
                    case OutfitCategories.Sleepwear:
                    case OutfitCategories.Swimwear:
                    case OutfitCategories.Athletic:
                    case OutfitCategories.Career:
                    case OutfitCategories.Outerwear:
                        allOptions.Add(new ChangeOutfit.Item(outfit.mKey, me));
                        break;
                }
            }

            CommonSelection<ChangeOutfit.Item>.Results sourceList = new CommonSelection<ChangeOutfit.Item>(Name, allOptions).SelectMultiple();
            if ((sourceList == null) || (sourceList.Count == 0)) return false;

            List<CategoryItem> allCategories = new List<CategoryItem>();
            foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
            {
                switch (category)
                {
                    case OutfitCategories.Everyday:
                    case OutfitCategories.Formalwear:
                    case OutfitCategories.Sleepwear:
                    case OutfitCategories.Swimwear:
                    case OutfitCategories.Athletic:
                    case OutfitCategories.Career:
                    case OutfitCategories.Outerwear:
                        allCategories.Add(new CategoryItem(category));
                        break;
                }
            }

            CommonSelection<CategoryItem>.Results destination = new CommonSelection<CategoryItem>(Name, allCategories).SelectMultiple();
            if ((destination == null) || (destination.Count == 0)) return false;

            foreach (ChangeOutfit.Item source in sourceList)
            {
                SimOutfit sourceOutfit = CASParts.GetOutfit(me, source.Value, false);

                foreach (CategoryItem item in destination)
                {
                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(me, new CASParts.Key(item.Value, -1), sourceOutfit))
                    { }

                    SpeedTrap.Sleep();
                }
            }

            if (me.CreatedSim != null)
            {
                me.CreatedSim.RefreshCurrentOutfit(false);
            }

            return true;
        }

        public class CategoryItem : ValueSettingOption<OutfitCategories>
        {
            public CategoryItem(OutfitCategories value)
                : base(value, OutfitBase.Item.GetCategoryName(value), 0)
            { }
        }
    }
}
