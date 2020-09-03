using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

// listing with options for careers (eg, min / max coworkers), then list of careers, finally set option

namespace NRaas.CareerSpace.Options.General.CareerLevel
{
    public class CarpoolTypeSetting : CareerLevelSettingOption, ICareerLevelOption
    {
        public override string GetTitlePrefix()
        {
            return "CarpoolType";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);

            if (result != OptionResult.Failure)
            {
                Item selection = new CommonSelection<Item>(Name, this.GetCarOptions()).SelectSingle();

                if (selection == null) return OptionResult.Failure;

                foreach (CareerLevelSettingOption.LevelData level in base.mPicks)
                {
                    if (level.mLevel == -1)
                    {
                        foreach (Career career in CareerManager.CareerList)
                        {
                            foreach (string branch in career.CareerLevels.Keys)
                            {
                                foreach (KeyValuePair<int, Sims3.Gameplay.Careers.CareerLevel> levelData in career.CareerLevels[branch])
                                {
                                    PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(level.mCareer, true);
                                    PersistedSettings.CareerLevelSettings levelSettings = settings.GetSettingsForLevel(level.mBranchName, level.mLevel, true);
                                    levelSettings.mCarpoolType = selection.Value;

                                    levelData.Value.CarpoolType = selection.Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(level.mCareer, true);
                        PersistedSettings.CareerLevelSettings levelSettings = settings.GetSettingsForLevel(level.mBranchName, level.mLevel, true);
                        levelSettings.mCarpoolType = selection.Value;

                        NRaas.Careers.Settings.SetCareerData(settings);
                    }
                }

                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.Occupation != null)
                    {
                        sim.Occupation.RescheduleCarpool();
                    }
                }

                Common.Notify(Common.Localize("Generic:Success"));
                return OptionResult.SuccessLevelDown;
            }

            return result;
        }

        public List<Item> GetCarOptions()
        {
            List<Item> results = new List<Item>();

            CarNpcManager manager = CarNpcManager.Singleton;

            if(manager == null) return results;

            foreach(CarNpcManager.NpcCars car in Enum.GetValues(typeof(CarNpcManager.NpcCars)))
            {
                ProductVersion version = manager.GetProductVersionForNpcCar(car);

                ResourceKey key = GlobalFunctions.CreateProductKey((ulong)car, version);

                BuildBuyProduct product = UserToolUtils.GetProduct(UserToolUtils.BuildBuyProductType.Object, key);

                if (product == null) continue;

                results.Add(new Item(car, product.CatalogName, new ThumbnailKey(product.ProductResourceKey, ThumbnailSize.Medium)));
            }

            return results;
        }

        public new class Item : ValueSettingOption<CarNpcManager.NpcCars>
        {
            public Item(CarNpcManager.NpcCars val, string name, ThumbnailKey key)
                : base(val, name, -1, key)
            {
            }
        }
    }
}