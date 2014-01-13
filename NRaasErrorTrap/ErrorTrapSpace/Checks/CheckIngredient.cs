using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckIngredient : Check<Ingredient>
    {
        public static Dictionary<Vector3, HarvestPlant> sMissingPlants = new Dictionary<Vector3, HarvestPlant>();
        public static Dictionary<Vector3, Ingredient> sIngredients = new Dictionary<Vector3, Ingredient>();

        static List<HarvestPlant> sMergedPlants = new List<HarvestPlant>();

        protected override bool PrePerform(Ingredient ingredient, bool postLoad)
        {
            return true;
        }

        protected override bool PostPerform(Ingredient ingredient, bool postLoad)
        {
            if (!ErrorTrap.Loading) return true;

            if (ingredient.Position != Vector3.Zero)
            {
                HarvestPlant plant;
                if (sMissingPlants.TryGetValue(ingredient.Position, out plant))
                {
                    sMissingPlants.Remove(ingredient.Position);

                    MergePlantSeed(plant, ingredient);
                }
                else
                {
                    sIngredients[ingredient.Position] = ingredient;

                    //DebugLogCorrection("Potential Ingredient: " + ErrorTrap.GetName(ingredient));
                }
            }

            return true;
        }

        public static void MergePlantSeed(HarvestPlant plant, Ingredient ingredient)
        {
            plant.Seed = ingredient;

            plant.PlantDef = PlantHelper.GetPlantDefinition(plant.Seed);
            plant.mQualityLevel = ingredient.Plantable.QualityLevel;

            plant.SetGrowthState(plant.GrowthState);

            sMergedPlants.Add(plant);

            ErrorTrap.LogCorrection("Plant Seed Merged: " + ErrorTrap.GetName(plant));
        }

        public override void Finish()
        {
            foreach (HarvestPlant plant in sMergedPlants)
            {
                if (HarvestPlant.FindSlottedHarvestable(plant) != null)
                {
                    plant.SetGrowthState(PlantGrowthState.Harvest);
                }
            }

            sMissingPlants.Clear();
            sIngredients.Clear();
            sMergedPlants.Clear();
        }
    }
}
