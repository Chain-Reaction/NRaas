using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class FixFishData : PreloadOption
    {
        public FixFishData()
        { }

        public override string GetTitlePrefix()
        {
            return "FixFishData";
        }

        public override void OnPreLoad()
        {
            Overwatch.Log(GetTitlePrefix());

            if (Fish.sFishData == null) return;

            if (Fish.sFishData.ContainsKey(FishType.Box)) return;

            FishData data = new FishData();
            data.IngredientData = new IngredientData("BoxIngredient", "", "", 0, 0, -1, WorldName.Ep3FocusTestWorld, ProductVersion.Store, ProductVersion.Store);
            data.LocationFound = WaterTypes.All;
            data.Level = 3;
            data.MinWeight = 1;
            data.MaxWeight = 10;
            data.OverMax = false;
            data.SkillPoints = 160;
            data.MedatorName = "fishingChest";
            data.EffectSize = EffectSize.Small;
            data.PrefferedBait = "All";
            data.MinPrice = 100;

            float num = data.MaxWeight - data.MinWeight;
            data.PriceWeightSlope = (1000f - data.MinPrice) / num;
            data.WeightSkillSlope = num / Fish.kLevelsAboveForMaxWeight;

            Fish.sFishData.Add(FishType.Box, data);

            Overwatch.Log("  Box Data Added");
        }
    }
}
