﻿using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.SimIFace;
using Sims3.UI;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Options.General.Careers
{
    public abstract class CareerSettingOption : OperationSettingOption<GameObject>
    {
        public List<OccupationNames> mPicks = new List<OccupationNames>();         

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, this.GetOptions()).SelectMultiple();

            foreach (Item item in selection)
            {
                mPicks.Add(item.Value);
            }

            if (mPicks.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Selection:Error"));
                return OptionResult.Failure;
            }

            return OptionResult.SuccessRetain;
        }

        public List<Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            results.Add(new Item(OccupationNames.Any, Common.Localize("Selection:All"), ThumbnailKey.kInvalidThumbnailKey));

            foreach (Career career in CareerManager.CareerList)
            {
                results.Add(new Item(career.Guid, career.GetLocalizedCareerName(false), new ThumbnailKey(ResourceKey.CreatePNGKey(career.CareerIconColored, ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium)));
            }

            return results;
        }

        public class Item : ValueSettingOption<OccupationNames>
        {
            public Item(OccupationNames val, string name, ThumbnailKey key)
                : base(val, name, -1, key)
            {
            }
        }
    }
}