using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Options.General.CareerLevel
{
    public abstract class CareerLevelSettingOption : OperationSettingOption<GameObject>
    {
        public List<LevelData> mPicks = new List<LevelData>();

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, this.GetCareerOptions()).SelectMultiple();

            List<CareerLevelItem> mLevelOptions = new List<CareerLevelItem>();
            
            foreach (Item item in selection)
            {
                mLevelOptions.AddRange(GetLevelOptions(item.Value));                
            }

            CommonSelection<CareerLevelItem>.Results levelSelection = new CommonSelection<CareerLevelItem>(Name, mLevelOptions).SelectMultiple();

            foreach (CareerLevelItem item in levelSelection)
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

        public List<Item> GetCareerOptions()
        {
            List<Item> results = new List<Item>();

            results.Add(new Item(OccupationNames.Any, Common.Localize("Selection:All"), ThumbnailKey.kInvalidThumbnailKey));

            foreach (Career career in CareerManager.CareerList)
            {
                results.Add(new Item(career.Guid, career.GetLocalizedCareerName(false), new ThumbnailKey(ResourceKey.CreatePNGKey(career.CareerIconColored, ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium)));
            }

            return results;
        }

        public List<CareerLevelItem> GetLevelOptions(OccupationNames career)
        {
            List<CareerLevelItem> results = new List<CareerLevelItem>();

            Career career2 = CareerManager.GetStaticCareer(career);

            if (career2 == null)
            {
                return results;
            }

            results.Add(new CareerLevelItem(new LevelData(career, string.Empty, -1), Common.Localize("Selection:All"), ThumbnailKey.kInvalidThumbnailKey));

            foreach (KeyValuePair<string, Dictionary<int, Sims3.Gameplay.Careers.CareerLevel>> staticData in career2.CareerLevels)
            {
                foreach (KeyValuePair<int, Sims3.Gameplay.Careers.CareerLevel> levelStaticData in staticData.Value)
                {
                    results.Add(new CareerLevelItem(new LevelData(career, staticData.Key, levelStaticData.Key), career2.Name + " - " + levelStaticData.Value.BranchName + " - " + Common.LocalizeEAString(false, levelStaticData.Value.mName) + " (" + EAText.GetNumberString(levelStaticData.Key) + ")", new ThumbnailKey(ResourceKey.CreatePNGKey(career2.CareerIconColored, ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium)));
                }
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

        public class CareerLevelItem : ValueSettingOption<LevelData>
        {
            public CareerLevelItem(LevelData val, string name, ThumbnailKey key)
                : base(val, name, -1, key)
            {
            }
        }

        public class LevelData
        {
            public OccupationNames mCareer;
            public string mBranchName;
            public int mLevel;

            public LevelData(OccupationNames career, string branchName, int level)
            {
                mCareer = career;
                mBranchName = branchName;
                mLevel = level;
            }
        }
    }
}